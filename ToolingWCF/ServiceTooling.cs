using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using ToolingLib;
using ToolingLib.Models;

namespace ToolingWCF
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceTooling : IServiceTooling
    {
        #region Fields

        private readonly IPressManager iPressManager;
        private readonly IEnumerable<IRecipe> iRecipe;
        private readonly IMachine iMachine;

        //private IMachine iMachine;
        private readonly IMagazineManager iMagazineManager;

        #endregion Fields

        #region Public Methods

        public ServiceTooling()
        {
        }

        /// <summary>
        /// Costruttore per DI
        /// </summary>
        /// <param name="iPressManager"></param>
        /// <param name="iMagazineManager"></param>
        /// <param name="iRecipe"></param>
        /// <param name="iMachine"></param>
        public ServiceTooling(IPressManager iPressManager, IMagazineManager iMagazineManager, IEnumerable<IRecipe> iRecipe, IMachine iMachine)
        {
            this.iPressManager = iPressManager;
            this.iRecipe = iRecipe;
            this.iMagazineManager = iMagazineManager;
            this.iMachine = iMachine;
        }

        /// <summary>
        /// Riceve la richiesta dal client e richiama il metodo <see cref="IPressManager.AddTool(int, int, int, double, out Exception)"/>
        /// </summary>
        /// <param name="PressId"></param>
        /// <param name="Width"></param>
        /// <param name="Position"></param>
        /// <param name="MagazineID"></param>
        /// <returns>Formato di risposta con valore booleano</returns>
        public ResponseFormatElse AddTool(int PressId, int Width, double Position, int MagazineID)
        {
            ResponseFormatElse rf = new ResponseFormatElse();
            bool status = iPressManager.AddTool(PressId, MagazineID, Width, Position, out Exception e);
            rf.Status = status ? 200 : 500;
            rf.Message = e.Message;
            rf.Data = status;

            return rf;
        }

        /// <summary>
        /// Riceve la richiesta dal client e richiama il metodo <see cref="IPressManager.AddTool(int, int, int, out Exception)"/>
        /// </summary>
        ///<param name="PressId"></param>
        /// <param name="Width"></param>
        /// <param name="MagazineID"></param>
        /// <returns>Formato di risposta con valore booleano</returns>
        public ResponseFormatElse AddToolFirstFreePosition(int PressId, int Width, int MagazineID)
        {
            ResponseFormatElse rf = new ResponseFormatElse();
            bool status = iPressManager.AddTool(PressId, MagazineID, Width, out Exception e);
            rf.Status = status ? 200 : 500;
            rf.Message = e.Message;
            rf.Data = status;

            return rf;
        }

        /// <summary>
        /// Riceve la richiesta dal client e richiama il metodo <see cref="IMagazineManager.GetStatusMagazines"/>
        /// </summary>
        /// <returns>Formato di risposta con array di MagazineTool</returns>
        public ResponseFormatMT GetStatusMagazines()
        {
            ResponseFormatMT rf = new ResponseFormatMT();
            MagazineTool[] status = iMagazineManager.GetStatusMagazines();
            rf.Status = status != null ? 200 : 500;
            rf.Message = status != null ? "Success" : "Error";
            rf.Data = status;

            return rf;
        }

        /// <summary>
        /// Riceve la richiesta dal client e richiama il metodo <see cref="IMagazineManager.GetStatusMagazine(int)"/>
        /// </summary>
        /// <param name="MagazineID"></param>
        /// <returns></returns>
        public ResponseFormatMT GetStatusMagazine(int MagazineID)
        {
            ResponseFormatMT rf = new ResponseFormatMT();
            MagazineTool[] status = iMagazineManager.GetStatusMagazine(MagazineID);
            rf.Status = status != null ? 200 : 500;
            rf.Message = status != null ? "Success" : "Error";
            rf.Data = status;

            return rf;
        }

        /// <summary>
        /// Riceve la richiesta dal client e richiama il metodo <see cref="IPressManager.GetStatusPress(int)"/>
        /// </summary>
        /// <param name="PressId"></param>
        /// <returns>Formato di risposta con array di ToolPress</returns>
        public ResponseFormatTP GetStatusPress(int PressId)
        {
            ResponseFormatTP rf = new ResponseFormatTP();
            ToolPress[] status = iPressManager.GetStatusPress(PressId);
            rf.Status = status != null ? 200 : 500;
            rf.Message = status != null ? "Success" : "Error";
            rf.Data = status;

            return rf;
        }

        /// <summary>
        /// Riceve la richiesta dal client e richiama il metodo <see cref="IMagazineManager.LoadMagazines(string, out Exception)"/>
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns>Formato di risposta con valore booleano</returns>
        public ResponseFormatElse LoadMagazine(string FileName)
        {
            ResponseFormatElse rf = new ResponseFormatElse();
            bool status = iMagazineManager.LoadMagazines(FileName, out Exception e);
            rf.Status = status ? 200 : 500;
            rf.Message = e.Message;
            rf.Data = status;

            return rf;
        }

        /// <summary>
        /// Riceve la richiesta dal client e richiama il metodo <see cref="IPressManager.LoadRecepie(int, RecipeModel, out Exception)"/>
        /// </summary>
        /// <param name="PressId"></param>
        /// <param name="RecipeName"></param>
        /// <param name="RecipeSource"></param>
        /// <returns>Formato di risposta con valore booleano</returns>
        public ResponseFormatElse LoadRecipe(int PressId, string RecipeName, string RecipeSource)
        {
            ResponseFormatElse rf = new ResponseFormatElse();
            RecipeModel recipe = new RecipeModel() { Name = RecipeName, Source = RecipeSource };
            bool status = iPressManager.LoadRecepie(PressId, recipe, out Exception e);
            rf.Status = status ? 200 : 500;
            rf.Message = e.Message;
            rf.Data = status;

            return rf;
        }

        /// <summary>
        /// Riceve la richiesta dal client e richiama il metodo <see cref="IPressManager.RemoveTool(int, int, double, out Exception)"/>
        /// </summary>
        /// <param name="PressId"></param>
        /// <param name="Width"></param>
        /// <param name="Position"></param>
        /// <returns>Formato di risposta con valore booleano</returns>
        public ResponseFormatElse RemoveTool(int PressId, int Width, double Position)
        {
            ResponseFormatElse rf = new ResponseFormatElse();
            bool status = iPressManager.RemoveTool(PressId, Width, Position, out Exception e);
            rf.Status = status ? 200 : 500;
            rf.Message = e.Message;
            rf.Data = status;

            return rf;
        }

        /// <summary>
        /// Riceve la richiesta dal client e richiama il metodo <see cref="IPressManager.RemoveAllTools(int, out Exception)"/>
        /// </summary>
        /// <param name="PressId"></param>
        /// <returns>Formato di risposta con valore booleano</returns>
        public ResponseFormatElse RemoveAllTools(int PressId)
        {
            ResponseFormatElse rf = new ResponseFormatElse();
            bool status = iPressManager.RemoveAllTools(PressId, out Exception e);
            rf.Status = status ? 200 : 500;
            rf.Message = e.Message;
            rf.Data = status;

            return rf;
        }

        /// <summary>
        /// Riceve la richiesta dal client e richiama il metodo <see cref="IMachine.Validation()"/>
        /// </summary>
        /// <returns>Formato di risposta con valore booleano</returns>
        public ResponseFormatElse Validation()
        {
            ResponseFormatElse rf = new ResponseFormatElse();
            bool status = iMachine.Validation();
            rf.Status = status ? 200 : 500;
            rf.Message = status ? "No Duplicated or Missing Tools" : "Duplicated or Missing Tools";
            rf.Data = status;

            return rf;
        }

        /// <summary>
        /// Riceve la richiesta dal client e richiama il metodo <see cref="IRecipe.AddNewRecipe(string, string, out string)"/>
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Formato di risposta con valore booleano</returns>
        public ResponseFormatElse NewRecipeFile(DataRequest data)
        {
            ResponseFormatElse rf = new ResponseFormatElse();
            Type type = null;

            switch (data.Source)
            {
                case "sql":
                    foreach (IRecipe recipe in iRecipe)
                    {
                        if (recipe.GetType() == typeof(RecipeSQL))
                        {
                            type = typeof(RecipeSQL);
                            break;
                        }
                        else
                        {
                            type = typeof(RecipeSQLite);
                        }
                    }
                    break;

                default:
                    type = typeof(RecipeJSON);
                    break;
            }
            bool status = iRecipe.FirstOrDefault(t => t.GetType() == type).AddNewRecipe(data.Json, data.Name, out string message);
            rf.Status = status ? 200 : 500;
            rf.Message = message;
            rf.Data = status;

            return rf;
        }

        /// <summary>
        /// Riceve la richiesta dal client e richiama il metodo <see cref="IRecipe.GetRecipeList()"/>
        /// </summary>
        /// <returns>Lista di recipes già nominate in base al formato</returns>
        public List<RecipeModel> GetRecipes()
        {
            List<RecipeModel> recipes = iRecipe.FirstOrDefault(t => t.GetType() == typeof(RecipeJSON)).GetRecipeList();

            if (iRecipe.FirstOrDefault(t => t.GetType() == typeof(RecipeSQL)) != null)
            {
                List<RecipeModel> recipeSQL = iRecipe.FirstOrDefault(t => t.GetType() == typeof(RecipeSQL)).GetRecipeList();

                foreach (RecipeModel recipe in recipeSQL)
                {
                    recipes.Add(recipe);
                }
            }
            else
            {
                List<RecipeModel> recipeSQLite = iRecipe.FirstOrDefault(t => t.GetType() == typeof(RecipeSQLite)).GetRecipeList();

                foreach (RecipeModel recipe in recipeSQLite)
                {
                    recipes.Add(recipe);
                }
            }

            return recipes;
        }

        /// <summary>
        /// Riceve la richiesta dal client e richiama il metodo <see cref="IPressManager.SaveBarAsRecipe(int, string, string, out string)"/>
        /// </summary>
        /// <param name="PressId"></param>
        /// <param name="format"></param>
        /// <param name="name"></param>
        /// <returns>Formato di risposta con valore booleano</returns>
        public ResponseFormatElse SaveBarAsRecipe(int PressId, string format, string name)
        {
            ResponseFormatElse rf = new ResponseFormatElse();
            bool status = iPressManager.SaveBarAsRecipe(PressId, format, name, out string message);
            rf.Status = status ? 200 : 500;
            rf.Message = message;
            rf.Data = status;

            return rf;
        }

        /// <summary>
        /// Riceve la richiesta dal client e richiama il metodo <see cref="IRecipe.DeleteRecipe(string)"/>
        /// </summary>
        /// <param name="format"></param>
        /// <param name="name"></param>
        /// <returns>Formato di risposta con valore booleano</returns>
        public ResponseFormatElse DeleteRecipe(string format, string name)
        {
            Type type = null;
            switch (format)
            {
                case "SQLite":
                    type = typeof(RecipeSQLite);
                    break;

                case "sql":
                    type = typeof(RecipeSQL);
                    break;

                default:
                    type = typeof(RecipeJSON);
                    break;
            }
            ResponseFormatElse rf = new ResponseFormatElse();
            bool status = iRecipe.FirstOrDefault(t => t.GetType() == type).DeleteRecipe(name); ;
            rf.Status = status ? 200 : 500;
            rf.Message = status ? $"Recipe {name} Delted Successfully!" : "An Error Occurred!";
            rf.Data = status;

            return rf;
        }

        /// <summary>
        /// Riceve la richiesta dal client e richiama il metodo <see cref="IPressManager.GetPressBars()"/>
        /// </summary>
        /// <returns>Un array contenente gli id delle presse</returns>
        public List<PressBar> GetPressBars()
        {
            return iPressManager.GetPressBars();
        }

        /// <summary>
        /// Riceve la richiesta dal client e richiama il metodo <see cref="IMagazineManager.GetMagazines()"/>
        /// </summary>
        /// <returns>Un array contenente gli id dei magazzini</returns>
        public int[] GetMagazines()
        {
            return iMagazineManager.GetMagazines();
        }

        #endregion Public Methods
    }
}