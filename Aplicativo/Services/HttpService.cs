using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aplicativo.Services
{
    public class HttpService 
    {
        private readonly HttpClient _httpClient;
  
        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }


        public async Task<bool> CheckHash(string hash)
        {
            var response = await _httpClient.PutAsync($"validacao/validar-hash?hash={hash}", null);
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
