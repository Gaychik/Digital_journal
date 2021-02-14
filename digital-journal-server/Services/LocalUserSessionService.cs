using DigitalJournal.Data.EntityGateway;
using DigitalJournal.Data.Security;
using DigitalJournal.Models;
using DigitalJournal.Models.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalJournal.Services
{
    internal static class LocalUserSessionsService
    {
        struct Session
        {
            public IAuthorizable User { get; set; }
            public DateTime LastOperation { get; set; }
            public Guid Token { get; set; }

            public bool IsActive => DateTime.Now - LastOperation < TimeSpan.FromHours(1);
        }
        static List<Session> Sessions { get; set; } = new List<Session>();

        /// <summary>
        /// Create user session for token
        /// </summary>
        /// <returns>Session Token</returns>
        public static Guid CreateSession(string login, string password)
        {
            Task.Run(() => Sessions.RemoveAll(session => !session.IsActive));
            using EntityGateway db = new EntityGateway();

            var passhash = StringHash.GetStringSha256Hash(password);
            IEnumerable<Human> enumerable = db.GetTable<Moderator>(x => x.Login == login && x.Passhash == passhash).ToList();
            var potentialUser = enumerable.FirstOrDefault();

            if (potentialUser == null)
                throw new UnauthorizedAccessException("Wrong credentials");

            var newSession = new Session() { User = potentialUser, LastOperation = DateTime.Now, Token = Guid.NewGuid() };
            Sessions.Add(newSession);
            return newSession.Token;
        }

        /// <summary>
        /// Check existance of session, returning users rights
        /// </summary>
        /// <param name="token">token of current session</param>
        /// <returns>rights category</returns>
        public static Rights CheckRights(Guid token)
        {
            var Check = Sessions.FirstOrDefault(x => x.Token == token);
            if (!Check.IsActive)
                throw new UnauthorizedAccessException(Check.User != null ? "Session expired" : "Wrong credentials");

            Check.LastOperation = DateTime.Now;
            return Check.User.Rights;
        }

        /// <summary>
        /// returns authed user
        /// </summary>
        /// <param name="token">session token</param>
        /// <returns>faculty id</returns>
        //public static IAuthorizable GetUser(Guid token)
        //{
        //    var Check = Sessions.FirstOrDefault(x => x.Token == token);
        //    if (!Check.IsActive)
        //        throw new UnauthorizedAccessException(Check.User != null ? "Session expired" : "Wrong credentials");
        //    Check.LastOperation = DateTime.Now;
        //    return Check.User;
        //}

        public static bool CheckExistance(Guid token)
        {
            var Check = Sessions.FirstOrDefault(x => x.Token == token);
            if (!Check.IsActive)
                throw new UnauthorizedAccessException(Check.User != null ? "Session expired" : "Wrong credentials");
            Check.LastOperation = DateTime.Now;
            return true;
        }

        public static Guid GetIdBySession(Guid token)
        {
            var Check = Sessions.FirstOrDefault(x => x.Token == token);
            if (!Check.IsActive)
                throw new UnauthorizedAccessException(Check.User != null ? "Session expired" : "Wrong credentials");
            
            Check.LastOperation = DateTime.Now;
            return Check.User.Id;
        }
    }
}