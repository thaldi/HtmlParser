using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;


namespace imdb.HtmlParser
{
    public class HtmlHelper
    {
        private HtmlDocument HtmlDocument { get; set; }
        private HtmlWeb HWeb { get; set; }

        public HtmlHelper()
        {
            HtmlDocument = new HtmlDocument();
            HWeb = new HtmlWeb();
        }

        //geting box office
        public List<BaseModel> GetBoxOffice()
        {
            try
            {
                var list = new List<BaseModel>();
                BaseModel newItem = null;

                HtmlDocument = HWeb.Load("http://www.imdb.com/chart/boxoffice");

                var data = from i in HtmlDocument.DocumentNode.Descendants("table")
                           from j in i.Descendants("tbody")
                           select j.Descendants("tr");

                foreach (var nodes in data)
                {
                    foreach (var node in nodes.Where(x => x.Name == "tr"))
                    {
                        var tempDoc = TempLoader(node.InnerHtml);

                        var d = from i in tempDoc.DocumentNode.Descendants("td")
                                select i;

                        newItem = new BaseModel();

                        foreach (var td in d)
                        {
                            if (td.Attributes["class"].Value == "posterColumn")
                            {
                                var temp = TempLoader(td.InnerHtml);

                                var poster = from i in temp.DocumentNode.Descendants("a")
                                             from j in i.Descendants("img")
                                             select j.Attributes["src"].Value;

                                newItem.TitlePosterUrl = StringParser(poster.FirstOrDefault());
                            }
                            if (td.Attributes["class"].Value == "titleColumn")
                                newItem.Title = StringParser(td.InnerText);
                            if (td.Attributes["class"].Value == "ratingColumn")
                                newItem.Gross = StringParser(td.InnerText);
                            if (td.Attributes["class"].Value == "weeksColumn")
                                newItem.Week = StringParser(td.InnerText);
                            if (td.Attributes["class"].Value == "watchlistColumn")
                            {
                                var temp = TempLoader(td.InnerHtml);

                                var id = from i in temp.DocumentNode.Descendants("div")
                                         select i.Attributes["data-tconst"].Value;

                                newItem.ID = StringParser(id.FirstOrDefault());
                            }
                        }
                        list.Add(newItem);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //geting coming soon
        public List<TheaterModel> GetInThreaters()
        {
            var list = new List<TheaterModel>();
            TheaterModel newItem = null;
            var tempHtml = string.Empty;
            uint count = 0;

            try
            {
                HtmlDocument = HWeb.Load("http://www.imdb.com/movies-in-theaters");

                var theaters = from i in HtmlDocument.DocumentNode.Descendants("div")
                               from j in i.Descendants("div")
                               from k in j.Descendants("div")
                               select k;

                foreach (var item in theaters)
                {
                    if (item.HasAttributes)
                    {
                        if (item.Attributes["class"] != null)
                        {
                            if (item.Attributes["class"].Value == "list detail sub-list")
                            {
                                count++;
                                if (count > 1)
                                {
                                    tempHtml = item.InnerHtml;
                                    count = 0;
                                    break;
                                }
                            }
                        }
                    }
                }

                var tempDoc = TempLoader(tempHtml);

                if (tempDoc != null)
                {
                    var tempData = from i in tempDoc.DocumentNode.Descendants("div")
                                   from j in i.Descendants("table")
                                   select j.Descendants("tbody");

                    foreach (var nodes in tempData)
                    {
                        foreach (var node in nodes)
                        {
                            var temp = TempLoader(node.InnerHtml);
                            var tds = from i in temp.DocumentNode.Descendants("td")
                                      select i;

                            newItem = new TheaterModel();

                            foreach (var item in tds)
                            {
                                if (item.Attributes["id"] != null)
                                {
                                    if (item.Attributes["id"].Value == "img_primary")
                                    {
                                        //getting poster url
                                        var imgNode = TempLoader(item.InnerHtml);
                                        var tempImg = from i in imgNode.DocumentNode.Descendants("img")
                                                      select i.Attributes["src"].Value;
                                        newItem.IMGUrl = tempImg.FirstOrDefault();

                                        //getting id
                                        var id = from i in imgNode.DocumentNode.Descendants("a")
                                                 where i.Attributes["href"] != null
                                                 select i.Attributes["href"].Value;
                                        newItem.ID = id.FirstOrDefault().Split('/').ToArray()[2];

                                    }
                                }

                                if (item.Attributes["class"] != null)
                                {
                                    if (item.Attributes["class"].Value == "overview-top")
                                    {
                                        var detailNode = TempLoader(item.InnerHtml);

                                        //getting title
                                        var tempName = from i in detailNode.DocumentNode.Descendants("h4")
                                                       from j in i.Descendants("a")
                                                       select j.InnerText;
                                        newItem.Title = StringParser(tempName.FirstOrDefault());

                                        //getting categories
                                        var tempCategory = from i in detailNode.DocumentNode.Descendants("p")
                                                           from j in i.Descendants("span")
                                                           select j;
                                        var categories = string.Empty;
                                        foreach (var c in tempCategory)
                                            categories += c.InnerText;
                                        newItem.Categories = StringParser(categories);

                                        //getting time
                                        var time = from i in detailNode.DocumentNode.Descendants("p")
                                                   from j in i.Descendants("time")
                                                   select j.InnerText;
                                        newItem.Time = time.FirstOrDefault();

                                        //getting rating
                                        var rating = from i in detailNode.DocumentNode.Descendants("div")
                                                     from k in i.Descendants("div")
                                                     from j in k.Descendants("span")
                                                     select j;
                                        foreach (var rate in rating)
                                        {
                                            if (rate.Attributes["class"] != null)
                                            {
                                                if (rate.Attributes["class"].Value.Trim() == "rating-rating")
                                                {
                                                    newItem.Rate = rate.InnerText;
                                                    break;
                                                }
                                            }
                                        }

                                        //getting story
                                        var storyNode = from i in detailNode.DocumentNode.Descendants("div")
                                                        where i.Attributes["class"] != null &&
                                                        i.Attributes["class"].Value == "outline"
                                                        select i.InnerText;
                                        newItem.Description = StringParser(storyNode.FirstOrDefault());

                                        //getting director      
                                        var directorsStr = string.Empty;
                                        var actorsStr = string.Empty;

                                        var directors = from i in detailNode.DocumentNode.Descendants("div")
                                                        from j in i.Descendants("span")
                                                        where j.Attributes["itemprop"] != null && j.Attributes["itemprop"].Value == "director"
                                                        select j.InnerText;
                                        foreach (var dd in directors)
                                            directorsStr += dd.Trim() + ",";
                                        newItem.Director = RemoveLastChar(StringParser(directorsStr));

                                        var actors = from i in detailNode.DocumentNode.Descendants("div")
                                                     from j in i.Descendants("span")
                                                     where j.Attributes["itemprop"] != null && j.Attributes["itemprop"].Value == "actors"
                                                     select j.InnerText;
                                        foreach (var aa in actors)
                                            actorsStr += aa.Trim() + ",";
                                        newItem.Stars = RemoveLastChar(StringParser(actorsStr));
                                    }

                                    //getting trailer url
                                    if (item.Attributes["class"].Value == "overview-bottom")
                                    {
                                        var trailerNode = TempLoader(item.InnerHtml);
                                        var trailerUrl = from i in trailerNode.DocumentNode.Descendants("a")
                                                         where i.Attributes["href"] != null
                                                         select i.Attributes["href"].Value;
                                        newItem.TrailerLink = "http://www.imdb.com" + trailerUrl.FirstOrDefault();
                                    }
                                }
                            }
                            list.Add(newItem);
                        }
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //geting title detail
        public TitleDetail GetTitleDetail(string id)
        {
            var detailItem = new TitleDetail();
            try
            {
                //set id into the link at calling this method
                HtmlDocument = HWeb.Load("http://www.imdb.com/title/tt3748528/"); 

                var data = from i in HtmlDocument.DocumentNode.Descendants("div")
                           select i;

                foreach (var item in data)
                {
                    if (item.HasAttributes)
                    {
                        //geting sumary
                        var summary = (from i in HtmlDocument.DocumentNode.Descendants("div")
                                       from j in i.Descendants("div")
                                       where j.Attributes["itemprop"] != null
                                       where j.Attributes["itemprop"].Value == "description"
                                       select
                                       j.InnerText).FirstOrDefault().Trim();
                        detailItem.Summary = summary;

                        //geting rate,year,rate count,categories
                        if (item.Attributes["class"] != null)
                        {
                            if (item.Attributes["class"].Value == "title_bar_wrapper")
                            {
                                var tempTitle = TempLoader(item.InnerHtml);
                                var titleData = from i in tempTitle.DocumentNode.Descendants("span")
                                                select i;
                                string categories = string.Empty;
                                foreach (var titleItem in titleData)
                                {
                                    if (!string.IsNullOrEmpty(GetAttributeValue(titleItem, "itemprop", "genre")))
                                        categories += GetAttributeValue(titleItem, "itemprop", "genre") + ",";
                                    if (!string.IsNullOrEmpty(GetAttributeValue(titleItem, "itemprop", "ratingValue")))
                                        detailItem.Rate = GetAttributeValue(titleItem, "itemprop", "ratingValue");
                                    if (!string.IsNullOrEmpty(GetAttributeValue(titleItem, "itemprop", "ratingCount")))
                                        detailItem.RateCount = GetAttributeValue(titleItem, "itemprop", "ratingCount");
                                    if (!string.IsNullOrEmpty(GetAttributeValue(titleItem, "id", "titleYear")))
                                        detailItem.Year = GetAttributeValue(titleItem, "id", "titleYear");
                                    detailItem.Categories = RemoveLastChar(categories);
                                }
                            }

                            //geting directors, writers, stars
                            if (item.Attributes["class"].Value.Trim() == "plot_summary_wrapper")
                            {
                                var temp = TempLoader(item.InnerHtml);
                                var personDetail = (from i in temp.DocumentNode.Descendants("div")
                                                    where i.HasAttributes
                                                    where i.Attributes["class"] != null
                                                    where i.Attributes["class"].Value.Trim() == "credit_summary_item"
                                                    select i).ToList();

                                if (personDetail.Count != 0)
                                {
                                    detailItem.Directors = PersonHtmlParse(personDetail[0]);
                                    detailItem.Writers = PersonHtmlParse(personDetail[1]);
                                    detailItem.Stars = PersonHtmlParse(personDetail[2]);
                                }
                            }

                            if (item.Attributes["class"].Value.Trim() == "subtext")
                            {
                                var time = (from i in TempLoader(item.InnerHtml).DocumentNode.Descendants("time")
                                            select i.InnerText).FirstOrDefault();
                                detailItem.Time = StringParser(time);
                            }
                            //break;
                        }
                    }
                }

                //geting title
                var titleStr = (from i in HtmlDocument.DocumentNode.Descendants("h1")
                                select i.InnerText).FirstOrDefault().ToString().Trim().Replace("&nbsp;", string.Empty);
                detailItem.Title = titleStr;

                //geting poster
                var imgUrl = (from i in HtmlDocument.DocumentNode.Descendants("div")
                              where i.Attributes["class"] != null
                              where i.Attributes["class"].Value == "poster"
                              from j in i.Descendants("img")
                              where j.Attributes["src"] != null
                              select j.Attributes["src"].Value).FirstOrDefault().ToString();
                detailItem.Poster = imgUrl;

                //geting release deta
                detailItem.ReleaseDate = GetTitleReleaseDates(id);

                //todo:geting trailer url
                var trailerUrl= (from i in HtmlDocument.DocumentNode.Descendants("div")
                                 where i.Attributes["class"] != null
                                 where i.Attributes["class"].Value == "slate"
                                 from j in i.Descendants("a")
                                 where j.Attributes["href"] != null
                                 select j.Attributes["href"].Value);


                return detailItem;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //getting specific title release dates of some countries
        public List<ReleaseDates> GetTitleReleaseDates(string id)
        {
            try
            {
                var dateList = new List<ReleaseDates>();

                HtmlDocument = HWeb.Load("http://www.imdb.com/title/tt3748528/releaseinfo");

                var data = from i in HtmlDocument.DocumentNode.Descendants("table")
                           where i.Attributes["id"] != null
                           where i.Attributes["id"].Value.Trim() == "release_dates"
                           from j in i.Descendants("tr")
                           select j;

                foreach (var item in data)
                {
                    var temp = TempLoader(item.InnerHtml);
                    var tempList = (from i in temp.DocumentNode.Descendants("td")
                                    select i).ToList();

                    dateList.Add(GetTdValue(tempList));
                }
                return dateList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //helper methods
        private HtmlDocument TempLoader(string html)
        {
            try
            {
                if (!string.IsNullOrEmpty(html))
                {
                    var tempDoc = new HtmlDocument();
                    tempDoc.LoadHtml(html);
                    return tempDoc;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private string StringParser(string value)
        {
            try
            {
                if (!string.IsNullOrEmpty(value))
                    value = value.Replace("/", string.Empty).Replace('\n', ' ').TrimEnd().TrimStart().Trim();

                return value;
            }
            catch (Exception ex)
            {
                return value;
            }
        }

        private string RemoveLastChar(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return value.Trim().Remove(value.Length - 1, 1);
            }
            return value;
        }

        //html helper mothods
        private string GetAttributeValue(HtmlNode node, string key, string value)
        {
            if (node != null)
                if (node.Attributes[key] != null)
                    if (node.Attributes[key].Value.Trim() == value)
                        return node.InnerText;
            return string.Empty;
        }

        private ReleaseDates GetTdValue(List<HtmlNode> nodes)
        {
            var dates = new ReleaseDates();

            dates.Country = nodes.FirstOrDefault().InnerText;
            dates.Date = nodes[1].InnerText;

            if (nodes.Count > 2)
                dates.Date += " " + StringParser(nodes[2].InnerText);

            return dates;
        }

        private string PersonHtmlParse(HtmlNode valuesItem)
        {
            string values = string.Empty;
            var list = from i in TempLoader(valuesItem.InnerHtml).DocumentNode.Descendants("span")
                       select i.InnerText;
            foreach (var d in list)
                if (!d.Contains("\n"))
                    values += d + ",";
            return RemoveLastChar(RemoveLastChar(values).Replace('|', ' ').Trim());
        }

        private string GetTitleTrailerUrl(string id,string videoId)
        {
            try
            {
                //set id and videoID into below url at calling this method
                HtmlDocument = HWeb.Load("http://www.imdb.com/title/tt3748528/videoplayer/vi1942205977");

                var url = (from i in HtmlDocument.DocumentNode.Descendants("iframe")
                           where i.HasAttributes
                           where i.Attributes["src"] != null
                           select i.Attributes["src"].Value).FirstOrDefault();
                return url;
            }
            catch
            {
                return string.Empty;
            }
        }

    }
}
