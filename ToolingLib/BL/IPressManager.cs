using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingLib
{
    public interface IPressManager
    {
        bool AddTool(int PressId, int Width, double Position, out Exception e);

        bool AddTool(int PressId, int Width, out Exception e);

        bool RemoveTool(int PressId, int Width, double Position, out Exception e);

        bool LoadRecepie(int PressId, RecipeModel Recipe, out Exception e);

        ToolPress[] GetStatusPress(int PressID);

        bool RemoveAllTools(int PressId, out Exception e);

        bool SaveBarAsRecipe(int PressId, string format, string name, out string message);

        List<int> GetPressBars();
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

        public bool AddTool(int PressId, int Width, double Position, out Exception e)
        {
            foreach (var press in presses)
            {
                if (press.PressId == PressId)
                {
                    if (press.CheckPosition(Width, Position, out e))
                    {
                        return press.AddTool(Width, Position, out e);
                    }
                    return false;
                }
            }
            e = new Exception("IPM-AddTool W/P: Invalid Press ID");
            log.Error(e.Message);
            return false;
        }

        public bool AddTool(int PressId, int Width, out Exception e)
        {
            foreach (var press in presses)
            {
                if (press.PressId == PressId)
                {
                    if (press.CheckForFreePosition(Width, out double position, out e))
                    {
                        return press.AddTool(Width, position, out e);
                    }
                    return false;
                }
            }

            e = new Exception("IPM-AddTool W: Invalid Press ID");
            log.Error(e.Message);
            return false;
        }

        public bool RemoveTool(int PressId, int Width, double Position, out Exception e)
        {
            foreach (var press in presses)
            {
                if (press.PressId == PressId)
                {
                    if (press.CheckToolOnBar(Width, Position, out e))
                    {
                        press.RemoveTool(Width, Position, out e);
                        return true;
                    }
                    return false;
                }
            }
            e = new Exception("IPM-RemoveTool: Invalid Press ID");
            log.Error(e.Message);

            return false;
        }

        public bool RemoveAllTools(int PressId, out Exception e)
        {
            foreach (var press in presses)
            {
                if (press.PressId == PressId)
                {
                    return press.RemoveAllTools(out e);
                }
            }
            e = new Exception("IPM-RemoveAllTools: Invalid Press ID");
            return false;
        }

        public ToolPress[] GetStatusPress(int PressID)
        {
            foreach (var press in presses)
            {
                if (press.PressId == PressID)
                {
                    return press.GetStatusPress();
                }
            }
            log.Error("IPM-GetStatusPress: Returning Null");
            return null;
        }

        public bool LoadRecepie(int PressId, RecipeModel Recipe, out Exception e)
        {
            foreach (var press in presses)
            {
                if (press.PressId == PressId)
                {
                    return press.LoadRecepie(Recipe, out e);
                }
            }
            e = new Exception("IPM-LoadRecipe: Invalid Recipe ID");
            log.Error(e.Message);
            return false;
        }

        public bool SaveBarAsRecipe(int PressId, string format, string name, out string message)
        {
            foreach (var press in presses)
            {
                if (press.PressId == PressId)
                {
                    return press.SaveBarAsRecipe(format, name, out message);
                }
            }
            message = "IPM-SaveBarAsRecipe: Invalid Recipe ID";
            return false;
        }

        public List<int> GetPressBars()
        {
            List<int> pressIds = new List<int>();
            foreach (var press in presses)
            {
                pressIds.Add(press.PressId);
            }
            return pressIds;
        }
    }
}