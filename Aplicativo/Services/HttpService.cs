using Aplicativo.Resources.Scaffolding;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace Aplicativo.Services
{
    public class HttpService 
    {
        private readonly HttpClient _httpClient;
  
        public HttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
            //se demorar mais que 10 segundos, o request é cancelado e uma exceção é lançada.
        }


        public async Task<QRCodePayload> CheckHash(string hash)
        {
            var response = await _httpClient.PutAsync($"validacao/validar-hash?hash={hash}", null);
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadFromJsonAsync<QRCodePayload>();
                result.HasFailed = false;
                return result;
            }
            else
            {
                return new QRCodePayload(true);
            }
        }
        public async Task<List<QRCodePayload?>> GetHistory()
        {
            var response = await _httpClient.GetAsync("validacao/historico");
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var result = await response.Content.ReadFromJsonAsync<List<QRCodePayload>>();
                result.ForEach(x => x.HasFailed = false);
                return result;
            }
            else
            {
                return null;
            }
        }


    }
}
