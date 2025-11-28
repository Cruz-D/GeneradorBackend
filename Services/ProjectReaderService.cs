using System;
using System.IO;
using System.Linq;
using System.Text;

namespace GeneradorBackend.Services
{
    internal sealed class ProjectReaderService
    {
        private readonly string[] _ignoredFolders;
        private readonly string[] _ignoredExtensions;

        public ProjectReaderService(string[]? ignoredFolders = null, string[]? ignoredExtensions = null)
        {
            _ignoredFolders = ignoredFolders ?? new[]
            {
                "bin", "obj", ".vs", ".git", ".idea", ".vscode", "node_modules", "Properties"
            };

            _ignoredExtensions = ignoredExtensions ?? new[]
            {
                ".user", ".suo", ".db", ".log", ".gitignore", ".gitattributes", ".copilot", ".dockerignore", ".http", ".Development.json", ".json", ".sln"
            };
        }

        public string ReadDirectoryTree(string rootPath, string indent = "")
        {
            if (string.IsNullOrWhiteSpace(rootPath) || !Directory.Exists(rootPath))
                return string.Empty;

            var sb = new StringBuilder();
            try
            {
                var dirInfo = new DirectoryInfo(rootPath);

                if ((dirInfo.Attributes & FileAttributes.Hidden) != 0 ||
                    (dirInfo.Attributes & FileAttributes.System) != 0 ||
                    _ignoredFolders.Contains(dirInfo.Name, StringComparer.OrdinalIgnoreCase))
                {
                    return string.Empty;
                }

                sb.AppendLine($"{indent}{dirInfo.Name}/");

                foreach (var file in dirInfo.GetFiles())
                {
                    if ((file.Attributes & FileAttributes.Hidden) != 0 ||
                        (file.Attributes & FileAttributes.System) != 0)
                        continue;

                    if (_ignoredExtensions.Any(ext => file.Name.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    sb.AppendLine($"{indent}  {file.Name}");
                }

                foreach (var subDir in dirInfo.GetDirectories())
                {
                    var subTree = ReadDirectoryTree(subDir.FullName, indent + "  ");
                    if (!string.IsNullOrWhiteSpace(subTree))
                        sb.Append(subTree);
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Omitir carpetas a las que no tenemos acceso
            }
            catch (PathTooLongException)
            {
                // Omitir entradas con path demasiado largo
            }
            return sb.ToString();
        }
    }
}
