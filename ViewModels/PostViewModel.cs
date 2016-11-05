using System;
using System.IO;
using System.Text.RegularExpressions;
using HeyRed.MarkdownSharp;
using LightBlog.Models.Posts;

namespace LightBlog.ViewModels
{
    public class PostViewModel
    {
        // Summon Cthulhu
        private static Regex TitleRegex = new Regex("<h1>.+</h1>");

        public DateTime Date { get; set; }

        public string Text { get; set; }  

        public string Markdown { get; set; }

        public string Title { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }
        public string Name { get; set; }

        public PostViewModel ()
        {
        }

        public PostViewModel (PostInformation postInformation)
        {
            Date = postInformation.Date;
            Markdown = File.ReadAllText(postInformation.FilePath);
            
            var converter = new Markdown();

            Text = converter.Transform(Markdown);

            var regexMatch = TitleRegex.Match(Text); 

            Title = regexMatch.Value.Replace("<h1>", string.Empty)
                .Replace("</h1>", string.Empty);

            if (!string.IsNullOrEmpty(regexMatch.Value))
            {
                Text = Text.Replace(regexMatch.Value, string.Empty);
            }            

            Year = postInformation.Date.Year;
            Month= postInformation.Date.Month;
            Name = postInformation.Name.Substring(0, 
                postInformation.Name.LastIndexOf("_"));
        }
    }
}