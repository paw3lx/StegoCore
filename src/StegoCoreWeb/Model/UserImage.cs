using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using StegoCore.Algorithms;

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

        public AlgorithmEnum algorithm { get; set;}

        public List<SelectListItem> Algorithms => new List<SelectListItem>
        {
            new SelectListItem { Value = ((int)AlgorithmEnum.Lsb).ToString(), Text = AlgorithmEnum.Lsb.ToString() },
            new SelectListItem { Value = ((int)AlgorithmEnum.ZhaoKoch).ToString(), Text = AlgorithmEnum.ZhaoKoch.ToString() },
        };
    }
}