using System.Xml.Serialization;

namespace Stroller.Contracts.Serializable
{
    [XmlRoot("Record", Namespace = "", IsNullable = false)]
    public class Record
    {
        [XmlArray("Images")]
        [XmlArrayItem("Image", typeof(Image))]
        public Image[] Images { get; set; }
    }
}
