using System.IO;
using System.Threading.Tasks;
using ImageSharp;
using ImageSharp.Formats;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using StegoCore;
using StegoCore.Algorithms;
using StegoCoreWeb.Extensions;

namespace StegoCoreWeb.Model.Logic
{
    public class SteganographyModel
    {
        private IHostingEnvironment _environment;
        public SteganographyModel(IHostingEnvironment hostingEnvironment)
        {
            _environment = hostingEnvironment;
        }

        public string GetUploadsPath()
        {
            var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            }
            return uploads;
        }

        public async Task<EmbedResult> Embed(IFormFile secret, AlgorithmEnum algorithm, string format, ISession session)
        {
            var userImage = session.Get<UserImage>(nameof(UserImage));
            var filePath = Path.Combine(GetUploadsPath(), userImage.Guid);
            var embedResult = new EmbedResult
            {
                Algorithm = algorithm,
                Success = false
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
                    EncoderOptions options = null;
                    var formatType = Helpers.FormatHelper.GetFormatByName(format);
                    if (formatType is JpegFormat)
                    {
                        options = new JpegEncoderOptions
                        {
                            Quality = 100
                        };
                    }
                    imageWithSecret.Save(Path.Combine(GetUploadsPath(), embedResult.Guid), formatType, options);
                    embedResult.Success = true;
                    embedResult.Format = userImage.EmbededFormat = format;
                    userImage.EmbededGuid = embedResult.Guid;
                }
                session.Set<UserImage>(nameof(UserImage), userImage);
            }
            else 
            {
                throw new InvalidDataException("You have to select secret file to embed in image");
            }
            return embedResult;
        }
    }
}