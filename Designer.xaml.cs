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
    /// Логика взаимодействия для Designer.xaml
    /// </summary>
    public partial class Designer : Window
    {
        public Designer()
        {
            InitializeComponent();
        }
        private System.Windows.Point? _movePointPanel;
        private System.Windows.Point? _movePointLabel;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                getatblesview();

            }catch(Exception ex)
            {
                MainWindow.MessageError(ex.Message);
            }
            
        }

        

        private void Panel_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _movePointPanel = null;
            (sender as StackPanel).ReleaseMouseCapture();
        }

        private void Panel_MouseMove(object sender, MouseEventArgs e)
        {
            if (_movePointPanel == null)
                return;
            Canvas.SetLeft(sender as StackPanel, (e.GetPosition(this) - (Vector)_movePointPanel.Value).X);
            Canvas.SetTop(sender as StackPanel,(e.GetPosition(this) - (Vector)_movePointPanel.Value).Y);
            
            
        }

        public void getatblesview()
        {
            Canvasblock.ClipToBounds = true;
            DB db = new DB();
            db.OpenConnection();
            MySqlCommand command = new MySqlCommand($"SHOW TABLES FROM `{db.getConnection().Database}`", db.getConnection());
            MySqlDataReader readertables = command.ExecuteReader();
            while (readertables.Read())
            {
                Random random = new Random();
                StackPanel panel = new StackPanel() { Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(26, 26, 26)) };
                panel.MouseDown += Panel_MouseDown;
                panel.MouseMove += Panel_MouseMove;
                panel.MouseUp += Panel_MouseUp;
                StackPanel stacknametable = new StackPanel();
                TextBlock nametable = new TextBlock() { HorizontalAlignment = HorizontalAlignment.Center, Foreground = System.Windows.Media.Brushes.White, FontSize = 16 };
                StackPanel parametres = new StackPanel();
                ListBox stackellips = new ListBox() { VerticalContentAlignment = VerticalAlignment.Center, Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(26, 26, 26)), BorderThickness = new Thickness(0) };
                ListBox stacknames = new ListBox() { VerticalContentAlignment = VerticalAlignment.Center, Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(26, 26, 26)), BorderThickness = new Thickness(0) };

                parametres.Orientation = Orientation.Horizontal;
                nametable.TextWrapping = TextWrapping.Wrap;
                nametable.Text = db.getConnection().Database + " : " + readertables.GetString(0);
                stacknametable.Children.Add(nametable);
                DB db1 = new DB();
                db1.OpenConnection();
                MySqlCommand cmd = new MySqlCommand($"SHOW COLUMNS FROM `{readertables.GetString(0)}`", db1.getConnection());
                MySqlDataReader readercolumn = cmd.ExecuteReader();
                while (readercolumn.Read())
                {
                    TextBox textparam = new TextBox() { BorderThickness = new Thickness(0.5), Height = 16, Margin = new Thickness(0, 4, 0, 4), FontSize = 12, VerticalContentAlignment = VerticalAlignment.Center, Foreground = System.Windows.Media.Brushes.White, Background = System.Windows.Media.Brushes.Transparent, Text = readercolumn.GetString("Field") + " : " + readercolumn.GetString("Type"), IsReadOnly = true, TextWrapping = TextWrapping.Wrap };
                    StackPanel ellipse = new StackPanel() { Width = 10, Height = 10, Background = System.Windows.Media.Brushes.White, Margin = new Thickness(5) };

                    stackellips.Items.Add(ellipse);
                    
                    stacknames.Items.Add(textparam);


                }
                readercolumn.Close();
                db1.CloseConnection();
                parametres.Children.Add(stackellips);
                parametres.Children.Add(stacknames);
                panel.Children.Add(stacknametable);
                panel.Children.Add(parametres);

                Canvas.SetLeft(panel, random.Next(1, 700));

                Canvas.SetTop(panel, random.Next(1, 350));
                Canvasblock.Children.Add(panel);
                //какая-то другая линия в тот же Bitmap

            }
            readertables.Close();
            db.CloseConnection();
        }
        private void Panel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _movePointPanel = e.GetPosition(sender as StackPanel);
            (sender as StackPanel).CaptureMouse();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        private void foreign_Click(object sender, RoutedEventArgs e)
        {
            new foreignkeywindow().Show();
        }

        private void Label_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _movePointLabel = null;
            (sender as Label).ReleaseMouseCapture();
        }
        private void Label_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _movePointLabel = e.GetPosition(sender as Label);
            (sender as Label).CaptureMouse();
        }

        private void Label_MouseMove(object sender, MouseEventArgs e)
        {
            if (_movePointLabel == null)
                return;
            Canvas.SetLeft(sender as Label, (e.GetPosition(this) - (Vector)_movePointLabel.Value).X);
            Canvas.SetTop(sender as Label, (e.GetPosition(this) - (Vector)_movePointLabel.Value).Y);
        }

        private void createtable_Click(object sender, RoutedEventArgs e)
        {
            new CreateTable().Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
    
}
