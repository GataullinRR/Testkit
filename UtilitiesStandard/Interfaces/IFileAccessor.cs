using System.IO;
using System.Threading.Tasks;

namespace Utilities.Interfaces
{
    public interface IFileAccessor
    {
        string Name { get; }
        IOAccess Access { get; }
        IDirectoryAccessor GetParrentDirectory();
        Task<Stream> OpenAsync(FileOpenMode mode, System.Threading.CancellationToken cancellation);
    }
}
