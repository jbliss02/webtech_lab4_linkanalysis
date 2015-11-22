using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webtech_lab4_linkanalysis
{
    public class Pages
    {
        public enum STRATEGY {UNORTHADOX, REMOVEDANGLERS, ADDVISITED_1 } //the solution we are using to create the depenedency matrix
        public STRATEGY approach;
        //UNORTHADOX - leaves dangling pages in (the axis then have different sizes and this is not an adjaceny matirx)
        //REMOVEDANGLERS - removes any pages in links that are not visited
        //ADDVISITED_1 - adds non visited links as visited, but with no inlinks

        public const double TELEPORT = 0.15;
        public const double CONVERGENCETARGET = 0.0001; 
        public Pages(STRATEGY approach) { this.approach = approach; }

        public List<Page> allVisited = new List<Page>(); //every visited page

        public List<String> allLinks; //every, unique, link in the pages list

        public Matrix matrix;

        private List<Double> averageConvergence = new List<double>(); //saves the average convergence on each iteration of pageRank
        private List<Double> averageConvergencePercent = new List<double>(); //saves the average convergence percent on each iteration of pageRank 

        public void PopulateMatrix()
        {
            //populates the matrix. Gets a list of all the unique inlinks. Each outlink object (CLASS) is then sent this list
            //the class converts this list into a matrix row

            matrix = new Matrix();

            PopulateAllLinks(); //extract the pages and links
            if (approach == STRATEGY.REMOVEDANGLERS) { RemoveAllDanglers(); }
            if (approach == STRATEGY.ADDVISITED_1) { AddUnvisited(); }

            for (int i = 0; i < allVisited.Count; i++)
            {
                matrix.rows.Add(allVisited[i].ReturnMatrixRow(allLinks));
            }

        }//PopulateMatrix

        private void PopulateAllLinks()
        {
            allLinks = new List<string>();

            for (int i = 0; i < allVisited.Count; i++)
            {
                for (int n = 0; n < allVisited[i].links.Count; n++)
                {
                    if (!allLinks.Exists(el => el == allVisited[i].links[n])) { allLinks.Add(allVisited[i].links[n]); }
                }
            }

        }//PopulateAllLinks

        private void RemoveAllDanglers()
        {
            //removes any page on the link axis that has not been visited
            for(int i = allLinks.Count - 1; i >= 0; i--)
            {
                if (!allVisited.Exists(el => el.pageUrl == allLinks[i])) { allLinks.RemoveAt(i); }
                
                //may have removed the last element, if so shift i
                if (i == allLinks.Count) { i--; }
            }

            //removes any visited page that is not linked to
            for (int i = allVisited.Count - 1; i >= 0; i--)
            {
                if (!allLinks.Exists(el => el == allVisited[i].pageUrl)) { allVisited.RemoveAt(i); }

                //may have removed the last element, if so shift i
                if (i == allVisited.Count) { i--; }
            }
       
        }//RemoveAllDanglers

        private void AddUnvisited()
        {
            //adds unvisited links as a page, but with no
            for(int i = 0; i < allLinks.Count; i++)
            {
                var exists = allVisited.Where(p => p.pageUrl == allLinks[i]).Any();
                if (!exists) { allVisited.Add(new Page(allLinks[i])); }

            }//allLinks

        }

        public void DoPageRanks(int n, string fileFolder)
        {
            //creates multiple pagerank iterations, terminates once the CONVERGENCETARGET has been hit 

            int count = 0;
            while (averageConvergencePercent.Count < 2 || (averageConvergencePercent.Last() - averageConvergencePercent[averageConvergencePercent.Count - 2]) > CONVERGENCETARGET)
            {                
                SetPageRanks();
                if (count > 1) { SetConvergence(); }
                if (count > 3) { Console.WriteLine(averageConvergencePercent.Last() - averageConvergencePercent[averageConvergencePercent.Count - 2]); }
                count++;               
            }

            WritePageRanks(fileFolder, count);

        }//DoPageRanks

        private void SetPageRanks()
        {
            //iterates over each page and sets the page rank - only iterates once, called many times for further iterations
            //TODO - This is screaming out for recursion

            for(int i = 0; i < allVisited.Count; i++)
            {
                double pageRankCalc = 0; //the running page rank calculation for i

                //for each page that has a link to the current page - PR / Outlinks of that page
                for(int n = 0; n < allVisited.Count; n++)
                {
                    if(allVisited[n].HasLink(allVisited[i].pageUrl) && n != i) //if page[n] has a link to page[i]
                    {
                        pageRankCalc += allVisited[n].pageRank.Last() / allVisited[n].links.Count;
                    }

                }//each page to check

                //all links have been iterated over, calculate the pagerank for this page
                allVisited[i].pageRank.Add(TELEPORT / allVisited.Count + ((1 - TELEPORT) * pageRankCalc));

            }//each page

        }//SetPageRanks

        private void SetConvergence()
        {
            //after each pageRank iteration, get the average convergence and add to the list
            double total = 0;
            for(int i = 0; i < allVisited.Count; i++)
            {
                total += allVisited[i].convergence;
            }

            averageConvergence.Add(total / allVisited.Count);

            if(averageConvergence.Count > 1)
            {
                double diffPercent = (averageConvergence.Last() - averageConvergence[averageConvergence.Count - 2]) / averageConvergence[averageConvergence.Count - 2];
                averageConvergencePercent.Add(diffPercent);
                //Console.WriteLine(diffPercent);
                //Console.WriteLine(total / allVisited.Count);
            }

        }//SetConvergence

        private void WritePageRanks(String fileFolder, int nIterations)
        {
            //writes the page ranks to a file
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileFolder + "pageRank015.txt"))
            {
                //write the headers
                file.WriteLine("# iterations: " + nIterations);
                for (int i = 0; i < allVisited.Count; i++) { file.Write(allVisited[i].pageUrl + " \t "); }
                file.WriteLine();

                //write the pageranks
                for (int i = 0; i < nIterations; i++)
                {
                    for (int n = 0; n < allVisited.Count; n++)
                    {
                        file.Write(allVisited[n].pageRank[i] + " \t ");
                    }

                    file.WriteLine();

                }//each iteration
            }

        }//WritePageRanks

    }

    public class Page
    {
        public string pageUrl { get; set; } //the HTML page

        public List<String> links { get; set; } //links contained within the HTML page

        public Page(string pageUrl) 
        { 
            this.pageUrl = pageUrl; 
            links = new List<string>();
            pageRank = new List<double>();
            pageRank.Add(1);           
        }

        public MatrixRow ReturnMatrixRow(List<String> outlinks)
        {
            //takes a list of outlinks, converts them into a list of MatrixColumns, marking whether this page has those links
            MatrixRow row = new MatrixRow();
            row.link = pageUrl;

            for (int i = 0; i < outlinks.Count; i++)
            {
                MatrixCell cell = new MatrixCell();
                cell.link = outlinks[i];
                cell.isLinked = HasLink(outlinks[i]);
                row.cells.Add(cell);
            }

            return row;

        }//ReturnMatrixColumn

        public Boolean HasLink(String inputLink)
        {
            //whether this Page has a particular link
            return (from link in links
                    where link == inputLink
                    select link).Any();
        }

        public List<double> pageRank { get; set; } //numeric value of the page rank, initially set to 1

        public Double convergence
        {   //returns the difference between the last 2 pageranks
            get
            {
                if (pageRank.Count < 2) { throw new Exception("Convergence cannot be returned, not enough pageranks"); }
                return pageRank[pageRank.Count - 1] - pageRank[pageRank.Count - 2];
            }
        }

    }
}


