using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SimpleKeyMan
{
    public static class ConfiguredHTTPClient
    {
        public static HttpClient _client {get; private set;}
        public static string _apiKey;

        public static void setupClinet(string DataServer, string key)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(DataServer);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _client.Timeout = new TimeSpan(0,0,30);
            _apiKey = key;
        }

        public static void setAPIKey(string key)
        {
            _apiKey = key;
        }
        public static void setHttpClientBaseURI(string uri)
        {
            _client.BaseAddress = new Uri(uri);
        }



        public static string ListSpaces(string key)
        {
            string value;
            try
            {
                var query = QueryBuilder("spaces","");
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Program.Base64Encode(key));
                value =_client.GetAsync(query).Result.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                value = e.Message;
            }
           return value;
        }

        public static string NewKey(string email, List<string> spaces)
        {
            string value;
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Program.Base64Encode(_apiKey));
                var query = QueryBuilder("new","email="+email+ListString("spaces", spaces));
                value = _client.GetAsync(query).Result.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                value = e.Message;
            }
            return value;
        }

        public static string UpdateKey(string email, List<string> spaces)
        {
            string value;
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Program.Base64Encode(_apiKey));
                var query = QueryBuilder("changeSpaces","email="+email+ListString("spaces", spaces));
                value = _client.GetAsync(query).Result.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                value = e.Message;
            }
            return value;
        }
        private static string ListString(string pramName, List<string> prams)
        {
            string output="";
            prams.ForEach(p => output+="&"+pramName+"="+p);
            return output;
        }
        public static string CanWrite(string key, string space)
        {
            string value;
            try
            {
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",Program.Base64Encode(key));
                value = _client.GetAsync(QueryBuilder("canWrite","space="+space)).Result.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                value = e.Message;
            }
           return value;
        }
        private static string QueryBuilder(string endpoint, string prms)
        {
            return endpoint+"?"+prms;
        }
        



    }
}