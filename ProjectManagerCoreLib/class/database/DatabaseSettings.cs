using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectManagerCoreLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectManagerCoreLib
{
    public class DatabaseSettings
    {
        // Variables générales
        private string configFile_path = "";

        // Champs de configurations
        private string database_server = "";
        private string user = "";
        private string password = "";
        private string database_name = "";

        public DatabaseSettings(string configFilePath)
        {
            this.configFile_path = configFilePath;
        }

        /// <summary>
        /// Lecture et mémorisation de la configuration enregistrées dans <c>Settings/database.json</c>
        /// </summary>
        public void ReadConfiguration()
        {
            // Lecture du fichier de paramètres dans "path"
            StreamReader reader = new StreamReader(this.configFile_path);
            JsonTextReader jsonReader = new JsonTextReader(reader);
            JObject jsonContent = (JObject)JToken.ReadFrom(jsonReader);
            jsonReader.Close();

            // Mémorisation des paramètres
            this.database_server = jsonContent["server_address"].ToString();
            this.user = jsonContent["user"].ToString();
            this.password = jsonContent["password"].ToString();
            this.database_name = jsonContent["database_name"].ToString();
        }

        /// <summary>
        /// Ecriture du fichier de configuration
        /// </summary>
        /// <param name="database_server">Adresse/serveur de la base de données</param>
        /// <param name="user">Nom d'utilisateur pour la connexion</param>
        /// <param name="password">Mot de passe pour la connexion</param>
        /// <param name="database_name">Nom de la base de données pour la connexion</param>
        public void WriteConfiguration(string database_server, string user, string password, string database_name)
        {
            if (!string.IsNullOrEmpty(database_server) && !string.IsNullOrEmpty(database_name))
            {
                // Mise en place des objets permettant d'écrire le fichier de configuration
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);

                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    writer.Formatting = Formatting.Indented;

                    // Ajout des valeurs
                    writer.WriteStartObject();
                    writer.WritePropertyName("server_address");
                    writer.WriteValue(database_server);
                    writer.WritePropertyName("user");
                    writer.WriteValue(user);
                    writer.WritePropertyName("password");
                    writer.WriteValue(password);
                    writer.WritePropertyName("database_name");
                    writer.WriteValue(database_name);
                    writer.WriteEndObject();
                }

                // Ecriture du fichier final
                File.WriteAllText(this.configFile_path, sb.ToString());
            }
            else
            {
                LogFile.writeLog("Le serveur de connexion ou le nom de la base de données n'est pas définie. Merci de vérifier vos paramètres et réessayez.");
            }
        }

        /// <summary>
        /// Obtient l'adresse/le serveur de la base de données
        /// </summary>
        /// <returns>Adresse/Serveur de la base de données</returns>
        public string GetDatabaseServer()
        {
            if (string.IsNullOrEmpty(this.database_server))
            {
                ReadConfiguration();

                if (string.IsNullOrEmpty(this.database_server))
                {
                    LogFile.writeLog("Aucun serveur de base de données n'est configuré. Vérifiez votre configuration et réessayez.");
                }
            }

            return this.database_server;
        }

        /// <summary>
        /// Obtient l'identifiant/l'utilisateur pour la connexion à la base de données
        /// </summary>
        /// <returns>Identifiant/Utilisateur pour la connexion à la base de données</returns>
        public string GetUser()
        {
            return this.user;
        }

        /// <summary>
        /// Obtient le mot de passe pour la connexion à la base de données
        /// </summary>
        /// <returns>Mot de passe pour la connexion à la base de données</returns>
        public string GetPassword()
        {
            return this.password;
        }

        /// <summary>
        /// Obtient le nom de la base de données à utiliser
        /// </summary>
        /// <returns>Nom de la base de données à utiliser</returns>
        public string GetDatabaseName()
        {
            if (string.IsNullOrEmpty(this.database_name))
            {
                ReadConfiguration();

                if (string.IsNullOrEmpty(this.database_name))
                {
                    LogFile.writeLog("Aucun nom de base de données n'est configuré. Vérifiez votre configuration et réessayez.");
                }
            }

            return this.database_name;
        }
    }
}
