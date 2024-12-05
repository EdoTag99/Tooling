using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Threading;
using ToolingWCF;
using ToolingLib;
using Unity;
using Unity.Resolution;
using System.Data.SQLite;
using System.Data.SqlClient;
using ToolingLib.Configuration;
using System.Configuration;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace ToolingConsoleApp
{
    public class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static IUnityContainer container;
        private static ToolingLibraryConfig config;

        /// <summary>
        /// SelfHosting del servizio
        /// </summary>
        /// <param></param>
        private static void Main()
        {
            container = new UnityContainer();

            config = (ToolingLibraryConfig)ConfigurationManager.GetSection("toolingConfig");

            CreateLoadingFile();
            CreateBaseRecipes();

            try
            {
                SqlConnection sqlConnection = new SqlConnection(config.DataBaseConfiguration.ConnectionString);
                sqlConnection.Open();
                container.RegisterType<IRecipe, RecipeSQL>("sql");
                sqlConnection.Close();
            }
            catch (Exception e)
            {
                log.Error($"Main: {e.Message}");
                container.RegisterType<IRecipe, RecipeSQLite>("sqlite");
            }
            container.RegisterType<IMagazineManager, MagazineManager>();
            container.RegisterType<IPressManager, PressManager>();
            container.RegisterType<IMachine, Machine>();
            container.RegisterType<IRecipe, RecipeJSON>("json");

            var recipes = container.ResolveAll<IRecipe>(new ParameterOverride("log", log), new ParameterOverride("connectionString", config.DataBaseConfiguration.ConnectionString), new ParameterOverride("recipeFolder", config.JsonRecipeConfiguration.FolderPath), new ParameterOverride("connectionStringSQLite", config.SQLiteConfiguration.SQLiteConnectionString));

            foreach (MagazineInstance magazine in config.MagazineCollection)
            {
                container.RegisterInstance<Magazine>($"{magazine.Id}", new Magazine(log, magazine.Id, magazine.FileLoader));
            }

            var magazines = container.ResolveAll<Magazine>(new ParameterOverride("log", log));
            var magazineManager = container.Resolve<IMagazineManager>(new ParameterOverride("log", log), new ParameterOverride("magazines", magazines));

            foreach (PressInstance press in config.PressCollection)
            {
                container.RegisterInstance<Press>($"{press.Id}", new Press(press.Id, press.PressBar, log, recipes, magazineManager));
            }

            var presses = container.ResolveAll<Press>();
            var pressManager = container.Resolve<IPressManager>(new ParameterOverride("log", log), new ParameterOverride("iMagazineManager", magazineManager), new ParameterOverride("iRecipe", recipes), new ParameterOverride("presses", presses));

            ServiceTooling st = container.Resolve<ServiceTooling>(new ParameterOverride("log", log), new ParameterOverride("iRecipe", recipes), new ParameterOverride("iMagazineManager", magazineManager), new ParameterOverride("iPressManagaer", pressManager));

            SQLiteConnection connection = new SQLiteConnection(config.SQLiteConfiguration.SQLiteConnectionString);
            try
            {
                connection.Open();

                SQLiteCommand sqliteCommand = connection.CreateCommand();
                sqliteCommand.CommandText = "CREATE TABLE IF NOT EXISTS RecipeTable (ID INTEGER PRIMARY KEY, RecipeID VARCHAR(25) NOT NULL, Position INT DEFAULT 0, Width INT DEFAULT 50)";
                sqliteCommand.ExecuteNonQuery();

                connection.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            ServiceHost host = null;
            Uri hostUri = new Uri($"http://{config.ServiceConfiguration.BaseUrl}:{config.ServiceConfiguration.Port}");
            Uri serviceUri = new Uri($"{hostUri.OriginalString}/{config.ServiceConfiguration.ServiceEndpoint}");
            try
            {
                host = new ServiceHost(st, hostUri);

                ServiceEndpoint endpoint = host.AddServiceEndpoint(typeof(IServiceTooling), new WebHttpBinding(), config.ServiceConfiguration.ServiceEndpoint);
                endpoint.Behaviors.Add(new WebHttpBehavior());

                log.Info($"Creating the host at: {hostUri}");

                host.Open();
                log.Info($"Hosting at: {hostUri}");

                using (WebChannelFactory<IServiceTooling> wcf = new WebChannelFactory<IServiceTooling>(serviceUri))
                {
                    log.Info($"Starting Tooling Service at: {serviceUri}");
                    IServiceTooling channel = wcf.CreateChannel();
                    log.Info($"Service Started at: {serviceUri}");

                    Console.WriteLine("Press [Enter] to close the service");
                    Console.ReadLine();
                }
                log.Info("Service Closed");
            }
            catch (CommunicationException cex)
            {
                log.Error(cex.Message);
                host.Abort();
            }

            host.Close();
            Thread.Sleep(1500);
        }

        public static void CreateLoadingFile()
        {
            Directory.CreateDirectory("C:\\ToolingFiles\\MagazineLoaders");
            foreach (MagazineInstance mi in config.MagazineCollection)
            {
                if (!File.Exists(mi.FileLoader))
                {
                    MagazineTP magazineTP = new MagazineTP() { MagazineTools = CreateMagazineToolObject(mi) };
                    string magazineToolsStr = JsonSerializer.Serialize(magazineTP);
                    File.WriteAllText(mi.FileLoader, magazineToolsStr);
                }
            }
        }

        private static void CreateBaseRecipes()
        {
            Directory.CreateDirectory("C:\\ToolingFiles\\Recipes");

            foreach (RecipeInstance recipe in config.RecipeCollection)
            {
                if (!File.Exists($"C:\\ToolingFiles\\Recipes\\{recipe.RecipeName}.json"))
                {
                    ToolRecipe recipeObj = new ToolRecipe() { BarConfiguration = CreateRecipeObject(recipe) };
                    string recipeStr = JsonSerializer.Serialize(recipeObj);
                    File.WriteAllText($"C:\\ToolingFiles\\Recipes\\{recipe.RecipeName}.json", recipeStr);
                }
            }
        }

        private static ToolPress[] CreateRecipeObject(RecipeInstance Recipe)
        {
            List<ToolPress> recipe = new List<ToolPress>();
            foreach (ToolInstance ti in Recipe.ToolCollection)
            {
                recipe.Add(new ToolPress(ti.Width, ti.Position));
            }
            return recipe.ToArray();
        }

        private static MagazineTool[] CreateMagazineToolObject(MagazineInstance Magazine)
        {
            List<MagazineTool> magazine = new List<MagazineTool>();
            foreach (MagazineToolInstance mti in Magazine.MagazineToolCollection)
            {
                magazine.Add(new MagazineTool(mti.Count, mti.Width));
            }
            return magazine.ToArray();
        }
    }
}