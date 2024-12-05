using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace ToolingLib
{
    public interface IRecipe
    {
        List<RecipeModel> GetRecipeList();

        List<ToolPress> GetRecipe(RecipeModel Recipe);

        bool AddNewRecipe(string json, string name, out string message);

        bool DeleteRecipe(string RecipeName);
    }

    public class RecipeJSON : IRecipe
    {
        private readonly string recipeFolder;

        private readonly log4net.ILog log;

        /// <summary>
        /// Costruttore per DI
        /// </summary>
        /// <param name="log"></param>

        public RecipeJSON(log4net.ILog log, string recipeFolder)
        {
            this.log = log;
            this.recipeFolder = recipeFolder;
        }

        /// <summary>
        /// Riempe una lista di Recipe in base ai file JSON presenti nella folder
        /// </summary>
        /// <returns>ReicpeModel List</returns>
        public List<RecipeModel> GetRecipeList()
        {
            string[] files = Directory.GetFiles(recipeFolder);
            List<RecipeModel> recipes = new List<RecipeModel>();
            foreach (string file in files)
            {
                if (!Path.GetFileNameWithoutExtension(file).Contains("tools"))
                {
                    recipes.Add(new RecipeModel() { Name = Path.GetFileNameWithoutExtension(file), Source = file });
                }
            }
            log.Info($"GetRecipeList: Got {files.Length} JSON Recipes");

            return recipes;
        }

        /// <summary>
        /// Ottiene e converte la recipe da un file JSON specificato all'interndo di RecipeModel
        /// </summary>
        /// <param name="Recipe"></param>
        /// <returns>La lista di ToolPress corrispondenti alla recipe</returns>
        public List<ToolPress> GetRecipe(RecipeModel Recipe)
        {
            ToolPress[] recipe = null;
            if (Recipe != null && Recipe.Source != null)
            {
                log.Info("GetRecipe: Getting Recipe as JSON File");
                try
                {
                    string jsonRecipe = File.ReadAllText(Recipe.Source);
                    recipe = JsonSerializer.Deserialize<ToolRecipe>(jsonRecipe).BarConfiguration;
                }
                catch (Exception e)
                {
                    log.Error($"GetRecipe: {e.Message}");
                }
            }
            log.Info("GetRecipe: Returning Recipe");
            return recipe.ToList();
        }

        /// <summary>
        /// Scrive un nuovo file JSON contenente la rappresentazione di una Recipe
        /// </summary>
        /// <param name="json"></param>
        /// <param name="message"></param>
        /// <returns>True se l'operazione è andata a buon fine, false altrimenti</returns>
        public bool AddNewRecipe(string json, string name, out string message)
        {
            message = "Adding Recipe as JSON File";
            log.Info($"AddNewRecipe: {message}");

            if (Directory.Exists(recipeFolder))
            {
                string newFileName = $"JSON_{name}.json";
                string newFile = Path.Combine(recipeFolder, newFileName);
                try
                {
                    using (FileStream fs = File.Create(newFile))
                    {
                        byte[] recipe = new UTF8Encoding(true).GetBytes(json);
                        fs.Write(recipe, 0, recipe.Length);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    message = ex.Message;
                    log.Error($"AddNewRecipe: {message}");
                    return false;
                }

                message = $"File created as {newFileName}";
                log.Info($"AddNewRecipe: {message}");
                return true;
            }

            message = "Folder Doesn't Exist";
            log.Error($"AddNewRecipe: {message}");
            return false;
        }

        /// <summary>
        /// Elimina il file JSON con il nome specificato
        /// </summary>
        /// <param name="RecipeName"></param>
        /// <returns></returns>
        public bool DeleteRecipe(string RecipeName)
        {
            string[] allFiles = Directory.GetFiles(recipeFolder);
            foreach (string file in allFiles)
            {
                if (Path.GetFileNameWithoutExtension(file) == RecipeName)
                {
                    File.Delete(file);
                    return true;
                }
            }

            return false;
        }
    }

    public class RecipeSQL : IRecipe
    {
        private readonly string connectionString;

        private readonly log4net.ILog log;

        /// <summary>
        /// Costruttore per DI
        /// </summary>
        /// <param name="log"></param>

        public RecipeSQL(log4net.ILog log, string connectionString)
        {
            this.log = log;
            this.connectionString = connectionString;
        }

        /// <summary>
        /// Ottiene la lista di Recipe presenti all'interno del DB
        /// </summary>
        /// <returns>Restituisce la lista di Recipe</returns>
        public List<RecipeModel> GetRecipeList()
        {
            List<RecipeModel> recipes = new List<RecipeModel>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = $"SELECT DISTINCT RecipeId FROM RecipesTable";
                    SqlCommand command = new SqlCommand(query, connection);
                    var count = 0;
                    connection.Open();
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            recipes.Add(new RecipeModel() { Name = $"{dataReader["RecipeId"]}", Source = "DB" });
                            count++;
                        }

                        connection.Close();
                        log.Info($"GetRecipeList: Got {recipes.Count} Recipes From SQL");
                    }
                }
            }
            catch (Exception e)
            {
                log.Error($"GetRecipeList: {e.Message}");
            }
            return recipes;
        }

        /// <summary>
        /// Ottiene la recipe specificata nel paramentro RecipeModel dal DB e la converte in una lista di ToolPress
        /// </summary>
        /// <param name="Recipe"></param>
        /// <returns>Restituisce l'array di ToolPress ottenuto</returns>
        public List<ToolPress> GetRecipe(RecipeModel Recipe)
        {
            log.Info("GetRecipe: Getting Recipe from DB");
            List<ToolPress> tools = new List<ToolPress>();
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = $"SELECT ToolWidth, ToolPosition FROM RecipesTable WHERE RecipeId = @FileName";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@FileName", Recipe.Name);
                    connection.Open();
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            int width = int.Parse(dataReader["ToolWidth"].ToString());
                            double position = double.Parse(dataReader["ToolPosition"].ToString());
                            ToolPress tp = new ToolPress(width, position);
                            tools.Add(tp);
                        }

                        connection.Close();
                    }
                }
            }
            catch (Exception e)
            {
                log.Error($"GetRecipe: {e.Message}");
            }
            return tools;
        }

        /// <summary>
        /// Serializza una nuova Recipe e la inserisce all'interno del DB
        /// </summary>
        /// <param name="json"></param>
        /// <param name="name"></param>
        /// <param name="message"></param>
        /// <returns>True se l'operazione è andata a buon fine, false altrimenti</returns>
        public bool AddNewRecipe(string json, string name, out string message)
        {
            message = "Adding Recipe to DB";
            log.Info($"AddNewRecipe: {message}");

            ToolPress[] recipe = JsonSerializer.Deserialize<ToolRecipe>(json).BarConfiguration;

            int recipeNumber = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string queryRecipeNumber = "SELECT count(DISTINCT RecipeId) as RecipeNumber FROM RecipesTable";
                    SqlCommand commandRN = new SqlCommand(queryRecipeNumber, connection);

                    using (SqlDataReader dataReader = commandRN.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            recipeNumber = int.Parse(dataReader["RecipeNumber"].ToString()) + 1;
                        }
                    }

                    var command = connection.CreateCommand();

                    foreach (ToolPress tp in recipe)
                    {
                        string query = $"INSERT INTO RecipeTable (RecipeId, ToolWidth, ToolPosition) VALUES ('SQL Recipe {recipeNumber}', '{tp.Width}', '{tp.Position}')";
                        command.CommandText = query;
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    log.Error($"AddNewRecipe: {message}");
                    return false;
                }

                message = $"Recpie Added to DB as SQL Recipe {recipeNumber}";
                log.Info($"AddNewRecipe: {message}");
                return true;
            }
        }

        /// <summary>
        /// Elimina la recipe con il nome specificato dal DB
        /// </summary>
        /// <param name="RecipeName"></param>
        /// <returns>True se l'operazione è andata a buon fine, false altrimenti</returns>
        public bool DeleteRecipe(string RecipeName)
        {
            log.Info($"DeleteRecipe: Deleting {RecipeName}");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "DELETE FROM RecipesTable WHERE RecipeId=@RecipeName";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@FileName", RecipeName);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    log.Error($"DeleteRecipe: {ex.Message}");
                    return false;
                }

                log.Info($"DeleteRecipe: Recipe {RecipeName} Removed");
                return true;
            }
        }
    }

    public class RecipeSQLite : IRecipe
    {
        private readonly string connectionStringSQLite;
        private readonly log4net.ILog log;

        public RecipeSQLite(log4net.ILog log, string connectionStringSQLite)
        {
            this.log = log;
            this.connectionStringSQLite = connectionStringSQLite;
        }

        /// <summary>
        /// Ottiene la lista di Recipe presenti all'interno del DB SQLite
        /// </summary>
        /// <returns>Restituisce la lista di Recipe</returns>
        public List<RecipeModel> GetRecipeList()
        {
            List<RecipeModel> recipes = new List<RecipeModel>();
            SQLiteConnection connection = new SQLiteConnection(connectionStringSQLite);
            try
            {
                string query = $"SELECT DISTINCT RecipeID FROM RecipeTable";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                var count = 0;
                connection.Open();
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        recipes.Add(new RecipeModel() { Name = $"{dataReader["RecipeID"]}", Source = "SQLite" });
                        count++;
                    }

                    connection.Close();
                    log.Info($"GetRecipeList: Got {recipes.Count} Recipes From SQL");
                }
            }
            catch (Exception e)
            {
                log.Error($"GetRecipeList: {e.Message}");
            }
            return recipes;
        }

        /// <summary>
        /// Ottiene la recipe specificata nel paramentro RecipeModel dal DB SQLite e la converte in una lista di ToolPress
        /// </summary>
        /// <param name="Recipe"></param>
        /// <returns>Restituisce l'array di ToolPress ottenuto</returns>
        public List<ToolPress> GetRecipe(RecipeModel Recipe)
        {
            log.Info("GetRecipe: Getting Recipe from SQLite");
            List<ToolPress> tools = new List<ToolPress>();

            SQLiteConnection connection = new SQLiteConnection(connectionStringSQLite);
            try
            {
                SQLiteCommand command = new SQLiteCommand("SELECT Position, Width FROM RecipeTable WHERE RecipeID = @FileName", connection);
                command.Parameters.AddWithValue("@FileName", Recipe.Name);
                connection.Open();
                using (SQLiteDataReader dataReader = command.ExecuteReader())
                {
                    while (dataReader.Read())
                    {
                        int width = int.Parse(dataReader["Width"].ToString());
                        double position = double.Parse(dataReader["Position"].ToString());
                        ToolPress tp = new ToolPress(width, position);
                        tools.Add(tp);
                    }

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.Error($"GetRecipe: {e.Message}");
            }

            return tools;
        }

        /// <summary>
        /// Serializza una nuova Recipe e la inserisce all'interno del DB SQLite
        /// </summary>
        /// <param name="json"></param>
        /// <param name="name"></param>
        /// <param name="message"></param>
        /// <returns>True se l'operazione è andata a buon fine, false altrimenti</returns>
        public bool AddNewRecipe(string json, string name, out string message)
        {
            message = "Adding Recipe to SQLite";
            log.Info($"AddNewRecipe: {message}");

            ToolPress[] recipe = JsonSerializer.Deserialize<ToolRecipe>(json).BarConfiguration;

            SQLiteConnection connection = new SQLiteConnection(connectionStringSQLite);
            try
            {
                connection.Open();
                SQLiteCommand sqliteCommand = new SQLiteCommand("SELECT count(DISTINCT RecipeID) as RecipeNumber FROM RecipeTable", connection);
                foreach (ToolPress tp in recipe)
                {
                    sqliteCommand.CommandText = "INSERT INTO RecipeTable (RecipeID, Position, Width) VALUES (@recipeNumber, @tpPosition, @tpWidth)";
                    sqliteCommand.Parameters.AddWithValue("@recipeNumber", $"SQLite_{name}");
                    sqliteCommand.Parameters.AddWithValue("@tpPosition", tp.Position);
                    sqliteCommand.Parameters.AddWithValue("@tpWidth", tp.Width);
                    sqliteCommand.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                message = ex.Message;
                log.Error($"AddNewRecipe: {message}");
                return false;
            }

            message = $"Recpie Added to SQLite as SQLite_{name}";
            log.Info($"AddNewRecipe: {message}");
            return true;
        }

        /// <summary>
        /// Elimina la recipe con il nome specificato dal DB
        /// </summary>
        /// <param name="RecipeName"></param>
        /// <returns>True se l'operazione è andata a buon fine, false altrimenti</returns>
        public bool DeleteRecipe(string RecipeName)
        {
            log.Info($"DeleteRecipe: Deleting {RecipeName}");

            SQLiteConnection connection = new SQLiteConnection(connectionStringSQLite);

            {
                try
                {
                    connection.Open();
                    SQLiteCommand sqliteCommand = new SQLiteCommand("DELETE FROM RecipeTable WHERE RecipeId=@RecipeName", connection);
                    sqliteCommand.Parameters.AddWithValue("@RecipeName", RecipeName);

                    sqliteCommand.ExecuteNonQuery();

                    connection.Close();
                }
                catch (Exception ex)
                {
                    log.Error($"DeleteRecipe: {ex.Message}");
                    return false;
                }

                log.Info($"DeleteRecipe: Recipe {RecipeName} Removed");
                return true;
            }
        }
    }
}