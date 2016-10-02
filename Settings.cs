using System;
using System.Collections.Generic;
using System.Linq;
using System.Configuration; 
using ConfigurationSettings = System.Configuration.ConfigurationManager;
using NLog;
using NLog.Internal;


namespace BotLinkedIn
{
    internal class Settings
    {
        private static Logger log = LogManager.GetCurrentClassLogger();
        //internal struct Application
        //{
        //    // Режим отладки. 
        //    internal static bool Debug;
        //}

        internal struct Selenium
        {
            // Время ожидания загрузки страницы.
            internal static int PageLoadTimeout;
            // Время ожидания загрузки элемента.
            internal static int ElementLoadTimeout;
            // Интервал опроса.
            internal static double PollingInterval;
        }

        internal struct BotSettings
        {
            internal static string CurrentUser;            // Используемый пользователь для работы бота
            internal static string LoginLinkedIn;          // Login LinkedIn
            internal static string PasswordLinkedIn;       // Пароль для LinkedIn
            internal static string LoginCrm;               // Login для CRM
            internal static string PasswordCrm;            // Password для CRM
            internal static string FullName;               // Полное имя пользователя

            internal static List<string> Category;         // Категории для поиска
            //internal static List<string> Groups;           // Группы для поиска 
            internal static List<string> Position;         // Должность для поиска
            internal static List<string> Country;          // Страны по которым ведется поиск
            internal static List<string> Technologies;     // Используемые технологии для поиска
            internal static List<string> ExcludeWord;      // Слова исключения для ограничения поиска контактов
            internal static string SubjectMessage;         // Subject отсылаемого сообщения
            internal static List<string> listIndustry;     // Индустрии по которым рассылка сообщений 

            internal static int CountInvite;               // Кол-во отсылаемых сообщений за сутки
            internal static int Mode;                      // Текущий режим работы бота 

        }


        private static void CreateList(string[] dataIn,List<string> dataOut)
        {
            foreach (string s in dataIn)
            {
                dataOut.Add(s);
            }
            return;
        }
        public static void Read()
        {
            string[] data;
            BotSettings.Category     = new List<string>();
            //BotSettings.Groups       = new List<string>();
            BotSettings.Position     = new List<string>();
            BotSettings.Country      = new List<string>();
            BotSettings.Technologies = new List<string>();
            BotSettings.ExcludeWord  = new List<string>();
            BotSettings.listIndustry = new List<string>();

           // Application.Debug = Convert.ToBoolean(ConfigurationSettings.AppSettings["Application_Debug"]);
            Selenium.PageLoadTimeout = Convert.ToInt32(ConfigurationSettings.AppSettings["Selenium_PageLoadTimeout"]);
            Selenium.ElementLoadTimeout = Convert.ToInt32(ConfigurationSettings.AppSettings["Selenium_ElementLoadTimeout"]);
            Selenium.PollingInterval = Convert.ToDouble(ConfigurationSettings.AppSettings["Selenium_PollingInterval"]);
            
            // Загрузка конфигурации Бота
            BotSettings.CurrentUser = ConfigurationSettings.AppSettings["UserBot"];
            BotSettings.LoginLinkedIn = ConfigurationSettings.AppSettings[BotSettings.CurrentUser+"_linkLogin"];
            BotSettings.PasswordLinkedIn = ConfigurationSettings.AppSettings[BotSettings.CurrentUser + "_linkPassword"];
            BotSettings.LoginCrm = ConfigurationSettings.AppSettings[BotSettings.CurrentUser + "_CrmLogin"];
            BotSettings.PasswordCrm = ConfigurationSettings.AppSettings[BotSettings.CurrentUser + "_CrmPassword"];
            BotSettings.FullName = ConfigurationSettings.AppSettings[BotSettings.CurrentUser + "_FullName"];
            BotSettings.CountInvite = Convert.ToInt32(ConfigurationSettings.AppSettings["CountInvite"]);
            BotSettings.Mode        = Convert.ToInt32(ConfigurationSettings.AppSettings["Mode"]);

            BotSettings.SubjectMessage = ConfigurationSettings.AppSettings["SubjectMessage"];

            CreateList(ConfigurationSettings.AppSettings["Category"].Split(','),BotSettings.Category);
            //CreateList(ConfigurationSettings.AppSettings["Groups"].Split(','), BotSettings.Groups);
            CreateList(ConfigurationSettings.AppSettings["Position"].Split(','), BotSettings.Position);
            CreateList(ConfigurationSettings.AppSettings["Country"].Split(','), BotSettings.Country);
            CreateList(ConfigurationSettings.AppSettings["Technologies"].Split(','), BotSettings.Technologies);
            CreateList(ConfigurationSettings.AppSettings["Exclude"].Split(','), BotSettings.ExcludeWord);
            //CreateList(ConfigurationSettings.AppSettings["SendIndustry"].Split(','), BotSettings.listIndustry);
            log.Info("Configuration load: Ok.");
        }
    }
}
