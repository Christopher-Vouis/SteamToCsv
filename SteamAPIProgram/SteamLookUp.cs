using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using CsvHelper;
using System.IO;
using System.Globalization;

namespace SteamAPIProgram
{
    class SteamLookUp
    {
        static HttpClient client;
        JObject titleList;
        IList<SteamResult> userAppList;
        string userId;
        public SteamLookUp(IEnumerable<char> id)
        {
            if (client == null)
            {
                client = new HttpClient();
            }

            try
            {
                if (CheckID(id))
                {
                    userId = string.Concat(id);
                }
                else
                {
                    throw new Exception();
                }
                titleList = GetAppList().Result;
                userAppList = GetUserAppList(userId).Result;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nError with the http request");
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Invalid Steam ID");
            }
        }

        private async Task<JObject> GetAppList()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("http://api.steampowered.com/ISteamApps/GetAppList/v0002/");
                response.EnsureSuccessStatusCode();
                string appListBody = await response.Content.ReadAsStringAsync();
                JObject appListJson = JObject.Parse(appListBody);
                return (JObject)appListJson["applist"];
            }
            catch
            {
                Console.WriteLine("Error retrieving app list");
                return null;
            }
        }

        private async Task<IList<SteamResult>> GetUserAppList(string userId)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync($"http://api.steampowered.com/IPlayerService/GetOwnedGames/v0001/?key=5B1C3581628E4CC1A238952C6BBCCDC0&steamid={userId}&format=json");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                JObject responseJson = JObject.Parse(responseBody);
                JArray jArray = (JArray)responseJson["response"]["games"];
                var resultList = jArray.ToObject<IList<SteamResult>>();

                foreach (SteamResult sr in resultList)
                {
                    sr.name = GetGameTitle(sr.appid);
                }

                return resultList;
            }
            catch
            {
                Console.WriteLine("Error retrieving Steam user information");
                return null;
            }
        }

        private string GetGameTitle(string id)
        {
            var currentGame = titleList.SelectToken($"$.apps[?(@.appid == {id})]");
            if (currentGame != null)
            {
                return currentGame["name"].ToString();
            }
            else
            {
                //Certain titles may exist in a Steam Library but are not listed in the Steam API's list of apps
                return "Unknown Title";
            }
        }

        public void PrintGameInformation()
        {
            foreach (SteamResult sr in userAppList)
            {
                sr.PrintInfo();
                Console.WriteLine();
            }
        }

        public static bool CheckID(IEnumerable<char> id)
        {
            return id.All(Char.IsDigit);
        }

        public void ExportToCSV()
        {
            try
            {
                using (var streamWriter = new StreamWriter("steam_collection.csv"))
                {
                    using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
                    {
                        csvWriter.WriteField("STEAM GAMES,\n\n", false);
                        csvWriter.Flush();

                        string entry = "Game Title, Hours Played, App Id,\n\n";
                        csvWriter.WriteField(entry, false);
                        csvWriter.Flush();

                        foreach (SteamResult sr in userAppList)
                        {
                            entry = $"\"{sr.GetNameNoSymbols()}\", {sr.HoursPlayed.ToString("F1")} hrs, {sr.appid},\n";
                            csvWriter.WriteField(entry, false);
                            csvWriter.Flush();
                        }
                    }
                    Console.WriteLine($"\nFile created at {((FileStream)streamWriter.BaseStream).Name}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error creative .csv");
                Console.WriteLine(e.Message);
            }
        }
    }
}
