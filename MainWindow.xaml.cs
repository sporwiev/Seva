using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Linq;
using MySqlConnector;
using Org.BouncyCastle.Asn1.X509.Qualified;
using Renci.SshNet;
using Renci.SshNet.Common;
using Seva;



namespace Seva
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public static bool isConnect = false;

        

        



        public MainWindow()
        {
            InitializeComponent();
            
            usedata.Content = DB.database;
            //Функция обновления данных получаемых с бд
            getTable();

            getData(Convertion(MenuTableBox.SelectedItem), usedata.Content.ToString());
            //getUsers();
           
        }

        
        
        public void getUsers()
        {
            try
            {
                DB dB = new DB();
                MySqlCommand command = new MySqlCommand($"use mysql; SELECT user FROM user", dB.getConnection()); //Команда в которой мы пишем условие где id = значению введенного пользователем
                dB.OpenConnection();
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ComboBoxItem item = new ComboBoxItem();
                        item.Content = reader.GetString(0);
                        //usersView.Items.Add(item);
                    }
                }
                reader.Close();
            }catch(Exception ex)
            {
                MessageError(ex.Message);
            }
            

        }
        //Кнопка перехода в новое окно
        private void delete(object sender, RoutedEventArgs e)
        {
                try
                {
                    DB dB = new DB();
                int id = primarykeylab.Content.ToString().IndexOf("n");
                string iden = "";
                    if(id != -1)
                    { 

                        iden = primarykeylab.Content.ToString().Substring(id + 2);
                         
                        MySqlCommand command = new MySqlCommand($"DELETE FROM {DelidTable.Text} WHERE `{iden}` = {DelId.Text}", dB.getConnection()); //Команда в которой мы пишем условие где id = значению введенного пользователем
                        dB.OpenConnection();
                        if (command.ExecuteNonQuery() == 1) // если запрос выдал 1 то норм
                        {
                            MessageInformation("Удалено");
                            getData(Convertion(MenuTableBox.SelectedItem), usedata.Content.ToString());
                        }
                        else
                        {
                            MessageError("Не удалось");
                        }
                        dB.CloseConnection();
                    }
                else
                {

                }
                   
                }
                catch (Exception ex)
                {
                    MessageError(ex.Message);
                }
        }

        /*        private void update(object sender, RoutedEventArgs e)
                {
                    try
                    {
                        DB dB = new DB();
                        MySqlCommand command = new MySqlCommand($"UPDATE `user` SET `lastName`=  '{UpLastName.Text}',`firstName`= '{UpFirstName.Text}' WHERE `lastName` = '{UpBeforeLastName.Text}'", dB.getConnection());//Выполняем запрос в котором мы получаем данные вводимые пооьзователем, далее пропишем условие где lastName = вводимые данные
                        dB.OpenConnection(); // Открывем соеднинение
                        if (command.ExecuteNonQuery() == 1) // если 1 то круто все получилось, 0 запрос не прошел
                        {
                            MessageError("Изменено");
                            getData();
                        }
                        else
                        {
                            MessageError("Не удалось");
                        }
                        dB.CloseConnection();//Закрываем соединение
                    }
                    catch (Exception ex)
                    {
                        MessageError(ex.Message);//Есди сработало исключение, произойдет в том случае если Mamp будет выключен
                    }
                }*/

        public void getTable()
        {

                MenuTableBox.Items.Clear();
                DB dB = new DB();
                // Таблица в которую помещаем данные
                ComboBoxItem createTable = new ComboBoxItem();
                createTable.VerticalAlignment = VerticalAlignment.Center;
                createTable.Content = "Create table";
                createTable.Selected += CreateTable_Selected;
                MySqlCommand mySqlCommand = new MySqlCommand($"SHOW TABLES", dB.getConnection()); //Запрос + Открытие подключения
                dB.OpenConnection();
            //using (dB.GetSshConnection())
            //{
            //dB.OpenSshConnection();
                using (MySqlDataReader reader = mySqlCommand.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {

                            ComboBoxItem menuTableItems = new ComboBoxItem();
                            menuTableItems.VerticalAlignment = VerticalAlignment.Center;
                            menuTableItems.Content = reader.GetString(0);
                            menuTableItems.Selected += MenuTableItems_Selected;
                            MenuTableBox.Items.Add(menuTableItems);
                        }

                        MenuTableBox.Items.Add(createTable);
                    }
                    MenuTableBox.SelectedItem = MenuTableBox.SelectedIndex = 0;
                }
                //}
               
        }

        private void CreateTable_Selected(object sender, RoutedEventArgs e)
        {
            ComboBoxItem comboBoxItem = sender as ComboBoxItem;
            if(comboBoxItem.Content.ToString() == "Create table")
            {
                CreateTable table = new CreateTable();
                table.Show();
            }
        }

        private void MenuTableItems_Selected(object sender, RoutedEventArgs e)
        {
            DB dB = new DB();
            string item = sender.ToString();
            int index = item.IndexOf(":");
            string column = item.Remove(1, index + 1);
            string column1 = column.Remove(0, 1);
            MySqlCommand mySqlCommand = new MySqlCommand($"SELECT * FROM {column1}", dB.getConnection()); //Запрос + Открытие подключения
            dB.OpenConnection();
            if (MenuTableBox.Items[MenuTableBox.Items.Count-1].ToString() == "Create table")
            {
                CreateTable table = new CreateTable();
                table.Show();
            }
            else
            {
                getData(column1, usedata.Content.ToString());
            }

        }
        public static string Convertion(object convert) 
        {
            if (convert == null)
            {
                return null;
            }
            else
            {
                string item = convert.ToString();
                int index = item.IndexOf(":");
                string column = item.Remove(1, index + 1);
                string column1 = column.Remove(0, 1);
                return column1;
            }
                
        }
        private void BtnPersonCreateAndUpdate_Click(Object sender, RoutedEventArgs e)
        {
            //Функция добавления пользователя
           /* add();
            //Функция обновления таблицы
            getData();
*/

        }

        public void getData(string table,string database)
        {
            
            DB dB = new DB();
                
            int i = 0;
            MySqlCommand showtables = new MySqlCommand($"SHOW TABLES FROM {database}", dB.getConnection());

            MySqlDataReader reader = showtables.ExecuteReader();
            while (reader.Read())
            {
                i++;
            }
            if(i != 0)
            {
                DataTable dt = new DataTable();
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter();//Адаптер 
                MySqlCommand mySqlCommand = new MySqlCommand($"SELECT * FROM {table}", dB.getConnection());
                //Запрос + Открытие подключения
                mySqlDataAdapter.SelectCommand = mySqlCommand;
                // Присваивание Дефолтных значение вDataGridCustomers
                if (table == "Create table")
                {

                }
                else
                {
                    mySqlDataAdapter.Fill(dt);//Присваеваем данные в таблицу
                    DataGridCustom.ItemsSource = dt.DefaultView;
                }
            }

            // Таблица в которую помещаем данные
            
                
            
        }
        public void add()
        {
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void deleteAll(object sender, RoutedEventArgs e)
        {
                try
                {
                    DB dB = new DB();


                    var res = MessageYesNo("Вы действительно хотите удалить все данные?");
                    if (res == true)
                    {
                        DataTable dt = new DataTable();
                        MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter();
                        MySqlCommand mySqlCommand = new MySqlCommand($"DELETE FROM `{DelidTable.Text}` WHERE `id` >= 0", dB.getConnection());
                        mySqlDataAdapter.SelectCommand = mySqlCommand;
                        mySqlDataAdapter.Fill(dt);//Присваеваем данные в таблицу
                        dataGrid.ItemsSource = dt.DefaultView;
                    getData(DelidTable.Text, usedata.Content.ToString());
                    }
                }
                catch (Exception ex)
                {
                    MessageError(ex.Message);
                }
            
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
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            bool isTableName = false;
            DB db = new DB();
            request req = new request();
            List<string> a = new List<string>() { "PROD","CAT","CAR","ORD","USE"};
            List<string> g = req.GetTables(db.getConnection().Database);
            for (int i = 0; i < g.Count;)
            {
                for (int j = 0; j < a.Count; j++)
                {
                    if (g[i].ToUpper().IndexOf(a[j]) == -1) {

                        isTableName = true;
                    }
                    else
                    {

                    }
                }
                break;

            }
            if (isTableName)
            {
                if (MessageYesNo("Данная база данных служит для учета магазина?") == true)
                {

                    if (MessageYesNo("Создать необходимые таблицы?") == true)
                    {
                        CreateStore();
                    }

                }
            }
            //*if (g[i].ToUpper().IndexOf("PROD") == -1  || g[i].ToUpper().IndexOf("CAT") == -1 || g[i].ToUpper().IndexOf("CAR") == -1 || g[i].ToUpper().IndexOf("ORD") == -1 || g[i].ToUpper().IndexOf("USE") == -1)
            /*{
                if(MessageYesNo("Данная база данных служит для учета магазина?") == true)
                {

                    if (MessageYesNo("Создать необходимые таблицы?") == true)
                    {
                        CreateStore();
                    }

                }
                else
                {
                    break;
                }
            }*//**/
            //}
        }

        public void CreateStore()
        {
            request request = new request();
            process processes = new process();
            processes.Show();

            processes.proccessbar.Maximum = tables.Length;
            for (int i = 0; i < tables.Length; i++)
            {
                Thread.Sleep(50);
                processes.proccessbar.Value = i;
                processes.processtext.Text = tables[i].Substring(0, 19);
                request.createtable(tables[i]);
            }
            Thread.Sleep(150);
            if (processes.proccessbar.Value == tables.Length - 1)
            {
                MainWindow main = new MainWindow();
                main.Activate();
                main.getTable();
                processes.Close();
                if (MessageYesNo("Магазин создан. Хотите заполнить таблицы данными автоматически?"))
                {
                    genericDataStore(20);
                }
            }
            
        }
        List<string> familiyaMen = new List<string>() {"Круглов","Абрикосов","Иванов","Курнышев","Владов","Анехин","Лебедев","Колгаков","Котин","Альбедов","Ников","Уликов","Кароткин","Мешков","Дятлов","Громов"};
        List<string> ImyaMen = new List<string>() { "Антон", "Владислав", "Михаил", "Константин", "Владимир", "Алексей", "Иван", "Павел", "Дмитрий", "Анатолий", "Артем", "Александр", "Виктор", "Сергей", "Олег", "Георгий" };
        List<string> OtchestvoMen = new List<string>() { "Сергеевич", "Владиславович", "Михайлович", "Константинович", "Владимирович", "Алексеевич", "Иванович", "Павлович", "Дмитриевич", "Анатольевич", "Артемович", "Александрович", "Викторович", "Олегович", "Георгевич","Даннович"};
        List<string> OtchestvoWo = new List<string>() { "Сергеевна", "Владиславовна", "Михайловна", "Константиновна", "Владимировна", "Алексеевна", "Ивановна", "Павловна", "Дмитриевна", "Анатольевна", "Артемовна", "Александровна", "Викторовна", "Олеговна", "Георгеевна","Ульяновна"};
        List<string> familiyaWo = new List<string>() { "Машкова", "Круглова","Напреева","Мишина", "Котова", "Нежена", "Олина", "Макарова", "Палона", "Малинина", "Машина", "Тумеева", "Котлина", "Данкова", "Толина", "Сотова","Томова"};
        List<string> ImyaWo = new List<string>() { "Маша", "Полина", "Татьяна", "Ольга", "Галина", "Алла", "Александра", "Дина", "Светлана", "Тамара", "Ульяна", "Ксения", "Анна", "Алина", "Наталья", "Елена"};
        List<string> heshmass = new List<string>() {"q","w","e","r","t","y","u","i","o","p", "a", "s", "d", "f", "g", "h", "j", "k","l", "z", "x", "c", "v", "b", "n", "m"};
        List<string> namecategory = new List<string>() {"Газировка","Пироги","Кофе","Алкоголь","Крупы","Выпечка","Другое"};
        List<string> productproisv = new List<string>() { "«ПРОДТОРГ»", "«Ульяновский сахарный завод»", "«ГАЗТОРГ", "«СОЮЗ»", "«КОНФЕКТУМ»" };
        List<string> color = new List<string>() { "красный", "оранжевый", "жёлтый", "зелёный", "голубой", "синий", "фиолетовый" };
        List<string> country = new List<string> { "Австралия", "Афганистан", "Бангладеш", "Бутан", "Вануату", "Вьетнам", "Индия", "Индонезия", "Камбоджа", "Кирибати", "Китай", "Корейская Народно-Демократическая Республика", "Лаосская Народно-Демократическая Республика", "Малайзия", "Мальдивские Острова", "Маршалловы Острова", "Микронезия (Федеративные Штаты)" };
        List<string> nameproduct = new List<string>() {"Печение","Конфеты","Баранира","Колбаса","Туалетная бумага","Губки","Швабра","Сахар"}; 







        public void genericDataStore(int count)
        {
            request req = new request();
            string data = "INSERT INTO `user`(`first_name`, `last_name`, `patronymic`, `date_reg`, `gender`, `login`, `password`, `photo`, `phone`, `token`, `role`, `bonusnumber`, `subscription`) VALUES ";
            for (int i = 0; i < count; i++)
            {
                string hash = "";
                int year = new Random().Next(1958, 2005);
                int randmouth = new Random().Next(1, 12);
                string mouth = randmouth.ToString().Length == 1 ? "0" + randmouth : randmouth.ToString();
                int randday = new Random().Next(1, 30);
                string day = randday.ToString().Length == 1 ? "0" + randday : randday.ToString();
                int randhour = new Random().Next(1, 24);
                int randminute = new Random().Next(1, 60);
                int randsecond = new Random().Next(1, 60);
                string hour = randhour.ToString().Length == 1 ? "0" + randhour : randhour.ToString();
                string minute = randminute.ToString().Length == 1 ? "0" + randminute : randminute.ToString();
                string second = randsecond.ToString().Length == 1 ? "0" + randsecond : randsecond.ToString();
                
                for(int j = 0; j < 20; j++)
                {
                    Thread.Sleep(50);
                    switch (j%5)
                    {
                        case 0:
                            hash += heshmass[new Random().Next(0,heshmass.Count)];
                            continue;
                        case 1:
                            hash += new Random().Next(0, heshmass.Count).ToString();
                            continue;
                        case 2:
                            hash += heshmass[new Random().Next(0, heshmass.Count)];
                            continue;
                        case 3:
                            hash += new Random().Next(0, 1) == 0 ? heshmass[new Random().Next(0,4)] : heshmass[j].ToUpper();
                            continue;
                        case 4:
                            hash += new Random().Next(0, 1) == 1 ? heshmass[new Random().Next(7, 10)].ToUpper() : new Random().Next(0,20).ToString();
                            continue;

                    }    
                }
                Thread.Sleep(50);
                if (i % 2 == 0)
                {
                    data += $"('{ImyaMen[new Random().Next(0, familiyaMen.Count)]}','{familiyaMen[new Random().Next(0, familiyaMen.Count)]}','{OtchestvoMen[new Random().Next(0, familiyaMen.Count)]}','{year}-{mouth}-{day} {hour}:{minute}:{second}','Мужчина','{year + mouth + day + ImyaMen[i]}','newpass','newphoto','{new Random().Next(890000000, 899999999) + new Random().Next(10, 99)} ','{hash}','0','500','basic'),";
                }
                else
                {
                    data += $"('{ImyaWo[new Random().Next(0, familiyaWo.Count)]}','{familiyaWo[new Random().Next(0, familiyaWo.Count)]}','{OtchestvoWo[new Random().Next(0, familiyaWo.Count)]}','{year}-{mouth}-{day} {hour}:{minute}:{second}','Женщина','{year + mouth + day + ImyaMen[i]}','newpass','newphoto','{new Random().Next(890000000, 899999999) + new Random().Next(10, 99)} ','{hash}','0','500','basic'),";
                }

                
            //INSERT INTO `user`(`id_user`, `first_name`, `last_name`, `patronymic`, `date_reg`, `gender`, `login`, `password`, `photo`, `phone`, `token`, `role`, `bonusnumber`, `subscription`) VALUES 
            }

            //MessageInformation(data);
            req.insert(usedata.Content.ToString(),data.Substring(0,data.Length - 1));
            data = "INSERT INTO `category`(`name`) VALUES ";
            for(int i = 0;i< namecategory.Count; i++)
            {
                data += "('" + namecategory[i] + "'),";
            }
            req.insert(usedata.Content.ToString(), data.Substring(0, data.Length - 1));
            data = "INSERT INTO `product`(`category_id`, `name`, `price`, `color`, `country`, `size`, `count`) VALUES ";
            for(int i = 0; i < count; i++)
            {
                data += "('" + new Random().Next(1,namecategory.Count) + "','" + nameproduct[new Random().Next(1, nameproduct.Count)]+ " " + productproisv[new Random().Next(1, productproisv.Count)] + "','" + new Random().Next(50,1000) + "','" + color[new Random().Next(1, color.Count)] + "','" + country[new Random().Next(1, country.Count)] + "','" + new Random().Next(1,700) + "','" + new Random().Next(1, 100) + "'),";

            }
            req.insert(usedata.Content.ToString(), data.Substring(0, data.Length - 1));

        }
        private void timerTick(object sender, EventArgs e)
        {
            
        }
        private void Min(object sender,EventArgs e)
        {
            if(this.WindowState == WindowState.Minimized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Minimized;
            }
        }
        DataGrid dataGrid = new DataGrid();
        private void DataGridCustomers_Loaded(object sender, RoutedEventArgs e)
        {

            /*DB dB = new DB();
            int count = 1;
            string names = "";

            MySqlCommand nameTable = new MySqlCommand("SHOW TABLES", dB.getConnection());
            dB.OpenConnection();
            MySqlDataReader reader = nameTable.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {

                    dataGrid.CanUserAddRows = true;
                    dataGrid.CanUserDeleteRows = true;
                    dataGrid.CanUserResizeRows = true;
                    dataGrid.AutoGenerateColumns = true;
                    DataTable dt = new DataTable();
                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter();
                    MySqlCommand tables = new MySqlCommand($"SELECT * FROM `{reader}`");
                    dB.OpenConnection();
                    mySqlDataAdapter.SelectCommand = tables;
                    mySqlDataAdapter.Fill(dt);//Присваеваем данные в таблицу
                    dataGrid.ItemsSource = dt.DefaultView;
                    Panel.Children.Add(dataGrid);

                }
            }
            else
            {
                MessageError("Таблицы не найдены, создать таблицу?", "Таблицы", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (MessageBoxButton.YesNo == MessageBoxButton.OK)
                {
                    CreateTable createTable = new CreateTable();
                    createTable.Show();
                }
            }
            reader.Close();
            */
        }

        public void getTriggers()
        {
            eventsListName.Items.Clear();
            eventsListName.ItemsSource = new request().GetTriggers(usedata.Content.ToString());
        }
        private void Search(object sender, RoutedEventArgs e)
        {
            CreateColumn.Items.Clear();
            DB db = new DB();
           
            MySqlCommand Insert = new MySqlCommand($"USE favorite; SELECT * FROM {NameTable.Text};", db.getConnection());
            if (NameTable.Text != "")
            {
                db.OpenConnection();


                try
                {

                    MySqlCommand nameTable = new MySqlCommand($"DESC {NameTable.Text}", db.getConnection());
                    db.OpenConnection();
                    MySqlDataReader reader1 = nameTable.ExecuteReader();
                    if (reader1.HasRows)
                    {
                        while (reader1.Read())
                        {
                            if ((reader1.GetValue(4) is string || reader1.GetValue(4) is int) || (reader1.GetValue(5).ToString() == "auto_increment"))
                            {

                            }
                            else
                            {
                                Label label = new Label();
                                TextBox box = new TextBox();
                                
                                /*Button del = new Button();
                                del.Content = "x";
                                del.FontSize = 5;
                                del.Width = 10;
                                del.Height = 10;*/
                                box.MaxLength = 100;
                                box.Width = 100;
                                label.Foreground = Brushes.White;
                                box.Foreground = Brushes.White;
                                box.Background = Brushes.Transparent;
                                box.BorderThickness = new Thickness(0, 0, 0, 1);
                                label.Content = reader1.GetString(0);
                                CreateColumn.Items.Add(label);
                                CreateColumn.Items.Add(box);
                                //CreateColumn.Items.Add(del);
                            }
                        }
                    }
                    reader1.Close();
                }catch(Exception ex)
                {
                    MessageError(ex.Message);
                }
                
            }
            else
            {
                MessageError("You not enterer table");
            }
        }
        string colunm,values,col,val;

        private void RemoveData(object sender, RoutedEventArgs e)
        {
            try
            {
                DB dB = new DB();
                string removeTable = Convertion(MenuTableBox.SelectedItem);
                
                if (MessageBox.Show("Вы действительно хотите удалить таблицу", "Удаление таблицы", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    DataTable dt = new DataTable();
                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter();
                    MySqlCommand remove = new MySqlCommand($"DROP TABLE {removeTable}", dB.getConnection());
                    mySqlDataAdapter.SelectCommand = remove;
                    dB.OpenConnection();
                    mySqlDataAdapter.Fill(dt);//Присваеваем данные в таблицу
                    DataGridCustom.ItemsSource = dt.DefaultView;
                    getTable();
                    getData(removeTable, usedata.Content.ToString());
                }
            }catch (Exception ex)
            {
                MessageError(ex.Message);
            }
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void UpdateSearchColumn(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateListBox.Items.Clear();
                DB db = new DB();
                if (UpdateSearchTable.SelectedItem.ToString() != "")
                {

                    string autoI = "";
                    int i = 0;
                    MySqlCommand auto = new MySqlCommand($"DESC {UpdateSearchTable.SelectedItem.ToString()}",db.getConnection());
                    db.OpenConnection();
                    MySqlDataReader reader = auto.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            i++;
                            if (reader.GetString(5) == "auto_increment")
                            {
                                autoI = reader.GetString(0);
                            }
                        }
                    }
                    reader.Close();
                    MySqlCommand nameTable = new MySqlCommand($"SHOW COLUMNS FROM {UpdateSearchTable.SelectedItem.ToString()}", db.getConnection());
                    db.OpenConnection();
                    MySqlDataReader reader1 = nameTable.ExecuteReader();
                    while (reader1.Read())
                    {
                        if (reader1.GetString(0) == autoI)
                        {

                        }
                        else
                        {
                            Label label = new Label();
                            TextBox box = new TextBox();
                            box.Background = Brushes.Transparent;
                            box.Foreground = Brushes.White;
                            box.BorderThickness = new Thickness(0, 0, 0, 1);
                            label.Foreground = Brushes.White;
                            box.MaxLength = 100;
                            box.Width = 100;

                            label.Content = reader1.GetString(0);
                            UpdateListBox.Items.Add(label);
                            UpdateListBox.Items.Add(box);
                        }
                    }


                    reader1.Close();

                }
                else
                {
                    MessageInformation("You not enterer table");
                }
            }catch(Exception ex)
            {
                MessageError(ex.Message);
            }
        }
        string upcol, upval, upcolval;

        private void Status(object sender, RoutedEventArgs e)
        {
            try
            {
                string privilage;
                DB dB = new DB();
                MySqlCommand insert = new MySqlCommand($"USE {/*Connected.database*/"favorite"}; SHOW GRANTS FOR '{/*Connected.username*/"root"}'@'{/*Connected.server*/"localhost"}';", dB.getConnection());
                dB.OpenConnection();
                
                MySqlDataReader reader = insert.ExecuteReader();
                while (reader.Read())
                {
                    privilage = reader.GetString(0);
                    int startindex = reader.GetString(0).IndexOf(" ");
                    int endindex = reader.GetString(0).IndexOf(" O");
                    int index = endindex - startindex;
                    string prig = privilage.Substring(startindex, index);
  


                }
                reader.Close();
            }
            catch(Exception ex) 
            {
                MessageError(ex.Message);

            }
        }

        List<string> limited = new List<string>() {"10","20","30","All"};
        private void Limited(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < 4; i++)
            {
                ComboBoxItem boxItem = new ComboBoxItem();
                boxItem.Content = limited[i];
                boxItem.Selected += SelectLimited;
                MenuLimitBox.Items.Add(boxItem);
            }
            MenuLimitBox.SelectedIndex = 0;
            

        }
        private void SelectLimited(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Convertion(sender) == "All")
                {
                    getData(Convertion(MenuTableBox.SelectedItem),usedata.Content.ToString());
                }
                else
                {
                    DB dB = new DB();
                    DataTable dt = new DataTable();
                    MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter();
                    MySqlCommand Limit = new MySqlCommand($"SELECT * FROM `{Convertion(MenuTableBox.SelectedItem)}` LIMIT {Convertion(sender)}", dB.getConnection());
                    dB.OpenConnection();
                    mySqlDataAdapter.SelectCommand = Limit;
                    mySqlDataAdapter.Fill(dt);//Присваеваем данные в таблицу
                    DataGridCustom.ItemsSource = dt.DefaultView;
                }
            }catch(Exception ex)
            {
                MessageError(ex.Message);
            }
        }

        private void Exits(object sender, RoutedEventArgs e)
        {
            
            if(MessageYesNo("Do you really want to leave") == true)
            {
                
                foreach(Window win in App.Current.Windows)
                {
                    win.Close();
                }
            }
            //this.Hide();
            //Connected connected = new Connected();
            //connected.Show();
        }
        private void PanelOpen(object sender, RoutedEventArgs e)
        {
            this.Hide();
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void MenuTableBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //ComboBoxItem item = (ComboBoxItem)sender;
            //MessageError(MenuTableBox.Items[1].ToString());
            //NameTable.Text = item.Content.ToString();
        }



        

   

        private void QuickAction_Click(object sender, RoutedEventArgs e)
        {
            Button quick = sender as Button;
            
            switch (quick.Content.ToString()) 
            {
                case "APK":
                    SendText.Text = $"ALTER TABLE {Convertion(MenuTableBox.SelectedItem)}\r\nADD PRIMARY KEY (ID)";
                    break;

                case "DPK":
                    SendText.Text = $"ALTER TABLE {Convertion(MenuTableBox.SelectedItem)}\r\nDROP PRIMARY KEY";
                    break;

                case "AFK":
                    SendText.Text = $"ALTER {Convertion(MenuTableBox.SelectedItem)}\r\nADD CONSTRAINT constraint_name\r\nFOREIGN KEY foreign_key_name(columns)\r\nREFERENCES parent_table(columns)\r\nON DELETE action\r\nON UPDATE action;";
                    break;

                case "DFK":
                    SendText.Text = $"ALTER TABLE {Convertion(MenuTableBox.SelectedItem)}\r\nDROP FOREIGN KEY constraint_name;";
                    break;

                case "ACU":
                    SendText.Text = $"ALTER TABLE {Convertion(MenuTableBox.SelectedItem)}\r\nADD UNIQUE (ID);";
                    break;

                case "DCU":
                    SendText.Text = $"ALTER TABLE {Convertion(MenuTableBox.SelectedItem)}\r\nDROP INDEX ID;";
                    break;

                case "S":
                    SendText.Text = $"SELECT * FROM {Convertion(MenuTableBox.SelectedItem)}";
                    break;

                case "U":
                    DB dBup = new DB();
                    dBup.OpenConnection();
                    string columns = "";
                    MySqlCommand command = new MySqlCommand($"SHOW COLUMNS FROM {Convertion(MenuTableBox.SelectedItem)};", dBup.getConnection());
                    MySqlDataReader reader = command.ExecuteReader();
                    int i = 0;
                    while (reader.Read())
                    {
                        i++;
                        columns += "`" + reader.GetString("Field") + $"`='[value-{i}]',"; 
                    }
                    columns = columns.Substring(0,columns.Length - 1);
                    SendText.Text = $"UPDATE `{Convertion(MenuTableBox.SelectedItem)}` SET {columns} WHERE 1";
                    break;

                case "D":
                    SendText.Text = $"DELETE FROM `{Convertion(MenuTableBox.SelectedItem)}` WHERE 0";
                    break;

                case "I":
                    DB dBi = new DB();
                    dBi.OpenConnection();
                    string columnsi = "";
                    string values = "";
                    MySqlCommand commandi = new MySqlCommand($"SHOW COLUMNS FROM {Convertion(MenuTableBox.SelectedItem)};", dBi.getConnection());
                    MySqlDataReader readeri = commandi.ExecuteReader();
                    int ii = 0;
                    while (readeri.Read())
                    {
                        ii++;
                        columnsi += "`" + readeri.GetString("Field") + "`, ";
                        values += $"'[value-{ii}]',";
                    }
                    columnsi = columnsi.Substring(0, columnsi.Length - 1);
                    values = values.Substring(0, values.Length - 1);
                   
                    SendText.Text = $"INSERT INTO `{Convertion(MenuTableBox.SelectedItem)}`({columnsi}) VALUES ({values})";
                    break;

                case "AC":
                    SendText.Text = $"";
                    break;

                case "DC":
                    SendText.Text = $"";
                    break;
            }
        }

        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MenuTableBox.Items.Count != 0)
                {
                    DelidTable.Text = Convertion(MenuTableBox.SelectedItem);
                    DB db = new DB();
                    db.OpenConnection();
                    MySqlCommand command = new MySqlCommand($"SHOW COLUMNS FROM `{DelidTable.Text}` WHERE `key` = 'pri'", db.getConnection());
                    MySqlDataReader reader = command.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        primarykeylab.Content = "Delete_on " + reader.GetString("Field");
                    }
                }
            }catch(Exception ex)
            {
                MessageError(ex.Message);
            }
        }

        private void TabItem_GotFocus_1(object sender, RoutedEventArgs e)
        {
            //MessageError(Convertion(item));
            foreach (var item in stacsql.Children)
            {
                if (item is StackPanel)
                {
                    StackPanel panel = item as StackPanel;
                    foreach (var child in panel.Children)
                    {
                        if (child is Button) 
                        { 
                            Button quickAction = child as Button;
                            if (quickAction.Content.ToString() != "Send")
                            {
                                quickAction.Click += QuickAction_Click;
                            }

                        }
                        
                    }
                   // Button quickAction = (Button)item;
                    
                }
            }
        }
        ComboBox boxcolumn = new ComboBox();
        ComboBox boxtable = new ComboBox();
        private void customparam_Selected(object sender, SelectionChangedEventArgs e)
        {
            //MainWindow mainWindow = new MainWindow();
            TextBox box = new TextBox();
            ComboBox comboBox = sender as ComboBox;
            ComboBoxItem combo = comboBox.SelectedItem as ComboBoxItem;
            box.Margin = new Thickness(0, 5, 0, 5);
            box.Background = Brushes.Transparent;
            box.Foreground = Brushes.White;
            box.BorderThickness = new Thickness(0, 0, 0, 1);
            Button button = new Button();
            button.Content = "Множественное добавление\r\nданных";
            button.HorizontalContentAlignment = HorizontalAlignment.Center;
            button.HorizontalAlignment = HorizontalAlignment.Center;
            
            button.Margin = new Thickness(0, 5, 0, 5);
            button.BorderThickness = new Thickness(0);
            button.Background = new SolidColorBrush(Color.FromRgb(42, 42, 42));
            button.Foreground = Brushes.White;
            Label label = new Label() { Content = "Количество строк",Background=Brushes.Transparent,FontSize = 17,Foreground = Brushes.White};
            //MessageError(combo.Content.ToString());
            usedata.Content = new DB().getConnection().Database;
            boxtable.ItemsSource = new request().GetTables(usedata.Content.ToString());
            boxtable.SelectionChanged += Boxtable_SelectionChanged;
            boxtable.SelectedIndex = 0;

            
            
            switch (combo.Content.ToString())
                {

                    case "":
                        
                    break;
                    case "multiple add data":
                    button.Click += mutiple_Click;
                        blockcustomparam.Children.Add(boxtable);
                    blockcustomparam.Children.Add(boxcolumn);
                    blockcustomparam.Children.Add(label);
                    blockcustomparam.Children.Add(box);
                    blockcustomparam.Children.Add(button);
                        break;
                    case "create store":
                        button.Content = "Создать магазин";
                        button.Click += CreateStore_Click;
                        blockcustomparam.Children.Add(button);
                        break;
                }
            
        }

        private void Boxtable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            boxcolumn.ItemsSource = new request().GetColumn(boxtable.SelectedItem.ToString());
            boxcolumn.SelectedIndex = 0;
        }

        private void mutiple_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ComboBox table = blockcustomparam.Children[0] as ComboBox;
                ComboBox column = blockcustomparam.Children[1] as ComboBox;
                TextBox box = blockcustomparam.Children[3] as TextBox;
                for (int i = 0; i < Convert.ToInt32(box.Text); i++)
                {
                    string item = new request().typecolumn(usedata.Content.ToString(), table.SelectedItem.ToString(), column.SelectedItem.ToString());
                    if (item.ToLower().IndexOf("int") != -1)
                    {
                        new request().insertOneColumn(table.SelectedItem.ToString(), column.SelectedItem.ToString(), new Random().Next(0, new request().maxValueCount(usedata.Content.ToString(), table.SelectedItem.ToString(), column.SelectedItem.ToString())).ToString());
                    }
                    else if (item.ToLower().IndexOf("var") != -1)
                    {
                        new request().insertOneColumn(table.SelectedItem.ToString(), column.SelectedItem.ToString(), "values");
                    }
                    else if (item.ToLower().IndexOf("char") != -1)
                    {
                        new request().insertOneColumn(table.SelectedItem.ToString(), column.SelectedItem.ToString(), "v");
                    }
                    else if (item.ToLower().IndexOf("bool") != -1)
                    {
                        new request().insertOneColumn(table.SelectedItem.ToString(), column.SelectedItem.ToString(), i%2 == 0 ? "true" : "false");
                    }
                    MessageInformation("Data inserted");
                }
            }catch (Exception ex)
            {
                MessageError(ex.Message);
            }

        }

        private void deleteColumn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox combobox = sender as ComboBox;
                ComboBoxItem item = combobox.SelectedItem as ComboBoxItem;
                if (item != null)
                {
                    if (MessageYesNo("Вы действительно хотите удалить колонку " + item.Content.ToString()) == true)
                    {
                        DB db = new DB();
                        db.OpenConnection();

                        MySqlCommand command = new MySqlCommand($"ALTER TABLE {Convertion(MenuTableBox.SelectedItem)} DROP COLUMN {item.Content}", db.getConnection());
                        command.ExecuteNonQuery();
                        getColumn(Convertion(MenuTableBox.SelectedItem), usedata.Content.ToString(), deleteColumn);
                        getData(Convertion(MenuTableBox.SelectedItem), usedata.Content.ToString());


                    }
                }
            }catch (Exception ex)
            {
                MessageError(ex.Message);
            }

        }
        public void getColumn(string table,string database,object update)
        {
            try
            {
                if (MenuTableBox.Items.Count != 0)
                {
                    ComboBox box = update as ComboBox;
                    box.Items.Clear();
                    DB db = new DB();
                    db.OpenConnection();
                    MySqlCommand command = new MySqlCommand($"USE {database};SHOW COLUMNS FROM {table}", db.getConnection());
                    MySqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        ComboBoxItem item = new ComboBoxItem();
                        item.Content = reader.GetString("Field");
                        box.Items.Add(item);
                    }
                }
            }catch(Exception ex)
            {
                MessageError(ex.Message);
            }
        }
        private void deleteColumn_Loaded(object sender, RoutedEventArgs e)
        {

            getColumn(Convertion(MenuTableBox.SelectedItem),usedata.Content.ToString(),deleteColumn);
        }

        private void eventsend_Click(object sender, RoutedEventArgs e)
        {

            var trigger = new TriggersWindow();
            trigger.tablebox.ItemsSource = new request().GetTables(usedata.Content.ToString());
            trigger.Show();
        }

        private void eventsList_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                eventsListName.ItemsSource = new request().GetTriggers(usedata.Content.ToString());
            }catch(Exception ex)
            {
                MessageError(ex.Message);
            }
        }

        

        private void eventsListName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try { 
                TriggerBox box = new TriggerBox();
                box.Foreground = Brushes.White;
                box.paramTriggers.ItemsSource = new request().GetTrigger(eventsListName.SelectedItem.ToString());
                box.paramTriggers.Foreground = Brushes.White;
               
                box.Show();
            }catch (Exception ex) 
            {
                MessageError(ex.Message);
            }
        }
        public static bool MessageYesNo(string message)
        {
            if (MessageBox.Show(message, "Question", MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                return true;

            }
            else
            {
                return false;
            }
        }
        private void CreateStore_Click(object sender, RoutedEventArgs e)
        {
            string[] nametable = {"user","cart","order","category","product"};
            string[] cart = {"id_cart","product_id","category_id","date","user_id"};
            string[] category = {"id_category","name"};
            string[] order = {"id_order","cart_id","user_id","status"};
            string[] product = {"id_product","category_id","name","price","color","country","size","count"};
            string[] user = {"id_user","first_name","last_name","patronymic","date_reg","gender","login","password","photo","phone","token","role","bonusnumber","subscription"};
            List<NameTables> tables = new List<NameTables>();
            //tables.Add(new NameTables() { user = user[1], cart = cart[1],category = category[1],order = order[1], product = product[1] });
            /*tables.AddRange(cart.FirstOrDefault());*/
            NameTables name = new NameTables();
            for(int i = 0; i < user.Length; i++)
            {
                if(i < user.Length)
                {
                    //tables.Add( { user = user[i] });
                    tables.Add(new NameTables
                    {
                        user = user[i],
                        category = i < category.Length ? category[i] : null,
                        cart = i < cart.Length ? cart[i] : null,
                        order = i < order.Length ? order[i] : null,
                        product = i < product.Length ? product[i] : null        
                    }) ;
                }
               /* if (i < cart.Length)
                {
                    tables.Add(new NameTables { cart = cart[i] });
                }
                if (i < category.Length)
                {
                    tables.Add(new NameTables { category = category[i] });
                }
                if (i < order.Length)
                {
                    tables.Add(new NameTables { order = order[i] });
                }
                if (i < product.Length)
                {
                    tables.Add(new NameTables { product = product[i] });
                }*/
            }

            Views views = new Views();
            StackPanel panel = new StackPanel();
            panel.Width = views.viewspanel.Width;
            panel.Height = views.viewspanel.Height;
            DataGrid grid = new DataGrid();
            grid.Foreground = Brushes.White;
            grid.Background = new SolidColorBrush(Color.FromRgb(42,42,42));
            grid.CanUserAddRows = true;
            grid.CanUserDeleteRows = true;
            grid.CanUserResizeRows = true;
            grid.AutoGenerateColumns = true;
            grid.BorderThickness = new Thickness(0);
            for (int i = 0; i < nametable.Length; i++)
            {
                Binding binding = new Binding();
                binding.Source = nametable[i];
                //grid.Columns.Add(new DataGridTextColumn { Header = nametable[i], Binding = binding});
                //grid.Columns[i].Width = 70;
                
                
            }
            grid.ItemsSource = tables;

            
            panel.Children.Add(grid);
            views.viewspanel.Content = panel;
            views.Show();


            //grid.Columns.Add();

            //scrollViewer.Content;
            
            
            
        }

        private void buttonnavleft_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void MenuTableBox_loaded(object sender, RoutedEventArgs e)
        {
            
            getTable();
            MenuTableBox.SelectedIndex = 0;
        }

        private void disignerTab_GotFocus(object sender, RoutedEventArgs e)
        {
            Designer designer = new Designer();

            

            designer.Show();
        }

        private void TabItem_GotFocus_2(object sender, RoutedEventArgs e)
        {
            ComboBoxItem item = MenuTableBox.SelectedItem as ComboBoxItem;
            NameTable.Text = item.Content.ToString();
        }
        private void UpdateSearchTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {   
            columntable.ItemsSource = new request().GetColumn(UpdateSearchTable.SelectedItem.ToString());
            columntable.SelectedIndex = 0;
            valuetable.ItemsSource = new request().GetValues(columntable.SelectedItem.ToString(),UpdateSearchTable.SelectedItem.ToString());
            
            
            /*foreach(var item in columntable.Items)
            {
                MessageBox.Show(item.ToString());
            }*/
        }
        private void columntable_Selected(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (UpdateSearchTable.SelectedItem != null && columntable.SelectedItem != null)
                {
                    //MessageError();
                    valuetable.ItemsSource = new request().GetValues((sender as ComboBox).SelectedItem.ToString(), UpdateSearchTable.SelectedItem.ToString());
                    valuetable.SelectedIndex = 0;
                    

                }
            }catch(Exception ex)
            {
                MessageError(ex.Message);
            }

        }

        private void TabItem_Loaded(object sender, RoutedEventArgs e)
        {
            request req = new request();
            UpdateSearchTable.ItemsSource = req.GetTables(usedata.Content.ToString());
            UpdateSearchTable.SelectedIndex = MenuTableBox.SelectedIndex;
        }

        private void Up_click(object sender, RoutedEventArgs e)
        {
            upcol = string.Empty;
            upval = string.Empty;
            upcolval = string.Empty;
            var label = new Label();
            var text = new TextBox();
            try
            {
                for (int i = 0; i < UpdateListBox.Items.Count; i++)
                {
                    if (UpdateListBox.Items[i] is Label)
                    {
                        label = UpdateListBox.Items[i] as Label;
                        upcol = "`" + label.Content.ToString() + "`= ";
                        upcolval += upcol;
                        //MessageInformation(label.Content.ToString());
                    }
                    if (UpdateListBox.Items[i] is TextBox)
                    {
                        text = UpdateListBox.Items[i] as TextBox;
                        if (text.Text == "" || text.Text == null || text.Text == string.Empty)
                        {
                            upcolval = upcolval.Remove(upcolval.Length - label.Content.ToString().Length - 4);
                        } 
                        else
                        {

                            upval = "'" + text.Text + "',";

                            upcolval += upval;
                        }

                         //MessageInformation(text.Text);
                    }
                }
                upcolval = upcolval.Remove(upcolval.Length - 1);
                
                DB db = new DB();
                DataTable dt = new DataTable();
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter();
                try
                {
                    MySqlCommand insert = new MySqlCommand($"UPDATE `{UpdateSearchTable.SelectedItem.ToString()}` SET {upcolval} WHERE {columntable.SelectedItem.ToString()} {(operatorstable.SelectedItem as ComboBoxItem).Content.ToString()} '{valuetable.SelectedItem.ToString()}'", db.getConnection());
                
                db.OpenConnection();
                
                mySqlDataAdapter.SelectCommand = insert;
                insert.ExecuteNonQuery();
                MessageInformation("Data updated");
                DataGridCustom.ItemsSource = dt.DefaultView;
                getTable();
                getData(UpdateSearchTable.Text,usedata.Content.ToString());
                columntable.ItemsSource = new request().GetColumn(UpdateSearchTable.SelectedItem.ToString());
                columntable.SelectedIndex = 0;
                valuetable.ItemsSource = new request().GetValues(columntable.SelectedItem.ToString(), UpdateSearchTable.SelectedItem.ToString());
                }
                catch (Exception adapter)
                {
                    MessageError(adapter.Message);
                }

            }
            catch (Exception ex)
            {
                MessageError(ex.Message);
            }
        }


        private void Send(object sender, RoutedEventArgs e)
        {
            try
            {
                DB dB = new DB();
                DataTable dt = new DataTable();
                MySqlDataAdapter mySqlDataAdapter = new MySqlDataAdapter();
                MySqlCommand send = new MySqlCommand(SendText.Text, dB.getConnection());
                mySqlDataAdapter.SelectCommand = send;
                dB.OpenConnection();
                mySqlDataAdapter.Fill(dt);//Присваеваем данные в таблицу
                DataGridCustom.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageError(ex.Message);
            }

        }

        private void valuetable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (valuetable.IsInitialized)
                {
                    if (valuetable.SelectedItem != null)    
                    {
                        DataGridCustom.ItemsSource = new request().getWhereValue(usedata.Content.ToString(), UpdateSearchTable.SelectedItem.ToString(), columntable.SelectedItem.ToString(), (operatorstable.SelectedItem as ComboBoxItem).Content.ToString(), valuetable.SelectedItem.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageError(ex.Message);
            }
        }

        private void UpdateData(object sender, RoutedEventArgs e)
        {
            getTable();
            getData(Convertion(MenuTableBox.SelectedItem), usedata.Content.ToString());
            
        }
        public void insertData() 
        {
            colunm = null;
            values = null;
            val = null;
            col = null;
            try
            {
                for (int i = 0; i < CreateColumn.Items.Count; i++)
                {
                    if (i % 2 == 1)
                    {
                        colunm = CreateColumn.Items[i].ToString();
                        int index = colunm.IndexOf(":");
                        string column = colunm.Remove(1, index + 1);
                        string column1 = column.Remove(0, 1);
                        column = "'" + column1 + "', ";
                        col += column;

                    }
                    if (i % 2 == 0)
                    {
                        values = CreateColumn.Items[i].ToString();
                        int index1 = values.IndexOf(":");
                        string com = values.Remove(0, index1 + 1);
                        if (com.Length == 2)
                        {
                            com = "`" + com + "`, ";
                            val += com;
                        }
                        else
                        {
                            string com1 = com.Remove(0, 1);
                            com = "`" + com1 + "`, ";
                            val += com;
                        }

                    }


                }

                val = val.Remove(val.Length - 2);
                col = col.Remove(col.Length - 2);


                DB db = new DB();
                MySqlCommand insert = new MySqlCommand($"INSERT INTO `{NameTable.Text}`({val}) VALUES ({col})", db.getConnection());
                db.OpenConnection();


                if (insert.ExecuteNonQuery() == 1)
                {
                    MessageInformation("Добавлено");
                    getData(NameTable.Text, usedata.Content.ToString());
                }
                else
                {
                    MessageError("Не удалось добавить данные");
                }
                colunm = string.Empty;
                values = string.Empty;
                col = string.Empty;
                val = string.Empty;
            }
            catch (Exception ex)
            {
                MessageError(ex.Message);
            }
        }

        public void insertDataTest(ListBox itemElement,TextBox nameTable, Label nameDatabase)
        {
            colunm = null;
            values = null;
            val = null;
            col = null;
            try
            {
                for (int i = 0; i < itemElement.Items.Count; i++)
                {
                    if (i % 2 == 1)
                    {
                        colunm = itemElement.Items[i].ToString();
                        int index = colunm.IndexOf(":");
                        string column = colunm.Remove(1, index + 1);
                        string column1 = column.Remove(0, 1);
                        column = "'" + column1 + "', ";
                        col += column;

                    }
                    if (i % 2 == 0)
                    {
                        values = itemElement.Items[i].ToString();
                        int index1 = values.IndexOf(":");
                        string com = values.Remove(0, index1 + 1);
                        if (com.Length == 2)
                        {
                            com = "`" + com + "`, ";
                            val += com;
                        }
                        else
                        {
                            string com1 = com.Remove(0, 1);
                            com = "`" + com1 + "`, ";
                            val += com;
                        }

                    }


                }

                val = val.Remove(val.Length - 2);
                col = col.Remove(col.Length - 2);


                DB db = new DB();
                MySqlCommand insert = new MySqlCommand($"INSERT INTO `{nameTable.Text}`({val}) VALUES ({col})", db.getConnection());
                db.OpenConnection();


                if (insert.ExecuteNonQuery() == 1)
                {
                    MessageInformation("Added");
                    getData(nameTable.Text, nameDatabase.Content.ToString());
                }
                else
                {
                    MessageError("Failed");
                }
                colunm = string.Empty;
                values = string.Empty;
                col = string.Empty;
                val = string.Empty;
            }
            catch (Exception ex)
            {
                MessageError(ex.Message);
            }
        }
        private void Insert_Click(object sender, RoutedEventArgs e)
        {


            insertData();
            

        }

        public static void MessageError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public static void MessageWarning(string message)
        {
            MessageBox.Show(message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        public static void MessageInformation(string message)
        {
            MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
    public class NameTables
    {
        public string user { get; set; }
        public string cart { get; set; }
        public string category { get; set; }
        public string product { get; set; }
        public string order { get; set; }
    }

    
}
