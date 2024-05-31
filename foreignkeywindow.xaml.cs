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

namespace Seva
{
    /// <summary>
    /// Логика взаимодействия для foreignkeywindow.xaml
    /// </summary>
    public partial class foreignkeywindow : Window
    {
        public foreignkeywindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            request request = new request();
            cTableBox.ItemsSource = request.GetTables(new DB().getConnection().Database);
            cTableBox.SelectedIndex = 0;
            cTableBox.SelectionChanged += CTableBox_SelectionChanged;
            databaseBox.ItemsSource = request.ShowDatabase();
            databaseBox.SelectedIndex = 0;
            databaseBox.SelectionChanged += DatabaseBox_SelectionChanged;
            Thread.Sleep(1000);


        }

        private void DatabaseBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                
                ComboBox combo = sender as ComboBox;
                request request = new request();
                databasetables.ItemsSource = request.GetTables(combo.SelectedItem.ToString());
                databasetables.SelectedIndex = 0;
                databasetables.SelectionChanged += Databasetables_SelectionChanged;
                
            }
            catch (Exception ex)
            {
                MainWindow.MessageError(ex.Message);
            }
        }

        private void Databasetables_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                fColumnBox.ItemsSource = null;
                ComboBox combo = sender as ComboBox;
                request request = new request();
                //MessageBox.Show(combo.SelectedItem.ToString());

                fColumnBox.ItemsSource = request.GetColumn(combo.SelectedItem.ToString());
                fColumnBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MainWindow.MessageError(ex.Message);
            }
        }

        private void CTableBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox combo = sender as ComboBox;
                request request = new request();
                cColumnBox.ItemsSource = request.GetColumn(combo.SelectedItem.ToString());
                cColumnBox.SelectedIndex = 0;
                
            }
            catch (Exception ex)
            {
                MainWindow.MessageError(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var req = new request().createForeignKey(new MainWindow().usedata.Content.ToString(), namekey.Text, cTableBox.SelectedItem.ToString(), cColumnBox.SelectedItem.ToString(), databasetables.SelectedItem.ToString(), databaseBox.SelectedItem.ToString(), fColumnBox.SelectedItem.ToString(), (ondel.SelectedItem as ComboBoxItem).Content.ToString(), (onup.SelectedItem as ComboBoxItem).Content.ToString());
                if (req == "true")
                {
                    MainWindow.MessageInformation(req);
                }
                else
                {
                    MainWindow.MessageError(req);
                }
            }catch(Exception ex)
            {
                MainWindow.MessageError(ex.Message);
            }
        }
    }
}
