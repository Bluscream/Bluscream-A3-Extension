using Maca134.Arma.DllExport;
using Maca134.Arma.Serializer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

namespace RealTimeAndWeather
{
    public class DllEntry
    {
        #region Public Fields

        private Dictionary<string, int> WeatherTable = new Dictionary<string, int>
        {
            {"", 0}
        };

        #endregion Public Fields

        /*private float ParseWeatherDescription(string description)
        {
            description = description.ToLower();
            switch (description)
            {
                case "few clouds":
                case "scattered clouds":
                case "broken clouds":
                case "rain":
                case "thunderstorm with light rain":
                case "thunderstorm with rain":
                case "thunderstorm with heavy rain":
                case "light thunderstorm":
                case "thunderstorm":
                case "heavy thunderstorm":
                case "ragged thunderstorm":
                case "thunderstorm with light drizzle":
                case "thunderstorm with drizzle":
                case "thunderstorm with heavy drizzle":
                case "light intensity drizzle":
                case "drizzle":
                case "heavy intensity drizzle":
                case "light intensity drizzle rain":
                case "drizzle rain":
                case "heavy intensity drizzle rain":
                case "shower rain and drizzle":
                case "heavy shower rain and drizzle":
                case "shower drizzle":
                case "light rain":
                case "moderate rain":
                case "heavy intensity rain":
                case "very heavy rain":
                case "extreme rain":
                case "freezing rain":
                case "light intensity shower rain":
                case "shower rain":
                case "heavy intensity shower rain":
                case "ragged shower rain":
                case "light snow":
                case "snow":
                case "heavy snow":
                case "sleet":
                case "light shower sleet":
                case "shower sleet":
                case "light rain and snow":
                case "rain and snow":
                case "light shower snow":
                case "shower snow":
                case "heavy shower snow":
                case "mist":
                case "smoke":
                case "haze":
                case "sand/ dust whirls":
                case "fog":
                case "sand":
                case "dust":
                case "volcanic ash":
                case "squalls":
                case "tornado":
                case "clear sky":
                case "few clouds: 11-25%":
                case "scattered clouds: 25-50%":
                case "broken clouds: 51-84%":
                case "overcast clouds: 85-100%":
            }
        }*/

        #region Public Methods

        [ArmaDllExport]
        public static string Invoke(string input, int size)
        {
            var ret = "?";
            try
            {
                var input_split = SplitArguments(input);
                switch (input_split[0].ToLower())
                {
                    case "blu_fnc_getfilesinpath":
                        var path = input_split[1].Replace("\"", string.Empty);
                        var fileList = new Arma.FileList(path, false);
                        ret = Converter.SerializeObject(fileList); break;

                    case "blu_fnc_getcurrentdatetime": // [year, month, day, hour, minute]: Array
                        ret = Converter.SerializeObject(new Arma.DateTime(DateTime.Now)); break;

                    case "blu_fnc_getcurrentweather":
                        return "Weather is not yet working, sorry!";
                        // http://ip-api.com/line/?fields=status,message,country,countryCode,region,regionName,city,zip,lat,lon,timezone
                        var ipData = _download_serialized_json_data<Classes.IPData>("http://ip-api.com/json/");
                        var weather = _download_serialized_json_data<Classes.WeatherData>(
                            $"http://api.openweathermap.org/data/2.5/weather?lat={ipData.lat}&lon={ipData.lon}");
                        // var x=Math.Cos(weather.wind.deg); var y = Math.Sin(weather.wind.deg);
                        return string.Join(",", new[]
                        {
                            weather.wind.speed.ToString(), // setWind [weather select 0, weather select 0, true];
                            weather.wind.deg.ToString(), // 1 setWindDir weather select 1;
                            DoubleFromPercent(weather.clouds.all).ToString(), // 1 setOvercast weather select 2;
                            DoubleFromPercent(weather.main.humidity).ToString() // 1 setRain weather select 3;
                        }); // forceWeatherChange;
                }

                ret = "[blu] Unknown command: " + input;
            }
            catch (Exception ex)
            {
                ret = "Error while processing command (" + input + "): " + ex.Message;
            }

            return ret;
        }

        private static string[] SplitArguments(string commandLine)
        {
            var parmChars = commandLine.ToCharArray();
            var inSingleQuote = false;
            var inDoubleQuote = false;
            for (var index = 0; index < parmChars.Length; index++)
            {
                if (parmChars[index] == '"' && !inSingleQuote)
                {
                    inDoubleQuote = !inDoubleQuote;
                    parmChars[index] = '\n';
                }
                if (parmChars[index] == '\'' && !inDoubleQuote)
                {
                    inSingleQuote = !inSingleQuote;
                    parmChars[index] = '\n';
                }
                if (!inSingleQuote && !inDoubleQuote && parmChars[index] == ' ')
                    parmChars[index] = '\n';
            }
            return (new string(parmChars)).Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }

        #endregion Public Methods

        #region Private Methods

        private static T _download_serialized_json_data<T>(string url) where T : new()
        {
            using (var w = new WebClient())
            {
                var json_data = string.Empty;
                // attempt to download JSON data as a string
                try
                {
                    json_data = w.DownloadString(url);
                }
                catch (Exception) { }
                // if string with JSON data is not empty, deserialize it to class and return its instance
                return !string.IsNullOrEmpty(json_data) ? JsonConvert.DeserializeObject<T>(json_data) : new T();
            }
        }

        private static double DoubleFromPercent(int percent)
        {
            return double.Parse(percent.ToString()) / 100;
        }

        #endregion Private Methods
    }
}