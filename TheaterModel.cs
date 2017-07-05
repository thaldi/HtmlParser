using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace imdb.HtmlParser
{
    public class TheaterModel
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Rate { get; set; }
        public string Description { get; set; }
        public string Director { get; set; }
        public string Stars { get; set; }
        public string TrailerLink { get; set; }
        public string IMGUrl { get; set; }
        public string Categories { get; set; }
        public string Time { get; set; }

    }
}
