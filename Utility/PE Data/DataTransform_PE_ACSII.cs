using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.PE_Data
{
    public class DataTransform_PE_ACSII
    {
        public static string PE_ACSII_ReferenceFIle = "ReferenceFile\\ori.ASC";
        public static bool Transform(string fileName, float[] xArray, float[] yArray, int count)
        {
            try
            {
                string source = PE_ACSII_ReferenceFIle;
                string target = fileName;
                File.Copy(source, target, true);

                List<int> list = new List<int>();
                for (int i = 0; i < count; i++)
                {
                    if (!list.Contains((int)xArray[i]))
                    {
                        string temp = ((int)xArray[i]).ToString("#0.000000") + " " + yArray[i].ToString("#0.000000") + Environment.NewLine;
                        File.AppendAllText(target, temp, Encoding.Default);
                        list.Add((int)xArray[i]);
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
