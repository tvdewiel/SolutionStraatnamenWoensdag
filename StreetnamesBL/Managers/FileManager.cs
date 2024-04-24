using StreetnamesBL.Exceptions;
using StreetnamesBL.Interfaces;
using StreetnamesBL.Model;
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

        public Dictionary<string,string> CheckZipFile(string zipFileName, List<string> fileNames)
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
                return map;
            }
            catch (ZipFileManagerException) { throw; }
            catch (Exception ex)
            {
                throw new FileManagerException($"CheckZipFile - {ex.Message}");
            }

        }

        public void CleanFolder(string folderName)
        {
            try
            {
                processor.CleanFolder(folderName);
            }
            catch (Exception ex) { throw new FileManagerException($"CleanFolder {ex.Message}"); }
        }

        public List<string> GetFilesFromZip(string fileName)
        {
            try
            {
                return processor.GetFileNamesFromZip(fileName);
            }
            catch(Exception ex) { throw new FileManagerException("GetFilesFromZip"); }

        }

        public bool IsFolderEmpty(string folderName)
        {
           try
            {
                return processor.IsFolderEmpty(folderName);
            }
            catch(Exception ex) { throw new FileManagerException($"IsFolderEmpty {ex.Message}"); }
        }

        public List<string> ProcessZip(string zipFileName, string unzipFolder)
        {
            List<string> messages=new List<string>();
            try
            {
                //unzip
                processor.UnZip(zipFileName, unzipFolder);
                //bestanden lezen
               
                List<Province> provinces = processor.ReadFiles(CheckZipFile(zipFileName,GetFilesFromZip(zipFileName)), unzipFolder);
                //resultaten wegschrijven
                processor.WriteResults(unzipFolder, provinces);
                //statistieken
                Statistics stats = CalculateStatistics(provinces);
                messages.AddRange(stats.Provinces.Select(x=>$"{x.Key} : {x.Value} municipalities"));
                messages.AddRange(stats.Municipalities.Select(x => $"{x.Key} : {x.Value} streets"));
                return messages;
            }
            catch(Exception ex) { throw new FileManagerException($"ProcessZip {ex.Message}"); }
        }
        private Statistics CalculateStatistics(List<Province> provinces)
        {
            var statistics=new Statistics();
            statistics.Provinces=provinces.ToDictionary(x=>x.Name,x=>x.GetMunicipalities().Count());
            statistics.Municipalities = provinces
                .SelectMany(list => list.GetMunicipalities(), (p, m) => new { name = (p.Name, m.Name), count = m.GetStreetNames().Count })
                .OrderBy(x => x.name)
                .ToDictionary(x => x.name, x => x.count);
            return statistics;
        }
    }
}
