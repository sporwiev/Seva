using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySqlConnector;
using Renci.SshNet;
using System.Text.Json;
using System.IO;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using MaterialDesignThemes.Wpf;
using System.Diagnostics;

namespace Seva
{
    /// <summary>
    /// Логика взаимодействия для Connected.xaml
    /// </summary>
    public class Account
    {
        public string User {  get; set; }
        public string Pass { get; set; }
    }

    public partial class Connected : Window
    {
        public Connected()
        {
            InitializeComponent();



            
            
        }
        //public static string user, pass;

        private void Connect_Click(object sender, RoutedEventArgs e)
        {
           try
            {
                MySqlConnection conn = new MySqlConnection($"server=localhost;port=3306;username=root;password=root;");
                conn.Open();
            if (conn.State == System.Data.ConnectionState.Open)
            {
                select select = new select();


                Account acc = new Account
                {
                    User = "root",
                    Pass = "root",
                };
                /*                string personJson = JsonSerializer.Serialize(acc);
                                using (StreamWriter fs = File.CreateText("../../user.json"))
                                {
                                    fs.WriteLine(personJson);

                                }*/
                //
                //file.Close();
                select.Show();
                //this.Close();
                Hide();
            }
                
                else
                {

                    MessageError("Not conntection");
                }
            }catch(Exception ex)
            {
                MessageError(ex.Message);
            }
                
        }
        public static void MessageError (string message) {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public static void MessageWarning(string message)
        {
            MessageBox.Show(message, "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        public static void MessageInformation(string message)
        {
            MessageBox.Show(message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
       
        
        private void Button_min(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_exit(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            //Process.Start("C:\\MAMP\\MAMP.exe");
            if (!new DirectoryInfo("C:\\MAMP").Exists)
            {
                if (MainWindow.MessageYesNo("Для использования программы нужна утилита 'MAMP'. После установки 'MAMP' перезапустите программу. Установить MAMP? \r\n https://downloads.mamp.info/MAMP-PRO-WINDOWS/releases/5.0.6/MAMP_MAMP_PRO_5.0.6.exe"))
                {
                    Process.Start("https://downloads.mamp.info/MAMP-PRO-WINDOWS/releases/5.0.6/MAMP_MAMP_PRO_5.0.6.exe");
                    Connect.IsEnabled = false;
                }
            }
            
        }
    }
}
