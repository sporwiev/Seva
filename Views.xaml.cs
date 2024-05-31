using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Seva
{
    /// <summary>
    /// Логика взаимодействия для Views.xaml
    /// </summary>
    public partial class Views : Window
    {
        public Views()
        {
            InitializeComponent();
        }
        string[] tables = { "CREATE TABLE `cart` (\r\n  `id_cart` int(11) NOT NULL,\r\n  `product_id` int(11) NOT NULL,\r\n  `category_id` int(11) NOT NULL,\r\n  `date` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,\r\n  `user_id` int(11) NOT NULL\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",
            "CREATE TABLE `category` (\r\n  `id_category` int(11) NOT NULL,\r\n  `name` varchar(100) NOT NULL\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",
            "CREATE TABLE `orders` (\r\n  `id_order` int(11) NOT NULL,\r\n  `cart_id` int(11) NOT NULL,\r\n  `user_id` int(11) NOT NULL,\r\n  `status` int(2) NOT NULL\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8;", 
            "CREATE TABLE `product` (\r\n  `id_product` int(11) NOT NULL,\r\n  `category_id` int(2) NOT NULL,\r\n  `name` varchar(100) NOT NULL,\r\n  `price` int(100) NOT NULL,\r\n  `color` varchar(100) NOT NULL,\r\n  `country` varchar(100) NOT NULL,\r\n  `size` int(100) NOT NULL,\r\n  `count` int(100) NOT NULL\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;",
            "CREATE TABLE `user` (\r\n  `id_user` int(11) NOT NULL,\r\n  `first_name` varchar(100) NOT NULL,\r\n  `last_name` varchar(100) NOT NULL,\r\n  `patronymic` varchar(100) NOT NULL,\r\n  `date_reg` datetime NOT NULL,\r\n  `gender` varchar(10) NOT NULL,\r\n  `login` varchar(100) NOT NULL,\r\n  `password` varchar(256) NOT NULL,\r\n  `photo` varchar(700) NOT NULL,\r\n  `phone` varchar(100) NOT NULL,\r\n  `token` varchar(256) NOT NULL,\r\n  `role` int(2) NOT NULL DEFAULT '0',\r\n  `bonusnumber` int(50) NOT NULL,\r\n  `subscription` varchar(100) NOT NULL\r\n) ENGINE=InnoDB DEFAULT CHARSET=utf8;", 
            "ALTER TABLE `cart`\r\n  ADD PRIMARY KEY (`id_cart`);", 
            "ALTER TABLE `category`\r\n  ADD PRIMARY KEY (`id_category`);", 
            "ALTER TABLE `orders`\r\n  ADD PRIMARY KEY (`id_order`);\r\n", 
            "ALTER TABLE `product`\r\n  ADD PRIMARY KEY (`id_product`);",
            "ALTER TABLE `user`\r\n  ADD PRIMARY KEY (`id_user`),\r\n  ADD UNIQUE KEY `login` (`login`);",
            "ALTER TABLE `cart`\r\n  MODIFY `id_cart` int(11) NOT NULL AUTO_INCREMENT;",
            "ALTER TABLE `category`\r\n  MODIFY `id_category` int(11) NOT NULL AUTO_INCREMENT;",
            "ALTER TABLE `orders`\r\n  MODIFY `id_order` int(11) NOT NULL AUTO_INCREMENT;",
            "ALTER TABLE `product`\r\n  MODIFY `id_product` int(11) NOT NULL AUTO_INCREMENT;",
            "ALTER TABLE `user`\r\n  MODIFY `id_user` int(11) NOT NULL AUTO_INCREMENT;" };
        private void create_Click(object sender, RoutedEventArgs e)
        {
            
            request request = new request();
            process processes = new process();
            processes.Show();
            
            processes.proccessbar.Maximum = tables.Length;
            for(int i = 0;i < tables.Length;i++)
            {
                Thread.Sleep(50);
                processes.proccessbar.Value = i;
                processes.processtext.Text = tables[i].Substring(0,19);
                request.createtable(tables[i]);
            }
            Thread.Sleep(150);
            MainWindow main = new MainWindow();
            main.Activate();
            main.getTable();
            processes.Close();
            Close();
            
        }
        /*public static string GenericData(bool user = false, bool cart = false, bool product = false, bool order = false,bool category = false)
        {

            string[] vowels = { "a", "e", "i", "o", "u", "y" };
            string[] consonant = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "n", "p", "q", "r", "s", "t", "v", "w", "x", "y", "z" };
            if (user)
            {
                string year = new Random().Next(1930, 2024).ToString();
                string month = new Random().Next(1, 12).ToString();
                string day = new Random().Next(1, 31).ToString();

                Random randlnconson = new Random();
                Random randlnvow = new Random();
                string first_name = ""; //Имя
                string last_name = ""; //Фамилия
                string patronymic = ""; //Отчество
                string fio;
                //string gender = "";
                for (int i = 0; i < 8; i++)
                {
                    if (i == 0)
                    {
                        last_name += consonant[randlnconson.Next(0, consonant.Length)].ToUpper();
                        first_name += consonant[randlnconson.Next(0, consonant.Length)].ToUpper();
                        patronymic += consonant[randlnconson.Next(0, consonant.Length)].ToUpper();
                    }
                    if (i < 5)
                    {
                        if (i % 2 == 0)
                        {
                            last_name += consonant[randlnconson.Next(0, consonant.Length)];
                        }
                        if (i % 2 == 1)
                        {
                            last_name += vowels[randlnvow.Next(0, vowels.Length)];
                        }

                    }
                    if (i < 4)
                    {
                        if (i % 2 == 0)
                        {
                            first_name += consonant[randlnconson.Next(0, consonant.Length)];
                        }
                        if (i % 2 == 1)
                        {
                            first_name += vowels[randlnvow.Next(0, vowels.Length)];
                        }
                    }
                    if (i % 2 == 0)
                    {
                        patronymic += consonant[randlnconson.Next(0, consonant.Length)];
                    }
                    if (i % 2 == 1)
                    {

                        patronymic += vowels[randlnvow.Next(0, vowels.Length)];
                    }



                }
                if (month.Length == 1)
                {
                    month = "0" + month;
                }
                if (day.Length == 1)
                {
                    day = "0" + day;
                }
                string date = $"{year}-{month}-{day}";

                fio = $"{last_name} {first_name} {patronymic}";
                //Console.WriteLine(fio + " " + fio + date + " " + date + " Male");
                return "\"" + fio + "\"," + "\"" + fio + date + "\"," + "\"" + date + "\",";
            }
            if (cart)
            {

            }
            if (product)
            {
                
            }
            if (order)
            {

            }
            if (category)
            {

            }
            //string[] datas = { };
            //string data = "";
            

            
        }*/
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
