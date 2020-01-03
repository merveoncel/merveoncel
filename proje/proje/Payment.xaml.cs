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
    /// Interaction logic for Payment.xaml
    /// </summary>
    public partial class Payment : Page
    {
        Dictionary<int, int> d;
        public Payment(List<product> products, Dictionary<int,int> dic)
        {
            InitializeComponent();
            double price = 0;
            foreach (var item in products)
            {
                sepetlist.Items.Add(item);
                price += item.Price * item.Quantity;
            }
            username.Content = Market.me.Username;
            phone.Content = Market.me.Phone;
            address.Content = Market.me.Address;
            totalprice.Content = price.ToString();
            d = dic;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Database.Connect();
            foreach (var item in d)
            {
                string command = "Update Products set Stock -= @qu where Id = @Id";
                SqlCommand sqlcmd = new SqlCommand(command, Database.sc);
                sqlcmd.Parameters.AddWithValue("@Id", item.Key);
                sqlcmd.Parameters.AddWithValue("@qu", item.Value);
                sqlcmd.ExecuteNonQuery();
            }

            MessageBox.Show("Your order has been taken!");
            Application.Current.MainWindow.Height = 800;
            Application.Current.MainWindow.Width = 800;
            this.NavigationService.Navigate(new Market());
        }
    }
}
