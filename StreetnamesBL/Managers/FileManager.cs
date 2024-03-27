using StreetnamesBL.Exceptions;
using StreetnamesBL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreetnamesBL.Managers
{
    public class FileManager
    {
        private IFileProcessor processor;

        public FileManager(IFileProcessor processor)
        {
            this.processor = processor;
        }

        public void CheckZipFile(string zipFileName, List<string> fileNames)
        {
            try
            {
                Dictionary<string, string> map = new();
                Dictionary<string, string> errors = new Dictionary<string, string>();
                List<string> configEntries = new List<string>() {
                "streetNames",
                "municipalityNames",
                "link_StreetName_Municipality",
                "link_Province_MunicipalityNames",
                "provinces"
            };
                if (!fileNames.Contains("FileNamesConfig.txt")) throw new ZipFileManagerException("FileNamesConfig.txt is missing");
                List<string> data = processor.GetFileNamesConfigInfoFromZip(zipFileName, "FileNamesConfig.txt");
                foreach (string line in data)
                {
                    string[] parts = line.Split(':');
                    map.Add(parts[0].Trim(), parts[1].Trim().Replace("\"", string.Empty));
                }
                //controleer entries config
                foreach (string entry in configEntries)
                {
                    if (!map.ContainsKey(entry)) errors.Add(entry, "missing in config");
                }
                //controleer bestanden in zip
                foreach (string file in map.Values)
                {
                    if (!fileNames.Contains(file)) errors.Add(file, "missing in zip");
                }
                if (errors.Count > 0)
                {
                    ZipFileManagerException ex = new ZipFileManagerException("Missing");
                    foreach (var e in errors)
                    {
                        ex.Data.Add(e.Key, e.Value);
                    }
                    throw ex;
                }
            }
            catch (ZipFileManagerException) { throw; }
            catch (Exception ex)
            {
                throw new FileManagerException($"CheckZipFile - {ex.Message}");
            }

        }

        public List<string> GetFilesFromZip(string fileName)
        {
            try
            {
                return processor.GetFileNamesFromZip(fileName);
            }
            catch(Exception ex) { throw new FileManagerException("GetFilesFromZip"); }

        }
    }
}
