using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ProjectManagerCoreLib
{
    /// <summary>
    /// Classe contenant des fonctionnalités utilitaires
    /// </summary>
    public class Utils
    {
        /// <summary>
        /// Permet d'obtenir le répertoire d'execution du module principal
        /// </summary>
        /// <returns>Chemin complet du répertoire d'execution du module principal (<c>""</c> si introuvable)</returns>
        public static string GetCoreExecutionFolder()
        {
            // Répertoire d'exécution actuel
            string path = Environment.CurrentDirectory;
            bool find = false;

            // Tant qu'on a pas trouvé le fichier "Project Manager Core.exe" ET que le chemin n'est pas vide
            while (!find & path != "")
            {
                // On liste les fichiers dans le répertoire "path"
                string[] filesInFolder = Directory.GetFiles(path);

                // Pour chaque fichier
                foreach(string file in filesInFolder)
                {
                    // Si le fichier est "Project Manager Core.exe" on arrête la boucle et on sort du while
                    if (Path.GetFileName(file) == "Project Manager Core.exe")
                    {
                        find = true;
                        break;
                    }
                }

                // Si le fichier n'a pas été trouvé dans le dernier foreach, on passe au dossier parent du dossier de recherche actuel
                if (!find)
                {
                    List<string> newPath = new List<string>(path.Split('\\'));
                    newPath.RemoveAt(newPath.Count - 1);
                    path = string.Join("\\", newPath);
                }
            }

            return path;
        }

        /// <summary>
        /// Hashe le texte avec l'algo SHA512
        /// </summary>
        /// <param name="text">Texte à hasher</param>
        /// <returns>Texte hashé</returns>
        public static string SHA256Hash(string text)
        {
            // Création d'un objet SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Hashage
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(text));

                // Conversion en string 
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static bool IsValidEmail(string mail)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(mail);
                return addr.Address == mail;
            }
            catch
            {
                return false;
            }
        }
    }
}
