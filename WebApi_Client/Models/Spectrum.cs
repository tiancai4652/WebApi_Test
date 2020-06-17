using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi_Client.Models
{
    public class Spectrum
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public List<SpectrumData> Data { get; set; }
    }
}
