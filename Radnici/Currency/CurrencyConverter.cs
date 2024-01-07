using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;

namespace Radnici.Currency
{
    public class CurrencyConverter
    {
        Dictionary<string, string> symbols;

        public Dictionary<string, string> GetSymbols()
        {
            if (symbols == null)
            {
                symbols = new Dictionary<string, string>();
                string responseContent = getResponseString($"exchangerates_data/symbols");

                Dictionary<string, object> responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);
                if ((bool)responseData["success"])
                {
                    var tempSymbols = (JObject)responseData["symbols"];
                    symbols = tempSymbols.ToObject<Dictionary<string, string>>();
                }
            }


            return symbols;
        }

        public double Convert(string fromCurrency, string toCurrency, double fromCurrencyAmount)
        {
            string responseContent = getResponseString($"exchangerates_data/convert?to={toCurrency}&from={fromCurrency}&amount={fromCurrencyAmount}");

            Dictionary<string, object> responseData = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);

            if ((bool)responseData["success"])
            {
                return (double)responseData["result"];
            }
            else
            {
                return -1;
            }

        }

        private string getResponseString(string relativeURI)
        {
            var client = new RestSharp.RestClient($"https://api.apilayer.com/");

            var request = new RestRequest(relativeURI);
            request.AddHeader("apikey", "q1jQ0Xgh25ZpcCdKo54jquMemJLXKvn0");

            RestResponse response = client.Execute(request);
            return response.Content;

        }
    }
}
