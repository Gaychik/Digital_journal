using DigitalJournal.Data.Security;
using DigitalJournal.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DigitalJournal.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        /// <summary>
        /// User auth
        /// </summary>
        /// <remarks>Create an instance of user on the server.</remarks>
        /// <param name="login">Login or student's case number</param>
        /// <param name="password">Password</param>
        /// <response code="401">Unauthorized</response>
        /// <response code="500">Internal Server Error</response>
        /// <response code="200">token within responce</response>
        [HttpPost]
        public IActionResult PostAuthUser(string login, string password)
        {
            try
            {
                var token = LocalUserSessionsService.CreateSession(login, password);
                return Ok(new { status = "ok", token });
            }
            catch (Exception E)
            {
                return StatusCode((int)(E is UnauthorizedAccessException ? HttpStatusCode.Unauthorized : HttpStatusCode.InternalServerError), new
                {
                    status = E is UnauthorizedAccessException ? "fail" : "shit!",
                    error = E.Message
                });
            }
        }
    }
}