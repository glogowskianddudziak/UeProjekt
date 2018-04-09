using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.WebPages;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using UeProject.Helpers;
using UeProject.Models.Crypto;

namespace UeProject.Controllers
{
    [RoutePrefix("api/crypto")]
    public class CryptocurrencyController : ApiController
    {
        private readonly string apiUrl;
        HttpClient client;
        private readonly IExchangeLogic _exchangeLogic;

        public CryptocurrencyController(IExchangeLogic exchangeLogic)
        {
            _exchangeLogic = exchangeLogic;
            apiUrl = "https://www.worldcoinindex.com/apiservice/json?key=" +
                     ConfigurationManager.AppSettings["WorldCoinIndexApiKey"];
            client = new HttpClient { BaseAddress = new Uri(apiUrl) };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }


        /// <summary>
        /// Convert given amount of one cryptocurrency to another
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Incorrect cryptocurrency label provided. Check /api/coins/getall </response>
        /// <response code="500">Connection to WorldCoinIndex api failed</response>
        /// <param name="amount">Amount of cryptocurrency to convert</param>
        /// <param name="from">Cryptocurrency label to convert to from</param>
        /// <param name="to">Cryptocurrency label to convert to</param>
        [HttpGet]
        [SwaggerResponse(500, "Connection to WorldCoinIndex api failed", typeof(string))]
        [SwaggerResponse(400, "Incorrect cryptocurrency label provided. Check /api/coins/getall ", typeof(string))]
        [SwaggerResponse(200, "OK", typeof(decimal))]
        [Route("convert/{amount}/{from}/{to}")]
        public async Task<HttpResponseMessage> Convert(decimal amount, string from, string to)
        {
            HttpResponseMessage responseMessage = await client.GetAsync(apiUrl);
            if (responseMessage.IsSuccessStatusCode)
            {
                if(from == to)
                    return Request.CreateResponse(HttpStatusCode.OK, 1);

                var responseData = responseMessage.Content.ReadAsStringAsync().Result;

                var apiCoins = JsonConvert.DeserializeObject<AllCoins>(responseData).Markets.ToList();
                var coins = apiCoins.Where(c => c.Label.StartsWith(from.ToUpper()+"/") || c.Label.StartsWith(to.ToUpper() + "/"))
                    .ToList();

                if (coins.Count < 2)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Did not find cryptocurrencies with given labels");

                decimal result;
                try
                {
                    result = _exchangeLogic.ConvertCryptoCurrency(
                        coins.Where(c => c.Label.StartsWith(from.ToUpper() + "/")).Select(c => c.Price_usd)
                            .FirstOrDefault(),
                        coins.Where(c => c.Label.StartsWith(to.ToUpper() + "/")).Select(c => c.Price_usd)
                            .FirstOrDefault(), amount);
                }
                catch (OverflowException)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Operation result too long.");
                }

                return Request.CreateResponse(HttpStatusCode.OK, $"{result:0.#####}");
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Connection to WorldCoinIndex api failed");
        }


        /// <summary>
        /// Get all available coins
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [SwaggerResponse(500, "Internal server error", typeof(string))]
        [SwaggerResponse(200, "OK", typeof(List<CoinVm>))]
        [Route("coins/getall")]
        public async Task<HttpResponseMessage> GetAvailableCoins()
        {
            HttpResponseMessage responseMessage = await client.GetAsync(apiUrl);
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;

                var coins = JsonConvert.DeserializeObject<AllCoins>(responseData).Markets
                    .Select(c => new SimpleListModel() {Label = c.Label, Name = c.Name}).ToList();

                foreach (var coin in coins)
                    coin.Label = coin.Label.Remove(coin.Label.Length - 4);

                return Request.CreateResponse(HttpStatusCode.OK, coins);
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Connection to WorldCoinIndex api failed");
        }

        /// <summary>
        /// Convert given amount of one cryptocurrency to another
        /// </summary>
        /// <response code="200">OK</response>
        /// <response code="400">Incorrect cryptocurrency label provided. Check /api/coins/getall </response>
        /// <response code="500">Connection to WorldCoinIndex api failed</response>
        /// <param name="amount">Amount of cryptocurrency to convert</param>
        /// <param name="from">Cryptocurrency label to convert to USD</param>
        [HttpGet]
        [SwaggerResponse(500, "Connection to WorldCoinIndex api failed", typeof(string))]
        [SwaggerResponse(400, "Incorrect cryptocurrency label provided. Check /api/coins/getall ", typeof(string))]
        [SwaggerResponse(200, "OK", typeof(decimal))]
        [Route("converttousd/{amount}/{from}")]
        public async Task<HttpResponseMessage> ConvertToDollars(decimal amount, string from)
        {
            HttpResponseMessage responseMessage = await client.GetAsync(apiUrl);
            if (responseMessage.IsSuccessStatusCode)
            {
                var responseData = responseMessage.Content.ReadAsStringAsync().Result;

                var apiCoins = JsonConvert.DeserializeObject<AllCoins>(responseData).Markets.ToList();
                var coin = apiCoins.FirstOrDefault(c =>
                    c.Label.StartsWith(from.ToUpper() + "/"));

                if (coin == null)
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Did not find cryptocurrencies with given labels");

                decimal result;
                try
                {
                    result = _exchangeLogic.ConvertCryptoCurrencyToDollars(coin.Price_usd, amount);
                }
                catch (OverflowException)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, "Operation result too long.");
                }

                return Request.CreateResponse(HttpStatusCode.OK, $"{result:0.#####}");
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError, "Connection to WorldCoinIndex api failed");
        }
    }
}
