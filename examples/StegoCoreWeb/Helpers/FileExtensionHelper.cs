using System.IO;
namespace StegoCoreWeb.Helpers
{
    public class FileExtensionHelper
    {
        public static string GetFileNameWithNewExtension(string fileName, string extension)
        {
            if (string.IsNullOrEmpty(fileName))
                return " ." + extension;
            return $"{Path.GetFileNameWithoutExtension(fileName)}.{extension}"; 
        }
    }
}