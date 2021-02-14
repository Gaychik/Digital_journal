using DigitalJournal.Data.Security;
using DigitalJournal.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalJournal.Data.EntityGateway
{
    public partial class EntityGateway
    {
        /// <summary>
        /// Get table with predicate where <paramref name="type"/> is inherited from <see cref="IEntity"/>
        /// </summary>
        /// <param name="predicate">For filtering entities</param>
        public IEnumerable<object> GetTable(Type type, Func<dynamic, bool> predicate) => 
            db.Set(type).Select(x => EntitySecurityAdapter.Decrypt(x)).Where(predicate);      // todo: optimize
        /// <summary>
        /// Get table with predicate
        /// </summary>
        /// <param name="predicate">For filtering entities</param>
        public IEnumerable<T> GetTable<T>(Func<dynamic, bool> predicate) where T : class, IEntity => 
            GetTable(typeof(T), predicate).Cast<T>(); 
        /// <summary>
        /// Get all data without filtering
        /// </summary>
        public IEnumerable<T> GetTable<T>() where T : class, IEntity => 
            GetTable<T>(x => true);
        /// <summary>
        /// Get all data without filtering where <paramref name="type"/> is inherited from <see cref="IEntity"/>
        /// </summary>
        public IEnumerable<object> GetTable(Type type) => 
            GetTable(type, x => true);
    }
}