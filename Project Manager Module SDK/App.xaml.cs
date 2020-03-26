using ProjectManagerCoreLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace Project_Manager_Module_SDK
{
    /// <summary>
    /// Logique d'interaction pour App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Code permettant l'ajout de dépendances externes
            var domain = AppDomain.CurrentDomain;
            domain.AssemblyResolve += LoadAssembly;
        }

        /// 
        /// Méthode permettant de charger les bibliothèque dans le répertoire parent et non le répertoire courant
        /// Toutefois, si le répertoire parent ne possède pas les bibliothèques, le module cherchera dans le répertoire courant
        /// 
        private Assembly LoadAssembly(object sender, ResolveEventArgs args)
        {
            Assembly result = null;
            if (args != null && !string.IsNullOrEmpty(args.Name))
            {
                // Obtient le dossier d'execution du module
                FileInfo info = new FileInfo(Assembly.GetExecutingAssembly().Location);

                // Obtient le dossier parent
                var folderPath = Directory.GetParent(info.Directory.FullName).ToString();

                // Construit les potentiels chemin d'accès aux dépendences
                var assemblyName = args.Name.Split(new string[] { "," }, StringSplitOptions.None)[0];
                var assemblyExtension = "dll";
                var assemblyPath = Path.Combine(folderPath, string.Format("{0}.{1}", assemblyName, assemblyExtension));

                // Si la dépendence est présente dans le répertoire parent
                if (File.Exists(assemblyPath))
                {
                    // On charge la dépendance du répertoire parent
                    result = Assembly.LoadFrom(assemblyPath);
                }
                else
                {
                    // On laisse le module chercher dans le répertoire courant
                    return args.RequestingAssembly;
                }
            }

            return result;
        }
    }
}
