using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace imdb.HtmlParser
{
    class Program
    {
        static void Main(string[] args)
        {
            var helper = new HtmlHelper();

            //var d = helper.GetInThreaters();
            //var dd = helper.GetBoxOffice();

            var ddd = helper.GetTitleDetail("");

            Console.ReadKey();
        }
    }
}
