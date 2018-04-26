using SixLabors.ImageSharp.Formats;

namespace StegoCoreWeb.Helpers
{
    public static class FormatHelper
    {
        public static IImageEncoder GetEncoderByName(string format)
        {
            switch(format){
                case "bmp":
                    return new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder();
                case "png":
                    return new SixLabors.ImageSharp.Formats.Png.PngEncoder();
                case "jpeg":
                    return new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder();
                default:
                    return new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder();
                
            }
        }
    }
}