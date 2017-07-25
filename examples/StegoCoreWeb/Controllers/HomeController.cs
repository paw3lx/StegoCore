using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageSharp;
using ImageSharp.Formats;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StegoCore;
using StegoCore.Algorithms;
using StegoCoreWeb.Extensions;
using StegoCoreWeb.Model;
using StegoCoreWeb.Model.Logic;

namespace StegoCoreWeb.Controllers
{
    public class HomeController : ControllerBase
    {
        protected readonly SteganographyModel _stegoModel;
        public HomeController(IHostingEnvironment env)
            :base(env)
        {
            _stegoModel = new SteganographyModel(env);
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
            var uploads = _stegoModel.GetUploadsPath();
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
            var userImage = HttpContext.Session.Get<UserImage>(nameof(UserImage));
            var uploads = _stegoModel.GetUploadsPath();
            if (userImage == null || userImage.Guid == null || !System.IO.File.Exists(Path.Combine(uploads, userImage.Guid)))
                return RedirectToAction("Index");
            return View(userImage);
        }

        public IActionResult GetUserImage()
        {
            var userImage = HttpContext.Session.Get<UserImage>(nameof(UserImage));
            var uploads = _stegoModel.GetUploadsPath();
            if (userImage != null && System.IO.File.Exists(Path.Combine(uploads, userImage.Guid)))
            {              
                var file = System.IO.File.ReadAllBytes(Path.Combine(uploads, userImage.Guid));
                return File(file, userImage.ContentType, userImage.FileName);
            }
            return StatusCode(404);
        }

        public IActionResult GetEmbededImage()
        {
            var userImage = HttpContext.Session.Get<UserImage>(nameof(UserImage));
            var uploads = _stegoModel.GetUploadsPath();
            if (userImage != null && System.IO.File.Exists(Path.Combine(uploads, userImage.EmbededGuid)))
            {              
                var file = System.IO.File.ReadAllBytes(Path.Combine(uploads, userImage.EmbededGuid));
                return File(file, "image/" + userImage.EmbededFormat, 
                    Helpers.FileExtensionHelper.GetFileNameWithNewExtension(userImage.FileName, userImage.EmbededFormat));
            }
            return StatusCode(404);
        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult RemoveImage(string id)
        {
            var userImage = HttpContext.Session.Get<UserImage>(nameof(UserImage));
            if (userImage != null && userImage.IsUserImage(id))
            {
                var filePath = Path.Combine(_stegoModel.GetUploadsPath(), id);     
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
                
                if (userImage.EmbededGuid == id)
                    return RedirectToAction("ShowImage");
            }
            return RedirectToAction("Index");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Embed(IFormFile secret, AlgorithmEnum algorithm, string format)
        {
            EmbedResult result;
            try {
                result = await _stegoModel.Embed(secret, algorithm, format, HttpContext.Session);
            }
            catch(Exception e){
                return Content($"Error while embending: {e.Message}");
            }
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Decrypt(string id, AlgorithmEnum algorithm)
        {
            var userImage = HttpContext.Session.Get<UserImage>(nameof(UserImage));
            var filePath = Path.Combine(_stegoModel.GetUploadsPath(), id);
            if (System.IO.File.Exists(filePath) && (userImage.Guid == id || userImage.EmbededGuid == id))
            {
                using(var stego = new Stego(filePath))
                {
                    var result = stego.Decode(algorithm);
                    return File(result, "application/octet-stream", "secret");
                }
            }
            return Content("Wrong id");
            
        }
    }
}
