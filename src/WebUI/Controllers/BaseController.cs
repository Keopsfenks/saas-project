using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
    [Authorize(Policy = "CookieAuth")]
    public class BaseController : Controller
    {
        protected string Language => HttpContext.Request.Headers.AcceptLanguage;
    }
}
