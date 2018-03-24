using System;
using System.Xml.Serialization;

namespace Stroller.Contracts.Serializable
{
    [Serializable]
    public class Image
    {
        [XmlAttribute("DirectoryName")]
        public string DirectoryName { get; set; }

        [XmlAttribute("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [XmlElement("Thumbnail")]
        public string Thumbnail { get; set; }

        [XmlAttribute("NumberOfChunks")]
        public int NumberOfChunks { get; set; }
    }
}
