using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

public class ProSwapperApi
{
    private static readonly HttpClient client = new HttpClient();

    public static async Task<JArray> FetchUserInfo(string username)
    {
        string apiUrl = $"https://api.proswapper.xyz/external/name/{username}";
        HttpResponseMessage response = await client.GetAsync(apiUrl);
        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            JArray userInfoList = JArray.Parse(responseBody);
            return userInfoList;
        }
        return null;
    }

    public static async Task<JArray> FetchRanksInfo(string accountId)
    {
        string ranksApiUrl = $"https://api.proswapper.xyz/ranks/lookup/id/{accountId}";
        HttpResponseMessage response = await client.GetAsync(ranksApiUrl);
        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            JArray ranksInfo = JArray.Parse(responseBody);
            return ranksInfo;
        }
        return null;
    }

    public static async Task Userinfo(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Introduce el nombre de usuario de Epic Games:");
            Console.ResetColor();
            return;
        }

        try
        {
            JArray userInfoList = await FetchUserInfo(username);

            if (userInfoList == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error al obtener información para el usuario {username}.");
                Console.ResetColor();
                return;
            }

            foreach (JObject userInfo in userInfoList)
            {
                string accountId = userInfo["id"].ToString();
                string displayName = userInfo.Value<string>("displayName") ?? "Desconocido";

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"");
                Console.WriteLine($"Nombre de pantalla: {displayName}");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Account ID: {accountId}");
                Console.ResetColor();

                if (userInfo["externalAuths"] != null)
                {
                    if (userInfo["externalAuths"]["psn"] != null)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkBlue;
                        Console.WriteLine($"\nAccount ID PSN: {userInfo["externalAuths"]["psn"]["externalAuthId"]}");
                        Console.WriteLine($"Nombre en pantalla de PSN: {userInfo["externalAuths"]["psn"]["externalDisplayName"]}");
                        Console.ResetColor();
                    }

                    if (userInfo["externalAuths"]["nintendo"] != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        foreach (JObject authId in userInfo["externalAuths"]["nintendo"]["authIds"])
                        {
                            Console.WriteLine($"\nAccount ID Nintendo: {authId["id"]}");
                        }
                        Console.ResetColor();
                    }
                }

                JArray ranksInfo = await FetchRanksInfo(accountId);

                if (ranksInfo != null)
                {
                    foreach (JObject item in ranksInfo)
                    {
                        string rankingType = item.Value<string>("rankingType");
                        string currentDivisionName = item.Value<string>("currentDivisionName");
                        double? promotionProgress = item.Value<double?>("promotionProgress");

                        if (rankingType == "ranked-br")
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"\nRanked Battle Royale: {currentDivisionName} ({promotionProgress.Value:P0})");
                            if (promotionProgress.HasValue)
                            {
                               
                            }
                            Console.ResetColor();
                        }
                        else if (rankingType == "ranked-zb")
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"\nRanked Zero Build: {currentDivisionName} ({promotionProgress.Value:P0})");
                            if (promotionProgress.HasValue)
                            {
                               
                            }
                            Console.ResetColor();
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {e.Message}");
            Console.WriteLine($"Error in Userinfo: {e}");
            Console.ResetColor();
        }
    }

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Introduce el nombre de usuario de Epic Games:");
        string username = Console.ReadLine();
        await Userinfo(username);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.ResetColor();
        Console.ReadKey();
    }
}