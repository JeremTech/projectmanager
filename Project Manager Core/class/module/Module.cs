using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Project_Manager_Core
{
    /// <summary>
    /// Classe représentant un module
    /// </summary>
    public class Module
    {
        private string file_name;
        private string name;
        private string description;
        private bool require_login;
        private BitmapImage logo;
        private FormatConvertedBitmap logo_greyscale;

        /// <summary>
        /// Construction d'un module à partir de son chemin d'accès
        /// </summary>
        /// <param name="path"></param>
        public Module(string path)
        {
            if (File.Exists(path))
            {
                // Enregistrement du nom de fichier du module (avec l'extension)
                this.file_name = Path.GetFileName(path);

                // Lecture du fichier app.json du module
                Stream propertiesFile = Assembly.LoadFile(path).GetManifestResourceStream("Project_Manager_Module.Module.app.json");
                StreamReader reader = new StreamReader(propertiesFile);
                JsonTextReader jsonReader = new JsonTextReader(reader);
                JObject jsonContent = (JObject)JToken.ReadFrom(jsonReader);

                // Enregistrement des propriétés
                this.name = jsonContent["name"].ToString();
                this.description = jsonContent["description"].ToString();
                this.require_login = (bool) jsonContent["require_login"];

                // Fermeture du lecteur
                jsonReader.Close();
                reader.Close();

                // Lecture du logo du module
                Stream imgStream = Assembly.LoadFile(path).GetManifestResourceStream("Project_Manager_Module.Module.logo.png");
                BitmapImage logo = new BitmapImage();
                logo.BeginInit();
                logo.StreamSource = imgStream;
                logo.EndInit();
                imgStream.Close();
                this.logo = logo;

                // Création de la version en nuances de gris du logo
                FormatConvertedBitmap logoGreyScaled = new FormatConvertedBitmap();
                logoGreyScaled.BeginInit();
                logoGreyScaled.Source = this.logo;
                logoGreyScaled.DestinationFormat = PixelFormats.Gray32Float;
                logoGreyScaled.EndInit();
                this.logo_greyscale = logoGreyScaled;
            }
        }

        /// <summary>
        /// Permet d'obtenir le nom du fichier du module (avec l'extension <c>.exe</c>)
        /// </summary>
        /// <returns>Nom du fichier du module</returns>
        public string getFileName()
        {
            return this.file_name;
        }

        /// <summary>
        /// Permet d'obtenir le nom du module
        /// </summary>
        /// <returns>Nom du module</returns>
        public string getName()
        {
            return this.name;
        }

        /// <summary>
        /// Permet d'obtenir la description du module
        /// </summary>
        /// <returns></returns>
        public string getDescription()
        {
            return this.description;
        }

        /// <summary>
        /// Permet de savoir si le module nécessite que l'utilisateur soit identifié ou non
        /// </summary>
        /// <returns><code>true</code> si oui, <code>false</code> sinon</returns>
        public bool requireLogin()
        {
            return this.require_login;
        }

        /// <summary>
        /// Permet de récupérer le logo du module
        /// </summary>
        /// <returns>Logo du module</returns>
        public BitmapImage getLogo()
        {
            return this.logo;
        }

        /// <summary>
        /// Permet de récupérer le logo du module en version nuances de gris
        /// </summary>
        /// <returns>Logo du module en nuances de gris</returns>
        public FormatConvertedBitmap getGreyScaledLogo()
        {
            return this.logo_greyscale;
        }
    }
}
