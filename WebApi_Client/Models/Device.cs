using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi_Client.Models
{
    public class Device
    {
        public string ID { get; set; }
        public string Name { get; set; }

        public List<Spectrum> Spectrum { get; set; }
    }
}
