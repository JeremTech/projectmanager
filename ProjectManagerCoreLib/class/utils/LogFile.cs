using ProjectManagerCoreLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagerCoreLib
{
    /// <summary>
    /// CLasse permettant de gérer les fichiers logs
    /// </summary>
    public class LogFile
    {
        // Nom du fichier log actuel
        public static string currentLogFileName = "pm_log";

        /// <summary>
        /// Met à jour le nom du fichier log à utiliser
        /// </summary>
        public static void updateCurrentLogFileName()
        {
            if (string.IsNullOrEmpty(TempData.currentLogFileName))
            {

                int log_number = 0;
                string[] files = Directory.GetFiles(Path.Combine(Utils.GetCoreExecutionFolder(), "Logs"));

                foreach (string file_path in files)
                {
                    int log_nb = int.Parse(Path.GetFileNameWithoutExtension(file_path).Split('_').Last());
                    if (log_nb > log_number) { log_number = log_nb; }
                }

                currentLogFileName = "pm_log_" + (log_number + 1);
                FileStream file = File.Create(Path.Combine(Utils.GetCoreExecutionFolder(), "Logs", currentLogFileName + ".txt"));
                file.Close();

                TempData.currentLogFileName = currentLogFileName;
            }
            else
            {
                currentLogFileName = TempData.currentLogFileName;
            }
        }

        /// <summary>
        /// Ecrit dans le fichier log le texte <code>text</code>
        /// </summary>
        /// <param name="text">Texte à écrire</param>
        public static void writeLog(string text)
        {
            File.AppendAllText(Path.Combine(Utils.GetCoreExecutionFolder(), "Logs", currentLogFileName + ".txt"), "[" + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "] " + text + Environment.NewLine);
        }
    }
}
