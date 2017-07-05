using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace imdb.HtmlParser
{
    public class TitleDetail
    {
        public string TrailerURl { get; set; }
        public string Poster { get; set; }
        public string Summary { get; set; }
        public string Rate { get; set; }
        public string RateCount { get; set; }
        public string Title { get; set; }
        public string Time { get; set; }
        public string Categories { get; set; }
        public string Directors { get; set; }
        public string Writers { get; set; }
        public string Stars { get; set; }
        public string Year { get; set; }
        public List<ReleaseDates> ReleaseDate { get; set; }
    }

    public class ReleaseDates
    {
        public string Country { get; set; }
        public string Date { get; set; }
    }


}
