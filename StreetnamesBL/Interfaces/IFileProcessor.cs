using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreetnamesBL.Interfaces
{
    public interface IFileProcessor
    {
        List<string> GetFileNamesFromZip(string fileName);
        List<string> GetFileNamesConfigInfoFromZip(string fileName,string configName);
    }
}
