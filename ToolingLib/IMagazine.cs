using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ToolingLib
{
    public interface IMagazine
    {
        #region Original Methods

        MagazineTool[] GetStatusMagazine();

        #endregion Original Methods

        #region Added Methods

        bool LoadMagazine(string FileName, out Exception e);

        bool CheckToolInMagazine(int Width, out Exception e);

        bool GetToolFromMagazine(int Width, out Exception e);

        bool StoreToolInMagazine(int Width, out Exception e);

        bool IsMagazineFull(out Exception e);

        bool CanInsertTool(int Width, out Exception e);

        MagazineTool[] GetAllTools();

        #endregion Added Methods
    }

    public class Magazine : IMagazine
    {
        private readonly log4net.ILog log;

        private MagazineTool[] magazineTools;

        private MagazineTool[] allTools;

        private readonly string fileLoader;

        public int MagazineId;

        /// <summary>
        /// Costruttore per DI
        /// </summary>
        /// <param name="log"></param>
        public Magazine(log4net.ILog log, int MagazineId, string fileLoader)
        {
            this.log = log;
            this.MagazineId = MagazineId;
            this.fileLoader = fileLoader;
        }

        /// <summary>
        /// Ottiene lo stato attuale del magazzino
        /// </summary>
        /// <returns>Lo stato attuale del magazzino sotto forma di array di MagazineTool</returns>
        public MagazineTool[] GetStatusMagazine()
        {
            return magazineTools;
        }

        /// <summary>
        /// Carica i tool nel magazzino dal file JSON specificato
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="e"></param>
        /// <returns>True se l'operazione è andata a buon fine, false altrimenti</returns>
        public bool LoadMagazine(string FileName, out Exception e)
        {
            try
            {
                string magazineLoader = File.ReadAllText(fileLoader);
                magazineTools = JsonSerializer.Deserialize<MagazineTP>(magazineLoader).MagazineTools;
                List<MagazineTool> allToolList = new List<MagazineTool>();
                foreach (var tool in magazineTools)
                {
                    allToolList.Add(new MagazineTool(tool.Count, tool.Width));
                }
                allTools = allToolList.ToArray();
            }
            catch (Exception ex)
            {
                e = ex;
                log.Error($"LoadMagazine: {e.Message}");
                return false;
            }
            e = new Exception($"LoadMagazine: Magazine #{MagazineId} Loaded Correctly");
            log.Info(e.Message);
            return true;
        }

        /// <summary>
        /// Controlla che il tool sia disponibile all'interno del magazzino
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="e"></param>
        /// <returns>True se il tool è disponibile, false altrimenti</returns>
        public bool CheckToolInMagazine(int Width, out Exception e)
        {
            foreach (var tool in magazineTools)
            {
                if (tool.Width == Width && tool.Count > 0)
                {
                    e = new Exception($"CheckToolInMagazine: Found Tool In Magazine #{MagazineId} | Width: {Width}");
                    log.Info(e.Message);
                    return true;
                }
            }
            e = new Exception($"CheckToolInMagazine: Tool Not Aviable: Width: {Width}");
            log.Error(e.Message);
            return false;
        }

        /// <summary>
        /// Rimuove il tool dal magazzino
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="e"></param>
        /// <returns>True se la procedura è andata a buon fine, false altrimenti</returns>
        public bool GetToolFromMagazine(int Width, out Exception e)
        {
            foreach (MagazineTool mg in magazineTools)
            {
                if (Width == mg.Width && mg.Count > 0)
                {
                    mg.Count--;
                    e = new Exception($"GetToolFromMagazine: Tool Found: Width: {Width}");
                    log.Info(e.Message);
                    return true;
                }
            }
            e = new Exception($"GetToolFromMagazine: Tool Doesn't Exist: Width {Width}");
            return false;
        }

        /// <summary>
        /// Inserisce il tool all'interno del magazine
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="e"></param>
        /// <returns>True se il tool viene reinserito con successo, false altrimenti</returns>
        public bool StoreToolInMagazine(int Width, out Exception e)
        {
            foreach (MagazineTool mg in magazineTools)
            {
                if (Width == mg.Width)
                {
                    mg.Count++;
                    e = new Exception($"StoreToolBackInMagazine: Tool Width : {Width} | Stored Correctly in Magazine #{MagazineId}");
                    log.Info(e.Message);
                    return true;
                }
            }
            e = new Exception($"StoreToolBackInMagazine: Tool Doesn't Exist | Width: {Width}");
            log.Error(e.Message);
            return false;
        }

        /// <summary>
        /// Ottiene la lista di tool inizialmente presenti all'interno del magazzino
        /// </summary>
        /// <returns>L'array corrispondente</returns>
        public MagazineTool[] GetAllTools()
        {
            return allTools;
        }

        /// <summary>
        /// Controlla se il magazzino è pieno
        /// </summary>
        /// <param name="e"></param>
        /// <returns>True se il magazzino non ha più spazi disponibili, false altrimenti</returns>
        public bool IsMagazineFull(out Exception e)
        {
            var result = true;
            foreach (var tool in magazineTools)
            {
                foreach (var mt in allTools)
                {
                    if (tool.Width == mt.Width && tool.Count != mt.Count)
                    {
                        result = false;
                    }
                }
            }
            if (result)
            {
                e = new Exception($"IsMagazineFull: Magazine #{MagazineId} is Full");
                log.Info(e.Message);
            }
            else
            {
                e = new Exception($"IsMagazineFull: Magazine #{MagazineId} is not Full");
                log.Info(e.Message);
            }
            return result;
        }

        /// <summary>
        /// Controlla se è possibile reinserire il tool con larghezza specificata, all'interno del magazzino
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="e"></param>
        /// <returns>True se è possibile inserire il tool, false altrimenti</returns>
        public bool CanInsertTool(int Width, out Exception e)
        {
            if (IsMagazineFull(out e))
            {
                return false;
            }
            else
            {
                foreach (var tool in magazineTools)
                {
                    foreach (var mt in allTools)
                    {
                        if (tool.Width == Width && mt.Width == Width && tool.Count < mt.Count)
                        {
                            e = new Exception($"CanInsertTool: Can Insert Tool in Magazine #{MagazineId}");
                            log.Info(e.Message);
                            return true;
                        }
                    }
                }
            }

            e = new Exception($"CanInsertTool: Can't Insert Tool in Magazine #{MagazineId}");
            log.Info(e.Message);
            return false;
        }
    }
}