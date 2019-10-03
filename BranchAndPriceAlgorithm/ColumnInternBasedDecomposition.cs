using System;
using System.Collections.Generic;
using System.Text;

namespace BranchAndPriceAlgorithm
{
    public class ColumnInternBasedDecomposition
    {
        public int theIntern;
        public bool[][][] S_tdh;
        public double desire;
        public double xRC;
        public double xVal;

        public void WriteXML(string Path)
        {
            Path = Path + "Info.xml";
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(ColumnInternBasedDecomposition));
            System.IO.FileStream file = System.IO.File.Create(Path);
            writer.Serialize(file, this);
            file.Close();
        }

        public ColumnInternBasedDecomposition ReadXML(string Path)
        {
            // Now we can read the serialized book ...  
            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(ColumnInternBasedDecomposition));

            System.IO.StreamReader file = new System.IO.StreamReader(Path);
            ColumnInternBasedDecomposition tmp = (ColumnInternBasedDecomposition)reader.Deserialize(file);
            file.Close();
            return tmp;
        }
    }
}
