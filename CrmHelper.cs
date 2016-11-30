using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

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
        private List<PersonTest> personListUpd = new List<PersonTest>();
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
            return;
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
            //browser.CrmNavigateTo(GetBaseUrl() + "index.php?module=Contacts&action=index&return_module=Contacts&return_action=DetailView");
            browser.CrmNavigateTo(GetBaseUrl() + "index.php?module=Contacts&action=index");
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

        //TODO - Добавить пагинацию
        public void EmailMassSending()
        {
            // IWebElement elList, elButton, elDateField, tstItem;
            IWebElement elList, elButton, elDateField;
            //changed curDate from (2013, 3, 1) than change it back
            //DateTime curDate = new DateTime(2013, 9, 16);
            DateTime curDate = new DateTime(2016, 10, 29);
            //DateTime curDate = new DateTime(2014, 1, 8);
            DateTime endTime = new DateTime(2016, 11, 1);
            string tmpStr = "";
            int i;
            // Переключится на режим расширенного поиска

            SetSearchMode(1);
            personList.Clear();
            elList = SeleniumHelper.WaitForElement(By.XPath("//*[@id='messagewassent_c_advanced']"));
            SelectElement sel = new SelectElement(elList);
            sel.SelectByText("Нет");
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
                    AddPersonRecord();
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
        public void AddPersonRecord()
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
                    person.messageText = tStr;
                    
                    //if (File.Exists(tStr))
                    //{
                    //    tmpStr = System.IO.File.ReadAllText(tStr);
                    //    person.messageText = tmpStr;
                    //    //person.isMessageSend = true;
                    //}
                    //else
                    //    continue;
                    //  Добавление в список
                    personList.Add(person);
                    Debug.WriteLine(person.ToString());


                    //сontinue;

                }
                else
                    break;
            }


            return;
        }
        //Для тестирования 
        // Добавить пагинацию нужна для поиска без дат, с датами маловероятно

        public void SearchUsersTest()
        {
            IWebElement elList, elButton, elDateField, nextPage;
            string tmpStr;
            //начальная и конечная дата поиска
            DateTime curDate = new DateTime(2013, 05, 30);
            DateTime endTime = new DateTime(2013, 06, 01);

            SetSearchMode(1);
            elList = SeleniumHelper.WaitForElement(By.XPath("//*[@id='messagewassent_c_advanced']"));
            SelectElement sel = new SelectElement(elList);
            sel.SelectByText("Нет");
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
            elDateField = SeleniumHelper.WaitForElement(By.Id("range_date_entered_advanced"));
            elDateField.Clear();

            tmpStr = curDate.Day.ToString() + "/" + curDate.Month.ToString() + "/" + curDate.Year.ToString();
            elDateField.SendKeys(tmpStr);

            int i = 0;
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

                    AddPersonRecord();
                    //curDate = curDate.AddDays(1);
                    if (SeleniumHelper.IsElementPresent(By.XPath(".//*[@id='listViewNextButton_top']")))
                    {
                        nextPage = SeleniumHelper.WaitForElement(By.XPath(".//*[@id='listViewNextButton_top']"));
                        if (nextPage.Enabled)
                        {
                            nextPage.Click();
                            System.Threading.Thread.Sleep(5000);
                            if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='MassUpdate']/div[1]/p")) == false)
                            {
                                // Найдена новая запись

                                AddPersonRecord();
                            }
                            // не работает
                            //if (nextPage.GetAttribute("disabled").Contains("null") == false)
                            //{
                            //    nextPage.Click();
                            //    System.Threading.Thread.Sleep(5000);
                            //    if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='MassUpdate']/div[1]/p")) == false)
                            //    {
                            //        // Найдена новая запись

                            //        AddPersonRecord();
                            //        //curDate = curDate.AddDays(1);
                            //    }
                            //    //AddPersonRecord();
                            //}
                        }
                    }
                }

                curDate = curDate.AddDays(1);
                if (curDate.CompareTo(endTime) == 0)
                {
                    break;
                }

            }


            for (i = 0; i < personList.Count(); i++)
            {

                //VeryfiUserList(personList[i]);
               CheckContactAccept(personList[i]);
               
            }
            //int j = 0;
            //for (j = 0; j < personListUpd.Count(); j++)
            //{

            //    VeryfiUserList(personListUpd[j]);
            //}
        }

        

        public void MarkAsPending(PersonTest person)
        {

            IWebElement tempInviteStateField, editButton, saveButton;

            browser.SetCurrentWindow(1);
            browser.CrmNavigateTo(person.crmLink);
            System.Threading.Thread.Sleep(5000);

            try
            {
                editButton = SeleniumHelper.WaitForElement(By.Id("edit_button"));
                editButton.Click();

                if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='MassUpdate']/div[1]/p")) == false)
                {


                    if (SeleniumHelper.IsElementPresent(By.Id("department")))
                    {
                        tempInviteStateField = SeleniumHelper.WaitForElement(By.Id("department"));
                        //tempInviteStateField.Clear();
                        //System.Threading.Thread.Sleep(5000);
                        //tempInviteStateField.SendKeys("Invitation is Pending");
                        Debug.WriteLine("Invitation is Pending"); 
                        //System.Threading.Thread.Sleep(5000);
                        //saveButton = SeleniumHelper.WaitForElement(By.Id("SAVE_HEADER"));
                        //saveButton.Click();
                        //System.Threading.Thread.Sleep(5000);

                    }
                }
            }

            catch (Exception ex)
            {
                return;
            }
        }


        public void MarkAsRejected(PersonTest person)
        {

            IWebElement tempInviteStateField, editButton, saveButton;

            browser.SetCurrentWindow(1);
            browser.CrmNavigateTo(person.crmLink);
            System.Threading.Thread.Sleep(5000);

            try
            {
                editButton = SeleniumHelper.WaitForElement(By.Id("edit_button"));
                editButton.Click();

                if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='MassUpdate']/div[1]/p")) == false)
                {

                    if (SeleniumHelper.IsElementPresent(By.Id("department")))
                    {

                        tempInviteStateField = SeleniumHelper.WaitForElement(By.Id("department"));
                        //tempInviteStateField.Clear();
                        //System.Threading.Thread.Sleep(5000);
                        //tempInviteStateField.SendKeys("Invitation Rejected");
                        Debug.WriteLine("Invitation Rejected");
                        System.Threading.Thread.Sleep(5000);
                    //    saveButton = SeleniumHelper.WaitForElement(By.Id("SAVE_HEADER"));
                    //    saveButton.Click();
                    //    System.Threading.Thread.Sleep(5000);
                    }
                }
                else
                {
                    return;

                }

            }
            catch (Exception ex)
            {
                return;
            }
        }

        public void MarkAsAccepted(PersonTest person)
        {
            IWebElement tempInviteStateField, editButton, saveButton;

            browser.SetCurrentWindow(1);
            browser.CrmNavigateTo(person.crmLink);

            try
            {
                editButton = SeleniumHelper.WaitForElement(By.Id("edit_button"));
                editButton.Click();
                System.Threading.Thread.Sleep(5000);

                if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='MassUpdate']/div[1]/p")) == false)
                {

                    //MarkAsAccepted in the field .//*[@id='department']

                    if (SeleniumHelper.IsElementPresent(By.Id("department")))
                    {
                        tempInviteStateField = SeleniumHelper.WaitForElement(By.Id("department"));
                        //tempInviteStateField.Clear();
                        //System.Threading.Thread.Sleep(5000);
                        tempInviteStateField.SendKeys("Invitation is Accepted");
                        Debug.WriteLine("Invitation is Accepted");
                        //System.Threading.Thread.Sleep(5000);
                        //saveButton = SeleniumHelper.WaitForElement(By.Id("SAVE_HEADER"));
                        //saveButton.Click();
                        //System.Threading.Thread.Sleep(5000);
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public void MarkAsUnreachableContact(PersonTest person)
        {
            IWebElement tempInviteStateField, editButton, saveButton;

            browser.SetCurrentWindow(1);
            browser.CrmNavigateTo(person.crmLink);

            try
            {
                editButton = SeleniumHelper.WaitForElement(By.Id("edit_button"));
                editButton.Click();

                if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='MassUpdate']/div[1]/p")) == false)
                {

                    //MarkAsAccepted in the field .//*[@id='department']

                    if (SeleniumHelper.IsElementPresent(By.Id("department")))
                    {
                        //tempInviteStateField = SeleniumHelper.WaitForElement(By.Id("department"));
                        //tempInviteStateField.Clear();
                        //System.Threading.Thread.Sleep(5000);
                        //tempInviteStateField.SendKeys("Contact not found");
                        Debug.WriteLine("Contact not found");
                        //System.Threading.Thread.Sleep(5000);
                        //saveButton = SeleniumHelper.WaitForElement(By.Id("SAVE_HEADER"));
                        //saveButton.Click();
                        //System.Threading.Thread.Sleep(5000);

                    }

                    else
                    {
                        return;
                    }
                }

            }
            catch (Exception ex)
            {
                return;
            }
        }


        /* IT WORKS
         */
        public bool CheckContactAccept(PersonTest person)
        {
            // elLogin, elPassword, signButton добавлены из-за блокировки сессии (Линкедин запрашивает авторизацию перед переходом на страницу юзера для отправки сообщения
            IWebElement elPassword, signButton, elLogin, elButton;
          


            try
            {
                //        // Перейти на страницу пользователя
                browser.LinkedInNavigateTo(person.linkedinlink);
                //browser.LinkedInNavigateTo("https://mx.linkedin.com/in/nathan-green-17018a127");
                // elButton = SeleniumHelper.WaitForElement(By.XPath("//*[@id='tc-actions-send-message']"));

                while (SeleniumHelper.IsElementPresent(By.XPath(".//*[@id='name']/h1/span/span")))
                {

                    if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='tc-actions-send-message']")))
                    {
                        MarkAsAccepted(person); //AddPersonRecord();
                        return true;
                    }
                        if (SeleniumHelper.IsElementPresent(By.XPath(".//*[@id='top-card']/div/div[1]/div/div[2]/div[2]/a")))
                        {
                            MarkAsAccepted(person); //AddPersonRecord();
                        return true;
                    
                        }

                    else if (SeleniumHelper.IsElementPresent(By.XPath(".//*[@id='top-card']/div/div[1]/div[2]/div[2]/div[2]/a")))
                    {
                        // Follow button - invitation is pending
                        elButton = SeleniumHelper.WaitForElement(By.XPath(".//*[@id='top-card']/div/div[1]/div[2]/div[2]/div[2]/a"));
                        if (elButton.Text == "Follow" || elButton.Text == "Invitation is pending")
                        {
                            MarkAsPending(person);
                            return true;

                        }
                        //    //Connect button - Invitation is rejected
                        else if (elButton.Text == "Connect")
                        {

                            MarkAsRejected(person);
                            return true;
                        }

                    }
                    else if (SeleniumHelper.IsElementPresent(By.XPath(".//*[@id='control_gen_10']")))
                    {
                        elButton = SeleniumHelper.WaitForElement(By.XPath(".//*[@id='control_gen_10']"));
                        if (elButton.Text == "Follow")
                        {
                            MarkAsPending(person);
                            return true;
                        }

                    }
                    else if (SeleniumHelper.IsElementPresent(By.XPath(".//*[@id='tc-actions-send-inmail']")))
                    {
                        MarkAsRejected(person);
                        return true;

                    }

                    else
                    {


                        while (SeleniumHelper.IsElementPresent(By.Id("session_key-login")))
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
                            //System.Threading.Thread.Sleep(5000);

                        }

                        //неправильная ссылка в линкедин 404 
                        while (SeleniumHelper.IsElementPresent(By.XPath(".//*[@id='pagekey-nprofile-public-not-found']")))
                        {
                            UpdateLinkUrl(person);
                            Debug.WriteLine("Link is Updated!");
                            //browser.SetCurrentWindow(1);
                            //browser.CrmNavigateTo(person.crmLink);
                            //personUpd.crmLink = person.crmLink;
                            //personUpd.linkedinlink = SeleniumHelper.WaitForElement(By.XPath("//*[@id='social_nework_url_c']")).Text;
                            //personListUpd.Add(personUpd);

                            //personList.Remove(person);
                            //personList.Add(person);
                            return true;
                        }
                        while (SeleniumHelper.IsElementPresent(By.XPath(".//*[@id='wiper']")))
                        {
                            linkedView.Login();
                            browser.LinkedInNavigateTo(person.linkedinlink);
                            System.Threading.Thread.Sleep(5000);
                        }
                        //главная
                        while (SeleniumHelper.IsElementPresent(By.XPath(".//*[@id='regForm']")))
                        {
                            linkedView.Login();
                            browser.LinkedInNavigateTo(person.linkedinlink);
                            System.Threading.Thread.Sleep(5000);
                        }

                        return false;
                    }
                }
            }
            
            catch (Exception ex)
            {
                return false;
            }
                return true;
        }





        //


        public void UpdateLinkUrl(PersonTest person)
        {
            IWebElement firstname, lastname, editButton, searchField, searchButton, linkName, linkSur, linkUrl, elLogin, elPassword, signButton, element, saveButton;
            string tmpStr, urlLink, linkFullName, searchUrl;
            browser.SetCurrentWindow(1);


            browser.CrmNavigateTo(person.crmLink);
            try
            {
                editButton = SeleniumHelper.WaitForElement(By.Id("edit_button"));
                editButton.Click();
                //получаем данные для поиска в crm
                firstname = SeleniumHelper.WaitForElement(By.Id("first_name"));
                lastname = SeleniumHelper.WaitForElement(By.Id("last_name"));
                tmpStr = firstname.GetAttribute("value") + ' ' + lastname.GetAttribute("value");
                //ищем в линкедин
                browser.SetCurrentWindow(0);
                searchUrl = "https://www.linkedin.com/vsearch/f?type=all&keywords=" + tmpStr;

                browser.LinkedInNavigateTo(searchUrl);
                //логин при блокировке сессии 
             
                     if (SeleniumHelper.IsElementPresent(By.XPath(".//*[@id='results']/li[2]/div/h3/a")))
                    {
                        linkName = SeleniumHelper.WaitForElement(By.XPath(".//*[@id='results']/li[2]/div/h3/a"));

                        //FOR EQUALITY CHECK IN CRM 
                        //linkName = SeleniumHelper.WaitForElement(By.XPath(".//*[@id='results']/li[2]/div/h3/a/b[1]"));
                        //linkSur = SeleniumHelper.WaitForElement(By.XPath(".//*[@id='results']/li[2]/div/h3/a/b[2]"));
                        //linkFullName = linkName.GetAttribute("value") + ' ' + linkSur.GetAttribute("value");
                        // linkFullName = linkName.ToString() + ' ' + linkSur.ToString(); RETURNS WEB ELEMENT
                        //if (linkFullName.Equals(tmpStr))
                        //{
                        browser.LinkedInNavigateTo(linkName.GetAttribute("href"));
                        System.Threading.Thread.Sleep(5000);

                        if (SeleniumHelper.IsElementPresent(By.XPath(".//*[@id='top-card']/div/div[2]/div[2]/ul/li[1]/dl/dd/a")))
                        {
                            linkUrl = SeleniumHelper.WaitForElement(By.XPath(".//*[@id='top-card']/div/div[2]/div[2]/ul/li[1]/dl/dd/a"));
                            urlLink = linkUrl.GetAttribute("href").ToString();
                            Debug.WriteLine(urlLink);

                            //browser.SetCurrentWindow(1);
                            //browser.CrmNavigateTo(person.crmLink);
                            //System.Threading.Thread.Sleep(5000);

                            //if (SeleniumHelper.IsElementPresent(By.XPath(".//*[@id='edit_button']")))
                            //{
                            //    editButton = SeleniumHelper.WaitForElement(By.XPath(".//*[@id='edit_button']"));
                            //    editButton.Click();
                            //    System.Threading.Thread.Sleep(5000);
                            //    element = SeleniumHelper.WaitForElement(By.XPath("//*[@id='social_nework_url_c']"));
                            //    element.Clear();
                            //    element.SendKeys(urlLink);
                            //    System.Threading.Thread.Sleep(5000);
                            //    saveButton = SeleniumHelper.WaitForElement(By.Id("SAVE_HEADER"));
                            //    System.Threading.Thread.Sleep(5000);
                            //    saveButton.Click();


                            //}
                        }
                        else if (SeleniumHelper.IsElementPresent(By.XPath(".//*[@id='top-card']/div/div[2]/div/ul/li[1]/dl/dd/a")))
                        {
                            linkUrl = SeleniumHelper.WaitForElement(By.XPath(".//*[@id='top-card']/div/div[2]/div/ul/li[1]/dl/dd/a"));
                            urlLink = linkUrl.GetAttribute("href").ToString();
                            Debug.WriteLine(urlLink);
                            //browser.SetCurrentWindow(1);
                            //browser.CrmNavigateTo(person.crmLink);
                            //System.Threading.Thread.Sleep(5000);

                            //if (SeleniumHelper.IsElementPresent(By.XPath(".//*[@id='edit_button']")))
                            //{
                            //    editButton = SeleniumHelper.WaitForElement(By.XPath(".//*[@id='edit_button']"));
                            //    editButton.Click();
                            //    System.Threading.Thread.Sleep(5000);
                            //    element = SeleniumHelper.WaitForElement(By.XPath("//*[@id='social_nework_url_c']"));
                            //    element.Clear();
                            //    element.SendKeys(urlLink);
                            //    System.Threading.Thread.Sleep(5000);
                            //    saveButton = SeleniumHelper.WaitForElement(By.Id("SAVE_HEADER"));
                            //    saveButton.Click();
                            //    System.Threading.Thread.Sleep(5000);



                            //}

                        }

                        while (SeleniumHelper.IsElementPresent(By.Id("session_key-login")))
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

                        //https://www.linkedin.com/public-profile/internal-server-error
                        while (SeleniumHelper.IsElementPresent(By.XPath(".//*[@id='wiper']")))
                        {
                            linkedView.Login();
                            browser.LinkedInNavigateTo(searchUrl);
                        }
                        //главная
                        while (SeleniumHelper.IsElementPresent(By.XPath(".//*[@id='regForm']")))
                        {
                            linkedView.Login();
                            browser.LinkedInNavigateTo(searchUrl);
                        }

                        //searchField = SeleniumHelper.WaitForElement(By.Id("main-search-box"));
                        //searchField.Clear();
                        //System.Threading.Thread.Sleep(5000);
                        //searchField.SendKeys(tmpStr);
                        //System.Threading.Thread.Sleep(5000);
                        //searchButton = SeleniumHelper.WaitForElement(By.XPath(".//*[@id='global-search']/fieldset/button"));
                        //System.Threading.Thread.Sleep(5000);
                        //searchButton.Click();
                        //System.Threading.Thread.Sleep(5000);

                        //Если пользователь не найден
                        while (SeleniumHelper.IsElementPresent(By.Id("pagekey-nprofile-public-not-found")))
                        {
                            MarkAsUnreachableContact(person);

                        }

                    }
                    //}
                    //CheckContactAccept(person);
                }
           
            //}
            catch (Exception ex)
            {
                return;
            }
        }
        public void VeryfiUserList(PersonTest person)
        {
            if (CheckContactAccept(person) == true)
            {
                AddUserCountry(person);
            }

        }

        public void AddUserCountry(PersonTest person)
        {
            IWebElement tempCountryField, editButton, saveButton, linkedCountry;
            string country;

            browser.SetCurrentWindow(0);

            browser.LinkedInNavigateTo(person.linkedinlink);
           System.Threading.Thread.Sleep(5000);

            if (SeleniumHelper.IsElementPresent(By.XPath(".//*[@id='location']/dl/dd[1]/span/a")))
            {
                linkedCountry = SeleniumHelper.WaitForElement(By.XPath("//*[@id='location']/dl/dd[1]/span/a"));
                country = linkedCountry.Text;

                browser.SetCurrentWindow(1);
                browser.CrmNavigateTo(person.crmLink);
                Debug.WriteLine(country);
                //System.Threading.Thread.Sleep(5000);


                //try
                //{
                //    editButton = SeleniumHelper.WaitForElement(By.Id("edit_button"));
                //    editButton.Click();

                //    //browser.SetCurrentWindow(1);
                //    //browser.CrmNavigateTo(GetBaseUrl() + "index.php?module=Contacts&action=EditView&return_module=Contacts&return_action=index");

                //    if (SeleniumHelper.IsElementPresent(By.XPath("//*[@id='MassUpdate']/div[1]/p")) == false)
                //    {

                //        if (SeleniumHelper.IsElementPresent(By.Id("description")))
                //        {
                //            tempCountryField = SeleniumHelper.WaitForElement(By.Id("description"));
                //            tempCountryField.Clear();
                //            System.Threading.Thread.Sleep(5000);

                //            tempCountryField.SendKeys(country);
                //            System.Threading.Thread.Sleep(5000);
                //            saveButton = SeleniumHelper.WaitForElement(By.Id("SAVE_HEADER"));
                //            saveButton.Click();
                //        }
                    }
                    else
                    {
                        return;
                //    }


                //}

                //catch (Exception ex)
                //{

                //}
                //return;
            }
        }
    }
}
    
    

   
//При Reject конекшена, при следующем запросе 
//    * если получатель выбрал I don't know 

//  Xpath текста You and this LinkedIn user don’t know anyone in common .//*[@id='main']/h1