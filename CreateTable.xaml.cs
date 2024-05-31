    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
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
    //using MySql.Data.MySqlClient;
    using MySqlConnector;
    using MySqlX.XDevAPI.Relational;

    namespace Seva
    {
        /// <summary>
        /// Логика взаимодействия для CreateTable.xaml
        /// </summary>
        public partial class CreateTable : Window
        {



            public CreateTable()
            {
                    InitializeComponent();   
            }
            bool _question = true;
            bool _new = false;
            private void Window_Loaded(object sender, RoutedEventArgs e)
            {
                _question = true;
            }

            private void cancel_Click(object sender, RoutedEventArgs e)
            {
                Close();
            }

            private void create_Click(object sender, RoutedEventArgs e)
            {
                try
                {
                    if (comm.Text[comm.Text.Length].ToString() != ";")
                    {
                        comm.Text += ";";
                    }
                    int finish = comm.Text.IndexOf(";");
                    if (comm.Text.ToUpper().IndexOf("USER", finish) != -1)
                    {
                        MainWindow.MessageWarning("Удалите все инструкции после знака ';'");
                    }
                    else
                    {
                        DB dB = new DB();
                        MySqlCommand command = new MySqlCommand(comm.Text, dB.getConnection());
                        if (command.ExecuteNonQuery() != -1)
                        {
                            Connected.MessageInformation("Тable is created");
                        }
                        dB.CloseConnection();
                        if (_new == true)
                        {
                            new request().createtable(comm.Text, "tablesuser");
                            Thread.Sleep(100);
                            new request().export("tablesuser");
                        

                        }
                        MainWindow main = new MainWindow();

                    }


                }
                catch(Exception ex)
                {
                    MainWindow.MessageError(ex.Message);
                }
            }
            List<string> tablesname = new request().GetTables("tablesuser");
            private void comm_TextChanged(object sender, TextChangedEventArgs e)
            {
                    try
                    {
                        Thread.Sleep(100);
                        foreach (var text in comm.Text.Split(' '))
                        {
                            if (_question == true)
                            {
                                for (int i = 1; i < tablesname.Count; i++)
                                {
                                    string textsub = tablesname[i].ToString().Substring(0, tablesname[i].Length - 2);
                                    if (text.IndexOf(textsub) != -1)
                                    {
                                        if (MainWindow.MessageYesNo($"Вы создаете таблицу {tablesname[i]}") == true)
                                        {
                                            if (MainWindow.MessageYesNo($"Cоздать таблицу {tablesname[i]} автоматически") == true)
                                            {
                                                Create(tablesname[i], new DB().getConnection().Database);
                                                new MainWindow().Show();
                                            }

                                            break;

                                        }
                                        else
                                        {
                                            _question = false;
                                            _new = true;
                                        }

                                    }
                                    else
                                    {
                                        _new = true;
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }


                    }
                    catch (Exception ex)
                    {
                        MainWindow.MessageError(ex.Message);
                    }
            }
            private async void Create(string nametable,string database)
            {
                try
                {
                    string sqltext = "";
                    using (FileStream filesssql = new FileStream(Environment.CurrentDirectory + $"\\tablesuser.sql", FileMode.OpenOrCreate))
                    {
                        byte[] buff = new byte[filesssql.Length];
                        await filesssql.ReadAsync(buff, 0, buff.Length);
                        sqltext = Encoding.Default.GetString(buff);

                    }
                    string sql = "";
                    int indexstarttable = sqltext.IndexOf($"CREATE TABLE `{nametable}`");
                    int indexendtable = sqltext.IndexOf(";", indexstarttable);
                    if (indexstarttable != -1 && indexendtable != -1)
                    {
                        sql += sqltext.Substring(indexstarttable, indexendtable - indexstarttable) + "; \r\n";
                        //MainWindow.MessageInformation(sql);
                    }


                    MainWindow.MessageInformation(sql);
                } catch(Exception ex) 
                {
                    MainWindow.MessageError(ex.Message);
                }
            }

            private void comm_KeyDown(object sender, KeyEventArgs e)
            {
                if(e.Key == Key.Enter)
                {
                    comm.Text += "\n";
                }
            }
        }
    }
