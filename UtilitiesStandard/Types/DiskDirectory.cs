using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Extensions;
using Utilities.Interfaces;

namespace Utilities.Types
{
    public class DiskDirectory : IDirectoryAccessor
    {
        static readonly PathFormat PATH_FORMAT = new PathFormat("\\", ":\\");

        readonly UPath _path;
        public IOAccess Access { get; }
        public string Name => new DirectoryInfo(_path.ToString()).Name;

        public DiskDirectory(string path, IOAccess access)
        {
            _path = new UPath(PATH_FORMAT, path);
            Access = access;
        }

        public async Task<IDirectoryAccessor> EnsureCreatedAsync(CancellationToken cancellation)
        {
            IOUtils.CreateDirectoryIfNotExist(_path.ToString());

            return this;
        }

        public IEnumerable<Task<IDirectoryAccessor>> EnumerateDirectoriesAsync(CancellationToken cancellation)
        {
            return Directory
                .GetDirectories(_path.ToString())
                .Select(path => Task.FromResult((IDirectoryAccessor)new DiskDirectory(path, Access)));
        }

        public IEnumerable<Task<IFileAccessor>> EnumerateFilesAsync(CancellationToken cancellation)
        {
            return Directory
                .GetFiles(_path.ToString())
                .Select(path => Task.FromResult((IFileAccessor)new DiskFile(path, Access)));
        }

        public async Task<IDirectoryAccessor> GetDirectoryAsync(UPath relativePath, CancellationToken cancellation)
        {
            var fullPath = _path.GetCopy().AppendSelf(relativePath);

            return new DiskDirectory(fullPath.ToString(), Access);
        }

        public async Task<IFileAccessor> GetFileAsync(UPath relativePath, CancellationToken cancellation)
        {
            var fullPath = _path.GetCopy().AppendSelf(relativePath);

            return new DiskFile(fullPath.ToString(), Access);
        }

        public IDirectoryAccessor GetParrentDirectory()
        {
            var dirPath = Path.GetDirectoryName(_path.ToString());

            return new DiskDirectory(dirPath, Access);
        }
    }
}
