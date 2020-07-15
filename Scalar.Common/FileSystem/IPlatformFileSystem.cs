using Scalar.Common.Tracing;

namespace Scalar.Common.FileSystem
{
    public interface IPlatformFileSystem
    {
        bool SupportsFileMode { get; }
        bool SupportsUntrackedCache { get; }
        void FlushFileBuffers(string path);
        void MoveAndOverwriteFile(string sourceFileName, string destinationFilename);
        bool TryGetNormalizedPath(string path, out string normalizedPath, out string errorMessage);
        bool IsExecutable(string filePath);
        bool IsSocket(string filePath);
        bool TryCreateDirectoryWithAdminAndUserModifyPermissions(string directoryPath, out string error);
        bool TryCreateOrUpdateDirectoryToAdminModifyPermissions(ITracer tracer, string directoryPath, out string error);
        bool IsFileSystemSupported(string path, out string error);
    }
}
