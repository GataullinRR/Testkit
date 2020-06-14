using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Types;

namespace Utilities.Interfaces
{
    public enum FileOpenMode
    {
        /// <summary>
        /// Opens existing file otherwise throws an exception
        /// </summary>
        OPEN_OR_NEW = default,
        /// <summary>
        /// Creates new file. Deletes existing if required
        /// </summary>
        NEW,
        /// <summary>
        /// If file does not exists exception will be thrown
        /// </summary>
        ONLY_OPEN,
    }

    public enum IOAccess
    {
        READ_ONLY,
        FULL,
    }

    public interface IDirectoryAccessor
    {
        IOAccess Access { get; }
        string Name { get; }
        IEnumerable<Task<IFileAccessor>> EnumerateFilesAsync(CancellationToken cancellation);
        IEnumerable<Task<IDirectoryAccessor>> EnumerateDirectoriesAsync(CancellationToken cancellation);
        /// <summary>
        /// Returns null if it's root directory (does not have a parrent)
        /// </summary>
        /// <returns></returns>
        IDirectoryAccessor GetParrentDirectory();
        /// <summary>
        /// Ensures that the directory exists. If does not, all required directories will be created.
        /// </summary>
        /// <returns></returns>
        Task<IDirectoryAccessor> EnsureCreatedAsync(CancellationToken cancellation);

        Task<IFileAccessor> GetFileAsync(UPath relativePath, CancellationToken cancellation);
        Task<IDirectoryAccessor> GetDirectoryAsync(UPath relativePath, CancellationToken cancellation);
    }
}
