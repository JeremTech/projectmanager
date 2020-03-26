using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using MySql.Data.MySqlClient;
using ProjectManagerCoreLib;
using Path = System.IO.Path;

namespace Project_Manager_Core
{
    /// <summary>
    /// Logique d'interaction pour Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        private DatabaseSettings databaseSettings;
        private bool hasModifications = false;

        public Settings()
        {
            InitializeComponent();

            // Chargement des paramètres de base de données actuels
            databaseSettings = new DatabaseSettings(Path.Combine(Utils.GetCoreExecutionFolder(), "Settings", "database.json"));
            databaseSettings.ReadConfiguration();

            // Affichage des paramètres de base de données actuels
            this.database_adress_textbox.Text = databaseSettings.GetDatabaseServer();
            this.database_name_textbox.Text = databaseSettings.GetDatabaseName();
            this.database_username_textbox.Text = databaseSettings.GetUser();
            this.database_password_passwordBox.Password = databaseSettings.GetPassword();

            // Remise à zéro des composants graphiques
            database_test_square_label.Foreground = new SolidColorBrush(Color.FromRgb(87, 131, 156));

        }

        private void save_button_Click(object sender, RoutedEventArgs e)
        {
            // Enregistrement des paramètres de connexion à la base de données
            databaseSettings.WriteConfiguration(this.database_adress_textbox.Text, this.database_username_textbox.Text, this.database_password_passwordBox.Password, this.database_name_textbox.Text);

            // Redesactivation du bouton
            save_button.IsEnabled = false;
            hasModifications = true;
        }

        // Evènement lié au changement du texte contenu dans la textbox de l'adresse/serveur de la base de données
        private void database_adress_textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(database_adress_textbox.Text))
            {
                database_adress_textbox.Background = new SolidColorBrush(Color.FromRgb(255,0,0));
            }
            else
            {
                database_adress_textbox.Background = new SolidColorBrush(Color.FromRgb(45, 45, 48));
            }

            updateSaveBoutonStatus();
        }

        // Evènement lié au changement du texte contenu dans la textbox du nom de la base de données
        private void database_name_textbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(database_name_textbox.Text))
            {
                database_name_textbox.Background = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            }
            else
            {
                database_name_textbox.Background = new SolidColorBrush(Color.FromRgb(45, 45, 48));
            }

            updateSaveBoutonStatus();
        }

        /// <summary>
        /// Fonction permettant de mettre à jour la propriété <c>enable</c> du bouton de sauvegarde ainsi que de rendre visible ou non le label d'informations
        /// </summary>
        private void updateSaveBoutonStatus()
        {
            if (!string.IsNullOrWhiteSpace(database_adress_textbox.Text) && !string.IsNullOrWhiteSpace(database_name_textbox.Text))
            {
                this.save_button.IsEnabled = true;
                this.database_test_connection_button.IsEnabled = true;
                this.info_label.Visibility = Visibility.Hidden;
            }
            else
            {
                this.database_test_connection_button.IsEnabled = false;
                this.save_button.IsEnabled = false;
                this.info_label.Visibility = Visibility.Visible;
            }
        }

        // Event "click" sur le bouton de tester de connexion à la base de données
        private async void database_test_connection_button_Click(object sender, RoutedEventArgs e)
        {
            bool isConn = false;
            MySqlConnection dbConnection = new MySqlConnection(string.Format("server={0};user={1};pwd={2};", database_adress_textbox.Text, database_username_textbox.Text, database_password_passwordBox.Password));
            
            // Mise à jour de l'interface
            database_test_connection_button.IsEnabled = false;
            database_test_progressbar.IsIndeterminate = true;
            database_adress_textbox.IsEnabled = false;
            database_name_textbox.IsEnabled = false;
            database_password_passwordBox.IsEnabled = false;
            database_username_textbox.IsEnabled = false;

            // Task permettant de vérifier la connexion à la base de données
            await Task.Factory.StartNew(() => 
            { 
                try
                {
                    dbConnection.Open();
                    isConn = true;
                }
                catch (ArgumentException a_ex)
                {
                    LogFile.writeLog("Erreur lors de la connexion à la base de données : " + a_ex.Message);
                    LogFile.writeLog("Exception : " + a_ex.ToString());
                    LogFile.writeLog("Merci de vérifier vos informations de connexion");
                }
                catch (MySqlException ex)
                {
                    LogFile.writeLog("Erreur lors de la connexion à la base de données : " + ex.Message);
                    LogFile.writeLog("Source : " + ex.Source);
                    LogFile.writeLog("Code erreur : " + ex.Number);

                    isConn = false;

                    // Liste des code erreurs : http://dev.mysql.com/doc/refman/5.0/en/error-messages-server.html
                    switch (ex.Number)
                    {
                        case 1042: // Impossible de se connecter
                            LogFile.writeLog("Erreur : Impossible de se connecter au serveur spécifié.");
                            break;
                        case 0: // Accès refusé (mauvais identifiants, ...)
                            LogFile.writeLog("Erreur : Accès refusé.");
                            LogFile.writeLog("Merci de vérifier vos informations de connexion");
                            break;
                        default:
                            LogFile.writeLog("Erreur : Inconnu");
                            break;
                    }
                }
                finally
                {
                    if (dbConnection.State == ConnectionState.Open)
                    {
                        dbConnection.Close();
                    }
                }
            });

            database_test_connection_button.IsEnabled = true;
            database_test_progressbar.IsIndeterminate = false;
            database_adress_textbox.IsEnabled = true;
            database_name_textbox.IsEnabled = true;
            database_password_passwordBox.IsEnabled = true;
            database_username_textbox.IsEnabled = true;


            if (isConn)
                database_test_square_label.Foreground = new SolidColorBrush(Color.FromRgb(0,255,0));
            else
                database_test_square_label.Foreground = new SolidColorBrush(Color.FromRgb(255,0,0));
        }

        // Evènement lié à la fermteture de la fenêtre
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Si des modifications ont eu lieu, il est nécessaire de redémarrer l'application
            if (this.hasModifications)
            {
                // Affichage d'un message de confirmation
                MessageBoxResult msgResult = MessageBox.Show("Afin de prendre en compte les modifications, Project Manager va redémarrer.", "Redémarrage requis", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                
                // Si l'utilisateur clique sur "OK", on redémarre
                if (msgResult == MessageBoxResult.OK)
                {
                    // Recupération du chemin de l'executable
                    var filePath = Assembly.GetExecutingAssembly().Location;

                    // Démarrage de la nouvelle instance
                    Process.Start(filePath);

                    // Arrêt de l'instance actuelle
                    Environment.Exit(0);
                }

                // On annule la fermeture de la fenêtre
                e.Cancel = true;
            }
        }
    }
}
