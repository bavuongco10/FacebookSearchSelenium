using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using OpenQA.Selenium;

namespace SeleniumHelloWorld
{
    internal class CrawlInfor
    {
        private int addedNumbers = 0;
        private List<string> errors = new List<string>(); 
        private static readonly int numberOfDrivers = 1;
        public TestBD1Entities db = new TestBD1Entities();
        private DriverFunction driverFunction = new DriverFunction();
        private IWebDriver[] drivers = new IWebDriver[numberOfDrivers];
        private List<Person> persons = new List<Person>();

        public CrawlInfor()
        {
            Init();
        }

        private void Init()
        {
            InitDrivers();
            //CrawlInformation(drivers[0], "https://mbasic.facebook.com/bui.thanh.ba.vuong/about?refid=17", "bui.thanh.ba.vuong");
            //CrawlInformation(drivers[0], "https://mbasic.facebook.com/profile.php?v=info&id=100003764546916", "100003764546916");
            //CrawlInformation(drivers[0], "https://mbasic.facebook.com/nthm273/about?refid=17", "nthm273");
            //CrawlInformation(drivers[0], "https://mbasic.facebook.com/Yusaku.Pham/about", "Yusaku.Pham");
            InitCrawler();
        }

        private void InitCrawler()
        {
            var commentOwners = db.Comments.Select(o => o.CommentOwner).Distinct().ToList();
            string commentOwnerUrl;
            foreach (var commentOwner in commentOwners)
            {
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

                CrawlInformation(drivers[0], commentOwnerUrl, owner);
            }
            db.SaveChanges();
        }

        private void InitDrivers()
        {
            for (var i = 0; i < numberOfDrivers; i++)
            {
                drivers[i] = driverFunction.InitDriver(drivers[i]);
            }
        }

        private void CrawlInformation(IWebDriver driver, string commentOwnerUrl, string owner)
        {
            drivers[0].Navigate().GoToUrl(commentOwnerUrl);
            if (drivers[0].Title.Contains("Page Not Found") || drivers[0].Title == "Facebook")
            {
                errors.Add(owner);   
                return;
            }
            Person person = new Person();
            person.CommentOwner = owner;
            person.FacebookName= GetFacebookLinkAndID(driver,owner)[0];
            person.FacebookId = GetFacebookLinkAndID(driver, owner)[1];
            person.Skills = GetContactAndBasicInfor(driver, ContactInforType.Skills)[0];
            person.Phone = GetContactAndBasicInfor(driver, ContactInforType.Mobile)[0];
            person.AddressName = GetContactAndBasicInfor(driver, ContactInforType.Address)[0];
            person.AddressMapLink = GetContactAndBasicInfor(driver, ContactInforType.Address)[1];
            person.Screennames = GetContactAndBasicInfor(driver, ContactInforType.Screennames)[0];
            person.Website = GetContactAndBasicInfor(driver, ContactInforType.Website)[0];
            person.Email = GetContactAndBasicInfor(driver, ContactInforType.Email)[0];
            person.Birthday = GetContactAndBasicInfor(driver, ContactInforType.Birthday)[0];
            person.Gender = GetContactAndBasicInfor(driver, ContactInforType.Gender)[0];
            person.InterestedIn = GetContactAndBasicInfor(driver, ContactInforType.Interested)[0];
            person.Languages = GetContactAndBasicInfor(driver, ContactInforType.Languages)[0];
            person.ReligousViews = GetContactAndBasicInfor(driver, ContactInforType.Religious)[0];
            person.ReligousViewsLink = GetContactAndBasicInfor(driver, ContactInforType.Religious)[1];
            person.PoliticalViews = GetContactAndBasicInfor(driver, ContactInforType.Political)[0];
            person.PoliticalViewsLink = GetContactAndBasicInfor(driver, ContactInforType.Political)[1];
            person.Relationship = GetContactAndBasicInfor(driver, ContactInforType.Relationship)[0];
            db.People.Add(person);
            var works = GetEducationAndWork(driver, ExperienceType.Work);
            db.PeopleWorkEdus.AddRange(CreatObjectWorkEdu(works, person.FacebookId,ExperienceType.Work.ToString()));
            var educations = GetEducationAndWork(driver, ExperienceType.Education);
            db.PeopleWorkEdus.AddRange(CreatObjectWorkEdu(educations, person.FacebookId, ExperienceType.Education.ToString()));

            var currentCity = GetContactAndBasicInfor(driver, ContactInforType.Current);
            if (currentCity[0] != null)
            {
                db.PeoplePlaces.Add(new PeoplePlace()
                {
                   FacebookID = person.FacebookId,
                   Name = currentCity[0],
                   Url = currentCity[1],
                   Role = ContactInforType.Current.ToString()
                });
            }
            var homeTown = GetContactAndBasicInfor(driver, ContactInforType.Hometown);
            if (homeTown[0] != null)
            {
                db.PeoplePlaces.Add(new PeoplePlace()
                {
                    FacebookID = person.FacebookId,
                    Name = homeTown[0],
                    Url = homeTown[1],
                    Role = ContactInforType.Hometown.ToString()
                });
            }

            db.FamilyRoles.AddRange(GetFamilyMemmbers(driver,person.FacebookId));
            addedNumbers += 1;
            //db.SaveChanges();
            if (addedNumbers % 10 == 0)
            {
                db.SaveChanges();
            }

        }
        
