using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml.Linq;
namespace Chiats
{
    public class Documentation
    {
        public class Comments
        {
            public string Summary { get; set; }
            public string Remarks { get; set; }
            public override string ToString()
            {
                return $"Summary : {Summary}";
            }

        }
        public Dictionary<string, Comments> Alls = new Dictionary<string, Comments>();
        public string Name { get; private set; }

        public Documentation(string documentFile)
        {
            XDocument doc = XDocument.Load(documentFile);
            XElement assembly_name = doc.Root.Element("assembly")?.Elements("name").FirstOrDefault();
            if (assembly_name != null)
            {
                this.Name = assembly_name.Value;
            }
            IEnumerable<XElement> members = doc.Root.Element("members")?.Elements("member");
            foreach (var member in members)
            {
                string n = member.Name.LocalName;
                string name = member.Attribute("name")?.Value;
                if (member is XElement element)
                {
                    string summary = element.Element("summary")?.Value;
                    string remarks = element.Element("remarks")?.Value;
                    Alls.Add(name, new Comments { Summary = summary, Remarks = remarks });
                }
            }
        }
    }
}

