using StreetnamesBL.Model;
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
        void CleanFolder(string folderName);
        bool IsFolderEmpty(string folderName);
        void UnZip(string zipFileName, string unzipFolder);
        List<Province> ReadFiles(Dictionary<string, string> dictionary, string unzipFolder);
        void WriteResults(string unzipFolder, List<Province> provinces);
    }
}
