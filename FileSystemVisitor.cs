namespace AdvancedCSharp
{
    public class FileSystemVisitor
    {
        private readonly Func<FileSystemInfo, bool> _filter;
        private readonly IDirectoryRepository _repository;

        public delegate void FileSystemVisitorHandler(FileSystemInfo info, VisitorEventArgs args);

        public event EventHandler? Start;
        public event EventHandler? Finish;
        public event FileSystemVisitorHandler? FileFound;
        public event FileSystemVisitorHandler? DirectoryFound;
        public event FileSystemVisitorHandler? FilteredFileFound;
        public event FileSystemVisitorHandler? FilteredDirectoryFound;

        public FileSystemVisitor(Func<FileSystemInfo, bool> filter, IDirectoryRepository repository)
        {
            ArgumentNullException.ThrowIfNull(filter, nameof(filter));
            ArgumentNullException.ThrowIfNull(repository, nameof(repository));

            _filter = filter;
            _repository = repository;
        }

        public IEnumerable<FileSystemInfo> Traverse(string rootPath)
        {
            OnStart();

            var rootDirectory = new DirectoryInfo(rootPath);
            //if (!rootDirectory.Exists)
            //    throw new DirectoryNotFoundException("Root directory not found.");

            var result = TraverseDirectory(rootDirectory).ToList();
            OnFinish();

            return result;
        }

        private IEnumerable<FileSystemInfo> TraverseDirectory(DirectoryInfo directory)
        {
            foreach (var file in _repository.GetFileInfos(directory.FullName))
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

            foreach (var dir in _repository.GetDirectoryInfos(directory.FullName))
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

public interface IDirectoryRepository
{
    IEnumerable<DirectoryInfo> GetDirectoryInfos(string path);
    IEnumerable<FileInfo> GetFileInfos(string path);
}

public class DirectoryRepository : IDirectoryRepository
{
    public IEnumerable<DirectoryInfo> GetDirectoryInfos(string path)
        => new DirectoryInfo(path).GetDirectories();

    public IEnumerable<FileInfo> GetFileInfos(string path)
        => new DirectoryInfo(path).GetFiles();
}
