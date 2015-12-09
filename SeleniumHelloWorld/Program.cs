using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SeleniumHelloWorld
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new input());
            //WriteTextFile();
            //ReadTextFile();
             var crawl = new Crawl();
            crawl.Main();

        }

        private static void ReadTextFile()
        {
            //File.ReadAllText()
        }

        private static void WriteTextFile()
        {
            TestBD1Entities db = new TestBD1Entities();
            using (StreamWriter _writer = new StreamWriter(@"D:\opinionfinderv2.0\comment"))
            {
                foreach (var comment in db.Comments)
                {
                    string fileName = @"D:\opinionfinderv2.0\database\docs\comment\" + comment.CommentId;
                    using (StreamWriter writer =
                        new StreamWriter(fileName))
                    {
                        writer.Write(comment.CommentContent);

                    }
                    _writer.WriteLine(fileName);
                }
            }
        }
    }
}