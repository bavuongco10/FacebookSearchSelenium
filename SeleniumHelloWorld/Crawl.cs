using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ClosedXML.Excel;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using System.Web;

// Get shares
//https://touch.facebook.com/browse/shares/?id=10152751123721371

namespace SeleniumHelloWorld
{
    internal class Crawl
    {
        public TestBD1Entities db = new TestBD1Entities();
        public Resources Resources = new Resources();
        public IWebDriver DriverSearch;
        public IWebDriver DriverPost;
        public IWebDriver DriverReply;

        private string SearchString
        {
            get
            {
                return EncodeSearchString("halong bay");
                
            }
        }
        public List<string> listUrl = new List<string>();  

        public void Main()
        {
            InitDriver();

            Navigate(DriverSearch, DriverType.Search);
            CrawlPostsFromSrearch(DriverSearch);
        }

        private void InitDriver()
        {
            // Driver for search page
            DriverSearch = new ChromeDriver(DriverService(), ChromeOptions());
            Login(DriverSearch);

            // Driver for post page
            DriverPost = new ChromeDriver(DriverService(), ChromeOptions());
            Login(DriverPost);

            //Driver for reply
            DriverReply = new ChromeDriver(DriverService(),ChromeOptions());
            Login(DriverReply);
        }

        private void CrawlPostsFromSrearch(IWebDriver driver)
        {
            

            int pastPostsLength = 0;
            while (true)
            {
                var postsWebdriver = driver.FindElements(By.ClassName("userContentWrapper"));
                //var postUrls = driver.FindElements(By.XPath("//*[@data-comment-prelude-ref='action_link_bling']"));
                for (int i = pastPostsLength; i < postsWebdriver.Count; i++)
                {
                    try
                    {


                        Post postModel = new Post();
                        var post = postsWebdriver[i];
                        string postUrl;

                        postUrl = post.FindElement(By.ClassName("_5pcq")).GetAttribute("href");

                        var dateTime = post.FindElement(By.ClassName("_5ptz")).GetAttribute("data-utime");
                        var postOwner =
                            post.FindElement(By.ClassName("fwb")).FindElements(By.TagName("a"))[1].GetAttribute("href");
                        postUrl = postUrl.Replace("www.facebook.com", "touch.facebook.com");
                        var postContent = post.FindElement(By.ClassName("userContent")).Text;

                        postModel.PostOwner = postOwner;
                        postModel.PostDateTime = dateTime;
                        postModel.PostContent = postContent;
                        postModel.PostLink = postUrl;
                        postModel.SearchString= SearchString;
                        db.Posts.Add(postModel);
                        db.SaveChanges();

                        CrawlCommentFromPost(DriverPost, postUrl);

                    }
                    catch (Exception)
                    {
                        
                    }
                }
                pastPostsLength = postsWebdriver.Count;
                JavaScriptController(driver, JavaScriptType.ScrollBy);
            }
        }


        private void Navigate(IWebDriver driver,DriverType driverType,string url=null)
        {
            switch (driverType)
            {
                case DriverType.Search:
                    driver.Navigate().GoToUrl("https://www.facebook.com/search/top/?q=" + SearchString);
                    break;
                case DriverType.Post:
                    driver.Navigate().GoToUrl(url);
                    break;
            }
        }

        private string EncodeSearchString(string searchString=null)
        {
            // TODO :  halong bay --> halong%20bay
            return HttpUtility.UrlEncode(searchString);
            //return "halong%20bay";
        }

        public void CrawlCommentFromPost(IWebDriver driver, string postUrl)
        {
            
            int lastCommentObjectsLength = 0;
            Navigate(driver, DriverType.Post, postUrl);
            while (true)
            {
                var commentObjects = driver.FindElements(By.ClassName("_2pis"));
                if (commentObjects.Count == lastCommentObjectsLength)
                {
                    break;
                }
                for (int i = lastCommentObjectsLength; i < commentObjects.Count; i++)
                {
                    Comment comment = new Comment();
                    var commentObject = commentObjects[i];
                    try
                    {
                        var commentOwner =
                            commentObject.FindElement(By.ClassName("_1s79"))
                                .GetAttribute("href")
                                .Replace("https://touch.facebook.com/", "");
                        var commentWebDriver = commentObject.FindElements(By.TagName("div"));
                        var commentContent = commentWebDriver[0].Text;
                        var commentDataTime =
                            commentWebDriver[1].FindElement(By.TagName("abbr"))
                                .GetAttribute("data-store")
                                .Split(',')[0].Split(':')[1];
                        comment.CommentOwner = commentOwner;
                        comment.CommentContent = commentContent;
                        comment.CommentDateTime = commentDataTime;
                        comment.PostLink = postUrl;

                        db.Comments.Add(comment);
                        //db.SaveChanges();

                        string replyUrl;
                        if (commentWebDriver.Count > 2)
                        {
                            replyUrl = commentWebDriver[2].FindElement(By.TagName("a")).GetAttribute("href");
                            //Crawl comment from reply
                            CrawCommentFromReply(DriverReply, replyUrl);
                        }
                        ClickNextCommentButton(driver);

                        
                    }
                    catch (Exception)
                    {

                    }

                }
                db.SaveChanges();
                
                lastCommentObjectsLength = commentObjects.Count;
            }
        }

