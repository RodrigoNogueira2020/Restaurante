using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Restaurante.ClienteMVC.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Restaurante.ClienteMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private static HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:2009/");
            _httpClient.Timeout = new TimeSpan(0, 0, 30);
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> ListarProdutos()
        {
            // Efetua um request Get assíncrono
            var resposta = await _httpClient.GetAsync("produto");

            // Caso o resquest não seja bem sucedido, é lançada uma exceção
            resposta.EnsureSuccessStatusCode();
            // Obtém a resposta em formato de texto
            var conteudo = await resposta.Content.ReadAsStringAsync();

            List<Produto> produtos = new List<Produto>();

            // Verificar se a resposta foi em JSON
            if (resposta.Content.Headers.ContentType != null &&
            resposta.Content.Headers.ContentType.MediaType == "application/json")
            {
                var opcoes = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                // Deserializar JSON para uma lista de objetos da classe Produto
                produtos = JsonSerializer.Deserialize<List<Produto>>(conteudo, opcoes);
            }

            // Envia a lista para view
            return View(produtos);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
