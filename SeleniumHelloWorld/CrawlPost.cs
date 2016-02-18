using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
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
    internal class CrawlPost
    {
        private DriverFunction driverFunction = new DriverFunction();
        private string cookiesFilePath = "d:\\cookies.txt";
        public TestBD1Entities db = new TestBD1Entities();
        public Resources Resources = new Resources();
        public IWebDriver DriverSearch;
        public IWebDriver DriverPost;
        public IWebDriver DriverReply;

        public CrawlPost()
        {
            Init();
        }
        private string SearchString
        {
            get
            {
                return EncodeSearchString("vietnamese food");
                
            }
        }
        public List<string> listUrl = new List<string>();  

        public void Init()
        {
            InitDrivers();
            Navigate(DriverSearch, DriverType.Search);
            CrawlPostsFromSrearch(DriverSearch);
        }

        private void InitDrivers()
        {
            DriverSearch=driverFunction.InitDriver(DriverSearch);
            
             DriverPost=driverFunction.InitDriver(DriverPost);

            DriverReply=driverFunction.InitDriver(DriverReply);
        }

        
        private void CrawlPostsFromSrearch(IWebDriver driver)
        {
            int pastPostsLength = 0;
            while (true)
            {
                try
                {
                    var postsWebdriver = driver.FindElements(By.ClassName("userContentWrapper"));
                    //var postUrls = driver.FindElements(By.XPath("//*[@data-comment-prelude-ref='action_link_bling']"));
                    for (int i = pastPostsLength; i < postsWebdriver.Count; i++)
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
                        postModel.SearchString = SearchString;
                        db.Posts.Add(postModel);
                        db.SaveChanges();

                        CrawlCommentFromPost(DriverPost, postUrl);
                    }
                    pastPostsLength = postsWebdriver.Count;
                    driverFunction.JavaScriptController(driver, JavaScriptType.ScrollBy);
                }
                catch (Exception)
                {
                    // ignored
                }

                
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
                        if (commentOwner.Contains("?fref=nf"))
                        {
                            commentOwner = commentOwner.Remove(commentOwner.IndexOf("?fref=nf", StringComparison.Ordinal));
                        }
                        else if (commentOwner.Contains("?refid="))
                        {
                            commentOwner = commentOwner.Remove(commentOwner.IndexOf("?refid=", StringComparison.Ordinal));
                        }

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

                        if (commentOwner.Contains("?fref=nf"))
                        {
                            commentOwner = commentOwner.Remove(commentOwner.IndexOf("?fref=nf", StringComparison.Ordinal));
                        }
                        else if (commentOwner.Contains("?refid="))
                        {
                            commentOwner = commentOwner.Remove(commentOwner.IndexOf("?refid=", StringComparison.Ordinal));
                        }

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