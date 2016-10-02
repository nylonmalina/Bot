using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.Extensions;
using NLog;
using System.IO;
using OpenPop.Pop3;
using System.Drawing.Imaging;

namespace BotLinkedIn 

{


    public class Browser
    {
        private static Browser instance;
        private Logger log = LogManager.GetCurrentClassLogger();
        public IWebDriver driver;
        //private IWebDriver driver;
        private EventFiringWebDriver firingDriver;
        private FirefoxProfile profile;

        private string handleLinkedIn;
        private string handleCrm;

        public static Browser Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Browser();
                }
                return instance;
            }
        }

        public void Start()
        {
            InitProfile();
            //InitExtensions();
            InitDriver();
            // Настроить Proxy server
            IniProxy("proxy.smart.argus", 3128, "", "");
            InitFiringDriver();
            driver.Manage().Window.Maximize();
            log.Info("Start Firefox.");
            handleLinkedIn = driver.WindowHandles[0];
            OpenNewTab();
            return ;
        }

        
        private void OpenNewTab()
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver; ;
            // Открыть новое окно 
            js.ExecuteScript("window.open(\"\")", "");
            handleCrm = driver.WindowHandles[1];
            return;
        }
        
        public void Quit()
        {
            if (driver == null)
            {
                return;
            }
            // Закрыть окна
            driver.SwitchTo().Window(handleLinkedIn).Close();
            driver.SwitchTo().Window(handleCrm).Close();
            // Закрыть browser
            driver.Quit();
            driver = null;
            log.Info("Close Firefox.");
        }
        public void SetCurrentWindow(int cur)
        {
            if (cur == 0) 
            {
                driver.SwitchTo().Window(handleLinkedIn);
            }
            else
            {
                driver.SwitchTo().Window(handleCrm);
            }
        }
        
        public void LinkedInNavigateTo(string Url)
        {
            driver.SwitchTo().Window(handleLinkedIn);
            driver.Navigate().GoToUrl(Url);
            return;
        }

        public void CrmNavigateTo(string Url)
        {
            driver.SwitchTo().Window(handleCrm);
            driver.Navigate().GoToUrl(Url);
            return;
        }

        public IWebElement LinkedFindElement(By by)
        {
            //driver.SwitchTo().Window(handleLinkedIn);
            return driver.FindElement(by);
        }
        public IWebElement CrmFindElement(By by)
        {
            //driver.SwitchTo().Window(handleCrm);
            return driver.FindElement(by);
        }
        
        private void FiringDriverNavigating(object sender, WebDriverNavigationEventArgs e)
        {
            log.Info(String.Format("Navigating to: {0}", e.Url));
        }

        private void FiringDriverNavigated(object sender, WebDriverNavigationEventArgs e)
        {
            log.Info(String.Format("Navigated to: {0}", e.Url));
        }

        private void FiringDriverExceptionThrown(object sender, WebDriverExceptionEventArgs e)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss");
            string fileName = String.Format("Exception-{0}.png", timestamp);
            fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, (@"Logs\" + fileName));
            driver.TakeScreenshot().SaveAsFile(fileName, ImageFormat.Png);
            log.Error(String.Format("Take screenshot: {0}", fileName));
        }


        public IWebDriver Driver()
        {
            return driver;
        }
        public IWebDriver FiringDriver()
        {
            return firingDriver;
        }

        private void InitProfile()
        {
            FirefoxProfileManager profileManager = new FirefoxProfileManager();
            // Firefox profile name: default, WebDriver,WebLinked
            profile = profileManager.GetProfile("WebDriver");
            //profile = profileManager.GetProfile("WebLinked");

            profile.AcceptUntrustedCertificates = true;
            profile.EnableNativeEvents = true;
            profile.SetPreference("browser.startup.homepage_override.mstone", "ignore");
            log.Info("Profile inited:");
            log.Info("  directory: {0}", profile.ProfileDirectory);
        }

        private void IniProxy(string proxy, int port, string userName, string password)
        {
            profile.SetPreference("network.proxy.http", proxy);
            profile.SetPreference("network.proxy.http_port", port);
            profile.SetPreference("network.proxy.ftp", proxy);
            profile.SetPreference("network.proxy.ftp_port", port);
            profile.SetPreference("network.proxy.socks", proxy);
            profile.SetPreference("network.proxy.socks_port", port);
            profile.SetPreference("network.proxy.ssl", proxy);
            profile.SetPreference("network.proxy.ssl_port", port);
            profile.SetPreference("network.proxy.user_name", userName);
            profile.SetPreference("network.proxy.password", password);
            profile.SetPreference("network.proxy.share_proxy_settings", true);
            // Proxy type: Direct = 0, Manual = 1, PAC = 2, AUTODETECT = 4, SYSTEM = 5
            profile.SetPreference("network.proxy.type", 5);
            log.Info("Change proxy server settings to:");
            log.Info("  network.proxy.http: {0}", proxy);
            log.Info("  network.proxy.hhttp_port: {0}", port);
            profile.SetPreference("network.proxy.no_proxies_on", "localhost, 127.0.0.1,crm.smart.argus");

        }

        private void InitExtensions()
        {
            var extensions = new List<string>();
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Extensions");
            extensions.Clear();
            if (Directory.Exists(path))
            {
                var files = Directory.EnumerateFiles(path, "*.xpi");
                foreach (string file in files)
                {
                    profile.AddExtension(file);
                }
            }
            else
            {
                log.Warn(String.Format("Extensions directory not found: {0}.", path));
            }
        }

        private void InitDriver()
        {
            driver = new FirefoxDriver(new FirefoxBinary(), new FirefoxProfile(), TimeSpan.FromSeconds(180));
            //driver = new FirefoxDriver();
            // Установить максимальное время загрузки страницы 
            driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromSeconds(Settings.Selenium.PageLoadTimeout));
            return;
        }

        private void InitFiringDriver()
        {
            firingDriver = new EventFiringWebDriver(driver);
            firingDriver.Navigating += FiringDriverNavigating;
            firingDriver.Navigated += FiringDriverNavigated;
            firingDriver.ExceptionThrown += FiringDriverExceptionThrown;
        }
    }
}
