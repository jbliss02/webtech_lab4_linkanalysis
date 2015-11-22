using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace webtech_lab4_linkanalysis
{
    class Program
    {

        static Pages visitedPages; //the pages that have been visited

        const string LINKFILE = @"http://www.dcs.bbk.ac.uk/~martin/sewn/ls4/sewn-crawl-2015.txt";
        const string TESTFILE = @"D:\bsc\web tec\coursework 4\test.txt";
        const string TESTFILE2 = @"D:\bsc\web tec\coursework 4\test2.txt";
        private const string FILEFOLDER = @"d:\bsc\web tec\coursework 4\";

        static void Main(string[] args)
        {
            visitedPages = new Pages(Pages.STRATEGY.REMOVEDANGLERS);
            ConvertFile(ReturnLinkFile());
            //ConvertFile(ReturnLinkFile_TEST());
            visitedPages.PopulateMatrix();
            visitedPages.matrix.DoQuestions(FILEFOLDER);
            visitedPages.DoPageRanks(60, FILEFOLDER);
            Console.WriteLine("Finished");
            Console.ReadLine();
        }

        static String ReturnLinkFile()
        {
            WebClient wc = new WebClient();
            Stream stream = wc.OpenRead(LINKFILE);
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        static String ReturnLinkFile_TEST()
        {
            return File.ReadAllText(TESTFILE2);
        }

        static void ConvertFile(string linkfile)
        {
            List<String> fileLines = linkfile.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();

            Page page = null;

            foreach(string line in fileLines)
            {               
                if (line.Length > 9 && line.Substring(0, 9) == "Visited: ") 
                {
                    if(page != null) visitedPages.allVisited.Add(page);
                    page = new Page(line.Trim().Substring(9));  //create new object as page visited
                }
                else if (line.Length > 6 && line.Trim().Substring(0, 6) == "Link: ") 
                {
                    page.links.Add(line.Trim().Substring(6)); //assume will never be null
                }
               
            }

            visitedPages.allVisited.Add(page); //last one

        }//ConvertFile

    }
}
