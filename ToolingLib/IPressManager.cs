using System;
using System.Collections.Generic;
using System.Linq;
using ToolingLib.Models;

namespace ToolingLib
{
    public interface IPressManager
    {
        bool AddTool(int PressId, int MagazineID, int Width, double Position, out Exception e);

        bool AddTool(int PressId, int MagazineID, int Width, out Exception e);

        bool RemoveTool(int PressId, int Width, double Position, out Exception e);

        bool LoadRecepie(int PressId, RecipeModel Recipe, out Exception e);

        ToolPress[] GetStatusPress(int PressID);

        //No Screenshot
        bool RemoveAllTools(int PressId, out Exception e);

        //No Screenshot
        bool SaveBarAsRecipe(int PressId, string format, string name, out string message);

        List<PressBar> GetPressBars();
    }

    public class PressManager : IPressManager
    {
        private readonly log4net.ILog log;
        private readonly IEnumerable<Press> presses;

        public PressManager(log4net.ILog log, IEnumerable<Press> presses)
        {
            this.log = log;
            this.presses = presses;
        }

        /// <summary>
        /// Aggiunge un tool alla pressa con id specificato, nella posizione specificata, prendendo il tool con larghezza specificata da un magazzino.
        /// </summary>
        /// <param name="PressId"></param>
        /// <param name="MagazineID"></param>
        /// <param name="Width"></param>
        /// <param name="Position"></param>
        /// <param name="e"></param>
        /// <returns>True se l'operazione è andata a buon fine, false altrimenti</returns>
        public bool AddTool(int PressId, int MagazineID, int Width, double Position, out Exception e)
        {
            var press = presses.Single(x => x.PressId == PressId);

            if (press != null)
            {
                if (press.CheckPosition(Width, Position, out e))
                {
                    return press.AddTool(Width, Position, MagazineID, out e);
                }
                else
                {
                    e = new Exception("IPM-AddTool W/P: Invalid Position");
                }
            }
            else
            {
                e = new Exception("IPM-AddTool W/P: Invalid Press ID");
            }

            log.Error(e.Message);
            return false;
        }

        /// <summary>
        /// Aggiunge un tool alla pressa con id specificato, nella prima posizione disponibile, prendendo il tool con larghezza specificata da un magazzino.
        /// </summary>
        /// <param name="PressId"></param>
        /// <param name="MagazineID"></param>
        /// <param name="Width"></param>
        /// <param name="e"></param>
        /// <returns>True se l'operazione è andata a buon fine, false altrimenti</returns>
        public bool AddTool(int PressId, int MagazineID, int Width, out Exception e)
        {
            var press = presses.Single(x => x.PressId == PressId);

            if (press != null)
            {
                if (press.CheckForFreePosition(Width, out double Position, out e))
                {
                    return press.AddTool(Width, Position, MagazineID, out e);
                }
                else
                {
                    e = new Exception("IPM-AddTool W: No Valid Position Found");
                }
            }
            else
            {
                e = new Exception("IPM-AddTool W: Invalid Press ID");
            }

            log.Error(e.Message);
            return false;
        }

        /// <summary>
        /// Rimuove il tool specificato dalla posizione specificata sulla pressa con id specificato e reinserisce il tool nel primo magazzino libero.
        /// </summary>
        /// <param name="PressId"></param>
        /// <param name="Width"></param>
        /// <param name="Position"></param>
        /// <param name="e"></param>
        /// <returns>True se l'operazione è andata a buon fine, false altrimenti</returns>
        public bool RemoveTool(int PressId, int Width, double Position, out Exception e)
        {
            var press = presses.Single(x => x.PressId == PressId);

            if (press != null && press.CheckToolOnBar(Width, Position, out e))
            {
                return press.RemoveTool(Width, Position, out e);
            }
            else
            {
                e = new Exception("IPM-RemoveTool: Invalid Press ID");
            }

            e = new Exception("IPM-RemoveTool: Invalid Press ID or Tool not on bar");
            log.Error(e.Message);

            return false;
        }

        /// <summary>
        /// Rimove tutti i tool presenti sulla pressa con id specificato
        /// </summary>
        /// <param name="PressId"></param>
        /// <param name="e"></param>
        /// <returns>True se l'operazione è andata a buon fine, false altrimenti</returns>
        public bool RemoveAllTools(int PressId, out Exception e)
        {
            var press = presses.Single(x => x.PressId == PressId);

            if (press != null)
            {
                return press.RemoveAllTools(out e);
            }

            e = new Exception("IPM-RemoveAllTools: Invalid Press ID");
            return false;
        }

        /// <summary>
        /// Ottiene lo stato della pressa con id specificato
        /// </summary>
        /// <param name="PressId"></param>
        /// <returns>L'array di ToolPress ottenuto</returns>
        public ToolPress[] GetStatusPress(int PressId)
        {
            var press = presses.Single(x => x.PressId == PressId);

            if (press != null)
            {
                return press.GetStatusPress();
            }

            log.Error("IPM-GetStatusPress: Returning Null");
            return null;
        }

        /// <summary>
        /// Carica la recipe con nome e tipo specificati nel RecipeModel e inserisce i tool sulla pressa con ID specificato
        /// </summary>
        /// <param name="PressId"></param>
        /// <param name="Recipe"></param>
        /// <param name="e"></param>
        /// <returns>True se l'operazione è andata a buon fine, false altrimenti</returns>
        public bool LoadRecepie(int PressId, RecipeModel Recipe, out Exception e)
        {
            var press = presses.Single(x => x.PressId == PressId);

            if (press != null)
            {
                return press.LoadRecepie(Recipe, out e);
            }

            e = new Exception("IPM-LoadRecipe: Invalid Recipe ID");
            log.Error(e.Message);
            return false;
        }

        /// <summary>
        /// Ottiene lo stato attuale della pressa con id specifiato e in base ai tool presenti definisce una nuova Recipe del formato specificato
        /// </summary>
        /// <param name="PressId"></param>
        /// <param name="format"></param>
        /// <param name="name"></param>
        /// <param name="message"></param>
        /// <returns>True se l'operazione è andata a buon fine, false altrimenti</returns>
        public bool SaveBarAsRecipe(int PressId, string format, string name, out string message)
        {
            var press = presses.Single(x => x.PressId == PressId);

            if (press != null)
            {
                return press.SaveBarAsRecipe(format, name, out message);
            }

            message = "IPM-SaveBarAsRecipe: Invalid Recipe ID";
            log.Error(message);
            return false;
        }

        /// <summary>
        /// Ottiene gli ID delle presse e li inserisce all'interno di una lista
        /// </summary>
        /// <returns>Restituisce la lista di ID</returns>
        public List<PressBar> GetPressBars()
        {
            List<PressBar> pressbars = new List<PressBar>();

            foreach (var press in presses)
            {
                pressbars.Add(new PressBar(press.PressId, press.toolBar.Length));
            }

            return pressbars;
        }
    }
}