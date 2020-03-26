using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectManagerCoreLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Project_Manager_Core
{
    public partial class MainWindow : Window
    {
        // Récupère le dossier depuis lequel est lancé l'application
        private string execution_folder = Environment.CurrentDirectory;

        // Variable permettant de mettre a jour des contrôles depuis des fonctions static
        public static MainWindow window = null;

        public MainWindow()
        {
            InitializeComponent();
            window = this;

            // Initialisation de la listeview qui contient les modules
            refreshAppList();
        }

        /// <summary>
        /// Fonction permettant de mettre à jour l'affichage de la liste des modules
        /// </summary>
        public static void refreshAppList()
        {
            // On vide la liste actuelle
            window.appList.Items.Clear();

            foreach (Module module in ModulesReader.loadedModules)
            {
                Image img = new Image();
                img.Height = 100;
                img.Width = 100;
                img.Tag = module;

                if ((module.requireLogin() && App.currentUser != null) || !module.requireLogin())
                    img.Source = module.getLogo();
                else
                    img.Source = module.getGreyScaledLogo();

                window.appList.Items.Add(img);
            }
        }

        /// <summary>
        /// Fonction déclenchée lors du changement de la selection dans la liste des modules
        /// </summary>
        /// <param name="sender">Listview qui déclenche l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void appList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Image image = (((ListView)sender).SelectedItem as Image);
            Module imgTag;

            if (image != null)
            {
                imgTag = (Module) image.Tag;

                if ((imgTag.requireLogin() && App.currentUser != null) || !imgTag.requireLogin())
                {
                    appName_Label.Visibility = Visibility.Visible;
                    appDesc_Label.Visibility = Visibility.Visible;
                    appName.Content = imgTag.getName();
                    appDesc.Content = imgTag.getDescription();
                }
                else
                {
                    ((ListView) sender).SelectedItem = null;
                }
            }
            else
            {
                appName_Label.Visibility = Visibility.Hidden;
                appDesc_Label.Visibility = Visibility.Hidden;
                appName.Content = "";
                appDesc.Content = "";
            }
        }

        /// <summary>
        /// Fonction déclenchée lors du double clique sur la liste des modules (afin de lancer le module sélectionné)
        /// </summary>
        /// <param name="sender">Listview qui déclenche l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void appList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Image image = (((ListView)sender).SelectedItem as Image);

            if (image != null)
            {
                Module imgTag = (Module) image.Tag;

                if(File.Exists(Path.Combine(execution_folder, "Modules", imgTag.getFileName())))
                {
                    Process.Start(Path.Combine(execution_folder, "Modules", imgTag.getFileName()));
                }
                else
                {
                    MessageBox.Show("Le module \"" + imgTag.getName() + "\" n'a pas été trouvé. Merci de vérifier le répertoire \"Modules\" puis ré-essayez.");
                }
            }
        }

        /// <summary>
        /// Fonction appelée lors du clique sur le bouton de connexion
        /// </summary>
        /// <param name="sender">Bouton qui déclenche l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void but_login_Click(object sender, RoutedEventArgs e)
        {
            ConnectionWindow window = new ConnectionWindow();
            window.ShowDialog();
        }

        /// <summary>
        /// Fonction appelée lors du clique sur le bouton de déconnexion
        /// </summary>
        /// <param name="sender">Bouton qui déclenche l'évènement</param>
        /// <param name="e">Arguments de l'évènement</param>
        private void But_logout_OnClick(object sender, RoutedEventArgs e)
        {
            // Suppression de l'utilisateur en mémoire
            App.currentUser = null;

            // Suppression du fichier temporaire de connexion
            if(File.Exists(Path.Combine(Utils.GetCoreExecutionFolder(), "Temp", "User.json")))
                File.Delete(Path.Combine(Utils.GetCoreExecutionFolder(), "Temp", "User.json"));

            // Actualisation de liste des application pour prendre en compte la déconnexion
            refreshAppList();

            // Mise à jour du titre dans la barre supérieure
            user_name_label.Content = "Bienvenue";

            // Affichage du bouton de connexion au lieu de celui de déconnexion
            MainWindow.window.but_login.Visibility = Visibility.Visible;
            MainWindow.window.but_logout.Visibility = Visibility.Hidden;
        }

        private void inscription_button_Click(object sender, RoutedEventArgs e)
        {
            Inscription window = new Inscription();
            window.ShowDialog();
        }

        private void settings_button_Click(object sender, RoutedEventArgs e)
        {
            Settings window = new Settings();
            window.ShowDialog();
        }
    }
}
