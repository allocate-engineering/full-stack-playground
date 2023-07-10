
using System.IO;
using System.Reflection;

namespace Allocate.Web.DemoApi.Helpers;

public static class ResourceFile
{
    private static readonly Assembly _assembly;

    static ResourceFile()
    {
        _assembly = Assembly.GetExecutingAssembly();
    }

    public static string Read(string name)
    {
        using Stream stream = _assembly.GetManifestResourceStream("Allocate.Web.DemoApi.Resources." + name);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}