        private void CrawCommentFromReply(IWebDriver driver, string replyUrl)
        {
            int lastCommentObjectsLength = 0;
            
            Navigate(driver, DriverType.Post, replyUrl);

            while (true)
            {
                
                var commentObjects = driver.FindElements(By.ClassName("_2pis"));
                if (commentObjects.Count == lastCommentObjectsLength)
                {
                    break;
                }
                for (int i = lastCommentObjectsLength; i < commentObjects.Count; i++)
                {
                    Comment comment = new Comment();
                    var commentObject = commentObjects[i];
                    try
                    {
                        var commentOwner =
                            commentObject.FindElement(By.ClassName("_1s79"))
                                .GetAttribute("href")
                                .Replace("https://touch.facebook.com/", "");
                        var commentWebDriver = commentObject.FindElements(By.TagName("div"));
                        var commentContent = commentWebDriver[0].Text;
                        var commentDataTime =
                            commentWebDriver[commentWebDriver.Count - 1].FindElement(By.TagName("abbr"))
                                .GetAttribute("data-store")
                                .Split(',')[0].Split(':')[1];
                        comment.CommentOwner = commentOwner;
                        comment.CommentContent = commentContent;
                        comment.CommentDateTime = commentDataTime;
                        comment.PostLink = replyUrl;
                        db.Comments.Add(comment);
                        ClickNextCommentButton(driver);
                    }
                    catch (Exception)
                    {

                    }
                }
                db.SaveChanges();
                lastCommentObjectsLength = commentObjects.Count;
                
            }
        }

        public string GetPostID(IWebDriver driver)
        {
            var href = driver.FindElement(By.XPath(".//*/a[contains(@class,'_15ko')]")).GetAttribute("href");
            var id = href.Split('&').FirstOrDefault(o => o.Contains("shareID=")).Replace("shareID=", "");
            return id;
        }

        private void ClickNextCommentButton(IWebDriver driver)
        {
            //bool clickable = true;
            //while (clickable)
            //{
                try
                {
                    driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMilliseconds(500));
                    driver.FindElement(By.XPath("//*[contains(@id,'see_')]/a")).Click();
                    //driver.FindElement(By.PartialLinkText("View next comments"));
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    //clickable = false;
                }
            //}
        }

        public ChromeDriverService DriverService()
        {
            var driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;
            return driverService;
        }

        public ChromeOptions ChromeOptions()
        {
            var imageSetting = new Dictionary<string, object>();
            imageSetting.Add("images", 2);
            var content = new Dictionary<string, object>();
            content.Add("profile.default_content_settings", imageSetting);

            var prefs = new Dictionary<string, object>();
            prefs.Add("prefs", content);

            var options = new ChromeOptions();
            var field = options.GetType()
                .GetField("additionalCapabilities", BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                var dict = field.GetValue(options) as IDictionary<string, object>;
                if (dict != null)
                    dict.Add(OpenQA.Selenium.Chrome.ChromeOptions.Capability, prefs);
            }
            return options;
        }

        public void Login(IWebDriver driver)
        {
            driver.Navigate().GoToUrl("https://mbasic.facebook.com/");
            LoginByAccount(driver);
        }

        public void LoginByAccount(IWebDriver driver)
        {
            driver.FindElement(By.Name("email")).Clear();
            driver.FindElement(By.Name("email")).SendKeys(Resources.userName);
            driver.FindElement(By.Name("pass")).Clear();
            driver.FindElement(By.Name("pass")).SendKeys(Resources.pass);
            driver.FindElement(By.Name("login")).Click();
        }


        public void LoginByCookies(IWebDriver driver)
        {
            
        }

        public void JavaScriptController(IWebDriver driver, JavaScriptType javaScriptType)
        {
            switch (javaScriptType)
            {
                    case JavaScriptType.ScrollBy:
                    ExecuteJavaScript(driver,"window.scrollBy(0,1000)");
                    break;
            }
        }

        public void ExecuteJavaScript(IWebDriver driver,string javaScript)
        {
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            string title = (string)js.ExecuteScript(javaScript);
        }
    }
}

public enum DriverType
{
    Search,
    Post,
    Reply
}

public enum JavaScriptType
{
    ScrollBy,
}