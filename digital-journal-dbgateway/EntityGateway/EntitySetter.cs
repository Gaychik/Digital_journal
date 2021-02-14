using DigitalJournal.Data.Context;
using DigitalJournal.Data.Security;
using DigitalJournal.Models;
using DigitalJournal.Models.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitalJournal.Data.EntityGateway
{
    public sealed partial class EntityGateway : IDisposable
    {
        private readonly DigitalJournalContext db = new DigitalJournalContext();

        /// <summary>
        /// Adds entity if Id is 0. Otherwise Updates it
        /// </summary>
        /// <exception cref="EntityException">Contains EF Exception and a standart HTTP-responce for client side</exception>
        public Guid AddOrUpdate<T>(T entity) where T : class, IEntity
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));
            try
            {
                if (entity.Id == Guid.Empty) 
                {
                    if (entity is Human)
                        (entity as Human).Passhash = StringHash.GetStringSha256Hash((entity as Human).Passhash);
                    db.Add(EntitySecurityAdapter.Encrypt(entity));
                }
                else
                    db.Update(EntitySecurityAdapter.Encrypt(entity));  //todo: bugfix user can not change his password or account is lost
                db.SaveChanges();
                return entity.Id;
            }
            catch (Exception E)
            {
                throw new EntityException(E, entity);
            }
        }

        /// <summary>
        /// Adds range of objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        public void AddRange<T>(T[] entities) where T: class, IEntity
        {
            try
            {
                Human[] temp = entities as Human[];
                if (temp is not null)
                {
                    foreach (Human item in temp)
                    {
                        item.Passhash = StringHash.GetStringSha256Hash(item.Passhash);
                    }
                    db.AddRange(temp.Select(x => EntitySecurityAdapter.Encrypt(x)));
                }
                else
                    db.AddRange(entities.Select(x => EntitySecurityAdapter.Encrypt(x)));
                db.SaveChanges();
            }
            catch (Exception E)
            {
                throw new EntityException("Failed to InsertBulk.", E, null);
            }
        }

        /// <summary>
        /// deletes obj from db
        /// </summary>
        /// <exception cref="EntityException">Contains EF Exception and a standart HTTP-responce for client side</exception>
        public void Delete(IEntity entity)
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            try
            {
                db.Remove(entity);
                db.SaveChanges();
            }
            catch (Exception E)
            {
                throw new EntityException("Smth went wrong during deleting. Check Inner Exception.", E, entity);
            }
        }

        /// <summary>
        /// deletes obj by Id from db
        /// </summary>
        /// <exception cref="EntityException">Contains EF Exception and a standart HTTP-responce for client side</exception>
        public void Delete(Type type, Guid id)
        {
            try
            {
                db.Remove(db.Find(type, id));
                db.SaveChanges();
            }
            catch (Exception E)
            {
                throw new EntityException("Smth went wrong by deleting from Id. Check Inner Exception.", E, null);
            }
        }

        #region IDisposable implementation
        private bool disposedValue;
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    db.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                disposedValue = true;
            }
        }

        // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~EntityGateway()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}