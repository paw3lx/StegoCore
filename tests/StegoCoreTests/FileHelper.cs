namespace StegoCoreTests
{
    public static class FileHelper
    {
        public static string GetPathToSecretData()
        {
            var fileInfo = new System.IO.FileInfo("../../../files/secret.txt");
            return fileInfo.FullName;
        }
        public static string GetPathToImage()
        {
            var fileInfo = new System.IO.FileInfo("../../../files/lena.bmp");
            return fileInfo.FullName;
        }

    }
}