using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreetnamesBL.Model
{
    public class Municipality
    {
        public int Id { get; set; }
        public string Name { get; set; }
        private SortedSet<string> streetNames=new SortedSet<string>();
        public void AddStreetName(string streetName)
        {
            streetNames.Add(streetName);
        }
        public IReadOnlyList<string> GetStreetNames()=>streetNames.ToList();
    }
}
