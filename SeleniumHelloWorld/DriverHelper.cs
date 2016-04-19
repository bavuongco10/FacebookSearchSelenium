using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumHelloWorld
{
    class DriverHelper
    {
        private Resources resources;

        public IWebDriver InitDriver(IWebDriver driver, Resources resources)
        {
            driver = new ChromeDriver(DriverService(),ChromeOptions());
            this.resources = resources;
            Login(driver);
            return driver;
        }

        public void Login(IWebDriver driver)
        {
            driver.Navigate().GoToUrl("https://mbasic.facebook.com/");
            LoginByAccount(driver);
        }

        public void LoginByAccount(IWebDriver driver)
        {
            driver.FindElement(By.Name("email")).Clear();
            driver.FindElement(By.Name("email")).SendKeys(resources.UserName);
            driver.FindElement(By.Name("pass")).Clear();
            driver.FindElement(By.Name("pass")).SendKeys(resources.Pass);
            driver.FindElement(By.Name("login")).Click();
        }


        public void LoginByCookies(IWebDriver driver, string cookiesFilePath)
        {
            StreamReader reader = new StreamReader(cookiesFilePath);
            int lines = int.Parse(reader.ReadLine() ?? "0");
            for (int i = 0; i < lines; i++)
            {
                var cookieString = reader.ReadLine().Split('-');
                driver.Manage().Cookies.AddCookie(new Cookie(cookieString[0], cookieString[1]));
            }
            driver.Navigate().Refresh();
        }


        public void JavaScriptController(IWebDriver driver, JavaScriptType javaScriptType)
        {
            switch (javaScriptType)
            {
                case JavaScriptType.ScrollBy:
                    ExecuteJavaScript(driver, "window.scrollBy(0,1000)");
                    break;
            }
        }

        public void ExecuteJavaScript(IWebDriver driver, string javaScript)
        {
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            string title = (string)js.ExecuteScript(javaScript);
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
    }
}
