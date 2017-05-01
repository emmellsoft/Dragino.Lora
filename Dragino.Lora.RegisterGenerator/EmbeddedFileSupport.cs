using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Dragino.Lora.RegisterGenerator
{
    internal static class EmbeddedFileSupport
    {
        public static XDocument LoadEmbeddedXml(string resourceName)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                using (XmlTextReader xmlTextReader = new XmlTextReader(stream))
                {
                    xmlTextReader.Normalization = false;
                    return XDocument.Load(xmlTextReader);
                }
            }
        }
    }
}