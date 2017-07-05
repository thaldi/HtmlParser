using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace imdb.HtmlParser
{
    public class BaseModel
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Gross { get; set; }
        public string Week { get; set; }
        public string TitlePosterUrl { get; set; }
    }
}
