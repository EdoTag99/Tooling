using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace ToolingLib
{
    public interface IPress
    {
        #region Original Methods

        bool AddTool(int Width, double Position, int MagazineID, out Exception e);

        bool AddTool(int Width, int MagazineID, out Exception e);

        bool RemoveTool(int Width, double Position, out Exception e);

        bool LoadRecepie(RecipeModel Recipe, out Exception e);

        ToolPress[] GetStatusPress();

        #endregion Original Methods

        #region Added Methods

        bool RemoveAllTools(out Exception e);

        bool CheckPosition(int Width, double Position, out Exception e);

        bool CheckForFreePosition(int Width, out double Position, out Exception e);

        bool CheckToolOnBar(int Width, double Position, out Exception e);

        bool SaveBarAsRecipe(string format, string name, out string message);

        #endregion Added Methods
    }

    public class Press : IPress
    {
        private readonly log4net.ILog log;
        private readonly IEnumerable<IRecipe> iRecipe;
        private readonly IMagazineManager iMagazineManager;
        public readonly ToolPress[] toolBar;

        public int PressId;

        //private ToolingData toolingData;

        /// <summary>
        /// Costruttore per DI
        /// </summary>
        /// <param name="log"></param>
        /// <param name="iRecipe"></param>
        /// <param name="iMagazine"></param>
        public Press(int PressId, int toolBarLenght, log4net.ILog log, IEnumerable<IRecipe> iRecipe, IMagazineManager iMagazineManager)
        {
            this.log = log;
            this.toolBar = new ToolPress[toolBarLenght];
            this.iRecipe = iRecipe;
            this.iMagazineManager = iMagazineManager;
            this.PressId = PressId;
        }

        /// <summary>
        /// Carica una recipe da un file JSON o da Database. Aggiunge ogni tool menzionato nella recipe alla pressa controllando la disponibilità dei tools ed eventuali collisioni
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="e"></param>
        /// <returns>True se l'operazione è andata a buon fine, false altrimenti</returns>
        public bool LoadRecepie(RecipeModel Recipe, out Exception e)
        {
            e = null;
            if (GetStatusPress() != null)
            {
                ToolPress[] currentTools = GetStatusPress();
                foreach (ToolPress tool in currentTools)
                {
                    RemoveTool(tool.Width, tool.Position, out e);
                }
            }

            bool result = true;
            List<ToolPress> recipe = null;
            try
            {
                if (Recipe.Source == "SQLite")
                {
                    recipe = iRecipe.FirstOrDefault(t => t.GetType() == typeof(RecipeSQLite)).GetRecipe(Recipe);
                }
                else if (Recipe.Source.Contains("json"))
                {
                    recipe = iRecipe.FirstOrDefault(t => t.GetType() == typeof(RecipeJSON)).GetRecipe(Recipe);
                }

                foreach (ToolPress t in recipe)
                {
                    if (CheckPosition(t.Width, t.Position, out e))
                    {
                        result = AddTool(t.Width, t.Position, -1, out e);
                    }
                    else
                    {
                        result = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                e = ex;
                log.Error($"LoadRecipe: {e.Message}");
                return false;
            }

            if (result)
            {
                e = new Exception("LoadRecipe: Recipe Loaded Correctly");
                log.Info(e.Message);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool SaveBarAsRecipe(string format, string name, out string message)
        {
            try
            {
                ToolRecipe tr = new ToolRecipe
                {
                    BarConfiguration = GetStatusPress()
                };

                string recipe = JsonSerializer.Serialize(tr);
                if (format == "json")
                {
                    iRecipe.FirstOrDefault(t => t.GetType() == typeof(RecipeJSON)).AddNewRecipe(recipe, name, out message);
                }
                else
                {
                    if (iRecipe.FirstOrDefault(t => t.GetType() == typeof(RecipeSQL)) != null)
                    {
                        iRecipe.FirstOrDefault(t => t.GetType() == typeof(RecipeSQL)).AddNewRecipe(recipe, name, out message);
                    }
                    else
                    {
                        iRecipe.FirstOrDefault(t => t.GetType() == typeof(RecipeSQLite)).AddNewRecipe(recipe, name, out message);
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                log.Error(message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Seleziona il tool nel magazzino basandosi sulle sue dimensioni. Li inserisce sulla pressa nella posizione specificata
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Position"></param>
        /// <param name="e"></param>
        /// <returns>True se l'operazione è andata a buon fine, false altrimenti</returns>
        public bool AddTool(int Width, double Position, int MagazineID, out Exception e)
        {
            if (iMagazineManager.GetToolFromMagazines(Width, MagazineID, out e))
            {
                ToolPress t = new ToolPress(Width, Position);
                for (int i = (int)Position; i < Width + Position; i++)
                {
                    toolBar[i] = t;
                }
                e = new Exception($"AddTool(W/P): Tool Added Correctly: Width {Width}, Position: {Position}");
                log.Info(e.Message);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Rimuove il tool dalla pressa in una posizione specifica e lo reinserisce all'interno dell magazzino
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Position"></param>
        /// <param name="e"></param>
        /// <returns>True se l'operazione è andata a buon fine, false altrimenti</returns>
        public bool RemoveTool(int Width, double Position, out Exception e)
        {
            if (iMagazineManager.StoreToolInMagazines(Width, out e))
            {
                for (int i = (int)Position; i < Width + Position; i++)
                {
                    toolBar[i] = null;
                }
                e = new Exception($"RemoveTool: {e.Message}");
                return true;
            }
            e = new Exception($"RemoveTool: {e.Message}");
            log.Error(e.Message);
            return false;
        }

        /// <summary>
        /// Recupera un array di tool attualmente presenti sulla pressa
        /// </summary>
        /// <returns>Array di ToolPress presenti sulla toolBar</returns>
        public ToolPress[] GetStatusPress()
        {
            ToolPress[] tp = toolBar.Where(x => x != null).ToArray();
            return tp.Distinct().ToArray();
        }

        /// <summary>
        /// Rimuove tutti i tool dalla pressa
        /// </summary>
        /// <param name="e"></param>
        /// <returns>True se ogni rimozione è andata a buon fine, false altrimenti</returns>
        public bool RemoveAllTools(out Exception e)
        {
            ToolPress[] toolPress = GetStatusPress();
            foreach (ToolPress tp in toolPress)
            {
                if (!RemoveTool(tp.Width, tp.Position, out e))
                {
                    return false;
                }
            }
            e = new Exception("All Tools Removed Correctly!");
            return true;
        }

        /// <summary>
        /// Controlla che la posizione in cui si vuole inserire il tool sia libera
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Position"></param>
        /// <returns>True se il posto è disponibile, false altrimenti</returns>
        public bool CheckPosition(int Width, double Position, out Exception e)
        {
            if (toolBar.Length >= Width + Position)
            {
                int counter = 0;
                for (int i = (int)Position; i < Width + Position; i++)
                {
                    if (toolBar[i] == null)
                    {
                        counter++;
                    }
                }
                if (counter == Width)
                {
                    e = new Exception($"CheckPosition: Free Position: Position: {Position}, Width: {Width}");
                    log.Info(e.Message);
                    return true;
                }
                e = new Exception($"CheckPosition: Impossible to Add Tool due to Collision!");
                log.Error(e.Message);
                return false;
            }
            else
            {
                e = new Exception($"CheckPosition: Position Out of Press Bar: Position: {Position}, Width: {Width} ");
                log.Error(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Scorre la toolBar della pressa in cerca di una posizione disponibile per il tool
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="p"></param>
        /// <param name="e"></param>
        /// <returns>True e la posizione disponibile se presente, false e -1 se non è disponibile alcuna posizione</returns>
        public bool CheckForFreePosition(int Width, out double Position, out Exception e)
        {
            int counter = 0;
            for (int i = 0; i < toolBar.Length; i++)
            {
                if (toolBar[i] == null)
                {
                    counter++;
                }
                else
                {
                    counter = 0;
                }

                if (counter == Width)
                {
                    Position = i - Width + 1;
                    e = new Exception($"CheckForFreePostion: Free Position Found: Position: {Position}");
                    log.Info(e.Message);
                    return true;
                }
            }

            e = new Exception($"CheckForFreePosition: No Free Positions Found: Width {Width}");
            log.Error(e.Message);
            Position = -1;
            return false;
        }

        /// <summary>
        /// Controlla che il tool specificato sia presente sulla pressa
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Position"></param>
        /// <param name="e"></param>
        /// <returns>True se il tool è stato trovato posizione specificata, false altrimenti</returns>
        public bool CheckToolOnBar(int Width, double Position, out Exception e)
        {
            ToolPress toolPress = toolBar[(int)Position];
            for (int i = (int)Position; i < Position + Width; i++)
            {
                if (toolBar[i] != toolPress)
                {
                    e = new Exception($"CheckToolOnBar: Wrong Tool Selected: Width: {Width}, Position: {Position}");
                    log.Error(e.Message);
                    return false;
                }
            }

            e = new Exception($"CheckToolOnBar: Tool Found: Width: {Width}, Position: {Position}");
            log.Info(e.Message);
            return true;
        }

        /// <summary>
        /// Seleziona automaticamente la prima posizione libera sulla pressa e posiziona il tool prendendolo dal magazzino
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="e"></param>
        /// <returns>True se l'operazione è andata a buon fine, false altrimenti</returns>
        [Obsolete("Use AddTool(Width,Position,MagazineID) instead")]
        public bool AddTool(int Width, int MagazineID, out Exception e)
        {
            if (CheckForFreePosition(Width, out double Position, out e) && AddTool(Width, Position, MagazineID, out e))
            {
                e = new Exception($"AddTool(W): {e.Message}");
                log.Info(e.Message);
                return true;
            }
            e = new Exception($"AddTool(W): {e.Message}");
            log.Error(e.Message);
            return false;
        }
    }
}