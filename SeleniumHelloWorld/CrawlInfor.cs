using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumHelloWorld
{
    class CrawlInfor
    {
        public TestBD1Entities testBd1Entities = new TestBD1Entities();
        private DriverFunction driverFunction = new DriverFunction();
        private static int numberOfDrivers = 1;
        private IWebDriver[] drivers = new IWebDriver[numberOfDrivers]; 
        public CrawlInfor()
        {
            Init();
        }

        private void Init()
        {
            InitDrivers();
            //CrawlInformation(drivers[0], "https://mbasic.facebook.com/bui.thanh.ba.vuong/about?refid=17");
            //CrawlInformation(drivers[0], "https://mbasic.facebook.com/profile.php?v=info&id=100003764546916");
            //CrawlInformation(drivers[0], "https://mbasic.facebook.com/nthm273/about?refid=17");
            CrawlInformation(drivers[0], "https://mbasic.facebook.com/Yusaku.Pham/about");
            //InitCrawler();
        }

        private void InitCrawler()
        {
            var commentOwners = testBd1Entities.Comments.Select(o => o.CommentOwner).Distinct().ToList();
            string commentOwnerUrl = "http://touch.facebook.com/";
            foreach (var commentOwner in commentOwners)
            {
                commentOwnerUrl = "http://touch.facebook.com/";
                var owner = commentOwner;
                if (owner.Contains("?fref=nf"))
                {
                    owner = owner.Replace("?fref=nf", "");
                }
                if (owner.Contains("profile.php?id="))
                {
                    //https://m.facebook.com/profile.php?v=info&id=100002917108835
                    commentOwnerUrl = commentOwnerUrl + owner;
                }
                else
                {
                    //https://mbasic.facebook.com/bui.thanh.ba.vuong/about?refid=17
                    commentOwnerUrl = commentOwnerUrl + owner + "/about?refid=17";
                }
                
                CrawlInformation(drivers[0],commentOwnerUrl);
            }
        }

        private void InitDrivers()
        {
            for (int i = 0; i < numberOfDrivers; i++)
            {
                drivers[i] = driverFunction.InitDriver(drivers[i]);
            }
        }

        private void CrawlInformation(IWebDriver driver, string commentOwnerUrl)
        {
            drivers[0].Navigate().GoToUrl(commentOwnerUrl);
            GetFamilyMemmbers(driver);
            //Get education
            GetEducationAndWork(driver, ExperienceType.Education);
            //Get Work
            GetEducationAndWork(driver,ExperienceType.Work);
            //GetCurrentCity
            GetLivedPlace(driver,PlaceLivedType.CurrentCity);
            //GetHomeTown
            GetLivedPlace(driver,PlaceLivedType.HomeTown);
            //GetMobilePhone if many separate by -
            GetContactAndBasicInfor(driver, ContactInforType.Mobile);
            //GetAddress
            GetContactAndBasicInfor(driver,ContactInforType.Address);
            GetContactAndBasicInfor(driver, ContactInforType.Screennames);
            GetContactAndBasicInfor(driver, ContactInforType.Website);
            GetContactAndBasicInfor(driver, ContactInforType.Email);


        }

        //private void GetSkillsAndRelationship(IWebDriver driver)
        //{
            
        //    string name = null;
        //    var drivers = driver.FindElements(By.XPath(".//*[@id='skills']/div/div[2]/div/span"));
        //    if (drivers.Count>0)
        //    {
        //        name = drivers[0].Text;
        //    }
        //}


        private void GetContactAndBasicInfor(IWebDriver driver,ContactInforType type)
        {
            string name = null;
            string url = null;
            string driverString = string.Format("//*[starts-with(@title,'{0}')]/table/tbody/tr/td[2]/div", type.ToString());
            switch (type)
            {
                    case ContactInforType.Mobile:
                    driverString += "/span/span";
                    break;
                    case ContactInforType.Address:
                    case ContactInforType.Religious:
                    case ContactInforType.Political:
                    driverString += "/a";
                    break;
                    case ContactInforType.Skills:
                    driverString = string.Format(".//*[@id='{0}']/div/div[2]/div/span",type.ToString().ToLower());
                    break;
                    case ContactInforType.Relationship:
                    driverString = string.Format(".//*[@id='{0}']/div/div[2]/div/div/div", type.ToString().ToLower());
                    break;
            }
            var drivers =
                driver.FindElements(By.XPath(driverString));
            if (drivers.Count > 1)
            {
                foreach (var item in drivers)
                {
                    name += item.Text + "-";
                }
            }
            else if (drivers.Count == 1)
            {
                name = drivers[0].Text;
                url = drivers[0].GetAttribute("href");
            }
        }

        private void GetLivedPlace(IWebDriver driver,PlaceLivedType type)
        {
            string currentCityName = null;
            string currentCityUrl = null;
            var currentCityDriver =
                driver.FindElements(By.XPath(string.Format(".//*[@id='living']/div/div[2]/div[{0}]/div/table/tbody/tr/td[2]/div/a",(int)type)));
            if (currentCityDriver.Count>0)
            {
                currentCityName = currentCityDriver[0].Text;
                currentCityUrl = currentCityDriver[0].GetAttribute("href");
            }
        }

        private static void GetEducationAndWork(IWebDriver driver,ExperienceType type)
        {
            var educationDrivers = driver.FindElements(By.XPath(string.Format("//*[@id='{0}']/div/div/div/div/div[1]",type.ToString().ToLower())));
            foreach (var educationDriver in educationDrivers)
            {
                // select in this node
                var tiles = educationDriver.FindElements(By.XPath("./div"));
                string educationUrl = null;
                string educationLevel = null;
                string educationTime = null;
                if (tiles.Count > 0)
                {
                    educationUrl = tiles[0].FindElement(By.TagName("a")).GetAttribute("href");
                }
                if (type == ExperienceType.Education)
                {
                    if (tiles.Count > 1)
                    {
                        educationLevel = tiles[1].Text;
                    }
                    if (tiles.Count > 2)
                    {
                        educationTime = tiles[2].Text;
                    }
                }
            }
        }
        private static void GetFamilyMemmbers(IWebDriver driver)
        {
            var drivers = driver.FindElements(By.XPath(string.Format(".//*[@id='family']/div/div[2]/div/div[1]/h3/a")));
            foreach (var item in drivers)
            {
                string familyMemmberUrl = item.GetAttribute("href");
                string familyMemmberRole = item.FindElement(By.XPath("../following-sibling::h3")).Text;
                
            }
        }
    }
}

public enum ExperienceType
{
    Education,
    Work
}

public enum PlaceLivedType
{
    Base,
    CurrentCity,
    HomeTown
}

public enum ContactInforType
{
    Mobile,
    Address,
    Screennames,
    Website,
    Email,
    Birthday,
    Gender,
    Interested,
    Languages,
    Religious,
    Political,
    Skills,
    Relationship
}

