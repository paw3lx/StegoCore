using System;

namespace StegoCoreWeb.Model
{
    public class UserImage
    {
        public string FileName { get; set; }

        public string Guid { get; set; }
        public string ContentType { get; set; }

        public string EmbededGuid { get; set; }

        public string EmbededFormat { get; set; }

        public bool IsUserImage(string id)
        {
            return Guid == id || EmbededGuid == id;
        }
    }
}