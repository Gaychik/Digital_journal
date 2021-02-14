using System;
using System.Collections.Generic;
using System.Net;

namespace DigitalJournal.Data
{
    /// <summary>
    /// Exception, declaring Entity problems
    /// </summary>
    public class EntityException : Exception
    {
        private readonly static Dictionary<HttpStatusCode, string> standartMessagePairs = new Dictionary<HttpStatusCode, string>()
        {
            { HttpStatusCode.ExpectationFailed, "Object is not valid for current model state!" },           //417
            { HttpStatusCode.InternalServerError, "Something went wrong..." },      //500
            { HttpStatusCode.BadRequest, "Wrong data!" },      //400
        };

        public object ReasonEntity { get; set; }
        public HttpStatusCode AssotiatedStatusCode { get; set; } = HttpStatusCode.BadRequest;

        /// <summary>
        /// Creates an exception with your message
        /// </summary>
        public EntityException(string message, Exception innerException, object entity) : this(message, innerException) =>
            ReasonEntity = entity;

        /// <summary>
        /// Exception without inner one
        /// </summary>
        public EntityException(string message, object entity, HttpStatusCode statusCode) : this(message, null, entity) =>
            AssotiatedStatusCode = statusCode;

        /// <summary>
        /// Exception without inner exception 
        /// </summary>
        public EntityException(object entity, HttpStatusCode statusCode) : this(standartMessagePairs[statusCode], entity, statusCode) { }

        /// <summary>
        /// Creates a typical exception based on inner one
        /// </summary>
        public EntityException(Exception innerException, object entity) : this(standartMessagePairs[HttpStatusCode.InternalServerError], innerException, entity) =>
            AssotiatedStatusCode = HttpStatusCode.InternalServerError;

        public EntityException(string message) : base(message) { }

        public EntityException(string message, Exception innerException) : base(message, innerException) { }
    }
}