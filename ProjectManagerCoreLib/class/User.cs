using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ProjectManagerCoreLib
{ 
    public class User
    {
        // Utilisateur actuel
        private static User currentUser;

        // Attributs de l'utilisateur
        public string ID_PROFIL;
        public string NOM;
        public string PRENOM;
        public string MAIL;
        public DateTime DATENAIS;
        public string DESCRIPTION;
        public string MDP;

        /// <summary>
        /// Créer un nouvel objet utilisateur
        /// </summary>
        /// <param name="id">Id utilisateur</param>
        /// <param name="nom">Nom</param>
        /// <param name="prenom">Prénom</param>
        /// <param name="mail">Mail</param>
        /// <param name="datenais">Date de naissance</param>
        /// <param name="desc">Description / Infos</param>
        /// <param name="mdp">Mot de passe (hashé)</param>
        public User(string id, string nom, string prenom, string mail, DateTime datenais, string desc, string mdp)
        {
            this.ID_PROFIL = id;
            this.NOM = nom;
            this.PRENOM = prenom;
            this.MAIL = mail;
            this.DATENAIS = datenais;
            this.DESCRIPTION = desc;
            this.MDP = mdp;
        }

        /// <summary>
        /// Constructeur permettant de créer un utilisateur vide
        /// </summary>
        public User()
        {

        }

        /// <summary>
        /// Tente de connecter un utilisateur et, le cas échant, créer un objet User
        /// </summary>
        /// <param name="mail">Identifiant</param>
        /// <param name="mdp">Mot de passe</param>
        public static User connectUser(string mail, string mdp)
        {
            // Hashage du mot de passe
            string mdpHash = Utils.SHA256Hash(mdp);

            // Création de la commande
            MySqlCommand command = RemoteDataBase.GetDatabaseConnection().CreateCommand();
            command.CommandText = "SELECT * FROM profil WHERE MAIL = @mail AND MDP = @mdp";
            command.Parameters.AddWithValue("@mail", mail);
            command.Parameters.AddWithValue("@mdp", mdpHash);

            // Overture de la connexion et execution de la commande SQL
            RemoteDataBase.GetDatabaseConnection().Open();
            MySqlDataReader rdr = command.ExecuteReader();

            // On vérifie si l'authentification a réussi
            if (rdr.Read())
            {
                if ((string) rdr["NOM"] != null && (string) rdr["PRENOM"] != null)
                {
                    User user = new User(rdr["ID_PROFIL"].ToString(), (string) rdr["NOM"], (string) rdr["PRENOM"],
                        (string) rdr["MAIL"], DateTime.Parse(rdr["DATENAIS"].ToString()), (string) rdr["DESCRIPTIION"],
                        (string) rdr["MDP"]);

                    TempData.writeUserTempFile(user);
                    return user;
                }
            }
            RemoteDataBase.GetDatabaseConnection().Close();

            return new User();
        }

        /// <summary>
        /// Fonction permettant d'ajouter un nouvel utilisateur à la base de données
        /// </summary>
        /// <param name="nom">Nom</param>
        /// <param name="prenom">Prénom</param>
        /// <param name="mail">Mail</param>
        /// <param name="datenais">Date de naissance</param>
        /// <param name="desc">Description / Infos</param>
        /// <param name="mdp">Mot de passe (hashé)</param>
        /// <returns><code>true</code> si l'ajout a réussi, <code>false</code> sinon</returns>
        public static bool createUser(string nom, string prenom, string mail, DateTime datenais, string desc, string mdp)
        {
            if (!string.IsNullOrWhiteSpace(nom) && !string.IsNullOrWhiteSpace(prenom) && !string.IsNullOrWhiteSpace(mail) && !string.IsNullOrWhiteSpace(desc) && !string.IsNullOrWhiteSpace(mdp))
            {
                // Création de la commande
                MySqlCommand command = RemoteDataBase.GetDatabaseConnection().CreateCommand();
                command.CommandText = "INSERT INTO profil (ID_PROFIL, NOM, PRENOM, MAIL, DATENAIS, DESCRIPTIION, MDP) VALUES (@id, @nom, @prenom, @mail, @datenais, @desc, @mdp)";
                command.Parameters.AddWithValue("@id", getMaxIndex() + 1);
                command.Parameters.AddWithValue("@nom", nom);
                command.Parameters.AddWithValue("@prenom", prenom);
                command.Parameters.AddWithValue("@mail", mail);
                command.Parameters.AddWithValue("@datenais", datenais.ToString("s"));
                command.Parameters.AddWithValue("@desc", desc);
                command.Parameters.AddWithValue("@mdp", mdp);

                // Overture de la connexion et execution de la commande SQL
                RemoteDataBase.GetDatabaseConnection().Open();
                int lineAffected = command.ExecuteNonQuery();

                RemoteDataBase.GetDatabaseConnection().Close();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Fonction permettant de mettre à jour les informations de l'utilisateur sur la base données et dans l'objet courant
        /// </summary>
        /// <param name="nom">Nom</param>
        /// <param name="prenom">Prénom</param>
        /// <param name="mail">Mail</param>
        /// <param name="datenais">Date de naissance</param>
        /// <param name="desc">Description / Infos</param>
        /// <param name="mdp">Mot de passe (hashé en SHA2 256)</param>
        /// <returns><code>true</code> si la modification a réussi, <code>false</code> sinon</returns>
        public bool updateUserInformations(string nom, string prenom, string mail, DateTime datenais, string desc, string mdp)
        {
            if(!string.IsNullOrWhiteSpace(nom) && !string.IsNullOrWhiteSpace(prenom) && !string.IsNullOrWhiteSpace(mail) && !string.IsNullOrWhiteSpace(desc) && !string.IsNullOrWhiteSpace(mdp))
            {
                // Création de la commande
                MySqlCommand command = RemoteDataBase.GetDatabaseConnection().CreateCommand();
                command.CommandText = "UPDATE profil SET NOM = @nom, PRENOM = @prenom, MAIL = @mail, DATENAIS = @datenais, DESCRIPTIION = @desc, MDP = @mdp WHERE ID_PROFIL = @id";
                command.Parameters.AddWithValue("@id", this.ID_PROFIL);
                command.Parameters.AddWithValue("@nom", nom);
                command.Parameters.AddWithValue("@prenom", prenom);
                command.Parameters.AddWithValue("@mail", mail);
                command.Parameters.AddWithValue("@datenais", datenais.ToString("s"));
                command.Parameters.AddWithValue("@desc", desc);
                command.Parameters.AddWithValue("@mdp", mdp);

                // Overture de la connexion et execution de la commande SQL
                RemoteDataBase.GetDatabaseConnection().Open();
                int lineAffected = command.ExecuteNonQuery();

                RemoteDataBase.GetDatabaseConnection().Close();

                // Vérification du bon fonctionnement de la modification
                if (lineAffected == 1)
                {
                    User us = connectUser(mail, mdp);
                    if(us.ID_PROFIL != null)
                    {
                        this.NOM = nom;
                        this.PRENOM = prenom;
                        this.MAIL = mail;
                        this.DATENAIS = datenais;
                        this.DESCRIPTION = desc;
                        this.MDP = mdp;

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Permet de déterminer si le mail est déjà utilisé
        /// </summary>
        /// <param name="mail">Mail à vérifier</param>
        /// <returns><code>true</code> si le mail est déjà utilisé, <code>false</code> sinon</returns>
        public static bool isMailUsed(string mail)
        {
            if(!string.IsNullOrWhiteSpace(mail))
            {
                // Création de la commande
                MySqlCommand command = RemoteDataBase.GetDatabaseConnection().CreateCommand();
                command.CommandText = "SELECT COUNT(*) FROM profil WHERE MAIL = @mail";
                command.Parameters.AddWithValue("@mail", mail);
                command.CommandType = CommandType.Text;

                // Overture de la connexion et execution de la commande SQL
                RemoteDataBase.GetDatabaseConnection().Open();
                string lineAffected = command.ExecuteScalar().ToString();

                RemoteDataBase.GetDatabaseConnection().Close();


                if (int.Parse(lineAffected) >= 1)
                {
                    return true;
                }

            }

            return false;
        }

        /// <summary>
        /// Permet d'obtenir l'ID le plus élevé déjà utilisé
        /// </summary>
        /// <returns></returns>
        public static int getMaxIndex()
        {

            // Création de la commande
            MySqlCommand command = RemoteDataBase.GetDatabaseConnection().CreateCommand();
            command.CommandText = "SELECT MAX(ID_PROFIL) as nb FROM profil";
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

        /// <summary>
        /// Retourne le mail/l'identifiant de l'utilisateur
        /// </summary>
        /// <returns>Mail/identifiant de l'utilisateur</returns>
        public string getMail()
        {
            return this.MAIL;
        }

        /// <summary>
        /// Retourne le nom de l'utilisateur
        /// </summary>
        /// <returns>Nom de l'utilisateur</returns>
        public string getNom()
        {
            return this.NOM;
        }

        /// <summary>
        /// Retourne le prénom de l'utilisateur
        /// </summary>
        /// <returns>Prénom de l'utilisateur</returns>
        public string getPrenom()
        {
            return this.PRENOM;
        }

        /// <summary>
        /// Retourne la date de naissance de l'utilisateur
        /// </summary>
        /// <returns>Date de naissance de l'utilisateur</returns>
        public DateTime getDateNais()
        {
            return this.DATENAIS;
        }
    }
}
