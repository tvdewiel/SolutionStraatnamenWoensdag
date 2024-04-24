using StreetnamesBL.Interfaces;
using StreetnamesBL.Model;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreetnamesDL
{
    public class FileProcessor : IFileProcessor
    {
        public void CleanFolder(string folderName)
        {
            DirectoryInfo dir=new DirectoryInfo(folderName);
            foreach(FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }
            foreach(DirectoryInfo di in dir.GetDirectories())
            {
                CleanFolder(di.FullName);
                di.Delete();
            }
        }

        public List<string> GetFileNamesConfigInfoFromZip(string fileName, string configName)
        {
            using (ZipArchive archive = ZipFile.OpenRead(fileName))
            {
                var entry = archive.GetEntry(configName);
                if (entry != null)
                {
                    List<string> data = new();
                    // Open a stream to read the file content
                    //using (Stream entryStream = entry.Open())
                    using (StreamReader reader = new StreamReader(entry.Open()))
                    {
                        // Read and process the content of the file
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            data.Add(line);
                        }
                        return data;
                    }
                }
                else throw new FileNotFoundException($"{configName} not found");
            }
        }

        public List<string> GetFileNamesFromZip(string fileName)
        {
          if (!File.Exists(fileName)) { throw new FileNotFoundException($"{fileName} not found" ); }
          using(var zipfile=ZipFile.OpenRead(fileName))
            {
                return zipfile.Entries.Select(x=>x.FullName).ToList();
            }
        }

        public bool IsFolderEmpty(string folderName)
        {
            DirectoryInfo dir = new DirectoryInfo(folderName);
            return (dir.GetFiles().Length == 0 && dir.GetDirectories().Length==0);
        }

        public List<Province> ReadFiles(Dictionary<string, string> filesMap, string dir)
        {
            Dictionary<int, Province> provinces = new Dictionary<int, Province>();
            Dictionary<int, Municipality> municipalities = new Dictionary<int, Municipality>();
            Dictionary<int, string> streetNames = new Dictionary<int, string>();
            //lees prov ids
            HashSet<int> provinceIds = new HashSet<int>();
            using (StreamReader p = new StreamReader(Path.Combine(dir, filesMap["provinces"])))
            {
                string[] ids = p.ReadLine().Trim().Split(",");
                foreach (string id in ids) { provinceIds.Add(Int32.Parse(id)); }
            }
            //lees prov namen + ids
            using (StreamReader gp = new StreamReader(Path.Combine(dir, filesMap["link_Province_MunicipalityNames"])))
            {
                string line;
                gp.ReadLine();
                while ((line = gp.ReadLine()) != null)
                {
                    string[] ss = gp.ReadLine().Trim().Split(";");
                    int municipalityId = Int32.Parse(ss[0]);
                    int provinceId = Int32.Parse(ss[1]);
                    string languageCode = ss[2];
                    string provinceName = ss[3];
                    if (provinceIds.Contains(provinceId) && (languageCode == "nl"))
                    {
                        if (!provinces.ContainsKey(provinceId))
                        {
                            provinces.Add(provinceId, new Province(provinceId, provinceName));
                        }
                        if (!provinces[provinceId].HasMunicipality(municipalityId))
                        {
                            provinces[provinceId].AddMunicipality(new Municipality(municipalityId));
                            municipalities.Add(municipalityId, provinces[provinceId].GetMunicipality(municipalityId));
                        }
                    }
                }
            }
            //lees gemeentenaam
            using (StreamReader m = new StreamReader(Path.Combine(dir, filesMap["municipalityNames"])))
            {
                string line;
                m.ReadLine();
                while ((line = m.ReadLine()) != null)
                {
                    string[] ss = m.ReadLine().Trim().Split(";");
                    int municipalityId = Int32.Parse(ss[1]);
                    string languageCode = ss[2];
                    string municiplaityName = ss[3];
                    if (languageCode == "nl")
                    {
                        if (municipalities.ContainsKey(municipalityId))
                            municipalities[municipalityId].Name = municiplaityName;
                    }
                }
            }
            //lees straatnaam
            using (StreamReader m = new StreamReader(Path.Combine(dir, filesMap["streetNames"])))
            {
                string line;
                m.ReadLine();
                while ((line = m.ReadLine()) != null)
                {
                    string[] ss = m.ReadLine().Trim().Split(";");
                    int streetnameId = Int32.Parse(ss[0]);
                    string streetName = ss[1];
                    streetNames.Add(streetnameId, streetName);
                }
            }
            //lees koppeling straatnamen
            using (StreamReader m = new StreamReader(Path.Combine(dir, filesMap["link_StreetName_Municipality"])))
            {
                string line;
                m.ReadLine();
                while ((line = m.ReadLine()) != null)
                {
                    string[] ss = m.ReadLine().Trim().Split(";");
                    int municipalityId = Int32.Parse(ss[1]);
                    int streetnameId = Int32.Parse(ss[0]);
                    if (municipalities.ContainsKey(municipalityId) && streetNames.ContainsKey(streetnameId)) {
                        municipalities[municipalityId].AddStreetName(streetNames[streetnameId]);    
                    }
                }
            }
            return provinces.Values.ToList();
        }

        public void UnZip(string zipFileName, string unzipFolder)
        {
            ZipFile.ExtractToDirectory(zipFileName, unzipFolder);
        }

        public void WriteResults(string unzipFolder, List<Province> provinces)
        {
            DirectoryInfo di = new DirectoryInfo(unzipFolder);

            foreach(Province province in provinces)
            {
                di.CreateSubdirectory(province.Name);
                foreach(Municipality m in province.GetMunicipalities())
                {
                    using(StreamWriter sw=new StreamWriter(Path.Combine(unzipFolder,province.Name,m.Name+".txt"))) 
                    { 
                        foreach(string street in m.GetStreetNames())
                        {
                            sw.WriteLine(street);
                        }
                    }
                }
            }
        }
    }
}
