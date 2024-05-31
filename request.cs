using MySqlConnector;
using MySqlX.XDevAPI.Relational;
using Syncfusion.Windows.Shared.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

namespace Seva
{
    public class request
    {

        public string createForeignKey(string database, string name, string constrTable, string constrColumn, string refernTable,string databaseref,string refernColumn, string on_del, string on_up)
        {
            try {
            DB dB = new DB();
            MySqlCommand command = new MySqlCommand($"USE {database}; ALTER TABLE `{constrTable}` ADD CONSTRAINT `{name}` FOREIGN KEY `{constrTable}`(`{constrColumn}`) REFERENCES `{databaseref}`.`{refernTable}`(`{refernColumn}`) ON DELETE {on_del} ON UPDATE {on_up};", dB.getConnection());
            dB.OpenConnection();
    
            if (command.ExecuteNonQuery() != -1)
            {   
                dB.CloseConnection();
                    //MessageBox.Show();
                return command.CommandText;
            }
            else
            {
                dB.CloseConnection();
                    //MessageBox.Show(command.CommandText);
                return command.CommandText;
            }
        }catch(Exception ex){
                
                return ex.Message;
            }
        }
        public bool createtable(string str)
        {
            try
            {
                DB dB = new DB();
                MySqlCommand command = new MySqlCommand(str, dB.getConnection());
                dB.OpenConnection();
                if (command.ExecuteScalar() != null)
                {
                    dB.CloseConnection();
                    return true;
                } else
                {
                    dB.CloseConnection();
                    return false;
                }
            } catch
            {
                return false;
            }
        }
        public int countDataInTable(string database,string table)
        {
            DB db = new DB();
            db.OpenConnection();
            MySqlCommand command = new MySqlCommand($"USE {database}; SELECT * FROM {table}", db.getConnection());
            MySqlDataReader reader = command.ExecuteReader();
            int count = 0;
            while (reader.Read())
            {
                count++;    
            }
            return count;
        }
        public bool createtable(string str, string database)
        {
            try
            {
                DB dB = new DB();
                MySqlCommand command = new MySqlCommand($"USE {database}; " + str, dB.getConnection());
                dB.OpenConnection();
                if (command.ExecuteScalar() != null)
                {
                    dB.CloseConnection();
                    return true;
                }
                else
                {
                    dB.CloseConnection();
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
        public List<string> GetValues(string column, string table)
        {
            DB db = new DB();
            db.OpenConnection();
            MySqlCommand command = new MySqlCommand($"SELECT {column} FROM {table}", db.getConnection());
            MySqlDataReader reader = command.ExecuteReader();
            List<string> a = new List<string>();
            while (reader.Read())
            {
                a.Add(reader.GetValue(0).ToString());
            }
            reader.Close();
            db.CloseConnection();
            return a;
        }
        public List<string> GetTables(string database)
        {
            DB db = new DB();
            db.OpenConnection();
            MySqlCommand command = new MySqlCommand($"SHOW TABLES FROM {database}", db.getConnection());
            MySqlDataReader reader = command.ExecuteReader();
            List<string> a = new List<string>();
            while (reader.Read())
            {
                a.Add(reader.GetString($"Tables_in_{database}"));
            }
            reader.Close();
            db.CloseConnection();
            return a;
        }
        public DataView getWhereValue(string database,string table,string column,string operation,string value)
        {
            DataTable dt = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            
            DB db = new DB();
            db.OpenConnection();
            MySqlCommand command = new MySqlCommand($"USE {database};SELECT * FROM {table} WHERE `{column}` {operation} '{value}'",db.getConnection());
            //command.ExecuteNonQuery();
            adapter.SelectCommand = command;
            adapter.Fill(dt);
            return dt.DefaultView;

        }
        public List<string> GetColumn(string table)
        {
            DB db = new DB();
            db.OpenConnection();
            MySqlCommand command = new MySqlCommand($"SHOW COLUMNS FROM {table};", db.getConnection());
            MySqlDataReader reader = command.ExecuteReader();
            List<string> a = new List<string>();
            while (reader.Read())
            {
                a.Add(reader.GetString("Field"));
            }
            reader.Close();
            db.CloseConnection();
            return a;
        }
        public List<string> GetTrigger(string triggername)
        {
            List<string> operators = new List<string>() {
"TRIGGER_SCHEMA",
"TRIGGER_NAME",
"EVENT_MANIPULATION",
            "EVENT_OBJECT_TABLE",
                "ACTION_TIMING",
                "CREATED",
                "DEFINER",
                "COLLATION_CONNECTION",
                "ACTION_STATEMENT"
            };
            DB db = new DB();
            db.OpenConnection();
            MySqlCommand command = new MySqlCommand($"SELECT * FROM INFORMATION_SCHEMA.TRIGGERS WHERE `TRIGGERS`.`TRIGGER_NAME` = '{triggername}';", db.getConnection());
            MySqlDataReader reader = command.ExecuteReader();
            List<string> a = new List<string>();
            string line = "";
            ListBoxItem definition = new ListBoxItem();
            definition.Content = " ";
           for(int i = 0;i< 65; i++)
            {
                line += "-";
            }
            while (reader.Read())
            {

                    a.Add(operators[0] + ": " + reader.GetValue(1).ToString());
                    a.Add(operators[1] + ": " + reader.GetValue(2).ToString());
                    a.Add(operators[2] + ": " + reader.GetValue(3).ToString());
                    a.Add(operators[3] + ": " + reader.GetValue(6).ToString());
                    a.Add(operators[4] + ": " + reader.GetValue(11).ToString());
                    a.Add(operators[5] + ": " + reader.GetValue(16).ToString());
                    a.Add(operators[6] + ": " + reader.GetValue(18).ToString());
                    a.Add(operators[7] + ": " + reader.GetValue(20).ToString());
                    foreach (char symb in reader.GetValue(9).ToString()) 
                    {
                    if (definition.Content.ToString().Length >= 50)
                        {
                            definition.Content += "\r\n";

                        }
                        else
                        {
                            definition.Content += symb.ToString();
                        }
                    }
                    a.Add(operators[8] + ": " + definition);
                    a.Add(line);



            }
            reader.Close();
            db.CloseConnection();
            return a;
        }
    
        public List<string> GetTriggers(string dbname)
        {
            DB db = new DB();
            db.OpenConnection();
            MySqlCommand command = new MySqlCommand($"SHOW TRIGGERS FROM {dbname};", db.getConnection());
            MySqlDataReader reader = command.ExecuteReader();
            List<string> a = new List<string>();
            while (reader.Read())
            {
                a.Add(reader.GetString("Trigger"));
            }
            reader.Close();
            db.CloseConnection();
            return a;
        }
        /* public ListBox getTriggers(string database)
         {
             DB db = new DB();
             db.OpenConnection();
             MySqlCommand command = new MySqlCommand($"USE {database};SHOW TRIGGERS", db.getConnection());
             MySqlDataReader reader = command.ExecuteReader();
             ListBox a = new ListBox();
             a.Height = 255;
             a.BorderThickness = new System.Windows.Thickness(0);
             a.Width = 170;
             a.Background = new SolidColorBrush(Color.FromRgb(42, 42, 42));
             a.Foreground = Brushes.White;
             while (reader.Read())
             {
                 StackPanel panel = new StackPanel();
                 panel.Orientation = Orientation.Horizontal;
                 Image image = new Image() { Source = new BitmapImage(new Uri(Environment.CurrentDirectory + "\\delete.png")), Width = 25, Height = 25};

                 Button btndel = new Button() { Width = 25,Height = 25,Content = image};
                 btndel.Click += Btndel_Click;


             }
             reader.Close();
             db.CloseConnection();
             return a;
         }

         private void Btndel_Click(object sender, System.Windows.RoutedEventArgs e)
         {
             int i = 0;
             var list = new MainWindow().eventsListName;
             foreach (var listblockbtnandnametrigger in list.Items)
             {
                 StackPanel panel = listblockbtnandnametrigger as StackPanel;
                 foreach(var block in panel.Children)
                 {
                     if((block is Button)
                     string nametrigger = block as 
                     if (block is Button && block as Button == sender as Button)
                     {

                         deleteTriggers(new MainWindow().usedata.Content.ToString(),);
                     }
                 }

                 i++;
             }
         }
         public void deleteTriggers(string database,string trigger)
         {
             DB db = new DB();
             db.OpenConnection();
             MySqlCommand command = new MySqlCommand($"USE {database};DROP TRIGGER {trigger}");
             command.ExecuteNonQuery();
         }*/
        public bool insertOneColumn(string table,string column,string value) 
        {
            DB db = new DB();
            db.OpenConnection();
            MySqlCommand command = new MySqlCommand($"INSERT INTO `{table}`(`{column}`) VALUES ('{value}')", db.getConnection());
            if(command.ExecuteNonQuery() == 0)
            { 
                return false;
            }
            else
            {
                return true; 
            }
        }
        public bool createdatabase (string name) 
        {
            DB db = new DB();
            db.OpenConnection();
            MySqlCommand command = new MySqlCommand($"CREATE DATABASE {name}", db.getConnection());
            if (command.ExecuteNonQuery() == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public string[] GetForeignKeyReferencesTable(string table,string ref_table)
        {   
            DB db = new DB ();
            db.OpenConnection();
            MySqlCommand command = new MySqlCommand($"SELECT TABLE_NAME, COLUMN_NAME, CONSTRAINT_NAME, REFERENCED_TABLE_NAME,REFERENCED_COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_NAME LIKE '{table}' AND REFERENCED_TABLE_NAME = '{ref_table}';",db.getConnection());
            MySqlDataReader reader = command.ExecuteReader();
            string[] columns = new string[2];
            while (reader.Read())
            {
                columns[1] = reader.GetString(1);
                columns[2] = reader.GetString(4);
            }
            return columns;
        }  
        public List<string> ShowDatabase()
        {
            DB db = new DB();
            db.OpenConnection();
            MySqlCommand command = new MySqlCommand($"SHOW DATABASES", db.getConnection());
            MySqlDataReader reader = command.ExecuteReader();
            List<string> a = new List<string>();
            while (reader.Read())
            {
                if (reader.GetString("Database") == "information_schema" || reader.GetString("Database") == "mysql" || reader.GetString("Database") == "performance_schema" || reader.GetString("Database") == "sys")
                {

                }
                else
                {
                    a.Add(reader.GetString("Database"));
                }
            }
            reader.Close();
            db.CloseConnection();
            return a;
        }
        public void insert(string database, string str)
        {
            DB db = new DB();
            db.OpenConnection();
            MySqlCommand command = new MySqlCommand($"USE {database};" + str, db.getConnection());
            command.ExecuteNonQuery();
            db.CloseConnection();
        }
        public string export(string database)
        {
            try
            {
                DB db = new DB();
                db.OpenConnection();
                string textexp = "";
                MySqlCommand commandtable = new MySqlCommand($"SHOW TABLES FROM `{database}`;", db.getConnection());
                MySqlDataReader readertable = commandtable.ExecuteReader();
                List<string> tables = new List<string>();
                while (readertable.Read())
                {
                    tables.Add(readertable.GetString($"Tables_in_{database}"));
                }
                readertable.Close();
                textexp += $"--------------start_{database}------------";
                for (int i = 0; i < tables.Count; i++)
                {
                    MySqlCommand commandexp = new MySqlCommand($"SHOW CREATE TABLE {tables[i]};", db.getConnection());
                    MySqlDataReader readerexp = commandexp.ExecuteReader();
                    while (readerexp.Read())
                    {
                        textexp += readerexp.GetValue(1).ToString() + "\r\n";
                        textexp += "-----------------------------------------------\r\n";
                    }
                    if (i == tables.Count - 1)
                    {
                        readerexp.Close();
                    }
                }
                textexp += $"--------------end_{database}------------";
                using (FileStream sw = new FileStream(Environment.CurrentDirectory + $"\\tablesuser.sql",FileMode.OpenOrCreate))
                {
                    byte[] buff = new byte[textexp.Length];
                    sw.Write(buff, 0, buff.Length);
                        //WriteLine(textexp);
                }
                return "database exported";
            }catch
            {
                return "database not exported";
            }
        }

        public int countColumn(string dbname, string table)
        {
            DB db = new DB();
            MySqlCommand command = new MySqlCommand($"SHOW COLUMNS FROM {table};", db.getConnection());
            db.OpenConnection();
            MySqlDataReader reader = command.ExecuteReader();
            int i = 0;
            while (reader.Read()) 
            {
                i++;        
            }
            reader.Close();
            db.CloseConnection();
            return i;
        }
        public string typecolumn(string dbname, string tablename,string column_name)
        {
            DB db = new DB();

            MySqlCommand command = new MySqlCommand($"USE {dbname}; DESC {tablename}", db.getConnection());
            db.OpenConnection();
            MySqlDataReader reader = command.ExecuteReader();
            List<string> list = new List<string>();
            while(reader.Read())
            {
                //list.Add(reader.GetString("Field"));
                //list.Add(reader.GetString("Type"));
                if (reader.GetString("Field").ToString() == column_name)
                {
                    return reader.GetString("Type").ToString();
                }
            }

            reader.Close();
            db.CloseConnection();
            return null;
            
        }
        public int maxValueCount(string dbname, string tablename, string column_name)
        {
            DB db = new DB();

            MySqlCommand command = new MySqlCommand($"USE {dbname}; DESC {tablename}", db.getConnection());
            db.OpenConnection();
            MySqlDataReader reader = command.ExecuteReader();
            List<string> list = new List<string>();
            while (reader.Read())
            {
                //list.Add(reader.GetString("Field"));
                //list.Add(reader.GetString("Type"));
                if (reader.GetString("Field").ToString() == column_name)
                {
                    int countstartindex = reader.GetString("Type").ToString().IndexOf("(");
                    if (countstartindex != -1)
                    {
                        int countendindex = reader.GetString("Type").ToString().IndexOf("(", countstartindex);
                        return Convert.ToInt32(reader.GetString("Type").ToString().Substring(countstartindex,countendindex-countstartindex));
                    }
                    
                    //return reader.GetString("Type").ToString();
                }
            }

            reader.Close();
            db.CloseConnection();
            return -1;

        }

    }
}
