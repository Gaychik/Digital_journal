using DigitalJournal.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DigitalJournal.Data.Security
{
    internal static class EntitySecurityAdapter
    {
        private static List<Type> CypheredTypes { get; set; }
        private static object Key { get; set; }
        /// <summary>
        /// 0 - encrypt,  1 - decrypt
        /// </summary>
        private static MethodInfo[] CypheringMethods { get; set; }     
        private static Type CypheringObject { get; set; }     
        
        public static T Encrypt<T>(T entity) where T : class, IEntity =>
            EncryptDecrypt(entity, 0);
        public static T Decrypt<T>(T entity) where T : class, IEntity =>
            EncryptDecrypt(entity, 1);


        static EntitySecurityAdapter()
        {
            var securitySettings = JObject.Parse(File.ReadAllText("dbgatewaysettings.json"))["security"];
            CypheredTypes = securitySettings["securedEntities"].Select(x => ModelController.GetTypeByNameFromModelAssembly(x.ToString())).ToList();

            try
            {
                CypheringObject = Assembly.Load((string)securitySettings["securityAssembly"]).GetType((string)securitySettings["cypherController"]);
                if (!CypheringObject.IsAssignableFrom(typeof(ICypher)))
                    throw new Exception();
            }
            catch { /*idc*/ }
            CypheringObject ??= typeof(DefaultEncryption);
            CypheringMethods = new[] { CypheringObject.GetMethod("Encrypt"), CypheringObject.GetMethod("Decrypt") };

            try
            {
                Assembly keyTypeAssembly = Assembly.Load((string)securitySettings["keyTypeAssembly"]);
                Type keyType = keyTypeAssembly.GetType((string)securitySettings["keyType"]);
                Key = securitySettings["key"].ToObject(keyType);
            }
            catch { /*idc*/ }
            Key ??= 200;

        }

        private static T EncryptDecrypt<T>(T entity, int ind) where T : class, IEntity
        {
            if (entity is null)
                throw new ArgumentNullException(nameof(entity));

            if (!CypheredTypes.Any(x => entity.GetType().IsAssignableTo(x)))
                return entity;

            T temp = (T)Activator.CreateInstance(entity.GetType());
            foreach (var property in entity.GetType().GetProperties().Where(x => x.CanWrite))     // todo: think of changing "just string cyphering" to smth more global
                property.SetValue(temp, property.PropertyType.Equals(typeof(string)) ? CypheringMethods[ind].Invoke(Activator.CreateInstance(CypheringObject), new[] { property.GetValue(entity), Key }) : property.GetValue(entity));

            return temp;
        }
    }
}