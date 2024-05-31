using MySqlConnector;
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

namespace Seva
{
    /// <summary>
    /// Логика взаимодействия для TriggersWindow.xaml
    /// </summary>
    public partial class TriggersWindow : Window
    {
        public TriggersWindow()
        {
            InitializeComponent();
        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox box = sender as ComboBox;
            DB db = new DB();
            db.OpenConnection();
            MySqlCommand command = new MySqlCommand("",db.getConnection());
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                box.Items.Add(reader.GetString(0));
            }
            reader.Close();
            db.CloseConnection();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DB db = new DB();
                db.OpenConnection();
                string def = definer.Text == "" ? null : definer.Text;
                MySqlCommand command = new MySqlCommand($"CREATE {def} TRIGGER `{triggername.Text}` {(timebox.SelectedItem as ComboBoxItem).Content} {(eventbox.SelectedItem as ComboBoxItem).Content} ON `{tablebox.SelectedItem}` FOR EACH ROW  {difinition.Text}", db.getConnection());
                command.ExecuteNonQuery();
                db.CloseConnection();
                new MainWindow().getTriggers();
                MainWindow.MessageInformation("Триггер создан");
                
            }catch (Exception ex)
            {
                MainWindow.MessageError(ex.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
