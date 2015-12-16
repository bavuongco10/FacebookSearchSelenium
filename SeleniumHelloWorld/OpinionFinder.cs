using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumHelloWorld
{
    class OpinionFinder
    {
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
