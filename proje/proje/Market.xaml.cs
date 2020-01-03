using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
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
    /// Interaction logic for Market.xaml
    /// </summary>
    public partial class Market : Page
    {
        public Market()
        {
            InitializeComponent();
            ReadProducts();
            ReadMe();
        }

        private void ReadMe()
        {
            me = ReadUser(Info.userId);
            s1.Text = me.Username;
            s2.Text = me.Password;
            s3.Text = me.Phone;
            s4.Text = me.Email;
            s5.Text = me.Address;
            s6.Text = me.Admin;
        }

        public static user me;

        private user ReadUser(int id)
        {
            Database.Connect();
            string command = "Select * from Users where Id = @Id";
            SqlCommand sqlcmd = new SqlCommand(command, Database.sc);
            sqlcmd.Parameters.AddWithValue("@Id", id);
            using (SqlDataReader r = sqlcmd.ExecuteReader())
            {
                while (r.Read())
                {
                    user x = new user();
                    x.Id = (int)r["Id"];
                    x.Username = (string)r["Username"];
                    x.Password = (string)r["Password"];
                    x.Phone = (string)r["Phone"];
                    x.Email = (string)r["Email"];
                    x.Address = (string)r["Address"];
                    x.Admin = (string)r["Admin"];
                    return x;
                }
            }
            return null;
        }

        private void ReadProducts()
        {
            productlist.Items.Clear();
            Database.Connect();
            string command = "Select * from Products";
            SqlCommand sqlcmd = new SqlCommand(command, Database.sc);
            using (SqlDataReader r = sqlcmd.ExecuteReader())
            {
                while (r.Read())
                {
                    product x = new product();
                    x.Id = (int)r["Id"];
                    x.Productname = (string)r["Productname"];
                    x.Price = (double)r["Price"];
                    x.Stock = (int)r["Stock"];
                    x.Image = (string)r["Image"];
                    if(x.Image != "")
                    {
                        using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(x.Image)))
                        {
                            x.LImage = BitmapFrame.Create(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                        }
                    }
                    productlist.Items.Add(x);
                }
            }

        }

        double price = 0;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (productlist.SelectedIndex < 0) return;
            product p = productlist.SelectedItem as product;
            price += p.Price;
            p.Quantity++;

            if (!sepetlist.Items.Contains(p))
            {
                sepetlist.Items.Add(p);
            }

            sepetlist.Items.Refresh();
            UpdatePrice();
        }

        private void RefreshList()
        {
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (sepetlist.SelectedIndex < 0) return;
            product p = productlist.SelectedItem as product;
            price -= p.Price;
            p.Quantity--;

            if (p.Quantity == 0)
                sepetlist.Items.Remove(p);

            sepetlist.Items.Refresh();
            UpdatePrice();
        }

        private void UpdatePrice()
        {
            pricelabel.Content = "Total Price: " + price.ToString();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Database.Connect();
            string command = "Update Users set Username = @Username, Password = @Password, @Phone = Phone, @Email = Email, Address = @Address where Id = @Id";
            SqlCommand sqlcmd = new SqlCommand(command, Database.sc);
            sqlcmd.Parameters.AddWithValue("@Id", me.Id);
            sqlcmd.Parameters.AddWithValue("@Username", s1.Text);
            sqlcmd.Parameters.AddWithValue("@Password", s2.Text);
            sqlcmd.Parameters.AddWithValue("@Phone", s3.Text);
            sqlcmd.Parameters.AddWithValue("@Email", s4.Text);
            sqlcmd.Parameters.AddWithValue("@Address", s5.Text);
            sqlcmd.ExecuteNonQuery();
            ReadMe();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if(sepetlist.Items.Count <= 0)
            {
                MessageBox.Show("Your basket is empty!");
                return;
            }
            List<product> list = new List<product>();
            Dictionary<int, int> dic = new Dictionary<int, int>();
            foreach (var item in sepetlist.Items)
            {
                product p = item as product;
                if(!dic.ContainsKey(p.Id))
                    dic.Add(p.Id, 0);
                dic[p.Id] += p.Quantity;
                list.Add(p);
                if(p.Stock<dic[p.Id])
                {
                    MessageBox.Show("Stock of " + p.Productname + " is limited to " + p.Stock + "!");
                    return;
                }
            }

            Application.Current.MainWindow.Width = 850;
            Application.Current.MainWindow.Height = 750;
            this.NavigationService.Navigate(new Payment(list,dic));
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Height = 325;
            Application.Current.MainWindow.Width = 325;
            this.NavigationService.Navigate(new giris());
        }
    }
}
