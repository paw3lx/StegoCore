using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace StegoCoreWeb.Controllers
{
    public abstract class ControllerBase : Controller
    {
        private IHostingEnvironment _environment;

        protected ControllerBase(IHostingEnvironment environment)
        {
            _environment = environment;
        }
    }
}