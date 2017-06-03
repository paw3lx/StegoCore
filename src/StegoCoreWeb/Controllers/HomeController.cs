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

namespace StegoCoreWeb.Controllers
{
    public class HomeController : ControllerBase
    {
        public HomeController(IHostingEnvironment env)
            :base(env)
        {
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
            var uploads = GetUploadsPath();
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
            var uploads = GetUploadsPath();
            if (userImage == null || userImage.Guid == null || !System.IO.File.Exists(Path.Combine(uploads, userImage.Guid)))
                return RedirectToAction("Index");
            return View(userImage);
        }

        public IActionResult GetUserImage()
        {
            var userImage = HttpContext.Session.Get<UserImage>(nameof(UserImage));
            var uploads = GetUploadsPath();
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
            var uploads = GetUploadsPath();
            if (userImage != null && System.IO.File.Exists(Path.Combine(uploads, userImage.EmbededGuid)))
            {              
                var file = System.IO.File.ReadAllBytes(Path.Combine(uploads, userImage.EmbededGuid));
                return File(file, "image/" + userImage.EmbededFormat, 
                    Helpers.FileExtensionHelper.GetFileNameWithNewExtension(userImage.FileName, userImage.EmbededFormat));
            }
            return StatusCode(404);
        }


        public IActionResult RemoveImage(string id)
        {
            var userImage = HttpContext.Session.Get<UserImage>(nameof(UserImage));
            if (userImage != null && userImage.IsUserImage(id))
            {
                var filePath = Path.Combine(GetUploadsPath(), id);     
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
            var userImage = HttpContext.Session.Get<UserImage>(nameof(UserImage));
            var uploads = GetUploadsPath();
            var filePath = Path.Combine(uploads, userImage.Guid);
            var embedResult = new EmbedResult
            {
                Algorithm = algorithm
            };
            if (secret != null && secret.Length > 0 && System.IO.File.Exists(filePath))
            {
                byte[] secretBytes = null;
                using (var memoryStrem = new MemoryStream())
                {
                    await secret.CopyToAsync(memoryStrem);
                    secretBytes = memoryStrem.ToArray();
                }
                using(var stego = new Stego(filePath))
                {
                    stego.SetSecretData(secretBytes);
                    var imageWithSecret = stego.Embed(algorithm);
                    imageWithSecret.Save(Path.Combine(uploads, embedResult.Guid), Helpers.FormatHelper.GetFormatByName(format));
                    embedResult.Success = true;
                    embedResult.Format = userImage.EmbededFormat = format;
                    userImage.EmbededGuid = embedResult.Guid;
                }
                HttpContext.Session.Set<UserImage>(nameof(UserImage), userImage);
            }
            else {
                return Content("You have to select secret file to embed in image");

            }
            
            return View(embedResult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Decrypt(string id, AlgorithmEnum algorithm)
        {
            var userImage = HttpContext.Session.Get<UserImage>(nameof(UserImage));
            var filePath = Path.Combine(GetUploadsPath(), id);
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
