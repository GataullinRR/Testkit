using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Utilities.Types
{
    public class DiskFile : IFileAccessor
    {
        readonly string _path;
        public IOAccess Access { get; }
        public string Name => Path.GetFileName(_path);

        public DiskFile(string path, IOAccess access)
        {
            _path = path;
            Access = access;
        }

        public IDirectoryAccessor GetParrentDirectory()
        {
            var dirPath = Path.GetDirectoryName(_path);

            return new DiskDirectory(dirPath, Access);
        }

        public async Task<Stream> OpenAsync(FileOpenMode mode, CancellationToken cancellation)
        {
            switch (Access)
            {
                case IOAccess.READ_ONLY:
                    break;
                case IOAccess.FULL:
                    IOUtils.CreateDirectoryIfNotExist(Path.GetDirectoryName(_path));
                    break;

                default:
                    throw new NotSupportedException();
            }

            return File.Open(_path, mapMode(), mapAccess(Access));

            FileMode mapMode()
            {
                switch (mode)
                {
                    case FileOpenMode.OPEN_OR_NEW:
                        return FileMode.OpenOrCreate;
                    case FileOpenMode.NEW:
                        return FileMode.Create;

                    default:
                        throw new NotSupportedException();
                }
            }
            System.IO.FileAccess mapAccess(IOAccess access)
            {
                switch (access)
                {
                    case IOAccess.READ_ONLY:
                        return System.IO.FileAccess.Read;
                    case IOAccess.FULL:
                        return System.IO.FileAccess.ReadWrite;

                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }
}
