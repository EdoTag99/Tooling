using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolingLib
{
    public interface IMagazineManager
    {
        bool LoadMagazines(string FileName, out Exception e);

        //bool CheckToolInMagazine(int Width, int MagazineID, out Exception e);

        bool GetToolFromMagazine(int Width, out Exception e);

        bool StoreToolInMagazine(int Width, out Exception e);

        MagazineTool[] GetStatusMagazine(int MagazineID);

        MagazineTool[] GetStatusMagazines();
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

            e = new Exception("LoadMagazines: Magazines Loaded Correctly");
            log.Info(e.Message);
            return true;
        }

        public bool GetToolFromMagazine(int Width, out Exception e)
        {
            foreach (var magazine in magazines)
            {
                if (magazine.CheckToolInMagazine(Width, out e))
                {
                    return magazine.GetToolFromMagazine(Width, out e);
                }
            }
            e = new Exception("GetToolFromMagazine: Impossible to get tool");
            log.Info(e.Message);
            return false;
        }

        public bool StoreToolInMagazine(int Width, out Exception e)
        {
            foreach (var magazine in magazines)
            {
                if (magazine.CanInsertTool(Width, out e))
                {
                    return magazine.StoreToolInMagazine(Width, out e);
                }
            }
            e = new Exception("StoreToolInMagazines: Impossible to store tool in magazines");
            return false;
        }

        public MagazineTool[] GetStatusMagazine(int MagazineID)
        {
            try
            {
                return magazines.Single(magazine => magazine.MagazineId == MagazineID).GetStatusMagazine();
            }
            catch (Exception e)
            {
                log.Error($"GetStatusMagazine: ID Doesn't Exist, {e.Message}");
                return null;
            }
        }

        public MagazineTool[] GetStatusMagazines()
        {
            List<MagazineTool> combinedMagazineTools = new List<MagazineTool>();
            foreach (var magazine in magazines)
            {
                MagazineTool[] magazineTools = magazine.GetStatusMagazine();
                if (magazineTools != null)
                {
                    foreach (var magazineTool in magazineTools)
                    {
                        if (combinedMagazineTools != null && magazineTools != null && combinedMagazineTools.Count() < magazineTools.Count())
                        {
                            combinedMagazineTools.Add(new MagazineTool(0, magazineTool.Width));
                        }

                        combinedMagazineTools.Single(tool => tool.Width == magazineTool.Width).Count += magazineTool.Count;
                    }
                }
                else
                {
                    return null;
                }
            }

            return combinedMagazineTools.ToArray();
        }
    }
}