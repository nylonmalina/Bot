using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;

namespace BotLinkedIn
{
    class TempCountrySearch
    { /*stuct чтоб не было ошибок
        */
        internal struct DataIn
        {
            internal string curIndustry;
            internal string fullName;
            internal string urlPath;

            internal string eUserName;
            internal string eHeadLine;
            internal string eLinkPage;
            internal string eSummary;
            internal string messageText;
            internal string userCountry;
            internal bool isMessageSend;
        }
        private Browser browser = Browser.Instance;
        private List<DataIn> data = new List<DataIn>();
        private CrmHelper crmView;
        private string curIndustry;
        private int cntInvite;
        private static Random randomGenerator;

        public void SetCrmHelper(CrmHelper view)
        {
            crmView = view;
            return;
        }

        /*
       * Наколение контактов с текущей страницы
       * 
       */
        public void ParseSearchInfoPage()
        {
            string t1;
            DataIn tst = new DataIn(); ;

            for (int e = 1; e < 10; e++)
            {
                t1 = "//*[@id='results']/li[" + e.ToString() + "]/div/div[3]/a";
                if (SeleniumHelper.IsElementPresent(By.XPath(t1)) == true)
                {
                    var element = SeleniumHelper.WaitForElement(By.XPath(t1));
                    if (element.Text == "Message")
                    {
                        t1 = "//*[@id='results']/li[" + e.ToString() + "]/div/h3/a";
                        var element1 = SeleniumHelper.WaitForElement(By.XPath(t1));
                        tst.urlPath = element1.GetAttribute("href").ToString();
                        tst.fullName = element1.Text;
                        tst.eUserName = "";
                        tst.eHeadLine = "";
                        tst.eLinkPage = "";
                        tst.eSummary = "";
                        tst.curIndustry = curIndustry;
                        // Сохраняем для последующей работы
                        data.Add(tst);
                    }
                }
            }
            return;
        }
    }
        /*Проверяем наличие контакта в CRM после получения нормальной ссылки на профиль
        */
      public bool CheckCRM (ref DataIn curData)
    {
        string tmpObj, tmpStr;
        IWebElement elButton, eFriend, eInvite, elInputBox;
        int rndTime;
        // Ожидание перехода на страницу пользователя
        rndTime = randomGenerator.Next(5, 250);
        System.Threading.Thread.Sleep(rndTime * 1000);
        // Переходим на страницу профиля пользователя
        browser.LinkedInNavigateTo(curData.urlPath);
        try
        {
            if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='top-card']/div/div[1]/div[2]/div[2]/div[2]/a")))
            {
                elButton = SeleniumHelper.WaitForElement(By.XPath("//*[@id='top-card']/div/div[1]/div[2]/div[2]/div[2]/a"));
            }
            else
            {
                if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='top-card']/div/div[1]/div/div[2]/div[2]/a")))
                {
                    elButton = SeleniumHelper.WaitForElement(By.XPath("//*[@id='top-card']/div/div[1]/div/div[2]/div[2]/a"));
                }
                else
                {
                    if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='top-card']/div/div[1]/div/div[2]/div[2]/div/a")))
                    {
                        elButton = SeleniumHelper.WaitForElement(By.XPath("//*[@id='top-card']/div/div[1]/div/div[2]/div[2]/div/a"));
                    }
                    else
                        return false;
                }
            }
            tmpObj = "0";
        }
        catch (Exception ex)
        {
            // Кнопка не найдена
            return false;
        }
        try
        {
            if (elButton.Text == "Connect")
            {
                curData.eUserName = "";
                curData.eHeadLine = "";
                curData.eLinkPage = "";
                curData.eSummary = "";

                curData.eUserName = SeleniumHelper.WaitForElement(By.XPath("//*[@id='name']/h1/span/span[1]")).Text;
                curData.eHeadLine = SeleniumHelper.WaitForElement(By.XPath("//*[@id='headline']/p")).Text;
                if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='top-card']/div/div[2]/div[2]/ul/li/dl/dd/a")))
                {
                    curData.eLinkPage = SeleniumHelper.WaitForElement(By.XPath("//*[@id='top-card']/div/div[2]/div[2]/ul/li/dl/dd/a")).Text;
                    tmpObj = "1";
                }
                else
                {
                    //*[@id='top-card']/div/div[2]/div/ul/li/dl/dd/a
                    if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='top-card']/div/div[2]/div/ul/li/dl/dd/a")))
                    {
                        curData.eLinkPage = SeleniumHelper.WaitForElement(By.XPath("//*[@id='top-card']/div/div[2]/div/ul/li/dl/dd/a")).Text;
                        tmpObj = "2";
                    }
                    else
                    {
                        ;
                    }
                }
                if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='background']")))
                {
                    curData.eSummary = SeleniumHelper.WaitForElement(By.XPath("//*[@id='background']")).Text.ToString();
                }

            }
        }
        catch (Exception ex)
        {
            return false;
        }
        if (curData.eLinkPage == "")
            return false;
    }


          ///*
    //* Поиск контактов по запросу
    //* 
    //*/
    public void SearchRequestedContacts()
    {
        int i, j, k, m;
        int cnt = 0;
        string tmpStr = "";
        string locUrl = "";

        DataIn curData;


        // Открыть новую вкладку
        // Обход ключевых слов

        for (i = 0; i < Settings.BotSettings.Category.Count; i++)
        {
            // Обход по странам
            for (j = 0; j < Settings.BotSettings.Country.Count; j++)
            {
                for (k = 0; k < Settings.BotSettings.Position.Count; k++)
                {
                    tmpStr = Settings.BotSettings.Category[i];
                    curIndustry = tmpStr;
                    locUrl = "";
                    locUrl = BuildSearchURL(tmpStr, Settings.BotSettings.Position[k], Settings.BotSettings.Country[j]);
                    // Очистить список
                    data.Clear();
                    // Выполнить поиск
                    browser.LinkedInNavigateTo(locUrl);
                    SeleniumHelper.WaitForElement(By.Id("srp_container"));
                    cnt++;
                    // Разобрать полученный результат
                    ParseResultRequest();
                    // Поиск элементов
                    System.Threading.Thread.Sleep(15000);
                }
            }
        }
        return;
    }
}
