using System;
using System.Collections.Generic;
using MySql.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows;
using System.IO;
using System.Reflection;
using System.Data;

namespace ProjectManagerCoreLib
{
    public class RemoteDataBase
    {
        // Global variables
        private static string connectionString = "server=localhost;user=root;pwd=;";
        private static MySqlConnection dbConnection = new MySqlConnection(connectionString);
        private static DatabaseSettings dbSettings = new DatabaseSettings("");
        private static bool isConnected = false;

        /// <summary>
        /// Met en place tout les éléments nécessaires à la connexion à la base de données
        /// </summary>
        /// <param name="settings">Objet contenant les paramètres de connexion à la base de données</param>
        public static void startConnection(DatabaseSettings settings)
        {
            // Mise en place de la connexion à la base de données
            dbSettings = settings;
            dbSettings.ReadConfiguration();
            connectionString = string.Format("server={0};user={1};pwd={2};", settings.GetDatabaseServer(), settings.GetUser(), settings.GetPassword());
            dbConnection = new MySqlConnection(connectionString);

            // 0n vérifie l'existance de la base de données
            if (RemoteDataBase.DBExists(dbConnection, settings.GetDatabaseName()))
            {
                // Mise à jour de l'objet de connexion
                connectionString = string.Format("server={0};user={1};pwd={2};database={3}", settings.GetDatabaseServer(), settings.GetUser(), settings.GetPassword(), settings.GetDatabaseName());
                dbConnection = new MySqlConnection(connectionString);
            }
        }

        /// <summary>
        /// Obtient l'objet de connexion à la base de données
        /// </summary>
        /// <returns>Objet de connexion à la base de données</returns>
        public static MySqlConnection GetDatabaseConnection()
        {
            return dbConnection;
        }

        /// <summary>
        /// Vérifie la connexion à la base de données (les paramètres doivent être lu avant d'appeler cette fonction)
        /// </summary>
        /// <returns><code>true</code> si la connxeion a réussie, <code>false</code> sinon</returns>
        public static bool checkDBConnection()
        {     
            bool isConn = false;

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

            isConnected = isConn;
            return isConn;
        }

        /// <summary>
        /// Vérifie si une base de donnée existe sur le serveur
        /// </summary>
        /// <param name="dbconn">Objet de connexion à la base de données</param>
        /// <param name="dbName">Nom de la base de donnée à vérifier</param>
        /// <returns><code>True</code> si la base de donnée existe, <code>False</code> sinon</returns>
        public static bool DBExists(MySqlConnection dbconn, string dbName)
        {
            bool functionReturnValue = false;
            MySqlCommand cmd = new MySqlCommand("SELECT SCHEMA_NAME FROM information_schema.SCHEMATA WHERE SCHEMA_NAME =\'" + dbName + "\'", dbconn);

            dbconn.Open();
            MySqlDataReader reader = cmd.ExecuteReader();
            functionReturnValue = reader.HasRows;
            dbconn.Close();

            return functionReturnValue;
        }

        /// <summary>
        /// Créer et configure la base de donnée <code>dbName</code>
        /// </summary>
        /// <param name="dbconn">Objet de connexion à la base de données</param>
        /// <param name="dbName">Nom de la base de données à créer</param>
        /// <returns></returns>
        public static void SetupDB(MySqlConnection dbconn, string dbName)
        {
            // On essaie de créer la base de données
            try
            {
                // Créer la base de données vide
                MySqlCommand cmd = new MySqlCommand("CREATE DATABASE " + dbName, dbconn);
                dbconn.Open();
                cmd.ExecuteNonQuery();

                // Définie la connexion sur la nouvelle base de données créée précédemment
                dbconn.ChangeDatabase(dbName);

                // Lecture du script en ressources
                Stream propertiesFile = Assembly.GetExecutingAssembly().GetManifestResourceStream("ProjectManagerCoreLib.data.projectmanager.sql");
                StreamReader reader = new StreamReader(propertiesFile);
                string content = reader.ReadToEnd();

                // Préparation et execution du script
                MySqlScript script = new MySqlScript(dbconn, content);
                script.Error += new MySqlScriptErrorEventHandler(script_Error);
                script.ScriptCompleted += new EventHandler(script_ScriptCompleted);
                script.StatementExecuted += new MySqlStatementExecutedEventHandler(script_StatementExecuted);

                script.Execute();
                dbconn.Close();

                connectionString = string.Format("server={0};user={1};pwd={2};database={3}", dbSettings.GetDatabaseServer(), dbSettings.GetUser(), dbSettings.GetPassword(), dbSettings.GetDatabaseName());
                dbConnection = new MySqlConnection(connectionString);
            }
            // On capture les éventuelle exceptions/erreurs
            catch(Exception ex)
            {
            }
        }

        #region Retours de script
        /// <summary>
        /// Fonction appelée lors de l'éxécution du script
        /// </summary>
        /// <param name="sender">Objet à l'origine de l'appel</param>
        /// <param name="args">Arguments d'appel</param>
        static void script_StatementExecuted(object sender, MySqlScriptEventArgs args)
        {
            LogFile.writeLog("Execution du script de création de la base de données...");
        }

        /// <summary>
        /// Fonction appelée lorsque le script s'est totalement exécuté
        /// </summary>
        /// <param name="sender">Objet à l'origine de l'appel</param>
        /// <param name="args">Arguments d'appel</param>
        static void script_ScriptCompleted(object sender, EventArgs e)
        {
            /// EventArgs e will be EventArgs.Empty for this method
            LogFile.writeLog("Execution du script de création de la base de données terminée ! ");
        }

        /// <summary>
        /// Fonction appelée lorsque le script à rencontré une erreur lors de l'exécution
        /// </summary>
        /// <param name="sender">Objet à l'origine de l'appel</param>
        /// <param name="args">Arguments d'appel</param>
        static void script_Error(Object sender, MySqlScriptErrorEventArgs args)
        {
            LogFile.writeLog("L'execution du script de création de la base de données a recontré une erreur : " + args.Exception.ToString());
        }
        #endregion
    }
}
