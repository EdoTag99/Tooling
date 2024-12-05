using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

namespace ToolingLib
{
    public interface IMagazineManager
    {
        bool LoadMagazines(string FileName, out Exception e);

        bool GetToolFromMagazines(int Width, int MagazineID, out Exception e);

        bool StoreToolInMagazines(int Width, out Exception e);

        MagazineTool[] GetStatusMagazine(int MagazineID);

        MagazineTool[] GetStatusMagazines();

        int[] GetMagazines();

        MagazineTool[] GetAllTools();
    }

    public class MagazineManager : IMagazineManager
    {
        private readonly log4net.ILog log;
        private readonly IEnumerable<Magazine> magazines;

        public MagazineManager(log4net.ILog log, IEnumerable<Magazine> magazines)
        {
            this.log = log;
            this.magazines = magazines;
        }

        /// <summary>
        /// Itera sui magazzini riempendoli attraverso il file JSON specificato
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool LoadMagazines(string FileName, out Exception e)
        {
            foreach (var magazine in magazines)
            {
                if (!magazine.LoadMagazine(FileName, out Exception ex))
                {
                    e = ex;
                    return false;
                }
            }

            e = new Exception("IMM-LoadMagazines: Magazines Loaded Correctly");
            log.Info(e.Message);
            return true;
        }

        /// <summary>
        /// Rimuove il tool specificato da un magazzino, sia specificato che non specificato
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="MagazineID"></param>
        /// <param name="e"></param>
        /// <returns>True se il tool è stato rimosso dal magazzino, false altrimenti</returns>
        public bool GetToolFromMagazines(int Width, int MagazineID, out Exception e)
        {
            e = null;

            if (MagazineID >= 0)
            {
                var magazine = magazines.Single(x => x.MagazineId == MagazineID);
                if (magazine.CheckToolInMagazine(Width, out e))
                {
                    return magazine.GetToolFromMagazine(Width, out e);
                }
            }
            else
            {
                foreach (var magazine in magazines)
                {
                    if (magazine.CheckToolInMagazine(Width, out e))
                    {
                        return magazine.GetToolFromMagazine(Width, out e);
                    }
                }
            }

            log.Info(e.Message);
            return false;
        }

        /// <summary>
        /// Inserisce il tool specificato nel primo magazzino con spazio disponibile
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="e"></param>
        /// <returns>True se è stato possibile inserire il tool in un magazzino, false altrimenti</returns>
        public bool StoreToolInMagazines(int Width, out Exception e)
        {
            foreach (var magazine in magazines)
            {
                if (magazine.CanInsertTool(Width, out e))
                {
                    return magazine.StoreToolInMagazine(Width, out e);
                }
            }
            e = new Exception("IMM-StoreToolInMagazines: Impossible to store tool in magazines");
            return false;
        }

        /// <summary>
        /// Ottiene lo stato attuale del magazzino con id specificato
        /// </summary>
        /// <param name="MagazineID"></param>
        /// <returns>L'array di MagazineTool corrispondente</returns>
        public MagazineTool[] GetStatusMagazine(int MagazineID)
        {
            try
            {
                return magazines.Single(magazine => magazine.MagazineId == MagazineID).GetStatusMagazine();
            }
            catch (Exception e)
            {
                log.Error($"IMM-GetStatusMagazine: ID Doesn't Exist, {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Ottiene lo stato di tutti i magazzini sommando il numero di tool in base alla loro larghezza
        /// </summary>
        /// <returns>L'array di magazine tool rapprensentate tutti i magazzini</returns>
        public MagazineTool[] GetStatusMagazines()
        {
            List<MagazineTool> combinedMT = new List<MagazineTool>();
            foreach (var magazine in magazines)
            {
                MagazineTool[] magazineTools = magazine.GetStatusMagazine();
                if (magazineTools != null)
                {
                    foreach (var magazineTool in magazineTools)
                    {
                        if (combinedMT != null && magazineTools != null && combinedMT.Count() < magazineTools.Count())
                        {
                            combinedMT.Add(new MagazineTool(0, magazineTool.Width));
                        }

                        combinedMT.Single(tool => tool.Width == magazineTool.Width).Count += magazineTool.Count;
                    }
                }
                else
                {
                    return null;
                }
            }

            return combinedMT.ToArray();
        }

        /// <summary>
        /// Ottiene la lista di ID dei magazzini e la inserisce in una lista
        /// </summary>
        /// <returns>La lista di id dei magazzini</returns>
        public int[] GetMagazines()
        {
            List<int> magazineIds = new List<int>();
            foreach (var magazine in magazines)
            {
                magazineIds.Add(magazine.MagazineId);
            }
            return magazineIds.ToArray();
        }

        /// <summary>
        /// Ottiene lo stato iniziale dei magazzini e somma il numero di tool in base alla loro larghezza
        /// </summary>
        /// <returns>L'array contenenete il numero di tool complessivi</returns>
        public MagazineTool[] GetAllTools()
        {
            List<MagazineTool> combinedAT = new List<MagazineTool>();
            foreach (var magazine in magazines)
            {
                MagazineTool[] magazineTools = magazine.GetAllTools();
                if (magazineTools != null)
                {
                    foreach (var magazineTool in magazineTools)
                    {
                        if (combinedAT != null && magazineTools != null && combinedAT.Count() < magazineTools.Count())
                        {
                            combinedAT.Add(new MagazineTool(0, magazineTool.Width));
                        }

                        combinedAT.Single(tool => tool.Width == magazineTool.Width).Count += magazineTool.Count;
                    }
                }
                else
                {
                    return null;
                }
            }

            return combinedAT.ToArray();
        }
    }
}