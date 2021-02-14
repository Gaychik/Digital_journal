using DigitalJournal.Data;
using DigitalJournal.Data.EntityGateway;
using DigitalJournal.Models;
using DigitalJournal.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;

namespace DigitalJournal.Controllers
{
    [Route("api/{Model}")]
    [ApiController]
    public class EntityController : ControllerBase, IDisposable
    {
        private Type EntityType { get; set; }

        private readonly EntityGateway entityGateway = new EntityGateway();
        
        private (Rights Read, Rights Write) Accessibility
        {
            get
            {
                var access = Attribute.GetCustomAttribute(EntityType, typeof(AccessibilityAttribute), false) as AccessibilityAttribute;
                return (access.Read, access.Write);
            }
        }

        private void AccessibilityCheck(Rights typeRights)
        {
            Rights rights = Rights.Guest;
            try
            {
                var temp = HttpContext.Request.Headers["Token"];
                var token = Guid.Parse(temp.ToString());
                rights = LocalUserSessionsService.CheckRights(token);
            }
            catch { /*idc*/ }
            if ((rights & typeRights) != rights)
                throw new UnauthorizedAccessException("You have no rights for that action!");
        }

        public EntityController(IHttpContextService service)
        {
            string type = service.ValueKeyModel;
            if ((type ?? "") != string.Empty && type.Length > 1)
                type = char.ToUpper(type[0]) + type[1..].ToLower();

            EntityType = ModelController.GetTypeByNameFromModelAssembly(type);            
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                AccessibilityCheck(Accessibility.Read);
                return Ok(entityGateway.GetTable(EntityType, x => x.Id == id).First());
            }
            catch (EntityException E)
            {
                return StatusCode((int)E.AssotiatedStatusCode, new { status = "fail", message = E.Message, root = E.Source });
            }
            catch (UnauthorizedAccessException E)
            {
                return Unauthorized(new { status = "fail", message = E.Message, root = "UnauthorizedAccess" });
            }
            catch (Exception E)
            {
                return StatusCode(500, new { status = "fail", message = E.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                AccessibilityCheck(Accessibility.Read);
                return Ok(entityGateway.GetTable(EntityType));
            }
            catch (EntityException E)
            {
                return StatusCode((int)E.AssotiatedStatusCode, new { status = "fail", message = E.Message, root = E.Source });
            }
            catch (UnauthorizedAccessException E)
            {
                return Unauthorized(new { status = "fail", message = E.Message, root = "UnauthorizedAccess" });
            }
            catch (Exception E)
            {
                return StatusCode(500, new { status = "fail", message = E.Message });
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] JObject jObject)
        {
            try
            {
                AccessibilityCheck(Accessibility.Write);

                jObject.Remove("token");
                entityGateway.AddOrUpdate((IEntity)jObject.ToObject(EntityType));
                return Ok(new { status = "ok" });
            }
            catch (EntityException E)
            {
                return StatusCode((int)E.AssotiatedStatusCode, new { status = "fail", message = E.Message, root = E.Source });
            }
            catch (UnauthorizedAccessException E)
            {
                return Unauthorized(new { status = "fail", message = E.Message, root = "UnauthorizedAccess" });
            }
            catch (Exception E)
            {
                return StatusCode(500, new { status = "fail", message = E.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteById(Guid id)
        {
            try
            {
                AccessibilityCheck(Accessibility.Write);

                entityGateway.Delete(EntityType, id);
                return Ok(new { status = "ok" });
            }
            catch (EntityException E)
            {
                return StatusCode((int)E.AssotiatedStatusCode, new { status = "fail", message = E.Message, root = E.Source });
            }
            catch (UnauthorizedAccessException E)
            {
                return Unauthorized(new { status = "fail", message = E.Message, root = "UnauthorizedAccess" });
            }
            catch (Exception E)
            {
                return StatusCode(500, new { status = "fail", message = E.Message });
            }
        }

        #region IDisposable implementation
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    entityGateway.Dispose();
                }

                // free unmanaged resources (unmanaged objects) and override finalizer
                // set large fields to null
                disposedValue = true;
            }
        }

        // override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~EntityController()
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