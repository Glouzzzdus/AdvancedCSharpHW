using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedCSharp
{
    public interface IDirectoryRepository
    {
        IEnumerable<DirectoryInfo> GetDirectoryInfos(string path);
        IEnumerable<FileInfo> GetFileInfos(string path);
    }
}
