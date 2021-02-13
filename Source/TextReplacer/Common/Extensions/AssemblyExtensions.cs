using System;
using System.IO;
using System.Reflection;

namespace TextReplacer.Common.Extensions
{
    public static class AssemblyExtensions
    {
        #region Public Methods

        public static string GetDirectory(this Assembly assembly)
        {
            string codeBase = assembly.CodeBase;
            var uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(path);
        }

        #endregion
    }
}