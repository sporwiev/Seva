using System;
using System.Collections.Generic;
using System.IO;
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
using System.Text.Json;
using Microsoft.VisualBasic;
using Essy.Tools.InputBox;


namespace Seva
{
    /// <summary>
    /// Логика взаимодействия для select.xaml
    /// </summary>
    public partial class select : Window
    {
        public select()
        {
            InitializeComponent();
            Showdatabase();
           
        }
        public void Showdatabase()
        {
            glavstack.Children.Clear();
            StackPanel panelblock = new StackPanel();
            panelblock.Orientation = Orientation.Horizontal;
            //glavstack.Orientation = Orientation.Horizontal;
            /*using (FileStream fs = new FileStream("../../user.json", FileMode.OpenOrCreate))
            {*/
                //Account acc = await JsonSerializer.DeserializeAsync<Account>(fs);
                //Console.WriteLine($"Name: {person?.Name}  Age: {person?.Age}");

                //MessageError();
                using (MySqlConnection conn = new MySqlConnection($"server=localhost;port=3306;username=root;password=root;"))
                {
                    conn.Open();
                    MySqlCommand command = new MySqlCommand($"SHOW DATABASES;", conn);
                    MySqlDataReader reader = command.ExecuteReader();
                    int i = 0;
                StackPanel stackplus = new StackPanel { Margin = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Center, Orientation = Orientation.Vertical };
                Button buttonplus = new Button { Background = Brushes.Transparent, Width = 100, BorderThickness = new Thickness(0) };
                buttonplus.Click += createdb;
                StackPanel panelplus = new StackPanel { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Center };
                Image imageplus = new Image { Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "/databases.png")), Height = 60, Width = 60 };
                TextBlock textplus = new TextBlock { Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Center, Text = "+", FontSize = 17};
                panelplus.Children.Add(imageplus);
                panelplus.Children.Add(textplus);
                buttonplus.Content = panelplus;
                stackplus.Children.Add(buttonplus);
                panelblock.Children.Add(stackplus);
                //glavstack.Children.Add(stackplus);

                while (reader.Read())
                    {
                        i++;
                        if (reader.GetString(0) == "sys" || reader.GetString(0) == "information_schema" || reader.GetString(0) == "performance_schema" || reader.GetString(0) == "mysql")
                        {

                        }
                        else
                        {
                            var stack = createBlock(reader);
                            //glavstack.Children.Add(stack);
                            //stack.Children.Add(text);
                            if(panelblock.Children.Count == 4)
                            {
                                glavstack.Children.Add(panelblock);
                                panelblock = new StackPanel();
                                panelblock.Orientation = Orientation.Horizontal;
                            
                            }
                            panelblock.Children.Add(stack);
                        //MessageBox.Show(reader.GetString(0));
                            //MessageError((i % 3).ToString());

                        }
                    }
                    reader.Close();
                    

                }
            




        }
        public StackPanel createBlock(MySqlDataReader reader)
        {
            StackPanel stack = new StackPanel { Margin = new Thickness(5), HorizontalAlignment = HorizontalAlignment.Center, Orientation = Orientation.Vertical };
            Button button = new Button { Background = Brushes.Transparent, Width = 100, BorderThickness = new Thickness(0) };
            button.Click += selectbd;
            StackPanel panel = new StackPanel { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Center };
            Image image = new Image { Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "/databases.png")), Height = 60, Width = 60 };
            TextBlock text = new TextBlock { Foreground = Brushes.White, HorizontalAlignment = HorizontalAlignment.Center, Text = reader.GetString(0) };
            panel.Children.Add(image);
            panel.Children.Add(text);
            button.Content = panel;
            stack.Children.Add(button);
            return stack;
        }
        private void selectbd(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            StackPanel stack = button.Content as StackPanel;
            TextBlock text = stack.Children[1] as TextBlock;
            DB.database = text.Text;
            MainWindow main = new MainWindow();
            main.Show();
        }
        private void createdb(object sender, RoutedEventArgs e)
        {
            string box = Interaction.InputBox("Введите название новой базы данных", "Создание базы данных");

            
            if (box.Length != 0 && new request().createdatabase(box))
            {
                new CreateTable().Show();
                //main.Show();
                
            }
            Showdatabase();
        }
    }
}
