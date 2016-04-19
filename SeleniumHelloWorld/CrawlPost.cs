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
        private DriverHelper _driverHelper = new DriverHelper();
        private string cookiesFilePath = "d:\\cookies.txt";
        public TestBD1Entities db = new TestBD1Entities();
        private Resources _resources;
        public IWebDriver DriverSearch;
        public IWebDriver DriverPost;
        public IWebDriver DriverReply;

        public CrawlPost(Resources resources)
        {
            _resources = resources;
            _searchString = EncodeSearchString(resources.SearchString);
        }

        private string _searchString;
        public List<string> listUrl = new List<string>();

        public void Init()
        {
            InitDrivers();
            Navigate(DriverSearch, DriverType.Search);
            CrawlPostsFromSrearch(DriverSearch);
        }

        private void InitDrivers()
        {
            DriverSearch=_driverHelper.InitDriver(DriverSearch,_resources);

             DriverPost=_driverHelper.InitDriver(DriverPost,_resources);

            DriverReply=_driverHelper.InitDriver(DriverReply,_resources);
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
                        postUrl = postUrl.Replace("web.facebook.com", "touch.facebook.com");
                        var postContent = post.FindElement(By.ClassName("userContent")).Text;

                        postModel.PostOwner = postOwner;
                        postModel.PostDateTime = dateTime;
                        postModel.PostContent = postContent;
                        postModel.PostLink = postUrl;
                        postModel.SearchString = _resources.SearchString;

                        var comments = CrawlCommentsFromPost(DriverPost, postUrl);
                        if (comments != null)
                        {
                            db.Comments.AddRange(comments);
                        }
                        db.Posts.Add(postModel);
                        db.SaveChanges();
                    }
                    pastPostsLength = postsWebdriver.Count;
                    _driverHelper.JavaScriptController(driver, JavaScriptType.ScrollBy);
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
                    driver.Navigate().GoToUrl("https://www.facebook.com/search/top/?q=" + _searchString);
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

        public List<Comment> CrawlCommentsFromPost(IWebDriver driver, string postUrl)
        {
            List<Comment> comments = new List<Comment>();
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

                        comments.Add(comment);

                        if (commentWebDriver.Count > 2)
                        {
                            var replyUrl = commentWebDriver[2].FindElement(By.TagName("a")).GetAttribute("href");
                            //Crawl comment from reply
                            var replies = CrawRepliesFromComment(DriverReply, replyUrl);
                            if (replies != null)
                            {
                                comments.AddRange(replies);
                            }
                        }
                        ClickNextCommentButton(driver);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                lastCommentObjectsLength = commentObjects.Count;
            }
            return comments;
        }

        private List<Comment> CrawRepliesFromComment(IWebDriver driver, string replyUrl)
        {
            List<Comment> replies = new List<Comment>();
            int lastReplyObjectsLength = 0;

            Navigate(driver, DriverType.Post, replyUrl);

            while (true)
            {

                var replyObjects = driver.FindElements(By.ClassName("_2pis"));
                if (replyObjects.Count == lastReplyObjectsLength)
                {
                    break;
                }
                for (int i = lastReplyObjectsLength; i < replyObjects.Count; i++)
                {
                    Comment reply = new Comment();
                    var replyObject = replyObjects[i];
                    try
                    {
                        var replyOwner =
                            replyObject.FindElement(By.ClassName("_1s79"))
                                .GetAttribute("href")
                                .Replace("https://touch.facebook.com/", "");

                        if (replyOwner.Contains("?fref=nf"))
                        {
                            replyOwner = replyOwner.Remove(replyOwner.IndexOf("?fref=nf", StringComparison.Ordinal));
                        }
                        else if (replyOwner.Contains("?refid="))
                        {
                            replyOwner = replyOwner.Remove(replyOwner.IndexOf("?refid=", StringComparison.Ordinal));
                        }

                        var commentWebDriver = replyObject.FindElements(By.TagName("div"));
                        var commentContent = commentWebDriver[0].Text;
                        var commentDataTime =
                            commentWebDriver[commentWebDriver.Count - 1].FindElement(By.TagName("abbr"))
                                .GetAttribute("data-store")
                                .Split(',')[0].Split(':')[1];
                        reply.CommentOwner = replyOwner;
                        reply.CommentContent = commentContent;
                        reply.CommentDateTime = commentDataTime;
                        reply.PostLink = replyUrl;

                        replies.Add(reply);

                        ClickNextCommentButton(driver);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
                lastReplyObjectsLength = replyObjects.Count;
            }
            return replies;
        }

        public string GetPostId(IWebDriver driver)
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