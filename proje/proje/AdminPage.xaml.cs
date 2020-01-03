using System;
using System.Collections.Generic;
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
    public class user
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Admin { get; set; }

        public override string ToString()
        {
            return Id + ", " + Username + ", " + Admin;
        }
    }

    public class product
    {
        public int Id { get; set; }
        public string Productname { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
        public int Quantity { get; set; } = 0;
        public string Image;

        public BitmapFrame LImage { get; set; }
           
        public override string ToString()
        {
            return Id + ", " + Productname + ", Price: " + Price + ", Stock: " + Stock;
        }
    }

    /// <summary>
    /// Interaction logic for AdminPage.xaml
    /// </summary>
    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
            ReadUsers();
            ReadProducts();
        }

        private void ReadUsers()
        {
            userlist.Items.Clear();
            Database.Connect();
            string command = "Select * from Users";
            SqlCommand sqlcmd = new SqlCommand(command, Database.sc);
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
                    userlist.Items.Add(x);
                }
            }

        }

        private void DeleteUser(int id)
        {
            Database.Connect();
            string command = "Delete from Users where Id = @Id";
            SqlCommand sqlcmd = new SqlCommand(command, Database.sc);
            sqlcmd.Parameters.AddWithValue("@Id", id);
            sqlcmd.ExecuteNonQuery();
        }

        private void AddUser(user x)
        {
            Database.Connect();
            string command = "Insert into Users values (@s1, @s2,@s3,@s4,@s5,@s6)";
            SqlCommand sqlcmd = new SqlCommand(command, Database.sc);
            sqlcmd.Parameters.AddWithValue("@s1", x.Username);
            sqlcmd.Parameters.AddWithValue("@s2", x.Password);
            sqlcmd.Parameters.AddWithValue("@s3", x.Phone);
            sqlcmd.Parameters.AddWithValue("@s4", x.Email);
            sqlcmd.Parameters.AddWithValue("@s5", x.Address);
            sqlcmd.Parameters.AddWithValue("@s6", x.Admin);
            sqlcmd.ExecuteNonQuery();
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
                    if (x.Image != "")
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

        private void DeleteProduct(int id)
        {
            Database.Connect();
            string command = "Delete from Products where Id = @Id";
            SqlCommand sqlcmd = new SqlCommand(command, Database.sc);
            sqlcmd.Parameters.AddWithValue("@Id", id);
            sqlcmd.ExecuteNonQuery();
        }

        private void AddProduct(product x)
        {
            Database.Connect();
            string command = "Insert into Products values (@s1, @s2,@s3,@s4)";
            SqlCommand sqlcmd = new SqlCommand(command, Database.sc);
            sqlcmd.Parameters.AddWithValue("@s1", x.Productname);
            sqlcmd.Parameters.AddWithValue("@s2", x.Price);
            sqlcmd.Parameters.AddWithValue("@s3", x.Stock);
            sqlcmd.Parameters.AddWithValue("@s4", x.Image);
            sqlcmd.ExecuteNonQuery();
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            user x = new user();
            x.Username = s1.Text;
            x.Password = s2.Text;
            x.Phone = s3.Text;
            x.Email = s4.Text;
            x.Address = s5.Text;
            x.Admin = s6.Text;
            AddUser(x);
            ReadUsers();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (userlist.SelectedIndex < 0) return;
            user x = userlist.SelectedItem as user;
            DeleteUser(x.Id);
            ReadUsers();
        }

        public static string ImageToBase64String(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            string r = Convert.ToBase64String(memStream.ToArray());
            memStream.Close();
            return r;
        }

        public static BitmapImage Base64StringToImage(string str)
        {
            using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(str)))
            {
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.EndInit();
                return image;
            }
        }

        //https://stackoverflow.com/questions/15779564/resize-image-in-wpf
        private static BitmapFrame CreateResizedImage(ImageSource source, int width, int height, int margin)
        {
            var rect = new Rect(margin, margin, width - margin * 2, height - margin * 2);

            var group = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(group, BitmapScalingMode.HighQuality);
            group.Children.Add(new ImageDrawing(source, rect));

            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
                drawingContext.DrawDrawing(group);

            var resizedImage = new RenderTargetBitmap(
                width, height,         // Resized dimensions
                96, 96,                // Default DPI values
                PixelFormats.Default); // Default pixel format
            resizedImage.Render(drawingVisual);

            return BitmapFrame.Create(resizedImage);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".png";
            dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                BitmapImage img = new BitmapImage();
                Uri uri = new Uri(filename);
                img.BeginInit();
                img.UriSource = uri;
                img.DecodePixelHeight = 100;
                img.DecodePixelWidth = 100;
                img.EndInit();
                //imgcontrol.Source = Base64StringToImage(ImageToBase64String(img));
                imgcontrol.Source = img;
                imgcontrol.Stretch = Stretch.Fill;
                p4.Text = ImageToBase64String(img);
                p4.IsEnabled = false;
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            product x = new product();
            x.Productname = p1.Text;
            x.Price = Convert.ToDouble(p2.Text);
            x.Stock = int.Parse(p3.Text);
            x.Image = p4.Text;
            AddProduct(x);
            ReadProducts();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (productlist.SelectedIndex < 0) return;
            product x = productlist.SelectedItem as product;
            DeleteProduct(x.Id);
            ReadProducts();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (productlist.SelectedIndex < 0) return;
            product x = productlist.SelectedItem as product;
            string command = "Update Products set Stock += @qu where Id = @Id";
            SqlCommand sqlcmd = new SqlCommand(command, Database.sc);
            sqlcmd.Parameters.AddWithValue("@Id", x.Id);
            sqlcmd.Parameters.AddWithValue("@qu", int.Parse(stockinc.Text));
            sqlcmd.ExecuteNonQuery();
            ReadProducts();
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.Height = 325;
            Application.Current.MainWindow.Width = 325;
            this.NavigationService.Navigate(new giris());
        }
    }
}
