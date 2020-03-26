using ProjectManagerCoreLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Project_Manager_Core
{
    /// <summary>
    /// Logique d'interaction pour Inscription.xaml
    /// </summary>
    public partial class Inscription : Window
    {
        // Variable privées
        private string name = "";
        private string first_name = "";
        private string mail = "";
        private DateTime birth_date = new DateTime();
        private string description = "";
        private string password = "";
        private bool isPasswordIdentical = false;

        // Constructeur de la fenêtre
        public Inscription()
        {
            InitializeComponent();

            birth_date_datepicker.DisplayDate = DateTime.Now;
        }

        private void ValiderInscription_Click(object sender, RoutedEventArgs e)
        {
            User.createUser(name_textbox.Text, firstname_textbox.Text, mail_textbox.Text, birth_date_datepicker.SelectedDate.Value, description_textbox.Text, Utils.SHA256Hash(password_textbox.Password));
            this.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void checkFormInfo(object sender, TextChangedEventArgs e)
        {
            if (ValiderInscription != null)
            {
                TextBox textbox = sender as TextBox;

                if(textbox != null)
                { 
                    switch (textbox.Tag as string)
                    {
                        case "name":
                            this.name = textbox.Text;
                            break;
                        case "first_name":
                            this.first_name = textbox.Text;
                            break;
                        case "mail":
                            this.mail = textbox.Text;
                            if (User.isMailUsed(this.mail) || !Utils.IsValidEmail(this.mail))
                            {
                                mail_textbox.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                                this.mail = "";
                            }
                            else
                            {
                                mail_textbox.Background = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                            }
                            break;
                        case "description":
                            this.description = textbox.Text;
                            break;
                    }
                }

                if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(first_name) && !string.IsNullOrWhiteSpace(mail) && (birth_date.Date != null) && !string.IsNullOrWhiteSpace(description) && isPasswordIdentical)
                {
                    ValiderInscription.IsEnabled = true;
                }
                else
                {
                    ValiderInscription.IsEnabled = false;
                }
            }
        }

        private void birth_date_datepicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            this.birth_date = birth_date_datepicker.SelectedDate.GetValueOrDefault();
            checkFormInfo(null, null);
        }

        private void password_textbox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            this.password = password_textbox.Password;
            checkFormInfo(null, null);
        }

        private void password_confirm_textbox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (password_confirm_textbox.Password == this.password && !string.IsNullOrEmpty(password_textbox.Password))
            {
                password_confirm_textbox.Background = new SolidColorBrush(Color.FromRgb(0,255,0));
                isPasswordIdentical = true;
            }
            else
            {
                password_confirm_textbox.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                isPasswordIdentical = false;
            }

            checkFormInfo(null, null);
        }
    }
}
