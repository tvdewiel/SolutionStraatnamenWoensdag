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

        public Province(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public void AddMunicipality(Municipality municipality)
        {
            municipalities.TryAdd(municipality.Id, municipality);
        }
        public IReadOnlyList<Municipality> GetMunicipalities()
        {
            return municipalities.Values.ToList();
        }

        public bool HasMunicipality(int municipalityId)
        {
            return municipalities.ContainsKey(municipalityId);
        }
        public Municipality GetMunicipality(int municipalityId)
        {
            return municipalities[municipalityId];
        }
    }
}
