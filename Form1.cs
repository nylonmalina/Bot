using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using NLog;


namespace BotLinkedIn
{
    public partial class Form1 : Form
    {
        private Browser browserIn = Browser.Instance;
        private LinkedHelper linkedView = new LinkedHelper();
        private CrmHelper crmView = new CrmHelper();
        //private Account searchView = new Account();
        public Form1()
        {
            InitializeComponent();
        }

        public void SearchNewContacts()
        {
            // Загрузить настройки
            Settings.Read();
            // Запуск 2 экземпляров броузера
            browserIn.Start();
            // Подключиться к LinkedIn
            linkedView.Login();
            // Подключиться к CRM
            crmView.Login();
            linkedView.SetCrmHelper(crmView);

            // Поиск контактов
            linkedView.SearchRequestedContacts();
            return;
        }

        /*
         * Функция массовой рассылки сообщений пользователю
         * 
         * 
         */
        private void MassEmailSending()
        {
            // Запуск 2 экземпляров броузера
            browserIn.Start();
            // Подключиться к LinkedIn
            linkedView.Login();
            // Подключиться к CRM
            crmView.Login();
            linkedView.SetCrmHelper(crmView);
            // Запустить рассылку
            crmView.EmailMassSending();
            return;
        }

        private void AcceptingDo()
        {
            // Запуск 2 экземпляров броузера
            browserIn.Start();
            // Подключиться к LinkedIn
            linkedView.Login();
            // Подключиться к CRM
            crmView.Login();
            linkedView.SetCrmHelper(crmView);
            linkedView.ProcessingAccepted();
            return;
        }


        private void AcceptingVerify()
        {
            // Запуск 2 экземпляров броузера
            browserIn.Start();
            // Подключиться к LinkedIn
            linkedView.Login();
            // Подключиться к CRM
            crmView.Login();
            linkedView.SetCrmHelper(crmView);
            linkedView.ProcessingAccepted();
            return;
        }

             
       
        public void SearchByCountry()
        {
            // Загрузить настройки
            Settings.Read();
            // Запуск 2 экземпляров броузера
            browserIn.Start();
            // Подключиться к LinkedIn
            linkedView.Login(); ;
            // Подключиться к CRM
            crmView.Login();
            linkedView.SetCrmHelper(crmView);
            //поиск
            linkedView.SearchByCountry();
            
            return;
        }

        public void CheckInviteAccept()
        {
            // Загрузить настройки
            Settings.Read();
            // Запуск 2 экземпляров броузера
            browserIn.Start();
            // Подключиться к LinkedIn
            linkedView.Login(); ;
            // Подключиться к CRM
            crmView.Login();
            linkedView.SetCrmHelper(crmView);
            //поиск
            crmView.SearchContactsByPeriod();

            return;
        }

        public void TestList()
        {
            Settings.Read();
            browserIn.Start();
            linkedView.Login();
            crmView.Login();
            linkedView.SetCrmHelper(crmView);
            crmView.SearchUsersTest();
           // crmView.AddPersonRecord();
            return;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.AppendText("LinkedIn Bot Running...");
            button1.Enabled = false;
            //TestFunction();
            // Загрузить настройки
            Settings.Read();
            switch (Settings.BotSettings.Mode)
            {
                case   1:   // Режим работы - Поиск контактов
                    SearchNewContacts();
                    break;
                case 2:     // Режим работы - подтверждение контактов
                    AcceptingDo();
                    break;
                    case 3:     // Режим работы - Рассылка писем
                    MassEmailSending();
                   break;
                case 4:     // Режим работы - Провека существующих контактов
                    AcceptingVerify();
                    break;
                //added for testing
                case 5:     // Режим работы - search user by country
                    CheckInviteAccept();
                    break;
                //added for testing
                case 6:     // Режим работы - add person record crm test 
                   TestList();
                    break;
                default:
                    break;
            }
            //MassEmailSending();
            button1.Enabled = true;
            return ;
        }
    }
}

