using ProjectManagerCoreLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace Project_Manager_Core
{
    /// <summary>
    /// Logique d'interaction pour LaunchWindow.xaml
    /// </summary>
    public partial class LaunchWindow : Window
    {
        // Récupère le dossier depuis lequel est lancé l'application
        private string execution_folder = Environment.CurrentDirectory;

        public LaunchWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Fonction s'executant après le chargement de la fenêtre
        /// </summary>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();

            // Vérification des dossiers
            statut_label.Content = "Vérification des dossiers";
            await Task.Factory.StartNew(checkFolders);

            // Initialisation des fichiers
            statut_label.Content = "Initialisation des fichiers";
            await Task.Factory.StartNew(initializeFiles);

            // Initialisation de la connexion à la base de données
            statut_label.Content = "Connexion à la base de données";
            await Task.Factory.StartNew(initializeDataBase);

            // Chargement des modules
            statut_label.Content = "Chargement des modules";
            ModulesReader.LoadModules();

            // Affichage de la fenêtre principale
            MainWindow window = new MainWindow();
            window.Show();

            // Fermeture de la fenêtre de démarrage
            this.Close();
        }

        /// <summary>
        /// Fonction chargée de vérifier l'existance des dossiers d'application
        /// </summary>
        private void checkFolders()
        {
            // Création des dossiers d'application s'ils n'existent pas
            if (!Directory.Exists(Path.Combine(execution_folder, "Modules")))
                Directory.CreateDirectory(Path.Combine(execution_folder, "Modules"));

            if (!Directory.Exists(Path.Combine(execution_folder, "Settings")))
                Directory.CreateDirectory(Path.Combine(execution_folder, "Settings"));

            if (!Directory.Exists(Path.Combine(execution_folder, "Logs")))
                Directory.CreateDirectory(Path.Combine(execution_folder, "Logs"));
        }

        /// <summary>
        /// Fonction chargée de créer tous les fichiers d'application
        /// </summary>
        private void initializeFiles()
        {
            // Configuration des logs
            LogFile.updateCurrentLogFileName();

            // Initialisation des fichiers temporaires
            TempData.InitializeTempDataFolder();
            TempData.WriteTempData();

            // Créations des fichiers de configuration s'ils n'existent pas
            if (!File.Exists(Path.Combine(execution_folder, "Settings", "database.json")))
            {
                Stream propertiesFile = Assembly.GetExecutingAssembly().GetManifestResourceStream("Project_Manager_Core.data.database.json");
                StreamReader reader = new StreamReader(propertiesFile);
                string content = reader.ReadToEnd();

                File.WriteAllText(Path.Combine(execution_folder, "Settings", "database.json"), content);
            }
        }

        /// <summary>
        /// Fonction chargée d'initialisé la connexion à la base de données
        /// </summary>
        private async void initializeDataBase()
        {
            // Lecture des paramètres de connexion à la base de données
            DatabaseSettings dbSettings = new DatabaseSettings(Path.Combine(execution_folder, "Settings", "database.json"));
            dbSettings.ReadConfiguration();

            // Vérification de la connexion à la base de données
            bool isConnected = RemoteDataBase.checkDBConnection();

            if (!isConnected)
            {
                // Echec de la connexion 
                MessageBox.Show("La connexion à la base de données a échouée. Vérifiez vos informations de connexion puis réessayez." + Environment.NewLine + "Pour plus d'nformations, consultez le fichier de logs \"" + LogFile.currentLogFileName + "\".", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                // Vérification de l'existence de la base de données
                bool dbExist = RemoteDataBase.DBExists(RemoteDataBase.GetDatabaseConnection(), dbSettings.GetDatabaseName());

                if(!dbExist)
                {
                    MessageBoxResult result = MessageBox.Show("La base de données \"" + dbSettings.GetDatabaseName() + "\" n'a pas été trouvée." + Environment.NewLine + "Souhaitez-vous la créer ?", "Base de données introuvable", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if(result == MessageBoxResult.Yes)
                    {
                        await Task.Factory.StartNew(() => RemoteDataBase.SetupDB(RemoteDataBase.GetDatabaseConnection(), dbSettings.GetDatabaseName()));
                    }
                }
                else
                {
                    RemoteDataBase.startConnection(dbSettings);
                }
            }
        }
    }
}
