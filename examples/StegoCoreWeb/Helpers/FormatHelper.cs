using ImageSharp.Formats;

namespace StegoCoreWeb.Helpers
{
    public static class FormatHelper
    {
        public static IImageFormat GetFormatByName(string format)
        {
            switch(format){
                case "bmp":
                    return new BmpFormat();
                case "png":
                    return new PngFormat();
                case "jpeg":
                    return new JpegFormat();
                default:
                    return new BmpFormat();
                
            }
        }
    }
}