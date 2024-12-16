using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Valorant;
using Valorant.API;
using static Globals;

namespace Prot1
{
    /// <summary>
    /// Interaction logic for LoginRegister.xaml
    /// </summary>
    public partial class LoginRegister : Window
    {
        public Dictionary<TextBlock, Control> TextBoxes;

        public LoginRegister()
        {
            InitializeComponent();

            LoadMainPage();
            return;

            VAuth = new Vexi(true);
            if (VAuth.TryAuthViaSavedCreds())
            {
                if (VAuth.UserAccountData != null && VAuth.IsAuthenticated)
                {
                    var mw = new MainWindow();
                    mw.Show();
                    this.Close();
                }
            }

                TextBoxes = new Dictionary<TextBlock, Control>()
            {
                {textUsername, txtUsername },
                {textPwd, txtPwd}, 
                {textPwdAgain, txtPwdAgain},
                {textMail, txtMail}
            };
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            this.DragMove();
        }

        private void LoadMainPage()
        {
            //var ud = VAuth.GetAccountData();

            //if (ud != null && ud.StatusCode == HttpStatusCode._200_OK && ud.Data != null)
            {
                var mw = new MainWindow();
                mw.Show();
                this.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string user = txtUsername.Text, pwd = txtPwd.Password;

            if (string.IsNullOrEmpty(pwd))
            {
                ErrorControl.Visibility = Visibility.Visible;
                ErrorLbl.Text = "Şifre boş olamaz!";
                return;
            }

            if (string.IsNullOrEmpty(user))
            {
                ErrorControl.Visibility = Visibility.Visible;
                ErrorLbl.Text = "Kullanıcı adı boş olamaz!";
                return;
            }

            switch ((sender as FrameworkElement).Tag.ToString())
            {
                case "register":

                    string pwdAgain = txtPwdAgain.Password, mail = txtMail.Text;

                    if (string.IsNullOrEmpty(mail))
                    {
                        ErrorControl.Visibility = Visibility.Visible;
                        ErrorLbl.Text = "E-Posta boş olamaz!";
                        return;
                    }

                    if (string.IsNullOrEmpty(pwdAgain))
                    {
                        ErrorControl.Visibility = Visibility.Visible;
                        ErrorLbl.Text = "Şifre tekrar boş olamaz!";
                        return;
                    }

                    if (pwdAgain != pwd)
                    {
                        ErrorControl.Visibility = Visibility.Visible;
                        ErrorLbl.Text = "Şifreler eşleşmiyor!";
                        return;
                    }

                    if (!Regex.IsMatch(mail, @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$"))
                    {
                        ErrorControl.Visibility = Visibility.Visible;
                        ErrorLbl.Text = "Geçerli bir eposta gerekmekte!";
                        return;
                    }

                    var rl = VAuth.RegisterAccount(user, pwd, mail);
                        if (rl != HttpStatusCode._200_OK)
                        {
                            ErrorControl.Visibility = Visibility.Visible;

                            switch (rl)
                            {
                                case HttpStatusCode._400_BadRequest:
                                    ErrorLbl.Text = "Kayıt başarısız!";
                                    break;
                                case HttpStatusCode._403_Forbidden:
                                    ErrorLbl.Text = "Kullanıcı adı veya eposta zaten kayıtlı!";
                                    break;
                            }

                            return;
                        }

                    MessageBox.Show("Kayıt başarıyla tamamlandı", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    Globals.GameAuth = Auth.GetAuthLocal(true);
                    if (Globals.GameAuth != null)
                    {
                        LoadMainPage();
                    }
                    break;
                case "login":
                    var l = VAuth.LoginToAccount(user, pwd);
                        if (l != HttpStatusCode._200_OK)
                        {
                            ErrorControl.Visibility = Visibility.Visible;

                            switch (l)
                            {
                                case HttpStatusCode._401_Unauthorized:
                                    ErrorLbl.Text = "Giriş başarısız!";
                                    break;
                            }

                            return;
                        }

                        LoadMainPage();
                    break;
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            string tag = ((Button)sender).Tag.ToString();

            switch (tag)
            {
                case "register":
                    title1.Text = "Giriş Yap";
                    desc1.Text = "VEXICORE'da mevcut bir hesabınız mı var, hemen giriş yapın";
                    button1.Content = "⮜ Giriş Yap ⮞";
                    title2.Text = "Kayıt Oluştur";
                    email_box.Visibility = Visibility.Visible;
                    pwdAgainBox.Visibility = Visibility.Visible;
                    SifremiUnuttumText.Visibility = Visibility.Collapsed;
                    button2.Content = "Kaydol";
                    button2.Tag = tag;
                    txtMail.Text = txtPwd.Password = txtPwdAgain.Password = txtUsername.Text = string.Empty;
                    ((Button)sender).Tag = "login";
                    ErrorLbl.Text = "HATA!";
                    ErrorControl.Visibility = Visibility.Collapsed;
                    break;
                case "login":
                    title1.Text = "Hesap Oluştur";
                    desc1.Text = "Hesap oluşturun, VEXICORE'u hemen kullanmaya başlayın";
                    button1.Content = "⮜ Hemen Başlayın ⮞";
                    title2.Text = "Giriş Yap";
                    email_box.Visibility = Visibility.Collapsed;
                    pwdAgainBox.Visibility = Visibility.Collapsed;
                    SifremiUnuttumText.Visibility = Visibility.Visible;
                    button2.Content = "Giris Yap";
                    button2.Tag = tag;
                    ((Button)sender).Tag = "register";
                    ErrorControl.Visibility = Visibility.Collapsed;
                    ErrorLbl.Text = "HATA!";
                    txtMail.Text = txtPwd.Password = txtPwdAgain.Password = txtUsername.Text = string.Empty;
                    break;
            }
        }

        private void field_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var control = sender as TextBlock;

            TextBoxes[control].Focus();
        }

        private void field_TextChanged(object sender, dynamic e)
        {
            var control = sender as Control;

            if (!string.IsNullOrEmpty(control is TextBox ? (control as TextBox).Text : (control as PasswordBox).Password) && (control is TextBox ? (control as TextBox).Text : (control as PasswordBox).Password).Length > 0)
                TextBoxes.Keys.FirstOrDefault(x => TextBoxes[x] == control).Visibility = Visibility.Collapsed;
            else
                TextBoxes.Keys.FirstOrDefault(x => TextBoxes[x] == control).Visibility = Visibility.Visible;
        }
    }
}
