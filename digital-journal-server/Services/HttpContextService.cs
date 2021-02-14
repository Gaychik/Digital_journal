using Microsoft.AspNetCore.Http;

namespace DigitalJournal.Services
{
    public interface IHttpContextService
    {
        string ValueKeyModel { get; }
    }
    public class HttpContextService : IHttpContextService
    {

        public HttpContextService(HttpContext httpContext)
        {
            ValueKeyModel = httpContext?.Request.RouteValues["Model"].ToString();
        }
        public string ValueKeyModel { get; }
    }
}