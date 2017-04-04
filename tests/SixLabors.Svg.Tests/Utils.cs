using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SixLabors.Svg.Tests
{
    static class Utils
    {
        private static readonly string root;

        static Utils()
        {
            var rootpath = new Uri(typeof(Utils).GetTypeInfo().Assembly.CodeBase).LocalPath;
            rootpath  = Path.GetDirectoryName(rootpath);
            var slnRoot = FindParentFolderContaining("SixLabors.Svg.sln", rootpath);
            root = Path.Combine(slnRoot, "tests", "SixLabors.Svg.Tests");
        }

        public static string GetPath(params string[] parts)
        {
            return Path.Combine(new[] { root }.Concat(parts).ToArray());
        }

        private static string FindParentFolderContaining(string item, string path = ".")
        {
            var items = Directory.EnumerateFileSystemEntries(path).Select(x=>Path.GetFileName(x));
            if (items.Any(x => x.Equals(item, StringComparison.OrdinalIgnoreCase)))
            {
                return Path.GetFullPath(path);
            }
            else
            {

                var parent = Path.GetDirectoryName(Path.GetFullPath(path));
                if (parent != null)
                {
                    return FindParentFolderContaining(item, parent);
                }
            }

            return null;
        }
    }
}
