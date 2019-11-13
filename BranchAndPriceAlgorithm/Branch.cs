using System;
using System.Collections.Generic;
using System.Text;

namespace BranchAndPriceAlgorithm
{
    public class Branch
    {
        public long BrID;

        // left child =0 right child =1, left means the branch is equal to 0, right means equal to 1
        public bool branch_status;

        // branching type
        public bool BrTypePrecedence;
        public bool BrTypeStartTime;
        public bool BrTypeMinDemand;
        public bool BrTypeResDemand;
        public bool BrTypeEmrDemand;


        // General Info BrPrecedence
        public int BrIntern;
        public int BrPrDisc; // BrPrDisc => BrDisc
        public int BrDisc;
        public int BrHospital;

        // General Info start time
        public int BrTime; // the rest is availble


        // General Info demand
        public double BrDemVal;
        public int BrWard;

        // info
        public double BrObj;
        public bool BrMIP;
        public Branch()
        {
            BrPrDisc = -1;
            BrDisc = -1;
            branch_status = false;
            BrHospital = -1;
            BrIntern = -1;
            BrID = 0; // the root branch ID is zero
            BrObj = 0;
            BrMIP = false;
            BrTypePrecedence = false;
            BrTypeStartTime = false;
            BrTime = -1;
            BrTypeMinDemand = false;
            BrTypeResDemand = false;
            BrTypeEmrDemand = false;
            BrDemVal = -1;
            BrWard = -1;
        }
        public Branch(Branch copy_member)
        {
            branch_status = copy_member.branch_status;
            BrPrDisc = copy_member.BrPrDisc;
            BrDisc = copy_member.BrDisc;
            BrHospital = copy_member.BrHospital;
            BrIntern = copy_member.BrIntern;
            BrID = copy_member.BrID;
            BrObj = copy_member.BrObj;
            BrMIP = copy_member.BrMIP;
            BrTypePrecedence = copy_member.BrTypePrecedence;
            BrTypeStartTime = copy_member.BrTypeStartTime;
            BrTime = copy_member.BrTime;
            BrTypeMinDemand = copy_member.BrTypeMinDemand;
            BrTypeResDemand = copy_member.BrTypeResDemand;
            BrTypeEmrDemand = copy_member.BrTypeEmrDemand;
            BrDemVal = copy_member.BrDemVal;
            BrWard = copy_member.BrWard;
        }

        public string displayMe() {
            return BrIntern+": "+ BrPrDisc + "=>" + BrDisc + "@ "+BrHospital;
        }

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
