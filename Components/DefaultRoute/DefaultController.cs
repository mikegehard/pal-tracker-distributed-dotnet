using Microsoft.AspNetCore.Mvc;

namespace DefaultRoute
{
    [Route("")]
    public class DefaultController : Controller
    {
        [HttpGet]
        public string Default() => "Noop!";
    }
}