        private List<PeopleWorkEdu> CreatObjectWorkEdu(List<List<string>> works, string id,string type)
        {
            List<PeopleWorkEdu> workList  =new List<PeopleWorkEdu>();
            foreach (var work in works)
            {
                if (work[0] != null)
                {
                    PeopleWorkEdu peopleWork = new PeopleWorkEdu();
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

        private List<String> GetFacebookLinkAndID(IWebDriver driver,string owner)
        {
            List<string> parts = new List<string>();
            string name = owner;
            string url = null;
            if (name.Contains("profile.php?id="))
            {
                url = name.Replace("profile.php?id=", "");
                name = string.Empty;
            }
            else
            {
                
                url =
                    driver.FindElement(By.XPath(".//*[@id='root']/div/div[1]/div[2]/div/div[1]/a")).GetAttribute("href");
                if (url.IndexOf("&id=")!=-1)
                {
                    url = url.Substring(url.IndexOf("&id=") + 4, 30);
                }
                else
                {
                    //url = url.Substring(url.IndexOf("_id=") + 4, 30);        
                    url = Regex.Match(url, @"\d+").Value;
                }
                if (url.IndexOf("&set=") !=-1)
                {
                    url = url.Substring(0, url.IndexOf("&set="));
                }
                
            }
            parts.Add(name);
            parts.Add(url);
            return parts;
        }

        private List<String> GetContactAndBasicInfor(IWebDriver driver, ContactInforType type)
        {
            List<string> info = new List<string>();
            string name = null;
            string url = null;
            var driverString = string.Format("//*[starts-with(@title,'{0}')]/table/tbody/tr/td[2]/div", type);
            switch (type)
            {
                case ContactInforType.Mobile:
                    driverString += "/span/span";
                    break;
                case ContactInforType.Address:
                case ContactInforType.Religious:
                case ContactInforType.Political:
                case ContactInforType.Current:
                case ContactInforType.Hometown:
                    driverString += "/a";
                    break;
                case ContactInforType.Skills:
                    driverString = string.Format(".//*[@id='{0}']/div/div[2]/div/span", type.ToString().ToLower());
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

            info.Add(name);
            info.Add(url);
            return info;
        }

        //private PeoplePlace GetLivedPlace(IWebDriver driver, PlaceLivedType type, string id)
        //{
        //    string currentCityName = null;
        //    string currentCityUrl = null;
        //    var currentCityDriver =
        //        driver.FindElements(
        //            By.XPath(string.Format(".//*[@id='living']/div/div[2]/div[{0}]/div/table/tbody/tr/td[2]/div/a",
        //                (int) type)));
        //    if (currentCityDriver.Count > 0)
        //    {
        //        currentCityName = currentCityDriver[0].Text;
        //        currentCityUrl = currentCityDriver[0].GetAttribute("href");
        //    }
        //    if (currentCityName != null)
        //    {
        //        PeoplePlace peoplePlace = new PeoplePlace()
        //        {
        //            FacebookID = id,
        //            Name = currentCityName,
        //            Url = currentCityUrl,
        //            Role = type.ToString()
        //        };
        //        return peoplePlace;
        //    }
        //    return null;
        //}

        private List<List<string>> GetEducationAndWork(IWebDriver driver, ExperienceType type)
        {
            List<List<string>> partsOfParts = new List<List<string>>();
            var educationDrivers =
                driver.FindElements(
                    By.XPath(string.Format("//*[@id='{0}']/div/div/div/div/div[1]", type.ToString().ToLower())));
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
                List<string> parts = new List<string>(){educationUrl,educationLevel,educationTime};
                partsOfParts.Add(parts);
            }
            return partsOfParts;
        }

        private List<FamilyRole> GetFamilyMemmbers(IWebDriver driver,string id)
        {
            List<FamilyRole> familyList  = new List<FamilyRole>();
           var drivers = driver.FindElements(By.XPath(".//*[@id=\'family\']/div/div[2]/div/div[1]/h3/a"));
            foreach (var item in drivers)
            {
                var familyMemmberUrl = item.GetAttribute("href");
                var familyMemmberRole = item.FindElement(By.XPath("../following-sibling::h3")).Text;
                familyList.Add(new FamilyRole()
                {
                    FacebookID1   = id,
                    FacebookID2 = familyMemmberUrl,
                    Role = familyMemmberRole
                });
            }

            return familyList;
        }
    }
}

public enum ExperienceType
{
    Education,
    Work
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
    Relationship,
    Current, //CurrentCity
    Hometown
}
