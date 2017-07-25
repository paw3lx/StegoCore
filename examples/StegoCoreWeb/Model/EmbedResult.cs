using StegoCore.Algorithms;

namespace StegoCoreWeb.Model
{
    public class EmbedResult
    {
        public EmbedResult()
        {
            Guid = System.Guid.NewGuid().ToString();
        }
        public string Guid { get; set; }
        public bool Success { get; set; }

        public AlgorithmEnum Algorithm { get; set; }

        public string Format;
    }
}