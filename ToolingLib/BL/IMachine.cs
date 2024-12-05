namespace ToolingLib
{
    //public interface IMachine : IPress, IMagazine
    public interface IMachine
    {
        bool Validation();
    }

    public class Machine : IMachine
    {
        private log4net.ILog log;

        private IPress iPress;
        private IMagazine iMagazine;

        /// <summary>
        /// Costruttore per DI
        /// </summary>
        /// <param name = "log" ></ param >
        /// < param name="iPress"></param>
        /// <param name = "iMagazine" ></ param >
        public Machine(log4net.ILog log)
        {
            this.log = log;
            this.iPress = iPress;
            this.iMagazine = iMagazine;
        }

        /// <summary>
        /// Controlla che non ci siano tool mancanti o duplicati sulla pressa e nel magazzino
        /// </summary>
        /// <returns>True se non ci sono elementi mancanti o duplicati, false altrimenti</returns>
        public bool Validation()
        {
            //ToolPress[] statusToolPress = iPress.GetStatusPress();
            MagazineTool[] statusMagazineTools = iMagazine.GetStatusMagazine();

            ToolPress[] statusToolPress = null; //DA ELIMINARE

            foreach (ToolPress tool in statusToolPress)
            {
                foreach (MagazineTool magazineTool in statusMagazineTools)
                {
                    if (tool.Width == magazineTool.Width)
                    {
                        magazineTool.Count++;
                        break;
                    }
                }
            }

            if (iMagazine.GetAllTools() == statusMagazineTools)
            {
                log.Info("Validation: No Duplicated Or Missing Tools");
                return true;
            }
            else
            {
                log.Error("Validation: Duplicated Or Missing Tools");
                return false;
            }
        }
    }
}