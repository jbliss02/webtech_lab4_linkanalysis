using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webtech_lab4_linkanalysis
{
    public class Matrix
    {
        public List<MatrixRow> rows = new List<MatrixRow>();

        private string fileFolder;
        private int totalInLinks; //set by question A
        private List<int> numberOfInlinks; //set by question A, totals by page

        private int totalOutLinks; //set by question B
        private List<int> numberOfOutlinks; //set by question B, totals by inlink
       
        public void DoQuestions(String fileFolder)
        {
            this.fileFolder = fileFolder;
            DoQuestionA();
            DoQuestionB();
            DoQuestionC();
            DoQuestionD();
            DoQuestionE();
        }
        
        public void DoQuestionA()
        {
            //The number of inlinks to each page
            totalInLinks = 0;
            numberOfInlinks = new List<int>();

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileFolder + "2a.txt"))
            {
                for (int i = 0; i < rows[0].cells.Count; i++) //start with the cells, all matrix rows will have same number of cells
                {
                    int count = 0;
                    for (int n = 0; n < rows.Count; n++)
                    {
                        if (rows[n].cells[i].isLinked) { count++; totalInLinks++; }
                    }

                    file.WriteLine(rows[0].cells[i].link + " - has " + count + " inlink(s)");
                    numberOfInlinks.Add(count);
                }
             
            }//using the file

        }//DoQuestionB

        public void DoQuestionB()
        {
            //The number of outlinks to each page
            totalOutLinks = 0;
            numberOfOutlinks = new List<int>();

            //add the numbers and allocated to rows[i].count
            for (int i = 0; i < rows.Count; i++)
            {
                for (int n = 0; n < rows[i].cells.Count; n++)
                {
                    if (rows[i].cells[n].isLinked) { rows[i].count++; totalOutLinks++; }
                }
            }

            //write to file
            using (System.IO.StreamWriter file =new System.IO.StreamWriter(fileFolder + "2b.txt"))
            {
                for (int i = 0; i < rows.Count; i++)
                {
                    file.WriteLine(rows[i].link + " - has " + rows[i].count + " outlinks");
                    numberOfOutlinks.Add(rows[i].count);
                }


            }//using the text file

        }//DoQuestionB

        public void DoQuestionC()
        {
            //average (mean) number of inlinks to a page, variance and the standard deviation
            //relies on question A being run as that sets totalInLinks

            int average = totalInLinks / rows.Count;

            //To calculate the Variance, take each difference, square it, and then average the result:

            //work out the difference for each inlink
            List<double> squaredDifference = new List<double>();

            for (int i = 0; i < rows[0].cells.Count; i++) //start with the cells, all matrix rows will have same number of cells
            {
                int count = 0;
                for (int n = 0; n < rows.Count; n++)
                {
                    if (rows[n].cells[i].isLinked) { count++; }
                }//each row

                int difference = count - average;
                squaredDifference.Add(Math.Pow((double)difference, 2)); //add the squared variance to the list
            }

            //work out the variance (the average of the squared difference)
            double variance = squaredDifference.Average();

            //work out the standard deviation (the square root of Variance)
            double standardDeviation = Math.Sqrt(variance);

            //write to a file
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileFolder + "2c.txt"))
            {
                file.WriteLine("Average is " + average);
                file.WriteLine("Variance is " + variance);
                file.WriteLine("Standard Deviation is " + standardDeviation);
            }//using the file

        }//DoQuestionC

        public void DoQuestionD()
        {
            //average (mean) number of outlinks from a page, variance and the standard deviation.
            //relies on question B being run as that sets totalInLinks

            int average = totalOutLinks / rows[0].cells.Count;

            //work out the difference for each outlink
            List<double> squaredDifference = new List<double>();

            for(int i = 0; i < rows.Count; i++)
            {
                int difference = rows[i].count - average;
                squaredDifference.Add(Math.Pow((double)difference, 2));
            }

            //work out the variance (the average of the squared difference)
            double variance = squaredDifference.Average();

            //work out the standard deviation (the square root of Variance)
            double standardDeviation = Math.Sqrt(variance);

            //write to a file
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileFolder + "2d.txt"))
            {
                file.WriteLine("Average is " + average);
                file.WriteLine("Variance is " + variance);
                file.WriteLine("Standard Deviation is " + standardDeviation);
            }//using the file

        }//DoQuestionD

        public void DoQuestionE()
        {
            //The degree distribution of inlinks and outlinks

            //outlinks
            List<DistributionRow> distribution_outlinks = new List<DistributionRow>(); //the distribution list
            DistributionRow row = null; //row that is inserted into the list
            numberOfOutlinks.Sort(); //sort the list and then iterate through

            for(int i = 0; i < numberOfOutlinks.Count; i++)
            {
                if (i == 0 || row.numberLinks != numberOfOutlinks[i])//start a new row
                {
                    if (row != null) { distribution_outlinks.Add(row); }
                    row = new DistributionRow(numberOfOutlinks[i]);
                }

                row.numberPages++;

            }//each numberOfOutlinks

            //write to a file
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileFolder + "2e_outlinks.txt"))
            {
                file.WriteLine("# outlinks \t # pages");
                for(int i = 0; i < distribution_outlinks.Count; i++)
                {
                    file.WriteLine(distribution_outlinks[i].numberLinks + " \t " + distribution_outlinks[i].numberPages);
                }

            }

            //inlinks
            List<DistributionRow> distribution_inlinks = new List<DistributionRow>(); //the distribution list
            row = null; //row that is inserted into the list
            numberOfInlinks.Sort(); //sort the list and then iterate through

            for (int i = 0; i < numberOfInlinks.Count; i++)
            {
                if (i == 0 || row.numberLinks != numberOfInlinks[i])//start a new row
                {
                    if (row != null) { distribution_inlinks.Add(row); }
                    row = new DistributionRow(numberOfInlinks[i]);
                }

                row.numberPages++;

            }//each numberOfOutlinks

            //write to a file
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileFolder + "2e_inlinks.txt"))
            {
                file.WriteLine("# inlinks \t # pages");
                for (int i = 0; i < distribution_inlinks.Count; i++)
                {
                    file.WriteLine(distribution_inlinks[i].numberLinks + " \t " + distribution_inlinks[i].numberPages);
                }

            }

        }//DoQuestionE

        private class DistributionRow
        {
            public int numberLinks { get; set; }
            public int numberPages { get; set; }
            public DistributionRow(int numberLinks) { this.numberLinks = numberLinks; numberPages = 0; }
        }

    }//public class Matrix

    public class MatrixRow
    {
        public string link { get; set; }
        public List<MatrixCell> cells = new List<MatrixCell>();
        public int count { get; set; }
    }

    public class MatrixCell
    {
        public string link { get; set; }
        public bool isLinked { get; set; }
    }
}

