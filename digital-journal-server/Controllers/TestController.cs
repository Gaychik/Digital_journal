using DigitalJournal.Data.EntityGateway;
using DigitalJournal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DigitalJournal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            using EntityGateway db = new EntityGateway();
            int i = db.GetTable<Subject>().Count();
            Subject[] subjects = new Subject[] 
            {
                new Subject{ Name = $"Subject {i}" },
                new Subject{ Name = $"Subject {i+1}" },
                new Subject{ Name = $"Subject {i+2}" },
                new Subject{ Name = $"Subject {i+3}" },
                new Subject{ Name = $"Subject {i+4}" },
            };
            db.AddRange(subjects);
            IEnumerable<Moderator> moderators = db.GetTable<Moderator>().ToList();
            if (!moderators.Any())
                db.AddOrUpdate(new Moderator
                {
                    Birthday = DateTime.Now,
                    Citizenship = "DEFAULT",
                    DocumentNumber = "DEFAULT DEFAULT",
                    DocumentType = DocumentType.Passport,
                    Login = "admin",
                    Name = "DEFAULT",
                    Passhash = "admin",
                    PensionInsurance = "DEFAULT",
                    Phone = "DEFAULT",
                    RegistrationAddress = "DEFAULT DEFAULT DEFAULT"
                });
            return Ok(new { status = "ok", shit = "lol + 5 subjects", admins = moderators });
        }
    }
}
