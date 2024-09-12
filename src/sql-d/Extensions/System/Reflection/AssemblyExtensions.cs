using System.Reflection;

namespace SqlD.Extensions.System.Reflection;

public static class AssemblyExtensions
{
    public static string GetDirectory(this Assembly assembly)
    {
        var assemblyUri = new Uri(assembly.CodeBase);
        var fileName = new FileInfo(assemblyUri.LocalPath);
        var workingDirectory = fileName.Directory.FullName;
        return workingDirectory;
    }
}