using ProjectManagerCoreLib;
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

namespace Project_Manager_Core
{
    /// <summary>
    /// Logique d'interaction pour ConnectionWindow.xaml
    /// </summary>
    public partial class ConnectionWindow : Window
    {
        public ConnectionWindow()
        {
            InitializeComponent();
        }

        // Action au clique sur le bouton de connexion
        private void Connection_button_Click(object sender, RoutedEventArgs e)
        {
            // Remise à zéro du label informatif
            label_info.Content = "";
            // Tentative de création d'un objet User à partir du login et du mot de passe
            User user = User.connectUser(identifiant_textBox.Text, password_textBox.Password);

            // Si la création n'a pas réussi
            if (user.getNom() == null)
            {
                // On vide le champs du mot de passe
                password_textBox.Password = "";
                // On affiche un message d'erreur
                label_info.Foreground = new SolidColorBrush(Color.FromRgb(255, 0, 0));
                label_info.Content = "Identifiant ou mot de passe invalide";
            }
            else
            {
                // On mémorise l'utilisateur authentifié dans la vriable globale
                App.currentUser = user;

                // Actualisation de liste des application pour activer celles qui nécéssite d'être authentifié
                MainWindow.refreshAppList();

                // Affichage du bouton de déconnexion au lieu de celui de connexion
                MainWindow.window.but_login.Visibility = Visibility.Hidden;
                MainWindow.window.but_logout.Visibility = Visibility.Visible;

                // Changement du texte du titre de la barre supérieure
                MainWindow.window.user_name_label.Content = string.Format("Bienvenue {0} {1} !", user.getPrenom(), user.getNom());

                // On ferme la fenêtre de connexion
                this.Close();
            }

        }

        private void mail_field_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(identifiant_textBox.Text) && !string.IsNullOrEmpty(password_textBox.Password))
                connection_button.IsEnabled = true;
            else
                connection_button.IsEnabled = false;
        }

        private void password_field_TextChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(identifiant_textBox.Text) && !string.IsNullOrEmpty(password_textBox.Password))
                connection_button.IsEnabled = true;
            else
                connection_button.IsEnabled = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            label_info.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            label_info.Content = "Saisissez votre identifianbt et votre mot de passe";
        }
    }
}
