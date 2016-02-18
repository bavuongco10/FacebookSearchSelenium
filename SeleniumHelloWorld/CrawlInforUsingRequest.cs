using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SeleniumHelloWorld
{
    internal class CrawlInforUsingRequest
    {
        private const int RANGE = 50;
        private List<People_test> persons = new List<People_test>(); 
        private  List<string> errorurls = new List<string>();
        private  int lastIndex = 0;
        public TestBD1Entities db = new TestBD1Entities();
        private  PageLanguage pageLanguage;
        private  readonly string cookiesFilePath = @"D:\cookies.txt";
        private List<string> list = new List<string>(); 

        private readonly string[] _invalidTitle =
        {
            "Facebook",
            "Page Not Found",
            "Không tìm thấy trang"
        };

        public CrawlInforUsingRequest()
        {
                InitCrawler();

        }

        private void InitCrawler()
        {
            var commentOwners = db.Comments.Select(o => o.CommentOwner).Distinct().Take(100).ToList();
            //var commentOwners = new List<string>() { "albert.ortega.904","Yusaku.Pham", "bui.thanh.ba.vuong", "100002917108835" };

            /// Thread compare record 3 times
            /// record          :10
            /// 1 thread        :18685--14475-13017
            /// Multi-thread    :5159--5455--5955
            /// Thread compare record 3 times
            /// record          :100
            /// 1 thread        :188205
            /// Multi-thread    :77220

            // 1 thread
            //var watch = Stopwatch.StartNew();
            //foreach (var commentOwner in commentOwners)
            //{
            //    string commentOwner;
            //    var commentOwnerUrl = GetCommentOwnerUrl(commentOwner, out commentOwner);

            //    CrawlInformation(commentOwnerUrl, commentOwner);
            //}
            //watch.Stop();
            //Console.WriteLine("<<<<<{0}>>>>>", watch.ElapsedMilliseconds);
            //db.SaveChanges();

            //Multi-thread
            var watch = Stopwatch.StartNew();
            Parallel.ForEach(
                commentOwners,
                //new ParallelOptions {MaxDegreeOfParallelism = 4},
                commentOwner =>
                {
                    var commentOwnerUrl = GetCommentOwnerUrl(commentOwner);

                    CrawlInformation(commentOwnerUrl, commentOwner);
                }
                );
            watch.Stop();
            db.People_test.AddRange(persons.GetRange(lastIndex, persons.Count-lastIndex));
            db.SaveChanges();

            var results = commentOwners.Except(list).ToList();

            Console.WriteLine("<<<<<{0}>>>>>", watch.ElapsedMilliseconds);
        }

        private string GetCommentOwnerUrl(string commentOwner)
        {
            string commentOwnerUrl;
            commentOwnerUrl = "http://mbasic.facebook.com/";
            var owner = commentOwner;
            if (owner.Contains("?fref=nf"))
            {
                owner = owner.Remove(owner.IndexOf("?fref=nf"));
            }
            else if (owner.Contains("?refid="))
            {
                owner = owner.Remove(owner.IndexOf("?refid="));
            }

            if (owner.Contains("profile.php?id=") || IsNumber(owner))
            {
                owner = owner.Replace("profile.php?id=", "");
                //https://m.facebook.com/profile.php?v=info&id=100002917108835
                commentOwnerUrl = commentOwnerUrl + "profile.php?v=info&id=" + owner;
            }
            else
            {
                //https://mbasic.facebook.com/bui.thanh.ba.vuong/about?refid=17
                commentOwnerUrl = commentOwnerUrl + owner + "/about";
            }
            return commentOwnerUrl;
        }

        private  bool IsNumber(string searchString)
        {
            return !string.IsNullOrEmpty(searchString) && searchString.All(char.IsDigit);
        }

        private  void CrawlInformation(string commentOwnerUrl, string commentOwner)
        {
            var cookieCollection = GetCookies(cookiesFilePath);
            var html = GetHtml(commentOwnerUrl, cookieCollection);
            if (html != null)
            {
                //WriteToTextFile(html);
                var document = ParseDocumentToXpath(html);
                HandleHtml(document, commentOwner);
            }
        }

        private  void WriteToTextFile(string html)
        {
            using (var writer = new StreamWriter(string.Format(@"d:\test\html{0}.html", DateTime.Now.ToFileTime())))
            {
                writer.Write(html);
            }
        }

        private  void HandleHtml(HtmlDocument document, string commentOwner)
        {
            // Get page Language 
            //Better already set facebook language to english do easy dealing with
            pageLanguage = GetPageLanguage(document);

            //check if profile exist
            var node = document.DocumentNode.SelectSingleNode("//title").InnerHtml;
            if (!_invalidTitle.Contains(node))
            {
                GetElements(document, commentOwner);
            }
        }

        private  HtmlDocument ParseDocumentToXpath(string html)
        {
            var document = new HtmlDocument();
            document.LoadHtml(html);
            return document;
        }

        private  void GetElements(HtmlDocument document, string commentOwner)
        {
            list.Add(commentOwner);
            var person = new People_test() ;
            person.CommentOwner = commentOwner ;
            var facebook = GetFacebookLinkAndID(document, commentOwner);
            person.FacebookName = facebook[0];
            person.FacebookId = facebook[1];
            person.Skills = GetContactAndBasicInfor(document, KeysElement.Instance.Skills[pageLanguage])[0];
            person.Phone = GetContactAndBasicInfor(document, KeysElement.Instance.Mobile[pageLanguage])[0];
            person.AddressName = GetContactAndBasicInfor(document, KeysElement.Instance.Address[pageLanguage])[0];
            person.AddressMapLink = GetContactAndBasicInfor(document, KeysElement.Instance.Address[pageLanguage])[1];
            person.Screennames = GetContactAndBasicInfor(document, KeysElement.Instance.Screennames[pageLanguage])[0];
            person.Website = GetContactAndBasicInfor(document, KeysElement.Instance.Website[pageLanguage])[0];
            person.Email = GetContactAndBasicInfor(document, KeysElement.Instance.Email[pageLanguage])[0];
            person.Birthday = GetContactAndBasicInfor(document, KeysElement.Instance.Birthday[pageLanguage])[0];
            person.Gender = GetContactAndBasicInfor(document, KeysElement.Instance.Gender[pageLanguage])[0];
            person.InterestedIn = GetContactAndBasicInfor(document, KeysElement.Instance.Interested[pageLanguage])[0];
            person.Languages = GetContactAndBasicInfor(document, KeysElement.Instance.Languages[pageLanguage])[0];
            person.ReligousViews = GetContactAndBasicInfor(document, KeysElement.Instance.Religious[pageLanguage])[0];
            person.ReligousViewsLink = GetContactAndBasicInfor(document, KeysElement.Instance.Religious[pageLanguage])[1];
            person.PoliticalViews = GetContactAndBasicInfor(document, KeysElement.Instance.Political[pageLanguage])[0];
            person.PoliticalViewsLink = GetContactAndBasicInfor(document, KeysElement.Instance.Political[pageLanguage])[1];
            person.Relationship = GetContactAndBasicInfor(document, KeysElement.Instance.Relationship[pageLanguage])[0];

            persons.Add(person);
            //db.People_test.Add(person);

            var works = GetEducationAndWork(document, ExperienceType.Work);
            //db.PeopleWorkEdus.AddRange(CreatObjectWorkEdu(works, person.FacebookId, ExperienceType.Work.ToString()));
            var educations = GetEducationAndWork(document, ExperienceType.Education);
            //db.PeopleWorkEdus.AddRange(CreatObjectWorkEdu(educations, person.FacebookId, ExperienceType.Education.ToString()));

            var currentCity = GetContactAndBasicInfor(document, KeysElement.Instance.CurrentCity[pageLanguage]);
            if (currentCity[0] != null)
            {
                db.PeoplePlaces.Add(new PeoplePlace
                {
                    FacebookID = person.FacebookId,
                    Name = currentCity[0],
                    Url = currentCity[1],
                    Role = KeysElement.Instance.CurrentCity[pageLanguage].ToString()
                });
            }
            var homeTown = GetContactAndBasicInfor(document, KeysElement.Instance.Hometown[pageLanguage].ToString());
            if (homeTown[0] != null)
            {
                db.PeoplePlaces.Add(new PeoplePlace
                {
                    FacebookID = person.FacebookId,
                    Name = homeTown[0],
                    Url = homeTown[1],
                    Role = KeysElement.Instance.Hometown[pageLanguage].ToString()
                });
            }


            //db.SaveChanges();
            //if (persons.Count % RANGE == 0)
            //{
            //    db.People_test.AddRange(persons.GetRange(lastIndex, lastIndex+RANGE));
            //    lastIndex = lastIndex+RANGE;
            //    db.SaveChanges();
            //    Console.WriteLine("<<<<<<<<<{0}>>>>>>>>>>", "Save");
            //}
            Console.WriteLine("---------{0}-------", persons.Count);
        }

        private  List<PeopleWorkEdu> CreatObjectWorkEdu(List<List<string>> works, string id, string type)
        {
            var workList = new List<PeopleWorkEdu>();
            foreach (var work in works)
            {
                if (work[0] != null)
                {
                    var peopleWork = new PeopleWorkEdu();
                    peopleWork.FacebookID = id;
                    peopleWork.Link = work[0];
                    peopleWork.Level = work[1];
                    peopleWork.DateTime = work[2];
                    peopleWork.Role = type;
                    workList.Add(peopleWork);
                }
            }
            return workList;
        }

        private  List<string> GetFacebookLinkAndID(HtmlDocument document, string owner)
        {
            var parts = new List<string>();
            var name = owner;
            string url = null;
            if (name.Contains("profile.php?id="))
            {
                url = name.Replace("profile.php?id=", "");
                name = string.Empty;
            }
            else
            {
                var nodes = document.DocumentNode.SelectNodes("//a[@href]");
                foreach (var node in nodes)
                {
                    var link = node.Attributes["href"].Value;
                    if (link.Contains("/photo.php?fbid") || link.Contains("profile_id="))
                    {
                        url = WebUtility.HtmlDecode(link);
                        break;
                    }
                }
                if (url.IndexOf("&id=") != -1)
                {
                    url = url.Substring(url.IndexOf("&id=") + 4, 30);
                }
                else
                {
                    //url = url.Substring(url.IndexOf("_id=") + 4, 30);        
                    url = Regex.Match(url, @"\d+").Value;
                }
                if (url.IndexOf("&set=") != -1)
                {
                    url = url.Substring(0, url.IndexOf("&set="));
                }
            }
            parts.Add(name);
            parts.Add(url);
            return parts;
        }

        private  List<string> GetContactAndBasicInfor(HtmlDocument document, object type)
        {
            var info = new List<string>();
            string name = null;
            string url = null;
            var driverString = string.Format("//*[starts-with(@title,'{0}')]/table/tr/td[2]/div", type);
            if (type == KeysElement.Instance.Mobile[pageLanguage])
            {
                driverString += "/span/span";
            }
            else if (type == KeysElement.Instance.Address[pageLanguage] || type == KeysElement.Instance.Religious[pageLanguage] ||
                     type == KeysElement.Instance.Political[pageLanguage] || type == KeysElement.Instance.CurrentCity[pageLanguage] ||
                     type == KeysElement.Instance.Hometown[pageLanguage] || type == KeysElement.Instance.Email[pageLanguage])
            {
                driverString += "/a";
            }
            else if (type == KeysElement.Instance.Skills[pageLanguage])
            {
                driverString = string.Format("//*[@id='{0}']/div/div[2]/div/span", type.ToString().ToLower());
            }
            else if (type == KeysElement.Instance.Relationship[pageLanguage])
            {
                driverString = string.Format("//*[@id='{0}']/div/div[2]/div/div/div", type.ToString().ToLower());
            }
            var drivers = document.DocumentNode.SelectNodes(driverString);
            var nodesCount = drivers == null ? 0 : drivers.Count;
            if (nodesCount > 1)
            {
                foreach (var item in drivers)
                {
                    name += item.InnerHtml + "-";
                }
            }
            else if (nodesCount == 1)
            {
                name = drivers[0].InnerHtml;
                url = drivers[0].Attributes["href"] != null ? drivers[0].Attributes["href"].Value : null;
            }

            info.Add(name);
            info.Add(url);
            return info;
        }

        private  List<List<string>> GetEducationAndWork(HtmlDocument document, ExperienceType type)
        {
            var partsOfParts = new List<List<string>>();
            var educationDrivers =
                document.DocumentNode.SelectNodes(string.Format("//*[@id='{0}']/div/div/div/div",
                    type.ToString().ToLower()));
            if (educationDrivers == null)
            {
                return partsOfParts;
            }
            foreach (var educationDriver in educationDrivers)
            {
                // select in this node
                var tiles = educationDriver.SelectNodes("div")[0];
                string educationUrl = null;
                string educationLevel = null;
                string educationTime = null;
                var tileCount = tiles.ChildNodes.Count;
                if (tileCount > 0)
                {
                    var node = tiles.SelectSingleNode(".//a").Attributes["href"];
                    educationUrl = node != null ? node.Value.Replace("/", "") : null;
                }
                if (type == ExperienceType.Education)
                {
                    if (tileCount > 1)
                    {
                        educationLevel = tiles.ChildNodes[1].InnerText;
                    }
                    if (tileCount > 2)
                    {
                        educationTime = tiles.ChildNodes[2].InnerText;
                    }
                }
                var parts = new List<string> { educationUrl, educationLevel, educationTime };
                partsOfParts.Add(parts);
            }
            return partsOfParts;
        }

        private  PageLanguage GetPageLanguage(HtmlDocument document)
        {
            var language = PageLanguage.vi;
            var nodes = document.DocumentNode.SelectNodes("//*[@id='viewport']/div[3]/div[1]/div/table/tbody/tr/td[1]/b");
            switch (nodes[0].InnerHtml)
            {
                case "Tiếng Việt":
                    language = PageLanguage.vi;
                    break;
                case "English (US)":
                    language = PageLanguage.en;
                    break;
            }
            return language;
        }

        private  string GetHtml(string url, CookieCollection cookieCollection)
        {
            string html = null;
            try
            {
                const string USER_AGENT = "Mozilla/2.0 (Windows NT 6.1; WOW64;) Gecko/20100101 Firefox/11.0";
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookieCollection);
                request.UserAgent = USER_AGENT;
                request.KeepAlive = false;
                request.AllowAutoRedirect = true;
                request.Timeout = 45000;

                var response = (HttpWebResponse)request.GetResponse();
                var streamReader = new StreamReader(response.GetResponseStream());
                html = streamReader.ReadToEnd();
            }
            catch (Exception)
            {
                errorurls.Add(url);
            }
            return html;
        }

        public CookieCollection GetCookies(string cookiesFilePath)
        {
            var cookieCollection = new CookieCollection();
            var reader = new StreamReader(cookiesFilePath);
            var lines = int.Parse(reader.ReadLine() ?? "0");
            for (var i = 0; i < lines; i++)
            {
                var cookieString = reader.ReadLine().Split('=');
                cookieCollection.Add(new Cookie(cookieString[0], cookieString[1]) { Domain = "facebook.com", Path = "/" });
                cookieCollection.Add(new Cookie(cookieString[0], cookieString[1])
                {
                    Domain = "touch.facebook.com",
                    Path = "/"
                });
                cookieCollection.Add(new Cookie(cookieString[0], cookieString[1])
                {
                    Domain = "www.facebook.com",
                    Path = "/"
                });
                cookieCollection.Add(new Cookie(cookieString[0], cookieString[1])
                {
                    Domain = "mbasic.facebook.com",
                    Path = "/"
                });
            }
            return cookieCollection;
        }
    }
}
