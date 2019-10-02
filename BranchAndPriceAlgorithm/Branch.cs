using System;
using System.Collections.Generic;
using System.Text;

namespace BranchAndPriceAlgorithm
{
    public class Branch
    {



        public void WriteXML(string Path)
        {
            Path = Path + "Info.xml";
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(Branch));
            System.IO.FileStream file = System.IO.File.Create(Path);
            writer.Serialize(file, this);
            file.Close();
        }

        public Branch ReadXML(string Path)
        {
            // Now we can read the serialized book ...  
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(Branch));

            System.IO.StreamReader file = new System.IO.StreamReader(Path);
            Branch tmp = (Branch)reader.Deserialize(file);
            file.Close();
            return tmp;
        }

    }
}
