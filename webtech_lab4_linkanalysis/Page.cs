using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webtech_lab4_linkanalysis
{
    public class Pages
    {
        public List<Page> list = new List<Page>();

        public List<String> allLinks; //every, unique, link in the pages list

        public Matrix matrix = new Matrix();
        public void PopulateMatrix()
        {
            //populates the matrix. Gets a list of all the unique inlinks. Each outlink object (CLASS) is then sent this list
            //the class converts this list into a matrix row

            matrix = new Matrix();
            PopulateAllLinks();

            for (int i = 0; i < list.Count; i++)
            {
                matrix.rows.Add(list[i].ReturnMatrixRow(allLinks));
            }

        }//PopulateMatrix

        private void PopulateAllLinks()
        {
            allLinks = new List<string>();

            for (int i = 0; i < list.Count; i++)
            {
                for (int n = 0; n < list[i].links.Count; n++)
                {
                    if (!allLinks.Exists(el => el == list[i].links[n])) { allLinks.Add(list[i].links[n]); }
                }
            }

        }//PopulateAllLinks

    }

    public class Page
    {
        public string pageUrl { get; set; } //the HTML page

        public List<String> links { get; set; } //links contained within the HTML page

        public Page(string pageUrl) { this.pageUrl = pageUrl; links = new List<string>(); }

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

        private Boolean HasLink(String inputLink)
        {
            //whether this Page has a particular link
            return (from link in links
                    where link == inputLink
                    select link).Any();
        }

    }
}


