﻿using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using OpenQA.Selenium.Remote;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLinkedIn
{
    /*
     * Класс реализующий работу с сервером http://www.linkedin.com 
     * 
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



    //class CrmHelper
    //{
    //    private Browser browser = Browser.Instance;
    //    private List<PersonTest> personList = new List<PersonTest>();


    class LinkedHelper
    {
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

        public void Login()
        {
            //  Кол-во отсылаемых приглашений
            cntInvite = Settings.BotSettings.CountInvite;
            browser.LinkedInNavigateTo("https://www.linkedin.com/uas/login?goback=&trk=hb_signin");
            SeleniumHelper.WaitForElement(By.Id("session_key-login"));
            var element = browser.LinkedFindElement(By.Id("session_key-login"));
            System.Threading.Thread.Sleep(5000);
            element.Clear();
            element.SendKeys(Settings.BotSettings.LoginLinkedIn);
            //System.Threading.Thread.Sleep(1000);
            SeleniumHelper.WaitForElement(By.Id("session_password-login"));
            element = browser.LinkedFindElement(By.Id("session_password-login"));
            System.Threading.Thread.Sleep(5000);
            element.Clear();
            element.SendKeys(Settings.BotSettings.PasswordLinkedIn);
            //System.Threading.Thread.Sleep(1000);
            SeleniumHelper.WaitForElement(By.Id("btn-primary"));
            element = browser.LinkedFindElement(By.Id("btn-primary"));
            System.Threading.Thread.Sleep(5000);
            element.Click();
            //SeleniumHelper.WaitForElement(By.Id("in-logo"));
            randomGenerator = new Random();
            return;
        }

        //        public void SearchUserCountry ()
        //        {
        //            UserCred testcred = new UserCred();
        //            testcred.email = "i.rebenok@argus-soft.net";
        //            testcred.password = "Iv@N5434LokC";
        //            testcred.index = 1;

        //{
        //    new Account("i.rebenok@argus-soft.net", "Iv@N5434LokC", 1),    // Create a new account
        //    new Account("cfilimonchuk1@gmail.com", "lin147258", 2)    // Create another account
        //};
        //        }



        private string BuildSearchURL(string industry, string position, string country)
        {
            string strUrl, tmpStr;
            tmpStr = "";
            strUrl = "";
            // Формируем ключевое слово для поиска
            // http://www.linkedin.com/vsearch/p?title=ceo&openAdvancedForm=true&titleScope=CP&locationType=Y&sortBy=R&f_N=F,S,A&f_G=us%3A0&f_I=4&rsid=2402161381370938688800&orig=ADVS

            strUrl = "http://www.linkedin.com/vsearch/p?";
            if (position != "")
            {
                tmpStr = position.Replace(" ", "%20");
                strUrl = strUrl + "&title=" + tmpStr;
            }

            strUrl = strUrl + "&openAdvancedForm=true&titleScope=CP&locationType=Y&sortBy=R&&f_N=S";
            if (country != "")
            {
                strUrl = strUrl + "&f_G=" + country + "%3A0";
            }


            if (industry != "")
            {
                //tmpStr = aqString.Replace(keyword,' ','%20'); f_FG
                strUrl = strUrl + "&f_I=" + industry;
                //strUrl = strUrl + "&f_FG=" + industry;

            }
            return strUrl;
        }

        /*
         * Наколение контактов с текущей страницы
         * 
         */
        //так вообще не работает
        public void ParseSearchInfoPage()
        {
            string t1, t2;
            DataIn tst = new DataIn(); ;

            for (int e = 1; e < 10; e++)
            {
                t1 = "//*[@id='results']/li[" + e.ToString() + "]/div/div[3]/a";
                if (SeleniumHelper.IsElementPresent(By.XPath(t1)) == true)
                {
                    var element = SeleniumHelper.WaitForElement(By.XPath(t1));
                    if (element.Text == "Connect")
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
                    //var element2 = SeleniumHelper.WaitForElement(By.XPath("//*[@id='results']/li[" + e.ToString() + "]/div/div[3]/a"));
                    else if (element.Text == "Message")
                    {
                        t1 = "//*[@id='results']/li[" + e.ToString() + "]/div/h3/a";
                        var element1 = SeleniumHelper.WaitForElement(By.XPath(t1));
                        tst.urlPath = element1.GetAttribute("href").ToString();
                        tst.fullName = element1.Text;
                        tst.eUserName = "";
                        tst.eLinkPage = "";
                        tst.eSummary = "";
                        tst.curIndustry = curIndustry;
                        t2 = ".//*[@id='results']/li[" + e.ToString() + "]/div/dl/dd[1]/bdi";
                        var element3 = SeleniumHelper.WaitForElement(By.XPath(t2));
                        tst.userCountry = element3.Text;

                        // Сохраняем для последующей работы
                        data.Add(tst);
                    }


                    return;
                }
            }
        }
        /*
         *  Отсылка запроса на добавлениие нового пользователя
         * 
         *
         */
        public bool SendInvite(ref DataIn curData)
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
            // Отсылаем запрос на Invite
            elButton.Click();
            System.Threading.Thread.Sleep(6000);
            if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='emailAddress-invitee-invitation']")))
            {
                // Обнаружена блокировка account
                // Задержка
                System.Threading.Thread.Sleep(randomGenerator.Next(10, 70) * 60 * 1000);
                return false;
            }
            eFriend = SeleniumHelper.WaitForElement(By.XPath("//*[@id='IF-reason-iweReconnect']"));
            // Элемент указанный ниже не найден на странице, предположим что это кнопка Invite
            eInvite = SeleniumHelper.WaitForElement(By.XPath("//*[@id='send-invite-button']"));
            elInputBox = SeleniumHelper.WaitForElement(By.XPath("//*[@id='greeting-iweReconnect']"));
            eFriend.Click();
            rndTime = randomGenerator.Next(5, 100);
            // Очистить текст сообщения
            if (File.Exists("data\\Messages\\Default.txt"))
            {
                tmpStr = System.IO.File.ReadAllText("data\\Messages\\Default.txt");
                elInputBox.Clear();
                elInputBox.SendKeys(tmpStr);
            }
            // Загрузить текст сообщения
            System.Threading.Thread.Sleep(rndTime * 1000);
            eInvite.Click();
            return true;
        }
        /*
         * Обработка информации запроса
         * 
         * 
         */
        public void ParseResultRequest()
        {
            int i, m;
            string pageUrl;
            string tmp;
            DataIn curData;


            List<string> pages = new List<string>();
            pages.Clear();
            ParseSearchInfoPage();
            for (i = 2; i < 11; i++)
            {
                pageUrl = "//*[@id='results-pagination']/ul/li[" + i.ToString() + "]/a";
                if (SeleniumHelper.IsElementPresent(By.XPath(pageUrl)) == true)
                {
                    var element = SeleniumHelper.WaitForElement(By.XPath(pageUrl));
                    tmp = element.GetAttribute("href");
                    // Добавить страницу для последующих поисков контакта
                    pages.Add(tmp);
                }
            }
            // Обрабатываем  следующие страницы для добавления
            for (i = 0; i < pages.Count; i++)
            {
                tmp = pages[i];
                browser.LinkedInNavigateTo(tmp);
                if (SeleniumHelper.IsElementPresent(By.Id("srp_container")) == true)
                {
                    ParseSearchInfoPage();
                    System.Threading.Thread.Sleep(5000);
                }
            }

            crmView.SetSearchMode(0);
            // Осуществить проход по страницам пользователей
            for (m = 0; m < data.Count; m++)
            {
                curData = data[m];
                if (crmView.SearchContact(curData.fullName, "") == true)
                {
                    // Такой контакт уже существует
                    continue;
                }
                else
                {
                    if (SendInvite(ref curData) == true)
                    {
                        crmView.CreateNewUserRecord(curData);
                        crmView.SearchContact("", "");
                        cntInvite--;
                        if (cntInvite < 0)
                        {
                            i = 0;
                            while (true)
                            {
                                System.Threading.Thread.Sleep(60000);
                                i++;
                            }
                        }
                    }
                }
            }

            return;
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
                         //Выполнить поиск
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
        // If user accepted an invitation

        public void CreateAcceptedRecord(string urlProfile)
        {
            IWebElement elName;
            DataIn person = new DataIn();
            // Переходим на страницу 
            browser.SetCurrentWindow(0);
            browser.LinkedInNavigateTo(urlProfile);

            person.messageText = "";
            person.isMessageSend = true;
            person.eLinkPage = urlProfile;
            person.curIndustry = "0";

            try
            {
                // Поучаем данные с элементов профиля пользователя
                //*[@id='tc-actions-send-message']
                elName = SeleniumHelper.WaitForElement(By.XPath("//*[@id='tc-actions-send-message']"));
                if (elName.Text != "Send a message")
                {
                    return;
                }
                // Получить имя пользователя
                elName = SeleniumHelper.WaitForElement(By.XPath("//*[@id='name']/h1/span/span[1]"));
                person.eUserName = elName.Text;
                // Текущая должность пользователя
                elName = SeleniumHelper.WaitForElement(By.XPath("//*[@id='headline']/p"));
                person.eHeadLine = elName.Text;
                // Резюме пользователя
                elName = SeleniumHelper.WaitForElement(By.XPath("//*[@id='background']"));
                person.eSummary = elName.Text;
                // Создаем новую запись
                crmView.CreateNewUserRecord(person);
                browser.SetCurrentWindow(0);
            }
            catch (Exception ex)
            {
                return;
            }

            return;
        }
        // Functionality needs to be changed as url is no longer exists, now functionality should be implemented through Connections       
        public void ProcessingAccepted()
        {
            IWebElement elLabel, elName;
            int i, j, countAccepted, k;
            int cntPage, allRecord;
            string tmpString;
            string userName;
            string urlStr;
            List<string> listUrl = new List<string>();

            crmView.SetSearchMode(0);
            urlStr = "https://www.linkedin.com/inbox/#sent?subFilter=invitation&keywords=&sortBy=";
            browser.LinkedInNavigateTo(urlStr);
            // //*[@id='message-list']/form/ol/li[10] Этикетка
            // //*[@id='message-list']/form/ol/li[3]/div/span[2] Надпись Accepted
            // /inbox/#sent?startRow=11&subFilter=invitation&keywords=&sortBy=
            j = 11;
            countAccepted = 0;
            cntPage = 0;
            allRecord = 0;
            listUrl.Clear();
            while (true)
            {
                k = 0;
                System.Threading.Thread.Sleep(5000);
                for (i = 1; i < 11; i++)
                {
                    tmpString = "//*[@id='message-list']/form/ol/li[" + i.ToString() + "]";

                    if (SeleniumHelper.IsElementPresent(By.XPath(tmpString)))
                    {
                        tmpString = "//*[@id='message-list']/form/ol/li[" + i.ToString() + "]/div/span[2]";
                        if (SeleniumHelper.IsElementPresent(By.XPath(tmpString)))
                        {
                            elLabel = SeleniumHelper.WaitForElement(By.XPath(tmpString));
                            if (elLabel.Text != "" && elLabel.Text.Contains("Accepted"))
                            {
                                elName = SeleniumHelper.WaitForElement(By.XPath("//*[@id='message-list']/form/ol/li[" + i.ToString() + "]/div/span[1]"));
                                userName = elName.Text;
                                userName = userName.Replace("To: ", "");
                                if (crmView.SearchContact(userName, "") == false)
                                {
                                    //elName.Click();
                                    browser.SetCurrentWindow(0);
                                    try
                                    {
                                        elName = SeleniumHelper.WaitForElement(By.XPath("//*[@id='message-list']/form/ol/li[" + i.ToString() + "]/div/div[2]/p/a"));
                                        //*[@id='message-list']/form/ol/li[2]/div/div[2]/p/a
                                        tmpString = elName.GetAttribute("href");
                                        browser.LinkedInNavigateTo(tmpString);
                                        System.Threading.Thread.Sleep(3000);
                                        //*[@id='content']/div/div[2]/p[2]/span[2]/span/strong/a
                                        // Получить ссылку на профиль пользователя для сохранени
                                        elName = SeleniumHelper.WaitForElement(By.XPath("//*[@id='content']/div/div[2]/p[2]/span[2]/span/strong/a"));
                                        tmpString = elName.GetAttribute("href");
                                        listUrl.Add(tmpString);
                                        //CreateAcceptedRecord(tmpString);

                                        System.Threading.Thread.Sleep(3000);
                                    }
                                    catch (Exception ex)
                                    {
                                        ;
                                    }
                                    browser.LinkedInNavigateTo(urlStr);
                                    System.Threading.Thread.Sleep(3000);
                                    allRecord++;
                                }
                                else
                                {
                                    if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='MassUpdate']/table/tbody/tr[3]/td[5]")))
                                    {
                                        elName = SeleniumHelper.WaitForElement(By.XPath("//*[@id='MassUpdate']/table/tbody/tr[3]/td[5]"));
                                        tmpString = elName.Text;
                                        listUrl.Add(tmpString);
                                        System.Threading.Thread.Sleep(3000);

                                    }
                                }
                                browser.SetCurrentWindow(0);
                                countAccepted++;
                                k++;
                            }
                        }
                    }
                }
                // Переход на следующую страницу
                j += 10;
                cntPage++;
                // Это последняя страница?
                if (cntPage == 35)
                    break;
                tmpString = "inbox/#sent?startRow=" + j.ToString() + "&subFilter=invitation&keywords=&sortBy=";
                urlStr = "https://www.linkedin.com/" + tmpString;
                browser.LinkedInNavigateTo(urlStr);
            }
            for (i = 0; i < listUrl.Count; i++)
            {
                CreateAcceptedRecord(listUrl[i]);
            }
            return;
        }

        
        ///*
        //*Получаем ссылку на профиль для поиска в CRM
        //* 
        //*/
        public void ParseSearchResult()
        {
            string t1, t2, t3, t4;

            DataIn tst = new DataIn();

            for (int e = 1; e < 10; e++)
            {       
                 t2 = ".//*[@id='results']/li/div/div[4]/a";
                var element2 = SeleniumHelper.WaitForElement(By.XPath(t2));
                if (SeleniumHelper.IsElementPresent(By.XPath(t2)) == true)
                {
                    if (element2.Text == "Message")
                    {
                        t1 = ".//*[@id='results']/li/div/h3/a";
                        var element1 = SeleniumHelper.WaitForElement(By.XPath(t1));
                        tst.urlPath = element1.GetAttribute("href").ToString();
                        tst.fullName = element1.Text;
                        tst.eUserName = "";
                        tst.eLinkPage = "";
                        t3 = ".//*[@id='results']/li/div/dl/dd[1]/bdi";
                        var element3 = SeleniumHelper.WaitForElement(By.XPath(t3));
                        tst.userCountry = element3.Text;
                        var element5 = SeleniumHelper.WaitForElement(By.XPath(".//*[@id='results']/li/div/dl/dd[2]"));
                        tst.curIndustry = element5.Text;
                    // Сохраняем для последующей работы
                    data.Add(tst);
                    }
                }


                else if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='results']/li[1]/div/div[4]/a")) == true)
                {
                //for (int e = 1; e < 10; e++)
                //{
                    var element1 = SeleniumHelper.WaitForElement(By.XPath("//*[@id='results']/li[" + e.ToString() + "]/div/div[4]/a"));
                    if (element1.Text == "Message")
                    {
                        t1 = "//*[@id='results']/li[" + e.ToString() + "]/div/h3/a";
                          var  element4 = SeleniumHelper.WaitForElement(By.XPath(t1));
                        tst.urlPath = element4.GetAttribute("href").ToString();
                        tst.fullName = element4.Text;
                        tst.eUserName = "";
                        tst.eLinkPage = "";
                        t2 = ".//*[@id='results']/li[" + e.ToString() + "]/div/dl/dd[1]/bdi";
                        var element3 = SeleniumHelper.WaitForElement(By.XPath(t2));
                        tst.userCountry = element3.Text;
                        t4 = ".//*[@id='results']/li[" + e.ToString() + "]/div/dl/dd[2]";
                        element4 = SeleniumHelper.WaitForElement(By.XPath(t4));
                        tst.curIndustry = element4.Text;
                        // Сохраняем для последующей работы
                        data.Add(tst);

                    }
                   return;
                }
            }
        }
    public bool GetProfileLink(ref DataIn curData)
        {
            IWebElement elButton;
            string tmpObj;
           
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
                        {
                            return false;
                        }
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
                if (elButton.Text == "Send a message")
                {
                    curData.eUserName = "";
                    curData.eHeadLine = "";
                    curData.eLinkPage = "";


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
            //if (curData.eLinkPage == "")
                return true;
        }

        /*
             * Обработка информации запроса c добавлением страны в CRM
             * 
             * 
             */
        public void ParseResult()
        {
            int i, m;
            string pageUrl;
            string tmp;
            DataIn curData;
            List<DataIn> Users = new List<DataIn>();
            List<string> pages = new List<string>();

            pages.Clear();
            //ParseSearchInfoPage();
            ParseSearchResult();
            for (i = 2; i < 11; i++)
            {
                pageUrl = "//*[@id='results-pagination']/ul/li[" + i.ToString() + "]/a";
                if (SeleniumHelper.IsElementPresent(By.XPath(pageUrl)) == true)
                {
                    var element = SeleniumHelper.WaitForElement(By.XPath(pageUrl));
                    tmp = element.GetAttribute("href");
                    // Добавить страницу для последующих поисков контакта
                    pages.Add(tmp);
                }
            }
            // Обрабатываем  следующие страницы для добавления
            for (i = 0; i < pages.Count; i++)
            {
                tmp = pages[i];
                browser.LinkedInNavigateTo(tmp);
                if (SeleniumHelper.IsElementPresent(By.Id("srp_container")) == true)
                {
                    ParseSearchResult();
                    //ParseSearchInfoPage();
                    System.Threading.Thread.Sleep(5000);
                }
            }

            crmView.SetSearchMode(0);
            // Осуществить проход по страницам пользователей
            for (m = 0; m < data.Count; m++)
            {
                curData = data[m];
                // if ((GetProfileLink(ref curData) == true) && (crmView.SearchContact(curData.fullName, curData.eLinkPage) == true))
                if ((GetProfileLink(ref curData) == true))
                {
                    if (crmView.SearchContact(curData.fullName, curData.eLinkPage) == true)
                    {
                        // Такой контакт уже существует

                        //    //добавить страну в CRM
                        //    crmView.AddUserCountry(curData.fullName, curData.userCountry);
                        //    Users.Add(curData);
                        //    continue;
                        //}
                        //else
                        //{
                        //    crmView.CreateNewUserRecord(curData);
                        //    crmView.SearchContact(curData.fullName, curData.eLinkPage);
                        //    crmView.AddUserCountry(curData.fullName, curData.userCountry);
                        //    Users.Add(curData);
                        //}
                        //    System.Threading.Thread.Sleep(60000);
                        //    i++;
                        //}

                    }
                    // выводим список
                    foreach (DataIn item in Users)
                        Console.WriteLine(item);
                    Console.ReadLine();


                    return;

                }
            }
        }
       
  

         ///*
            //* Поиск контактов по стране
            //* 
            //*/
        public void SearchByCountry()
{
    int j, k, m;
    int cnt = 0;

    string locUrl = "https://www.linkedin.com/vsearch/p?f_N=F,A&rsid=3858883691477677585309&openFacets=N,G,CC&f_G=va%3A0&orig=FCTD";
    //string strUrl;
    DataIn curData;

    //Обход по странам
    browser.LinkedInNavigateTo(locUrl);
    SeleniumHelper.WaitForElement(By.Id("srp_container"));
    cnt++;
            //Разобрать результаты поиска
            //ParseSearchInfoPage();
           // ParseSearchResult();
            //Получить ссылку со страницы и страну в ParseResult

            //Разобрать полученный результат
            ParseResult();
             //Вывести список
            //MessageBox.Show("Name: " + user.name + Environment.NewLine + "Email: " + user.email + Environment.NewLine + "Age: " + user.age + Environment.NewLine);
            // Console.WriteLine(curData.ToString());
             System.Threading.Thread.Sleep(15000);
            }        
          }
    }
//}
        




   
