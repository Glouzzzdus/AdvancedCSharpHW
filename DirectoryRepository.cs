using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedCSharp
{
    public class DirectoryRepository : IDirectoryRepository
    {
        public IEnumerable<DirectoryInfo> GetDirectoryInfos(string path)
        {
            return new DirectoryInfo(path).GetDirectories();
        }

        public IEnumerable<FileInfo> GetFileInfos(string path)
        {
            return new DirectoryInfo(path).GetFiles();
        }
    }
}
