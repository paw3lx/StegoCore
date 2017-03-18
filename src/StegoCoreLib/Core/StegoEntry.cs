namespace StegoCore.Core
{
    using System.Collections;
    using System.IO;
    using ImageSharp;

    public class StegoEntry : StegoBase
    {   
        public StegoEntry(Stream imageStream, Stream secretData)
        {
            image = new Image(imageStream);
            this.LoadSecretData(secretData);
        }

        public StegoEntry(string imagePath)
        {
            image = new Image(imagePath);

        }

        public StegoEntry(byte[] imageBytes)
        {
            image = new Image(imageBytes);

        }

        public void SaveImage(string path)
        {
            image.Save(path);
        }

        public void SaveImage(MemoryStream stream)
        {
            image.Save(stream);
        }

        protected void LoadSecretData(string filePath)
        {
            byte[] buffer = null;
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                buffer = new byte[fs.Length];
                fs.Read(buffer, 0, (int)fs.Length);
            }
            this.secretDataBits = new BitArray(buffer);
        }

        protected void LoadSecretData(Stream stream)
        {
            byte[] buffer = null;
            stream.Read(buffer, 0, (int) stream.Length);
            this.secretDataBits = new BitArray(buffer);
        }

        protected void LoadSecretData(byte[] bytes)
        {
            this.secretDataBits = new BitArray(bytes);
        }


    }
}