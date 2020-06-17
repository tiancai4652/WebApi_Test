using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class BaseDataConvertor
    {
        public static List<T> ConvertIListToList<T>(System.Collections.IList gbList) where T : class
        {
            if (gbList != null && gbList.Count > 0)
            {
                List<T> list = new List<T>();
                for (int i = 0; i < gbList.Count; i++)
                {
                    T temp = gbList[i] as T;
                    if (temp != null)
                        list.Add(temp);
                }
                return list;
            }
            return null;
        }
    }
}
