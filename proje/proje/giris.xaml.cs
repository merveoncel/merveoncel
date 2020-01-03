using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace proje
{
    /// <summary>
    /// Interaction logic for giris.xaml
    /// </summary>
    public partial class giris : Page
    {
        public giris()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Database.Connect();
            string command = "Select * from Users where Username=@s1 and Password=@s2";
            SqlCommand sqlcmd = new SqlCommand(command, Database.sc);
            sqlcmd.Parameters.AddWithValue("@s1", s1.Text);
            sqlcmd.Parameters.AddWithValue("@s2", s2.Password);
            bool f = false;
            using (SqlDataReader r = sqlcmd.ExecuteReader())
            {
                while (r.Read())
                {
                    f = true;
                    int id = (int)r["Id"];
                    Info.userId = id;
                    Info.admin = (string)r["Admin"];
                    break;
                }
            }

            if(f && Info.userId > 0)
            {
                if(Info.admin == "true")
                {
                    Application.Current.MainWindow.Height = 600;
                    Application.Current.MainWindow.Width = 800;
                    this.NavigationService.Navigate(new AdminPage());
                }
                else
                {
                    Application.Current.MainWindow.Height = 800;
                    Application.Current.MainWindow.Width = 800;
                    this.NavigationService.Navigate(new Market());//navigation to the market page
                }
            }


        }
    }
}
