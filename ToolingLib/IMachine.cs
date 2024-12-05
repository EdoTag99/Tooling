using System.Linq;

namespace ToolingLib
{
    //public interface IMachine : IPress, IMagazine

    public interface IMachine
    {
        bool Validation();
    }

    public class Machine : IMachine
    {
        private readonly log4net.ILog log;

        private readonly IMagazineManager magazineManager;

        private readonly IPressManager pressManager;

        public Machine(log4net.ILog log, IMagazineManager magazineManager, IPressManager pressManager)

        {
            this.log = log;
            this.magazineManager = magazineManager;
            this.pressManager = pressManager;
        }

        /// <summary>
        /// Controlla che non ci siano tool private mancanti o duplicati sulla pressa e nel magazzino
        /// </summary>
        /// <returns>True se non ci sono elementi mancanti o duplicati, false altrimenti</returns>
        public bool Validation()
        {
            bool result = true;
            var currentStatus = magazineManager.GetStatusMagazines().ToList();

            foreach (var pressId in pressManager.GetPressBars())
            {
                foreach (var tool in pressManager.GetStatusPress(pressId.PressID))
                {
                    currentStatus.Single(x => x.Width == tool.Width).Count++;
                }
            }

            foreach (var tool in magazineManager.GetAllTools().ToList())
            {
                if (currentStatus.Single(x => x.Width == tool.Width).Count != tool.Count)
                {
                    result = false;
                }
            }

            if (result)
            {
                log.Info("Validation: No Duplicated Or Missing Tools");
            }
            else
            {
                log.Error("Validation: Duplicated Or Missing Tools");
            }
            return result;
        }
    }
}