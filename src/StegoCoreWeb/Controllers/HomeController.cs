using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StegoCore;
using StegoCoreWeb.Extensions;
using StegoCoreWeb.Model;

namespace StegoCoreWeb.Controllers
{
    public class HomeController : ControllerBase
    {
        private IHostingEnvironment _environment;

        public HomeController(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            if (file.Length > 0)
            {
                var userImage = new UserImage
                {
                    FileName = file.FileName,
                    ContentType = file.ContentType,
                    Guid = System.Guid.NewGuid().ToString()
                };
                using (var fileStream = new FileStream(Path.Combine(uploads, userImage.Guid), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                HttpContext.Session.Set<UserImage>(nameof(UserImage), userImage);    
            }
            return Json("ok");
        }

        public IActionResult ShowImage()
        {
            return View();
        }

        public IActionResult GetUserImage()
        {
            var userImage = HttpContext.Session.Get<UserImage>(nameof(UserImage));
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            if (userImage != null && System.IO.File.Exists(Path.Combine(uploads, userImage.Guid)))
            {              
                var file = System.IO.File.ReadAllBytes(Path.Combine(uploads, userImage.Guid));
                return File(file, userImage.ContentType, userImage.FileName);
            }
            return Unauthorized();
        }


        public IActionResult RemoveImage()
        {
            var userImage = HttpContext.Session.Get<UserImage>(nameof(UserImage));
            if (userImage != null)
            {
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", userImage.Guid);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
                HttpContext.Session.Remove(nameof(UserImage));
            }
            return RedirectToAction("Index");
        }
    }
}
