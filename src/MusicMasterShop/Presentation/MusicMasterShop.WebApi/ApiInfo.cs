using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace MusicMasterShop.WebApi
{
    [ExcludeFromCodeCoverage]
    public static class ApiInfo
    {
        public const string BaseUrl = "mmshop";
        public const string Title = "Api da music master shop";

        public static string? GetVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = $"{fvi.FileMajorPart}.{fvi.FileMinorPart}.{fvi.FileBuildPart}";

            return version;
        }
    }
}
