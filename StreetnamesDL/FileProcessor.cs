using StreetnamesBL.Interfaces;
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
    }
}
