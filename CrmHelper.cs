using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotLinkedIn
{
    /*
     * Класс реализующий работу с CRM 
     * 
     */
    //internal struct PersonIn
    //{
    //    internal string curIndustry;
    //    internal string crmLink;
    //    internal string urlLink;
    //    internal bool isMessageSend;
    //    internal string messageText;
    //}
    //added for testing need to be deleted later
    internal struct PersonTest
    {
        internal string crmLink;
        internal string linkedinlink;
        internal string messageText;
        //internal bool isMessageSend;
    }
     
     
    class CrmHelper
    {
        private Browser browser = Browser.Instance;
        private List<PersonTest> personList = new List<PersonTest>();
       // private List<PersonIn> personList = new List<PersonIn>();

        private string GetBaseUrl()
        {
            if (Settings.BotSettings.CurrentUser == "Gena" || Settings.BotSettings.CurrentUser == "")
            {
                return "http://printers.smart.argus/sugarcrm/";
            }
            else
            {
                return "http://crm.smart.argus/";
            }
        }
        private LinkedHelper linkedView = new LinkedHelper();
        public void SetLinkedHelper(LinkedHelper view)
        {
            linkedView = view;
            return ;
        }

        public void Login()
        {
            IWebElement element;
            browser.CrmNavigateTo(GetBaseUrl());

            element = SeleniumHelper.WaitForElement(By.Name("user_name"));
            element.Clear();
            element.SendKeys(Settings.BotSettings.LoginCrm);

            element = SeleniumHelper.WaitForElement(By.Name("user_password"));
            element.Clear();
            element.SendKeys(Settings.BotSettings.PasswordCrm);

            element = SeleniumHelper.WaitForElement(By.Name("Login"));
            element.Click();
            element = SeleniumHelper.WaitForElement(By.XPath("//*[@id='companyLogo']"));
            browser.CrmNavigateTo(GetBaseUrl() + "index.php?module=Contacts&action=index&return_module=Contacts&return_action=DetailView");
            return;
        }

        public bool SearchContact(string userName, string userUrl)
        {
            IWebElement elUserName, elUserUrl, elUserSearch;
            string firstName, lastName;
            browser.CrmNavigateTo(GetBaseUrl() + "index.php?module=Contacts&action=index&return_module=Contacts&return_action=DetailView");

            try
            {
                if (SeleniumHelper.IsElementPresent(By.Id("search_name_basic")))
                {
                    elUserName = SeleniumHelper.WaitForElement(By.Id("search_name_basic"));
                    elUserName.Clear();
                    if (userName != "")
                    {
                        firstName = userName.Split(' ')[0];
                        lastName = userName.Split(' ')[1];
                        elUserName.SendKeys(firstName + " " + lastName);
                    }
                }
                if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='social_nework_url_c_basic']")))
                {
                    elUserUrl = SeleniumHelper.WaitForElement(By.XPath("//*[@id='social_nework_url_c_basic']"));
                    elUserUrl.Clear();
                    if (userUrl != "")
                    {
                        elUserUrl.SendKeys(userUrl);
                    }
                }
                if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='search_form_submit']")))
                {
                    elUserSearch = SeleniumHelper.WaitForElement(By.XPath("//*[@id='search_form_submit']"));
                    elUserSearch.Submit();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='MassUpdate']/table/tbody/tr[3]/td[3]/b/a")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CreateNewUserRecord(DataIn userData)
        {
            string firstName, lastName;
            IWebElement element;

            firstName = userData.eUserName.Split(' ')[0];
            lastName = userData.eUserName.Split(' ')[1];
            browser.SetCurrentWindow(1);
            browser.CrmNavigateTo(GetBaseUrl() + "index.php?module=Contacts&action=EditView&return_module=Contacts&return_action=index");
            // Получаем элементы для доступа
            // Имя
            if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='first_name']")))
            {
                element = SeleniumHelper.WaitForElement(By.XPath("//*[@id='first_name']"));
                element.Clear();
                element.SendKeys(firstName);
            }

            // Фамилия
            if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='last_name']")))
            {
                element = SeleniumHelper.WaitForElement(By.XPath("//*[@id='last_name']"));
                element.Clear();
                element.SendKeys(lastName);
            }

            // Должность
            if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='title']")))
            {
                element = SeleniumHelper.WaitForElement(By.XPath("//*[@id='title']"));
                element.Clear();
                element.SendKeys(userData.eHeadLine);
            }

            // Индустрия
            if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='industryid_c']")))
            {
                element = SeleniumHelper.WaitForElement(By.XPath("//*[@id='industryid_c']"));
                element.Clear();
                element.SendKeys(userData.curIndustry);
            }

            // Ссылка на профиль
            if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='social_nework_url_c']")))
            {
                element = SeleniumHelper.WaitForElement(By.XPath("//*[@id='social_nework_url_c']"));
                element.Clear();
                element.SendKeys(userData.eLinkPage);
            }

            // Описание
            if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='description']")))
            {
                element = SeleniumHelper.WaitForElement(By.XPath("//*[@id='description']"));
                element.Clear();
                try
                {
                    // Если нет возможности набрать оставляем как есть
                    //element.SendKeys(userData.eSummary);
                    ;
                }
                catch (Exception ex)
                {
                    ;
                }
            }
            //*[@id='messagewassent_c']
            // Признак отсылки сообщения
            if (SeleniumHelper.IsElementPresent(By.Id("messagewassent_c")))
            {
                element = SeleniumHelper.WaitForElement(By.Id("messagewassent_c"));
                if (userData.isMessageSend == true)
                    element.Click();
            }
            // Кнопка сохранить
            if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='SAVE_HEADER']")))
            {
                element = SeleniumHelper.WaitForElement(By.XPath("//*[@id='SAVE_HEADER']"));
                element.Click();
            }
            return;
        }

        /*
         * Переключение режима поиска
         * 
         * mode - 0 - Базовый режим поиска
         *        1 - Расширенный режим поиска
         */
        public void SetSearchMode(int mode)
        {
            IWebElement element;
            browser.CrmNavigateTo(GetBaseUrl() + "index.php?module=Contacts&action=index&return_module=Contacts&return_action=DetailView");
            if (mode == 0)
            {
                // Режим базового поиска //*[@id='basic_search_link']
                if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='basic_search_link']")) == true)
                {
                    // Режим поиска "Базовый"
                    element = SeleniumHelper.WaitForElement(By.XPath("//*[@id='basic_search_link']"));
                    element.Click();
                }
                ;
            }
            if (mode == 1)
            {
                // Режим расширенного поиска
                if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='advanced_search_link']")) == true)
                {
                    // Режим поиска "Базовый"
                    element = SeleniumHelper.WaitForElement(By.XPath("//*[@id='advanced_search_link']"));
                    element.Click();
                }
            }
            return;
        }

        //public void AddPersonRecord()
        //{
        //    IWebElement elcrmLink, elUserLink, elIndustry;
        //    string tmpStr, tStr;
        //    int i, j;
        //    PersonIn person = new PersonIn();
        //    //*[@id='MassUpdate']/table/tbody/tr[4]/td[3]/b/a
        //    //*[@id='MassUpdate']/table/tbody/tr[4]/td[5]
        //    //*[@id='MassUpdate']/table/tbody/tr[4]/td[8]
        //    // .//*[@id='listViewNextButton_bottom']
        //    for (i = 0; i < 20; i++)
        //    {
        //        tmpStr = "//*[@id='MassUpdate']/table/tbody/tr[" + (i + 3).ToString() + "]/td[3]/b/a";
        //        if (SeleniumHelper.IsElementPresent(By.XPath(tmpStr)))
        //        {
        //            elcrmLink = SeleniumHelper.WaitForElement(By.XPath(tmpStr));
        //            tmpStr = "//*[@id='MassUpdate']/table/tbody/tr[" + (i + 3).ToString() + "]/td[5]";
        //            elUserLink = SeleniumHelper.WaitForElement(By.XPath(tmpStr));
        //            tmpStr = "//*[@id='MassUpdate']/table/tbody/tr[" + (i + 3).ToString() + "]/td[8]";
        //            elIndustry = SeleniumHelper.WaitForElement(By.XPath(tmpStr));
        //            if (elIndustry.Text == "")
        //                person.curIndustry = "0";
        //            else
        //                person.curIndustry = elIndustry.Text;
        //            person.curIndustry = person.curIndustry.Replace(" ", "");
        //            person.urlLink = elUserLink.Text;
        //            person.crmLink = elcrmLink.GetAttribute("href").ToString();
        //            person.messageText = "";
        //            person.isMessageSend = false;
        //            if (Settings.BotSettings.listIndustry[0] == "")
        //            {
        //                // Заполнение поля Меssage
        //                tStr = "data\\MassEmail\\" + Settings.BotSettings.CurrentUser + "\\Default.txt";
        //                if (File.Exists(tStr))
        //                {
        //                    tmpStr = System.IO.File.ReadAllText(tStr);
        //                    person.messageText = tmpStr;
        //                    person.isMessageSend = true;
        //                }
        //                else
        //                    continue;
        //                // Добавление в список
        //                personList.Add(person);
        //                continue;
        //            }

        //            for (j = 0; j < Settings.BotSettings.listIndustry.Count(); j++)
        //            {
        //                tStr = Settings.BotSettings.listIndustry[j];
        //                if (tStr == person.curIndustry)
        //                {
        //                    // Заполнение поля Меssage
        //                    tStr = "data\\MassEmail\\" + Settings.BotSettings.CurrentUser + "\\Message_" + person.curIndustry + ".txt";
        //                    if (File.Exists(tStr))
        //                    {
        //                        tmpStr = System.IO.File.ReadAllText(tStr);
        //                        person.messageText = tmpStr;
        //                        person.isMessageSend = true;
        //                    }
        //                    else
        //                        continue;
        //                    // Добавление в список
        //                    personList.Add(person);
        //                    continue;
        //                }
        //            }
        //        }
        //        else
        //            break;
        //    }

        //    return;
        //}

        //bool UnSetMessageSend(PersonTest person)
        //{
        //    IWebElement tstItem;
        //    int i = 0;

        //    browser.CrmNavigateTo(person.crmLink);
        //    System.Threading.Thread.Sleep(3000);
        //    try
        //    {
        //        tstItem = SeleniumHelper.WaitForElement(By.XPath("//*[@id='edit_button']"));
        //        tstItem.Click();
        //        System.Threading.Thread.Sleep(3000);

        //        tstItem = SeleniumHelper.WaitForElement(By.XPath("//*[@id='messagewassent_c']"));
        //        if (tstItem.Selected == true)
        //        {
        //            tstItem.Click();
        //        }
        //        //tstItem = SeleniumHelper.WaitForElement(By.XPath("//*[@id='edit_button']"));
        //        //tstItem.Click();
        //        tstItem = SeleniumHelper.WaitForElement(By.XPath("//*[@id='SAVE_HEADER']"));
        //        tstItem.Click();
        //        System.Threading.Thread.Sleep(3000);

        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        /*
         * Отсылка пользовательского сообщения Пользователю
         * 
         */
        //rename
        
        // testing - public void SendMailUser()
        //public bool SendMailUser(PersonIn person)
        //{
        //    IWebElement elButton, elSubject, elMessage, elSendButton, elCancelButton, elUserName;
        //    string tmpStr, firstName, lastName, fullName;
        //    try
        //    {
        //        // Перейти на страницу пользователя
        //        browser.LinkedInNavigateTo(person.urlLink);
        //        //browser.LinkedInNavigateTo("https://mx.linkedin.com/in/nathan-green-17018a127");
        //        elButton = SeleniumHelper.WaitForElement(By.XPath("//*[@id='tc-actions-send-message']"));
        //        if (elButton.Text == "Send a message")
        //        {
        //            elUserName = SeleniumHelper.WaitForElement(By.XPath("//*[@id='name']/h1/span/span[1]"));
        //            fullName = elUserName.Text;

        //            elButton.Click();
        //            //System.Threading.Thread.Sleep(3000);
        //            elSubject = SeleniumHelper.WaitForElement(By.XPath("//*[@id='subject-msgForm']"));
        //            elMessage = SeleniumHelper.WaitForElement(By.XPath("//*[@id='body-msgForm']"));
        //            elCancelButton = SeleniumHelper.WaitForElement(By.XPath("//*[@class='dialog-close']"));
        //            elSendButton = SeleniumHelper.WaitForElement(By.XPath("//*[@id='compose-dialog-submit']"));
        //            // Установить заголовок сообщения
        //            elSubject.Clear();
        //            firstName = fullName.Split(' ')[0];
        //            lastName = fullName.Split(' ')[1];

        //            tmpStr = Settings.BotSettings.SubjectMessage;
        //            tmpStr = tmpStr.Replace("%FullName", fullName);
        //            tmpStr = tmpStr.Replace("%FirstName", firstName);
        //            tmpStr = tmpStr.Replace("%LastName", lastName);
        //            elSubject.SendKeys(tmpStr);

        //            tmpStr = person.messageText;
        //            tmpStr = tmpStr.Replace("%FullName", fullName);
        //            tmpStr = tmpStr.Replace("%FirstName", firstName);
        //            tmpStr = tmpStr.Replace("%LastName", lastName);
        //            elMessage.Clear();
        //            elMessage.SendKeys(tmpStr);
        //            //elCancelButton.Click();
        //            elSendButton.Click();
        //            //System.Threading.Thread.Sleep(5000);
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //    return true;
        //}
        
        /*
       * Отсылка пользовательского сообщения Пользователю test с voidом
       * 
       */
        //public void SendMailUser()
        public bool SendMailUser(PersonTest person)
        {
            // elLogin, elPassword, signButton добавлены из-за блокировки сессии (Линкедин запрашивает авторизацию перед переходом на страницу юзера для отправки сообщения
            IWebElement elButton, elSubject, elMessage, elSendButton, elCancelButton, elUserName, elPassword, signButton, elLogin;
            string tmpStr, firstName, lastName, fullName;
            try
            {
                //        // Перейти на страницу пользователя
                browser.LinkedInNavigateTo(person.linkedinlink);
                //browser.LinkedInNavigateTo("https://mx.linkedin.com/in/nathan-green-17018a127");
               // elButton = SeleniumHelper.WaitForElement(By.XPath("//*[@id='tc-actions-send-message']"));
              
                if (SeleniumHelper.IsElementPresent(By.Id("session_key-login")))
                {
                    SeleniumHelper.WaitForElement(By.Id("session_key-login"));
                    elLogin = browser.LinkedFindElement(By.Id("session_key-login"));
                    System.Threading.Thread.Sleep(5000);
                    elLogin.Clear();
                    elLogin.SendKeys(Settings.BotSettings.LoginLinkedIn);
                    SeleniumHelper.WaitForElement(By.XPath(".//*[@id='session_password-login']"));
                    elPassword = browser.LinkedFindElement(By.XPath(".//*[@id='session_password-login']"));
                    elPassword.SendKeys(Settings.BotSettings.PasswordLinkedIn);
                    System.Threading.Thread.Sleep(5000);
                    SeleniumHelper.WaitForElement(By.Id("btn-primary"));
                    signButton = browser.LinkedFindElement(By.Id("btn-primary"));
                    System.Threading.Thread.Sleep(5000);
                    signButton.Click();
                    //linkedView.LoginSessionBlocked();
                }
                else
                {
                    return false;
                }
                elButton = SeleniumHelper.WaitForElement(By.XPath("//*[@id='tc-actions-send-message']"));
                if (elButton.Text == "Send a message")
                {
                    elUserName = SeleniumHelper.WaitForElement(By.XPath("//*[@id='name']/h1/span/span[1]"));
                    fullName = elUserName.Text;

                    elButton.Click();
                    System.Threading.Thread.Sleep(3000);
                    elSubject = SeleniumHelper.WaitForElement(By.XPath("//*[@id='subject-msgForm']"));
                    elMessage = SeleniumHelper.WaitForElement(By.XPath("//*[@id='body-msgForm']"));
                    elCancelButton = SeleniumHelper.WaitForElement(By.XPath("//*[@class='dialog-close']"));
                    elSendButton = SeleniumHelper.WaitForElement(By.XPath("//*[@id='compose-dialog-submit']"));
                    // Установить заголовок сообщения
                    elSubject.Clear();
                    firstName = fullName.Split(' ')[0];
                    lastName = fullName.Split(' ')[1];

                    tmpStr = Settings.BotSettings.SubjectMessage;
                    tmpStr = tmpStr.Replace("%FullName", fullName);
                    tmpStr = tmpStr.Replace("%FirstName", firstName);
                    tmpStr = tmpStr.Replace("%LastName", lastName);
                    elSubject.SendKeys(tmpStr);

                    tmpStr = person.messageText;
                    tmpStr = tmpStr.Replace("%FullName", fullName);
                    tmpStr = tmpStr.Replace("%FirstName", firstName);
                    tmpStr = tmpStr.Replace("%LastName", lastName);
                    elMessage.Clear();
                    elMessage.SendKeys(tmpStr);
                    //elCancelButton.Click();
                    elSendButton.Click();
                   // System.Threading.Thread.Sleep(5000);
                    //        }
                    //        else if (SeleniumHelper.IsElementPresent(By.XPath(".//*[@id='session_key-login']")))
                    //        {
                    //            elLogin.Clear();
                    //        }
                    //        if (SeleniumHelper.IsElementPresent(By.XPath(".//*[@id='session_key-login']")))
                    //        {
                    //            linkedView.Login();
                    //        }
                    //        return false;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        return false;
                    //    }
                    //}
                    //    return true;
                    //}

                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public void EmailMassSending()
        {
            // IWebElement elList, elButton, elDateField, tstItem;
            IWebElement elList, elButton, elDateField;
            //changed curDate from (2013, 3, 1) than change it back
            //DateTime curDate = new DateTime(2013, 9, 16);
            DateTime curDate = new DateTime(2013, 7, 12);
            //DateTime curDate = new DateTime(2014, 1, 8);
            DateTime endTime = new DateTime(2016, 9, 20);
            string tmpStr = "";
            int i;
            // Переключится на режим расширенного поиска

            SetSearchMode(1);
            personList.Clear();
            elList = SeleniumHelper.WaitForElement(By.XPath("//*[@id='messagewassent_c_advanced']"));
            SelectElement sel = new SelectElement(elList);
            sel.SelectByText("Да");
            //sel.SelectByText("Нет");
            //elList = SeleniumHelper.WaitForElement(By.XPath("//*[@id='assigned_user_id_advanced']"));
            //sel = new SelectElement(elList);
            //sel.SelectByText(Settings.BotSettings.FullName);             
            // Список кому запрещено отсылать
            elList = SeleniumHelper.WaitForElement(By.XPath("//*[@id='do_not_call_advanced']"));
            sel = new SelectElement(elList);
            sel.SelectByText("Нет");
            elList = SeleniumHelper.WaitForElement(By.Id("assigned_user_id_advanced"));
            sel = new SelectElement(elList);
            tmpStr = Settings.BotSettings.FullName.ToString();
            sel.SelectByText(tmpStr);
            elDateField = SeleniumHelper.WaitForElement(By.XPath("//*[@id='range_date_sent_4_c_advanced']"));
            elDateField.Clear();

            elButton = SeleniumHelper.WaitForElement(By.XPath("//*[@id='search_form_submit_advanced']"));
            tmpStr = curDate.Day.ToString() + "/" + curDate.Month.ToString() + "/" + curDate.Year.ToString();

            //endTime = DateTime.Now.Date.AddDays(1);
            endTime = endTime.AddDays(1);
            elDateField = SeleniumHelper.WaitForElement(By.XPath("//*[@id='range_date_entered_advanced']"));
            elDateField.Clear();
            elDateField.SendKeys(tmpStr);

            i = 0;
            while (true)
            {
                tmpStr = curDate.Day.ToString() + "/" + curDate.Month.ToString() + "/" + curDate.Year.ToString();
                elDateField = SeleniumHelper.WaitForElement(By.XPath("//*[@id='range_date_entered_advanced']"));
                elButton = SeleniumHelper.WaitForElement(By.XPath("//*[@id='search_form_submit_advanced']"));

                elDateField.Clear();
                elDateField.SendKeys(tmpStr);
                elButton.Click();
                System.Threading.Thread.Sleep(5000);
                if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='MassUpdate']/div[1]/p")) == false)
                {
                    // Найдена новая запись
                    // AddPersonRecord();
                    AddPersonRecordForMassEmail();
                    //curDate = curDate.AddDays(1);
                }
                curDate = curDate.AddDays(1);
                if (curDate.CompareTo(endTime) == 0)
                {
                    break;
                }
                               
            }
            for (i = 0; i < personList.Count(); i++)
            {
                SendMailUser(personList[i]);
                //if (SendMailUser(personList[i]) == false)
                //{
                //    UnSetMessageSend(personList[i]);
                //}
              }
            return;
        }
        public void AddPersonRecordForMassEmail()
        {
            IWebElement elcrmLink, elUserLink;
            string tmpStr, tStr;
            int i, j;
            PersonTest person = new PersonTest();
            //    *[@id='MassUpdate']/table/tbody/tr[4]/td[3]/b/a
            //    *[@id='MassUpdate']/table/tbody/tr[4]/td[5]
            //    *[@id='MassUpdate']/table/tbody/tr[4]/td[8]
            //     .//*[@id='listViewNextButton_bottom']
            for (i = 0; i < 20; i++)
            {
                //имя пользователя в результатах поиска сrmLink
                tmpStr = "//*[@id='MassUpdate']/table/tbody/tr[" + (i + 3).ToString() + "]/td[3]/b/a";
                if (SeleniumHelper.IsElementPresent(By.XPath(tmpStr)))
                {
                    elcrmLink = SeleniumHelper.WaitForElement(By.XPath(tmpStr));
                    //linkedIn Link
                    tmpStr = "//*[@id='MassUpdate']/table/tbody/tr[" + (i + 3).ToString() + "]/td[5]";
                    elUserLink = SeleniumHelper.WaitForElement(By.XPath(tmpStr));
                    // industryId
                    //tmpStr = "//*[@id='MassUpdate']/table/tbody/tr[" + (i + 3).ToString() + "]/td[8]";
                    //elIndustry = SeleniumHelper.WaitForElement(By.XPath(tmpStr));
                    //if (elIndustry.Text == "")
                    //    person.curIndustry = "0";
                    //else
                    //    person.curIndustry = elIndustry.Text;
                    //person.curIndustry = person.curIndustry.Replace(" ", "");
                    person.linkedinlink = elUserLink.Text;
                    person.crmLink = elcrmLink.GetAttribute("href").ToString();
                    //      здесь была работа с сообщениями 
                    // Заполнение поля Меssage
                    tStr = "data\\MassEmail\\" + Settings.BotSettings.CurrentUser + "\\Default.txt";
                    if (File.Exists(tStr))
                    {
                        tmpStr = System.IO.File.ReadAllText(tStr);
                        person.messageText = tmpStr;
                        //person.isMessageSend = true;
                    }
                    else
                        continue;
                    //                 Добавление в список
                    personList.Add(person);
                    //                здесь был сontinue
                    return;
                }

                //
                  public bool SearchContactByName(string userName)
        {
            IWebElement elUserName, elUserSearch;
            string firstName, lastName;
            browser.CrmNavigateTo(GetBaseUrl() + "index.php?module=Contacts&action=index&return_module=Contacts&return_action=DetailView");

            try
            {
                if (SeleniumHelper.IsElementPresent(By.Id("search_name_basic")))
                {
                    elUserName = SeleniumHelper.WaitForElement(By.Id("search_name_basic"));
                    elUserName.Clear();
                    if (userName != "")
                    {
                        firstName = userName.Split(' ')[0];
                        lastName = userName.Split(' ')[1];
                        elUserName.SendKeys(firstName + " " + lastName);
                    }
                }
               
                if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='search_form_submit']")))
                {
                    elUserSearch = SeleniumHelper.WaitForElement(By.XPath("//*[@id='search_form_submit']"));
                    elUserSearch.Submit();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='MassUpdate']/table/tbody/tr[3]/td[3]/b/a")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //                continue;
        //            }

        //            for (j = 0; j < Settings.BotSettings.listIndustry.Count(); j++)
        //            {
        //                tStr = Settings.BotSettings.listIndustry[j];
        //                if (tStr == person.curIndustry)
        //                {
        //                     Заполнение поля Меssage
        //                    tStr = "data\\MassEmail\\" + Settings.BotSettings.CurrentUser + "\\Message_" + person.curIndustry + ".txt";
        //                    if (File.Exists(tStr))
        //                    {
        //                        tmpStr = System.IO.File.ReadAllText(tStr);
        //                        person.messageText = tmpStr;
        //                        person.isMessageSend = true;
        //                    }
        //                    else
        //                        continue;
        //                     Добавление в список
        //                    personList.Add(person);
        //                    continue;
        //                }
        //            }
        //        }
        //        else
        //            break;
        //    }

        //    return;
    }
                   }
    }
}
