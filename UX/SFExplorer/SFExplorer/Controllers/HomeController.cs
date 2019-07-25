using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SFExplorer.Models;

namespace SFExplorer.Controllers
{
    public class HomeController : Controller
    {
        private const string AppName = "TestSfApp";
        private const string ServiceName  = "TestStatefulService";
        private const string PartitionKey = "1";
        private const string baseUrl = "http://sonal-desktop:8954";
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> StatefulService()
        {
            List<Collection> collections = new List<Collection>();
            string data = await GetStatefulServiceData(AppName, ServiceName);
            if (!string.IsNullOrEmpty(data))
            {
                StatefulServiceData statefulServiceData = JsonConvert.DeserializeObject<StatefulServiceData>(data);
                collections = statefulServiceData.Collections;
            }

            foreach (var collection in collections)
            {
                if (collection.Type.Contains("DistributedDictionary"))
                {
                    collection.Url = string.Format("https://localhost:44399/Home/ReliableDictionary?dictionaryName={0}",
                        collection.Name);
                }
            }

            StatefulServiceViewModel model = new StatefulServiceViewModel()
            {
                AppName = AppName,
                ServiceName = ServiceName,
                Collections = collections
            };

            return View(model);
        }

        public IActionResult Actors()
        {
            ViewData["Message"] = "Actor data show up here";

            return View();
        }

        public async Task<ActionResult> ReliableDictionary(string dictionaryName)
        {
            List<string> keyValuePairs = new List<string>();
            string data = await GetReliableDictionaryData(AppName, ServiceName, dictionaryName);
            if (!string.IsNullOrEmpty(data))
            {
                ReliableDictionaryData reliableDictionaryData = JsonConvert.DeserializeObject<ReliableDictionaryData>(data);
                keyValuePairs = reliableDictionaryData.KeyValuePairs;
            }

            ReliableDictionaryViewModel model = new ReliableDictionaryViewModel()
            {
                AppName = AppName,
                ServiceName = ServiceName,
                CollectionName = dictionaryName,
                KeyValuePairs = keyValuePairs
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<string> GetStatefulServiceData(string appName, string serviceName)
        {
            string responseContent = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                string requestUri = string.Format("{0}/api/statefulsvcreliablestate/{1}/{2}/{3}", baseUrl, appName, serviceName, PartitionKey);
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                HttpResponseMessage response = null;
                try
                {
                    response = await client.SendAsync(requestMessage);
                }
                catch (Exception e)
                {
                    return responseContent;
                }

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (response.Content != null)
                    {
                        responseContent = await response.Content.ReadAsStringAsync();
                    }
                }

                if (!string.IsNullOrEmpty(responseContent) && !responseContent.Equals("[]"))
                {
                    responseContent = responseContent.Substring(2, responseContent.Length - 4);
                    responseContent = responseContent.Replace("\\", "");
                    responseContent = responseContent.Replace("\",\"", ",");
                    responseContent = string.Format("\"Collections\":[{0}]", responseContent);
                    responseContent = "{" + responseContent + "}";
                }
                return responseContent;
            }
        }

        private async Task<string> GetReliableDictionaryData(string appName, string serviceName, string dictionaryName)
        {
            string responseContent = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                string requestUri = string.Format("{0}/api/statefulsvcreliablestate/{1}/{2}/{3}/{4}", baseUrl, appName, serviceName, PartitionKey, dictionaryName);
                var requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
                requestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = null;
                try
                {
                    response = await client.SendAsync(requestMessage);
                }
                catch (Exception e)
                {
                    return responseContent;
                }

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (response.Content != null)
                    {
                        responseContent = await response.Content.ReadAsStringAsync();
                    }
                }

                if (!string.IsNullOrEmpty(responseContent) && !responseContent.Equals("[]"))
                {
                    responseContent = string.Format("\"keyvaluepairs\":{0}", responseContent);
                    responseContent = "{" + responseContent + "}";
                }

                return responseContent;
            }
        }
    }
}
