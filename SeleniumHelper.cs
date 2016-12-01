using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace BotLinkedIn
{
    internal class SeleniumHelper
    {
      
        private static Browser browser = Browser.Instance;
        public static bool IsElementPresent(By by)
        {
            try
            {
                browser.Driver().FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public static bool IsElementPresentAndDisplayed(By by)
        {
            try
            {
                var element = browser.Driver().FindElement(by);
                return (element.Displayed);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public static IWebElement WaitForElement(By by)
        {
            var wait = new WebDriverWait(browser.Driver(), TimeSpan.FromSeconds(Settings.Selenium.ElementLoadTimeout));
            wait.PollingInterval = TimeSpan.FromSeconds(Settings.Selenium.PollingInterval);
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(NoSuchFrameException));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException), typeof(StaleElementReferenceException));
            wait.Message = string.Format("Элемент \"{0}\" не найден.", by.ToString());

            var element = wait.Until(ExpectedConditions.ElementExists(by));
            return element;
        }

        public static void WaitForElementDisappear(By by)
        {
            var wait = new WebDriverWait(browser.Driver(), TimeSpan.FromSeconds(Settings.Selenium.ElementLoadTimeout));
            wait.PollingInterval = TimeSpan.FromSeconds(Settings.Selenium.PollingInterval);
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(NoSuchFrameException));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException), typeof(StaleElementReferenceException));
            wait.Message = string.Format("Элемент \"{0}\" не исчез.", by.ToString());

            wait.Until(d =>
            {
                return d.FindElements(by).Count == 0;
            });
        }

        public static void WaitForElementValueCnhanged(By by, string value)
        {
            var wait = new WebDriverWait(browser.Driver(), TimeSpan.FromSeconds(Settings.Selenium.ElementLoadTimeout));
            wait.PollingInterval = TimeSpan.FromSeconds(Settings.Selenium.PollingInterval);
            wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(NoSuchFrameException));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException), typeof(StaleElementReferenceException));
            wait.Message = string.Format("Значение элемента \"{0}\" не поменялось.", by.ToString());

            wait.Until(d =>
            {
                return !d.FindElement(by).Text.Equals(value);
            });
        }

        public static void WaitUntil(Func<bool> condition)
        {
            var wait = new WebDriverWait(browser.Driver(), TimeSpan.FromSeconds(Settings.Selenium.ElementLoadTimeout));
            wait.PollingInterval = TimeSpan.FromSeconds(Settings.Selenium.PollingInterval);
            wait.Until(d => condition());
        }

        public static void WaitForPageToBeReady()
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)browser.Driver();

            //This loop will rotate for 100 times to check If page Is ready after every 1 second.
            //You can replace your if you wants to Increase or decrease wait time.
            for (int i = 0; i < 400; i++)
            {
                try
                {
                    System.Threading.Thread.Sleep(1000);
                }
                catch (Exception ex) { }
                //To check page ready state.

                if (js.ExecuteScript("return document.readyState").ToString().Equals("complete"))
                {
                    break;
                }
            }
        }

        }

    }
