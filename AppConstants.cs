using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace imdb.HtmlParser
{
    public class AppConstants
    {
        public static string ComingSoonLink
        {
            get
            {
                return ConfigurationManager.AppSettings["comingSoonLink"];
            }
        }

        public static string BoxOfficeLink
        {
            get
            {
                return ConfigurationManager.AppSettings["boxOfficeLink"];
            }
        }

        public static string InThreatesLink
        {
            get
            {
                return ConfigurationManager.AppSettings["inThreaterLink"];
            }
        }
    }
}
