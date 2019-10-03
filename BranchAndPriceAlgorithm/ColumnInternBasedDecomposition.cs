using System;
using System.Collections.Generic;
using System.Text;

namespace BranchAndPriceAlgorithm
{
    public class ColumnInternBasedDecomposition
    {
        public int theIntern;
        public bool[][][] S_tdh;
        public int totalChange;
        public double desire;
        public double xRC;
        public double xVal;
        public double objectivefunction;

        public void initial(int timePeriod, int discipline, int hospital)
        {
            theIntern = -1;
            new DataLayer.ArrayInitializer().CreateArray(ref S_tdh, timePeriod, discipline, hospital, false);
            totalChange = 0;
            desire = 0;
            xRC = 0;
            xVal = 0;
            objectivefunction = 0;
        }
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
