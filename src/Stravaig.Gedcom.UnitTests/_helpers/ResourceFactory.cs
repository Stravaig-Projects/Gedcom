using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Stravaig.Gedcom.UnitTests._helpers
{
    public class ResourceFactory
    {
        private static string BaseNamespace => typeof(ResourceFactory).Namespace?.Replace($@".{nameof(_helpers)}", string.Empty);

        public static string Get(Type baseType, string resourceName)
        {
            string fullResourceName = GetFullResourceName(baseType, resourceName);
            return Get(fullResourceName);
        }

        public static TextReader GetReader(Type baseType, string resourceName)
        {
            string fullResourceName = GetFullResourceName(baseType, resourceName);
            return GetReader(fullResourceName);
        }
        
        public static Stream GetStream(Type baseType, string resourceName)
        {
            string fullResourceName = GetFullResourceName(baseType, resourceName);
            return GetStream(fullResourceName);
        }
      
        public static string Get(string fullResourceName)
        {
            try
            {
                using var reader = GetReader(fullResourceName);
                string result = reader.ReadToEnd();
                return result;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Something went wrong attempting to retrieve resource named \"{fullResourceName}\". See inner exception for more details.", ex);
            }
        }

        public static TextReader GetReader(string fullResourceName)
        {
            try
            {
                Stream stream = GetStream(fullResourceName);
                return new StreamReader(stream, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Something went wrong attempting to retrieve resource named \"{fullResourceName}\". See inner exception for more details.", ex);
            }
        }

        public static Stream GetStream(string fullResourceName)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                return assembly.GetManifestResourceStream(fullResourceName);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Something went wrong attempting to retrieve resource named \"{fullResourceName}\". See inner exception for more details.", ex);
            }
        }

        public static string GetFileName(Type baseType, string resourceName)
        {
            string typePath = baseType.FullName
                .Substring(BaseNamespace.Length + 1)
                .Replace('.', Path.DirectorySeparatorChar);
            var fullPath = Path.Join(Environment.CurrentDirectory, "_resources", typePath, resourceName);
            if (File.Exists(fullPath))
                return fullPath;
            
            throw new FileNotFoundException(
                $"Cannot find the resource file. Was it set to \"CopyAlways\"?{Environment.NewLine}Current Directory is \"{Environment.CurrentDirectory}\".{Environment.NewLine}File Path: \"{fullPath}\".", fullPath);
        }
        
        private static string GetFullResourceName(Type baseType, string resourceName)
        {
            string typeName = baseType.FullName.Substring(BaseNamespace.Length + 1);
            string fullResourceName = $"{BaseNamespace}._resources.{typeName}.{resourceName}";
            return fullResourceName;
        }
    }
}