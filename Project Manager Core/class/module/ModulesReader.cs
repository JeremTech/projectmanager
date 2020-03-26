using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Project_Manager_Core
{
    /// <summary>
    /// Classe permettant le chargement et le traitement des modules
    /// </summary>
    public class ModulesReader
    {
        // Dossier d'éxecution de l'application "core"
        private static string execution_folder = Environment.CurrentDirectory;
        // Liste des noms des fichiers de module
        private static List<string> moduleFileNameList = new List<string>();
        // Liste des module chargés (objets des modules chargés)
        public static List<Module> loadedModules = new List<Module>();
        // Liste des propriétés des modules
        private static Dictionary<string, JObject> modulesProperties = new Dictionary<string, JObject>();

        /// <summary>
        /// CHarge et traite les modules présents dans le répertoires <c>Modules</c>
        /// </summary>
        public static void LoadModules()
        {
            RefreshModuleFileNameList();

            foreach (string moduleFileName in moduleFileNameList)
            {
                loadedModules.Add(new Module(Path.Combine(execution_folder, "Modules", moduleFileName)));
            }

            //ReadAllModulesProperties();
            //WriteModulesJsonFile();
        }

        /// <summary>
        /// Obtient la liste des modules chargés par <c>LoadModules</c>
        /// </summary>
        /// <returns>Liste contenant les noms de fichiers des modules chargés</returns>
        public static List<string> GetModulesFileNameList()
        {
            return moduleFileNameList;
        }

        /// <summary>
        /// Rafraîchi la liste des noms des modules présents dans le répertoire <c>Modules</c>
        /// </summary>
        private static void RefreshModuleFileNameList()
        {
            moduleFileNameList.Clear();
            string[] files = Directory.GetFiles(Path.Combine(execution_folder, "Modules"), "*.exe");
            
            foreach(string module in files)
            {
                moduleFileNameList.Add(Path.GetFileName(module));
            }
        }

        /// <summary>
        /// Récupère les propriétés du module dont le nom du fichier <paramref name="moduleFileName"/> est passé en paramètre
        /// </summary>
        /// <param name="moduleFileName">Nom du fichier du module (avec l'extension <c>.exe</c>)</param>
        public static void ReadModuleProperties(string moduleFileName)
        {
            Stream propertiesFile = Assembly.LoadFile(Path.Combine(execution_folder, "Modules", moduleFileName)).GetManifestResourceStream("Project_Manager_Module.Module.app.json");
            StreamReader reader = new StreamReader(propertiesFile);
            JsonTextReader jsonReader = new JsonTextReader(reader);

            modulesProperties.Add(moduleFileName, (JObject)JToken.ReadFrom(jsonReader));

            jsonReader.Close();
            reader.Close();
        }

        /// <summary>
        /// Récupère les propriétés de l'ensemble des modules chargés
        /// </summary>
        private static void ReadAllModulesProperties()
        {
            modulesProperties.Clear();

            foreach(string moduleName in moduleFileNameList)
            {
                ReadModuleProperties(moduleName);
            }
        }

        /// <summary>
        /// Ecrit un fichier json dans le répertoire <c>Modules</c> contenant les propriétés de l'ensemble des modules chargés
        /// </summary>
        public static void WriteModulesJsonFile()
        {
            if(moduleFileNameList.Count != 0)
            {
                ReadAllModulesProperties();

                string json = JsonConvert.SerializeObject(modulesProperties, Formatting.Indented);
                File.WriteAllText(Path.Combine(execution_folder, "Modules", "modules.json"), json);
            }
        }
    }
}
