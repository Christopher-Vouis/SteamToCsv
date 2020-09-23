using System;

public class SteamResult
{
    private string playtime_forever;
    public string appid { get; set; }
    public string Playtime_forever
    {
        get
        {
            return playtime_forever;
        }
        set
        {
            playtime_forever = value;
            HoursPlayed = int.Parse(playtime_forever) / 60.0;
        }
    }
    public string playtime_windows_forever { get; set; }
    public string playtime_mac_forever { get; set; }
    public string playtime_linux_forever { get; set; }
    public string name { get; set; }
    public double HoursPlayed { get; private set; }

    public void PrintInfo()
    {
        Console.WriteLine($"Title: {name}");
        Console.WriteLine($"App Id: {appid}");
        Console.WriteLine($"Play Time: {HoursPlayed.ToString("F1")}");
    }

    public string GetNameNoSymbols()
    {
        string cleanName = name.Replace("®", string.Empty);
        cleanName = cleanName.Replace("™", string.Empty);

        return cleanName;
    }
}
