// See https://aka.ms/new-console-template for more information

using System;
using System.Net.Http.Json;
using System.Threading;
using Tomlyn;

class Program
{
    public class Config
    {
        public string ibmApiKey { get; set; }
        public string city {get; set;}
        public string playlistLocation { get; set; }
    }
    static async Task<string> fetchWeather(string city, string ibmApiKey)
    {
        HttpClient client = new HttpClient();

        var zipCodesReq = await client.GetFromJsonAsync<LocationSearchResponse>(
            $"https://api.weather.com/v3/location/search?query={city}&language=en-US&format=json&locationType=city&countryCode=US&adminDistrictCode=GA&apiKey={ibmApiKey}");
        string zip = zipCodesReq?.location?.postalKey?[0];
        var ccReq = await client.GetFromJsonAsync<CurrentConditions>(
            $"https://api.weather.com/v3/wx/observations/current?postalKey={zip}&units=e&language=en-US&format=json&apiKey={ibmApiKey}");

        string ccFull = $"Currently at {city}\n{ccReq?.wxPhraseMedium}\n{ccReq?.temperature} degrees\nWind: {ccReq?.windDirectionCardinal} {ccReq?.windSpeed} mph";
        
        return ccFull;
    }
   public class CurrentConditions
    {
        public int? cloudCeiling { get; set; }
        public int cloudCover { get; set; }
        public string cloudCoverPhrase { get; set; }
        public string dayOfWeek { get; set; }
        public string dayOrNight { get; set; }
        public long expirationTimeUtc { get; set; }
        public int iconCode { get; set; }
        public int iconCodeExtend { get; set; }
        public string obsQualifierCode { get; set; } 
        public int? obsQualifierSeverity { get; set; } 
        public double precip1Hour { get; set; } 
        public double precip6Hour { get; set; } 
        public double precip24Hour { get; set; } 
        public double pressureAltimeter { get; set; } 
        public double pressureChange { get; set; } 
        public double pressureMeanSeaLevel { get; set; } 
        public int pressureTendencyCode { get; set; } 
        public string pressureTendencyTrend { get; set; } 
        public int relativeHumidity { get; set; } 
        public double snow1Hour { get; set; } 
        public double snow6Hour { get; set; } 
        public double snow24Hour { get; set; } 
        public string sunriseTimeLocal { get; set; } 
        public long sunriseTimeUtc { get; set; } 
        public string sunsetTimeLocal { get; set; } 
        public long sunsetTimeUtc { get; set; } 
        public int temperature { get; set; } 
        public int temperatureChange24Hour { get; set; } 
        public int temperatureDewPoint { get; set; } 
        public int temperatureFeelsLike { get; set; } 
        public int temperatureHeatIndex { get; set; } 
        public int temperatureMax24Hour { get; set; } 
        public int temperatureMaxSince7Am { get; set; } 
        public int temperatureMin24Hour { get; set; } 
        public int temperatureWetBulbGlobe { get; set; } 
        public int temperatureWindChill { get; set; } 
        public string uvDescription { get; set; } 
        public int uvIndex { get; set; }
        public string validTimeLocal { get; set; }
        public long validTimeUtc { get; set; }
        public double visibility { get; set; }
        public int windDirection { get; set; }
        public string windDirectionCardinal { get; set; }
        public int? windGust { get; set; } 
        public int windSpeed { get; set; }
        public string wxPhraseLong { get; set; }
        public string wxPhraseMedium { get; set; }
        public string wxPhraseShort { get; set; }
    } 
    public class LocationSearchResponse
    {
        public LocationData location { get; set; }
    }

    public class LocationData
    {
        public string[] postalKey { get; set; }
    }
    
    static string weatherDisplay;
    static Task Main()
    {
        try
        {
            string configFile = File.ReadAllText("config.toml");
        
            var config = TomlSerializer.Deserialize<Config>(configFile);
            Console.Clear();
            Console.WriteLine("CLITV - Written by Mice");
            Console.WriteLine("Inspired by nezTV");
            Console.WriteLine("https://github.com/miceoroni/CLITV");

            _ = Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        if (config != null) weatherDisplay = await fetchWeather(config.city, config.ibmApiKey);
                    }
                    catch (Exception ex)
                    {
                        weatherDisplay = $"Weather unavailable: {ex.Message}";
                    }
                    await Task.Delay(TimeSpan.FromMinutes(10));
                }
            });
            while (true)
            {
                Console.SetCursorPosition(0, 4);
                Console.Write($"Current Time: {DateTime.Now:G}");
                Console.SetCursorPosition(0, 5);
                Console.Write(weatherDisplay);
                Thread.Sleep(1000);
            }
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    }
}