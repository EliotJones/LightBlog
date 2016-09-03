using System;
using System.Globalization;
using System.IO;

namespace LightBlog.Models.Posts
{
    public class PostInformation
    {
        public DateTime Date { get; }

        public string Name { get; }

        public string FilePath { get; }

        public bool ValidPost { get; } = true;

        public PostInformation (string filePath)
        {
            FilePath = filePath;

            try
            {
                var datePart = filePath.Substring(filePath.LastIndexOf("_") + 1)
                    .Replace(".txt", string.Empty);

                Name = filePath.Substring(filePath.LastIndexOf(Path.DirectorySeparatorChar));
                Date = DateTime.ParseExact(datePart, "ddMMyyyy", CultureInfo.InvariantCulture); 
            }
            catch (Exception)
            {
                ValidPost = false;
            }           
        }
    }
}