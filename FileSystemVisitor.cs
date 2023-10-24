using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdvancedCSharp
{
    public class FileSystemVisitor
    {
        private readonly Func<FileSystemInfo, bool> _filter;
        private readonly string _rootPath;

        public delegate void FileSystemVisitorHandler(FileSystemInfo info, VisitorEventArgs args);

        public event EventHandler Start;
        public event EventHandler Finish;
        public event FileSystemVisitorHandler FileFound;
        public event FileSystemVisitorHandler DirectoryFound;
        public event FileSystemVisitorHandler FilteredFileFound;
        public event FileSystemVisitorHandler FilteredDirectoryFound;

        public FileSystemVisitor(string rootPath, Func<FileSystemInfo, bool>? filter = null)
        {
            _rootPath = rootPath;
            _filter = filter;
        }

        public IEnumerable<FileSystemInfo> Traverse()
        {
            OnStart();

            var rootDirectory = new DirectoryInfo(_rootPath);
            if (!rootDirectory.Exists)
                throw new DirectoryNotFoundException("Root directory not found.");

            var result = TraverseDirectory(rootDirectory).ToList();
            OnFinish();

            return result;
        }

        private IEnumerable<FileSystemInfo> TraverseDirectory(DirectoryInfo directory)
        {
            foreach (var file in directory.GetFiles())
            {
                var args = new VisitorEventArgs();
                OnFileFound(file, args);

                if (args.Terminate) yield break;
                if (!args.Exclude && (_filter == null || _filter(file)))
                {
                    var filteredArgs = new VisitorEventArgs();
                    OnFilteredFileFound(file, filteredArgs);

                    if (filteredArgs.Terminate) yield break;
                    if (!filteredArgs.Exclude) yield return file;
                }
            }

            foreach (var dir in directory.GetDirectories())
            {
                var args = new VisitorEventArgs();
                OnDirectoryFound(dir, args);

                if (args.Terminate) yield break;
                if (!args.Exclude && (_filter == null || _filter(dir)))
                {
                    var filteredArgs = new VisitorEventArgs();
                    OnFilteredDirectoryFound(dir, filteredArgs);

                    if (filteredArgs.Terminate) yield break;
                    if (!filteredArgs.Exclude) yield return dir;
                }

                foreach (var fsi in TraverseDirectory(dir))
                {
                    yield return fsi;
                }
            }
        }

        protected virtual void OnStart() => Start?.Invoke(this, EventArgs.Empty);
        protected virtual void OnFinish() => Finish?.Invoke(this, EventArgs.Empty);
        protected virtual void OnFileFound(FileSystemInfo file, VisitorEventArgs args) => FileFound?.Invoke(file, args);
        protected virtual void OnDirectoryFound(FileSystemInfo dir, VisitorEventArgs args) => DirectoryFound?.Invoke(dir, args);
        protected virtual void OnFilteredFileFound(FileSystemInfo file, VisitorEventArgs args) => FilteredFileFound?.Invoke(file, args);
        protected virtual void OnFilteredDirectoryFound(FileSystemInfo dir, VisitorEventArgs args) => FilteredDirectoryFound?.Invoke(dir, args);
    }
}

