using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSitesDownloader
{
    class Program
    {
        public class School
        {
            public string Name { get; set; }
            public List<string> Departments { get; set; }

            public School()
            {
                this.Departments = new List<string>();
            }
        }

        class DataExtractor
        {
            /// <summary>
            /// Gets school info: name and departments name from html polish Wikipedia page.
            /// </summary>
            /// <param name="filePath">Path to locally saved html file.</param>
            /// <returns>School object with name and list of departments name.</returns>
            public static School GetSchool(string filePath)
            {
                HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();

                htmlDoc.OptionFixNestedTags = true;
                var e = htmlDoc.Encoding;
                htmlDoc.Load(filePath, Encoding.UTF8);

                var school = new School();

                if (htmlDoc.ParseErrors != null && htmlDoc.ParseErrors.Count() > 0)
                {
                    // Handle any parse errors as required
                }
                else
                {

                    if (htmlDoc.DocumentNode != null)
                    {
                        var nodes = htmlDoc.DocumentNode.Descendants();
                        school.Name = nodes.First(x => x.Id == "firstHeading").InnerText;
                        var content = nodes.First(x => x.Id == "mw-content-text").Descendants().Where(x=>x.Name=="li").ToList();

                        foreach (var item in content)
                        {
                            if(item.InnerText.Contains("Wydział")) {
                                school.Departments.Add(item.InnerText);
                            }
                        }
                    }
                }

                return school;
            }
        }

        static void Main(string[] args)
        {
            var schools = new List<School>();
            var dir = @"C:\Users\ignac_000\Desktop\stronywiki_uczelnie";
            var files = Directory.GetFiles(dir);

            foreach (var item in files)
            {
                schools.Add( DataExtractor.GetSchool(item));
            }
            string json = JsonConvert.SerializeObject(schools);
            var sw = File.CreateText($@"C:\Users\ignac_000\Desktop\strony_wydzialy\_strony_json_{DateTime.Now}.txt");
            sw.Write(json);
            sw.Close();
        }
    }
}
