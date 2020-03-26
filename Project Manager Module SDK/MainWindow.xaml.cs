using ProjectManagerCoreLib;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Project_Manager_Module_SDK
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Initialisation des fichiers temporaires
            TempData.InitializeTempDataFolder();
            TempData.ReadTempData();

            // Configuration des logs
            LogFile.updateCurrentLogFileName();

            // Lecture des paramètres de connexion à la base de données
            DatabaseSettings dbSettings = new DatabaseSettings(System.IO.Path.Combine(Utils.GetCoreExecutionFolder(), "Settings", "database.json"));
            dbSettings.ReadConfiguration();
        }
    }
}
