using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ProjectManagerCoreLib
{
    public class Project
    {
        // Attributs du projet
        public string ID_PROJET;
        public string NOM;
        public string DESCRIPTION;
        public DateTime DATECREA;
        public string ETAT;
        public DateTime DEADLINE;
        public string POURCENTAGE;

        /// <summary>
        /// Fonction permettant de créer un objet projet vide
        /// </summary>
        public Project()
        {

        }

        /// <summary>
        /// Créer un objet "projet" (ne l'enregistre pas dans la base de données)
        /// </summary>
        /// <param name="id">ID du projet</param>
        /// <param name="nom">Nom du projet</param>
        /// <param name="description">Description du projet</param>
        /// <param name="date_creation">Date de création du projet</param>
        /// <param name="etat">Etat su projet (facultatif)</param>
        /// <param name="deadline">Date limite du projet (facultatif)</param>
        /// <param name="pourcentage">Pourcentage d'avancement du projet (facultatif)</param>
        public Project(string id, string nom, string description, DateTime date_creation, string etat = null, DateTime deadline = new DateTime(), string pourcentage = null)
        {
            this.ID_PROJET = id;
            this.NOM = nom;
            this.DESCRIPTION = description;
            this.DATECREA = date_creation;
            this.ETAT = etat;
            this.DEADLINE = deadline;
            this.POURCENTAGE = pourcentage;
        }

        /// <summary>
        /// Permet d'enregistrer un nouveau projet sur la base de données
        /// </summary>
        /// <param name="nom">Nom du projet</param>
        /// <param name="description">Description du projet</param>
        /// <param name="date_creation">Date de création du projet</param>
        /// <param name="etat">Etat su projet (facultatif)</param>
        /// <param name="deadline">Date limite du projet (facultatif)</param>
        /// <param name="pourcentage">Pourcentage d'avancement du projet (facultatif)</param>
        /// <returns><code>true</code> si l'ajout a réussi, <code>false</code> sinon</returns>
        public static bool createProject(string nom, string description, DateTime date_creation, string etat = null, DateTime deadline = new DateTime(), string pourcentage = null)
        {
            if (!string.IsNullOrEmpty(nom) && !string.IsNullOrEmpty(description))
            {
                // Création de la commande
                MySqlCommand command = RemoteDataBase.GetDatabaseConnection().CreateCommand();
                command.CommandText = "INSERT INTO projet (ID_PROJET, NOM, DESCRIPTION, DATECREA, ETAT, DEADLINE, POURCENTAGE) VALUES (@id, @nom, @desc, @datecrea, @etat, @deadline, @pourcentage)";
                command.Parameters.AddWithValue("@id", getMaxIndex());
                command.Parameters.AddWithValue("@nom", nom);
                command.Parameters.AddWithValue("@desc", description);
                command.Parameters.AddWithValue("@datecrea", date_creation.ToString("s"));
                command.Parameters.AddWithValue("@etat", etat);
                command.Parameters.AddWithValue("@deadline", deadline.ToString("s"));
                command.Parameters.AddWithValue("@pourcentage", pourcentage);

                // Overture de la connexion et execution de la commande SQL
                RemoteDataBase.GetDatabaseConnection().Open();
                int lineAffected = command.ExecuteNonQuery();

                RemoteDataBase.GetDatabaseConnection().Close();

                // Vérification du bon fonctionnement de la modification
                if (lineAffected == 1)
                {
                    return true;
                }

            }

            return false;
        }

        /// <summary>
        /// Permet de mettre à jour les informations du projet sur la base de données et dans l'objet courant
        /// </summary>
        /// <param name="nom">Nom du projet</param>
        /// <param name="description">Description du projet</param>
        /// <param name="date_creation">Date de création du projet</param>
        /// <param name="etat">Etat su projet (facultatif)</param>
        /// <param name="deadline">Date limite du projet (facultatif)</param>
        /// <param name="pourcentage">Pourcentage d'avancement du projet (facultatif)</param>
        /// <returns><code>true</code> si la mise à jour a réussi, <code>false</code> sinon</returns>
        public bool updateProjectInformations(string nom, string description, DateTime date_creation, string etat = null, DateTime deadline = new DateTime(), string pourcentage = null)
        {
            if (!string.IsNullOrEmpty(nom) && !string.IsNullOrEmpty(description))
            {
                // Création de la commande
                MySqlCommand command = RemoteDataBase.GetDatabaseConnection().CreateCommand();
                command.CommandText = "UPDATE projet SET NOM = @nom, DESCRIPTION = @desc, DATECREA = @datecrea, ETAT = @etat, DEADLINE = @deadline, POURCENTAGE = @pourcentage WHERE ID_PROJET = @id";
                command.Parameters.AddWithValue("@id", this.ID_PROJET);
                command.Parameters.AddWithValue("@nom", nom);
                command.Parameters.AddWithValue("@desc", description);
                command.Parameters.AddWithValue("@datecrea", date_creation.ToString("s"));
                command.Parameters.AddWithValue("@etat", etat);
                command.Parameters.AddWithValue("@deadline", deadline.ToString("s"));
                command.Parameters.AddWithValue("@pourcentage", pourcentage);

                // Ouverture de la connexion et execution de la commande SQL
                RemoteDataBase.GetDatabaseConnection().Open();
                int lineAffected = command.ExecuteNonQuery();

                RemoteDataBase.GetDatabaseConnection().Close();

                // Vérification du bon fonctionnement de la modification
                if (lineAffected == 1)
                {
                    this.NOM = nom;
                    this.DESCRIPTION = description;
                    this.DATECREA = date_creation;
                    this.ETAT = etat;
                    this.DEADLINE = deadline;
                    this.POURCENTAGE = pourcentage;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Permet d'obtenir un objet "Projet" depuis son ID
        /// </summary>
        /// <param name="id">ID du projet à récupérer</param>
        /// <returns>Retourne un objet Project si la récupération a fonctionné, null sinon</returns>
        public static Project getProjectFromID(string id)
        {
            // Création de la commande
            MySqlCommand command = RemoteDataBase.GetDatabaseConnection().CreateCommand();
            command.CommandText = "SELECT * FROM projet WHERE ID_PROJET = @id";
            command.Parameters.AddWithValue("@id", id);

            // Overture de la connexion et execution de la commande SQL
            RemoteDataBase.GetDatabaseConnection().Open();
            MySqlDataReader rdr = command.ExecuteReader();

            // On vérifie qu'un projet a été retourné
            if (rdr.Read())
            {
                if ((string)rdr["NOM"] != null)
                {
                    Project project = new Project((string) rdr["ID"], (string)rdr["NOM"], (string)rdr["DESCRIPTION"], DateTime.Parse(rdr["DATECREA"].ToString()), rdr["ETAT"].ToString(), DateTime.Parse(rdr["DEADLINE"].ToString()), rdr["POURCENTAGE"].ToString());

                    return project;
                }
            }
            RemoteDataBase.GetDatabaseConnection().Close();

            return null;
        }

        /// <summary>
        /// Permet d'obtenir l'ID le plus élevé déjà utilisé
        /// </summary>
        /// <returns></returns>
        public static int getMaxIndex()
        {
            // Création de la commande
            MySqlCommand command = RemoteDataBase.GetDatabaseConnection().CreateCommand();
            command.CommandText = "SELECT MAX(ID_PROJET) as nb FROM projet";
            command.CommandType = CommandType.Text;

            // Overture de la connexion et execution de la commande SQL
            RemoteDataBase.GetDatabaseConnection().Open();
            string id = command.ExecuteScalar().ToString();

            // On vérifie si l'authentification a réussi
            if (!string.IsNullOrEmpty(id))
            {
                RemoteDataBase.GetDatabaseConnection().Close();
                return int.Parse(id);
            }

            RemoteDataBase.GetDatabaseConnection().Close();
            return 0;
        }
    }
}
