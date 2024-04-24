using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreetnamesBL.Model
{
    public class Statistics
    {
        public Dictionary<string,int> Provinces { get; set; } = new Dictionary<string,int>();
        public Dictionary<(string,string),int> Municipalities { get; set; }=new();
    }
}
