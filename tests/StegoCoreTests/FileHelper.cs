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
            return GetPathToImage("lena.bmp");
        }

        public static string GetPathToImage(string fileName)
        {
            var fileInfo = new System.IO.FileInfo("../../../files/" + fileName);
            return fileInfo.FullName;
        }

    }
}