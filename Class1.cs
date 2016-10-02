using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLinkedIn
{
   
    public class Account    // Stores the email and password of each account
    {
        
        public string Email;
        public string Password;
        private CrmHelper crmView;
        int Index;
        private Browser browser = Browser.Instance;

        private void SetCrmHelper(CrmHelper view)
        {
            crmView = view;
            return;
        }

        public Account(string email, string password, int index)    // Constructor
        {
            Email = email;
            Password = password;
            Index = index;
        }
       
        //}
        // Keep all the accounts in one place
        List<Account> testAccounts = new List<Account>()
{
    new Account("ary@argus-soft.net", "rufus279", 1),    // Create a new account
    new Account("cfilimonchuk1@gmail.com", "lin147258", 2)    // Create another account
};
        List<Account> botAccounts = new List<Account>()
{
    new Account("i.rebenok@argus-soft.net", "Iv@N5434LokC", 1),
    new Account("a.veresova@argus-soft.net", "@nGeL@43FtHe", 2),
    new Account("v.prolesok@argus-soft.net", "VikT0R830HbNmD", 3),
    new Account("a.rumina@argus-soft.net","@nN@8971SVxAP", 4),
    new Account("e.solonicina@argus-soft.net", "ElEn@0954VbEf", 5),
    new Account("m.sergievsky@argus-soft.net", "mich@el523GhYd", 6),
    new Account("a.kashina@argus-soft.net", "arigato1", 7),
    //new Account("vikki.sales87@gmail.com", "DbRN0HbZ777")
};
        public void LoginAllBots()
        {

            foreach (Account account in testAccounts)
            {
                browser.LinkedInNavigateTo("https://www.linkedin.com/uas/login?goback=&trk=hb_signin");
                SeleniumHelper.WaitForElement(By.Id("session_key-login"));
                var element = browser.LinkedFindElement(By.Id("session_key-login"));
                System.Threading.Thread.Sleep(5000);
                element.Clear();
                element.SendKeys(account.Email);
                System.Threading.Thread.Sleep(1000);
                SeleniumHelper.WaitForElement(By.Id("session_password-login"));
                element = browser.LinkedFindElement(By.Id("session_password-login"));
                System.Threading.Thread.Sleep(5000);
                element.Clear();
                element.SendKeys(account.Password);
                System.Threading.Thread.Sleep(1000);
                SeleniumHelper.WaitForElement(By.Id("btn-primary"));
                element = browser.LinkedFindElement(By.Id("btn-primary"));
                System.Threading.Thread.Sleep(5000);
                element.Click();
                // do search
                SearchByCountry();
                //logout
                Actions actions = new Actions(browser.driver);
                SeleniumHelper.WaitForElement(By.XPath(".//*[@id='img-defer-id-1-61312']"));
                element = browser.LinkedFindElement(By.XPath(".//*[@id='img-defer-id-1-61312']"));
                System.Threading.Thread.Sleep(5000);
                actions.MoveToElement(element);
                SeleniumHelper.WaitForElement(By.XPath(".//*[@id='account-sub-nav']/div/div[2]/ul/li[1]/div/span/span[3]"));
                element = browser.LinkedFindElement(By.XPath(".//*[@id='account-sub-nav']/div/div[2]/ul/li[1]/div/span/span[3]"));
                System.Threading.Thread.Sleep(5000);
                actions.MoveToElement(element);
                actions.Click().Build().Perform();

            }
        }
        //Получение
        public void SearchByCountry()
        {
            // список/словарь для хранения пользователей 
            Dictionary<string, int> users = new Dictionary<string, int>();
            String searchUrl = "https://www.linkedin.com/vsearch/p?type=people&orig=FCTD&rsid=5225796901475164704017&pageKey=oz-winner&trkInfo=tarId%3A1475153538610&trk=global_header&search=Search&f_G=ch%3A4938,ch%3A4930,ch%3A4928,ch%3A4929,ch%3A4935,ch%3A4937,ch%3A4932,ch%3A4934,ch%3A4936,de%3A4953,de%3A4966,de%3A5000,de%3A4944,de%3A4977,de%3A5026,de%3A5007,de%3A4980,de%3A4998&openFacets=N,G,CC&f_N=F";
            IWebElement country, count, linkName; // linkUrl;    //аботает добавить элементы для сравнения с контактами в CRM
            int i, k, userCount, userCountUpd, curUserCount, botIndex, cntPage, allRecord, j, l;
            cntPage = 0;
            allRecord = 0;
            String userCountry, userCounts, linkNameS;
            botIndex = Index;

            browser.LinkedInNavigateTo(searchUrl); //переходим на страницу поиска с нужными странами
            j = 11;
            for (i = 2, k = 2; i <= 19; i++, k++)
            {
                try
                {
                    // Получаем данные location начало - .//*[@id='facet-G']/fieldset/div/ol/li[2]/div/label/bdi, конец - .//*[@id='facet-G']/fieldset/div/ol/li[19]/div/label/bdi
                    //"//*[@id='message-list']/form/ol/li[" + i.ToString() + "]/div/div[2]/p/a"
                    country = SeleniumHelper.WaitForElement(By.XPath(".//*[@id='facet-G']/fieldset/div/ol/li[" + i.ToString() + "]/div/label/bdi"));
                    userCountry = country.GetAttribute("title");
                    //Получаем данные count начало - .//*[@id='facet-G']/fieldset/div/ol/li[2]/div/span, конец - .//*[@id='facet-G']/fieldset/div/ol/li[19]/div/span
                    count = SeleniumHelper.WaitForElement(By.XPath(".//*[@id='facet-G']/fieldset/div/ol/li[" + k.ToString() + "]/div/span"));
                    userCounts = count.GetAttribute("facet-count");
                    userCount = Int32.Parse(userCounts);
                    users.TryGetValue(userCountry, out curUserCount);
                    userCountUpd = 0;
                    //Проверяем есть ли контакт в CRM
                    if (userCount != 0)
                    {
                        for (l = 1; l < 11; l++)
                        {
                            linkName = SeleniumHelper.WaitForElement(By.XPath(".//*[@id='results']/li[" + l.ToString() + "]/div/h3/a"));
                            linkNameS = linkName.ToString();                         
                            crmView.SetSearchMode(0);
                            crmView.SearchContactByName(linkNameS);
                        }
                        if (botIndex >= 2 && users.ContainsKey(userCountry) == true)
                        {
                            userCount = userCountUpd + curUserCount;
                        }
                        // Переход на следующую страницу
                        j += 10;
                        cntPage++;
                        // Это последняя страница?
                        if (cntPage == 35)
                            break;
                        tmpString = "inbox/#sent?startRow=" + j.ToString() + "&subFilter=invitation&keywords=&sortBy=";
                        urlStr = "https://www.linkedin.com/" + tmpString;
                        crm                    }
                    users.Add(userCountry, userCount);
                }
                catch (Exception ex)
                {
                    return;
                }


                return;
            }
        }
    }

}