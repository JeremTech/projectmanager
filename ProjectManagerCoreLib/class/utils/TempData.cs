using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerCoreLib
{
    /// <summary>
    /// Classe permettant la gestion de données temporaires inter-module
    /// </summary>
    public class TempData
    {
        // Variables générales
        private static string filePath = "";

        // Variables de données générales
        public static string currentLogFileName { get; set; }

        /// <summary>
        /// Initialisation du fichier de données temporaires
        /// </summary>
        public static void InitializeTempDataFolder()
        {
            filePath = Path.Combine(Utils.GetCoreExecutionFolder(), "Temp", "TempData.json");

            if (Environment.CurrentDirectory == Utils.GetCoreExecutionFolder())
            {
                if (!Directory.Exists(Path.Combine(Utils.GetCoreExecutionFolder(), "Temp")))
                    Directory.CreateDirectory(Path.Combine(Utils.GetCoreExecutionFolder(), "Temp"));
            }
        }

        /// <summary>
        /// Fonction permettant de lire le fichier de données temporaires
        /// </summary>
        public static void ReadTempData()
        {
            // Lecture du fichier dans "filePath"
            StreamReader reader = new StreamReader(filePath);
            JsonTextReader jsonReader = new JsonTextReader(reader);
            JObject jsonContent = (JObject)JToken.ReadFrom(jsonReader);

            // Mémorisation des données
            currentLogFileName = jsonContent["currentLogFileName"].ToString();
        }

        /// <summary>
        /// Fonction permettant d'écrire le fichier de données temporaires
        /// </summary>
        public static void WriteTempData()
        {
            // Mise en place des objets permettant d'écrire le fichier
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;

                // Ajout des valeurs
                writer.WriteStartObject();
                writer.WritePropertyName("currentLogFileName");
                writer.WriteValue(currentLogFileName);
                writer.WriteEndObject();
            }

            // Ecriture du fichier final
            File.WriteAllText(filePath, sb.ToString());
        }

        /// <summary>
        /// Fonction créant un utilisateur depuis le fichier temporaire dédié
        /// </summary>
        /// <returns>Utilisateur stocké dans le fichier temporaire</returns>
        public static User readUserTempFile()
        {
            string userTempFile = Path.Combine(Utils.GetCoreExecutionFolder(), "Temp", "User.json");

            // Lecture du fichier dans "filePath"
            StreamReader reader = new StreamReader(userTempFile);
            JsonTextReader jsonReader = new JsonTextReader(reader);
            JObject jsonContent = (JObject)JToken.ReadFrom(jsonReader);

            return new User(jsonContent["ID_PROFIL"].ToString(), jsonContent["NOM"].ToString(), jsonContent["PRENOM"].ToString(), jsonContent["MAIL"].ToString(), DateTime.Parse(jsonContent["DATENAIS"].ToString()), jsonContent["DESCRIPTION"].ToString(), jsonContent["MDP"].ToString());
        }

        /// <summary>
        /// Fonction permettant d'écrire un fichier temporaire contenant les informations de l'utilisateurs
        /// </summary>
        public static void writeUserTempFile(User user)
        {
            string file_path = Path.Combine(Utils.GetCoreExecutionFolder(), "Temp", "User.json");

            // Mise en place des objets permettant d'écrire le fichier de configuration
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);

            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;

                // Ajout des valeurs
                writer.WriteStartObject();
                writer.WritePropertyName("ID_PROFIL");
                writer.WriteValue(user.ID_PROFIL);
                writer.WritePropertyName("NOM");
                writer.WriteValue(user.NOM);
                writer.WritePropertyName("PRENOM");
                writer.WriteValue(user.PRENOM);
                writer.WritePropertyName("MAIL");
                writer.WriteValue(user.MAIL);
                writer.WritePropertyName("DATENAIS");
                writer.WriteValue(user.DATENAIS.ToString("dd-MM-yyyy"));
                writer.WritePropertyName("DESCRIPTION");
                writer.WriteValue(user.DESCRIPTION);
                writer.WritePropertyName("MDP");
                writer.WriteValue(user.MDP);
                writer.WriteEndObject();
            }

            // Ecriture du fichier final
            File.WriteAllText(file_path, sb.ToString());
        }
    }
}
