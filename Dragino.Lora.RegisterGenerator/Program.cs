using System.IO;
using System.Text;
using System.Xml.Linq;

namespace Dragino.Lora.RegisterGenerator
{
    /// <summary>
    /// This program generates the SX1276Registers code file given the XML template in this project (see file paths below).
    /// </summary>
    internal static class Program
    {
        static void Main()
        {
            XDocument xml = EmbeddedFileSupport.LoadEmbeddedXml("Dragino.Lora.RegisterGenerator.Registers.SX1276.xml");

            var codeGenerator = new RegisterCodeGenerator(xml, true);

            File.WriteAllText(@"..\..\..\Dragino.Lora\Lora\Transceivers\SX1276Registers.cs", codeGenerator.Generate(), Encoding.UTF8);
        }
    }
}
