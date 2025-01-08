using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CosmicExplorer
{
    public class BodyService
    {
        HttpClient httpClient;
        private bool IsBusy = false;
        public string status = "";

        private string jsonAsString;
        public BodyService ()
        {
            if(CheckIfFileExists("planetinfo.json"))
            {
                GetBodiesFromFile();
            }
            else
            {
                httpClient = new HttpClient();
                _ = GetBodiesFromHttp();
            } 
            
        }

        public List<Body> bodyList = new List<Body>();

        public async void GetBodiesFromFile()
        {
            string filePath = Path.Combine(FileSystem.AppDataDirectory, "planetinfo.json");
            string contents = await File.ReadAllTextAsync(filePath);


            bodyList = JsonSerializer.Deserialize<Root>(contents).bodies;
        }

        public async Task GetBodiesFromHttp()
        {
            try
            {
                var response = await httpClient.GetAsync("https://api.le-systeme-solaire.net/rest/bodies/");
                if (response.IsSuccessStatusCode)
                {
                    string contents = await response.Content.ReadAsStringAsync();
                    bodyList = JsonSerializer.Deserialize<Root>(contents).bodies;

                    await SaveJsonToFile(contents);

                    //bodyList = bodyResponse.bodies;
                    
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error in HTTP request ", ex.Message, "OK");
            }
        }

        private async Task SaveJsonToFile(string jsonContent)
        {
            try
            {
                string fileName = "planetinfo.json";

                // Get the path to the AppDataDirectory
                string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                // Write the JSON content to the file
                await File.WriteAllTextAsync(filePath, jsonContent);
            }
            catch (Exception ex)
            {
                // Handle any errors
                await Shell.Current.DisplayAlert("Error in reading local", ex.Message, "OK");
            }
        }

        private bool CheckIfFileExists(string fileName)
        {
            // Get the full path to the file
            string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            // Check if the file exists
            return File.Exists(filePath);
        }

        public List<Body> GetBodyList()
        {
            return bodyList;
        }

    }
}
