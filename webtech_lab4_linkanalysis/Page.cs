using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webtech_lab4_linkanalysis
{
    public class Page
    {
        public string pageUrl { get; set; } //the HTML page
        
        public List<String> links { get; set; } //links contained within the HTML page

        public Page(string pageUrl) { this.pageUrl = pageUrl; links = new List<string>(); }
    }
}
