using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ProjectManagerCoreLib;

namespace Project_Manager_Core
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Variable globale ayant en mémoire l'utilisateur courant
        /// </summary>
        public static User currentUser = null;
    }
}
