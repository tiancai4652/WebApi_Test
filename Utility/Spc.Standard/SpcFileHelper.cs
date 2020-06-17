using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Standard
{
    public class SpcFileHelper
    {
        public static SpcResult Read(string path)
        {
            RSpcFile spc = new RSpcFile();

            //判断Spc是否加密，如果加密，解密
            if (!spc.IsRightSpcFile(path))
            {
                Application.DoEvents();
                //进行解密操作
                string sDesFile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
                sDesFile = Path.Combine(sDesFile, "DesTmp.spc");
                Des desTmp = Des.Instance();
                if (desTmp.DecryptFile(path, sDesFile))
                {
                    path = sDesFile;
                }
            }

            if (spc.Open(path))
            {
                float[] xData, yData;
                spc.Read(out xData, out yData);
                SpcResult r = new SpcResult()
                {
                    XData = xData,
                    YData = yData,
                    Length = xData.Length
                };
                spc.Close();
                return r;
            }
            else
            {
                return null;
            }
        }

    

        public static void SaveToTxt(string path, params SpcResult[] results)
        {
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))//创建写入文件 
                {
                    StreamWriter sw = new StreamWriter(fs);
                    for (int n = 0; n < results.Length; ++n)
                    {
                        sw.WriteLine("-----------------------------------------");
                        sw.WriteLine("XData" + "                    " + "YData");//开始写入值
                        var result = results[n];
                        for (int i = 0; i < result.Length; ++i)
                        {
                            if (result.XData[i] < 0 || result.XData[i] > 3200)
                            {
                                continue;
                            }
                            sw.WriteLine(result.XData[i] + "                    " + result.YData[i]);//开始写入值
                        }
                    }
                    sw.Close();
                    fs.Close();
                }
            }
            catch { }
        }
    }
}
