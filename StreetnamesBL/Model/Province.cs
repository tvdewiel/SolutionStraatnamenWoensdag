using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreetnamesBL.Model
{
    public class Province
    {
        public int Id { get; set; }
        public string Name { get; set; }
        private Dictionary<int,Municipality> municipalities=new();
        public void AddMunicipality(Municipality municipality)
        {
            municipalities.TryAdd(municipality.Id, municipality);
        }
        public IReadOnlyList<Municipality> GetMunicipalities()
        {
            return municipalities.Values.ToList();
        }
    }
}
