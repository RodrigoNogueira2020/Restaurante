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
            var resposta = await _httpClient.GetAsync("produto");

            resposta.EnsureSuccessStatusCode();

            var conteudo = await resposta.Content.ReadAsStringAsync();

            List<Produto> produtos = new List<Produto>();

            if (resposta.Content.Headers.ContentType != null &&
            resposta.Content.Headers.ContentType.MediaType == "application/json")
            {
                var opcoes = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                produtos = JsonSerializer.Deserialize<List<Produto>>(conteudo, opcoes);
            }
            return View(produtos);
        }

        /*public async Task<IActionResult> AdicionarProduto()
        {
            HttpContent content = new StringContent();

            var resposta = await _httpClient.PostAsync("produto", content);

            resposta.EnsureSuccessStatusCode();

            var conteudo = await resposta.Content.ReadAsStringAsync();

            List<Produto> produtos = new List<Produto>();

            if (resposta.Content.Headers.ContentType != null &&
            resposta.Content.Headers.ContentType.MediaType == "application/json")
            {
                var opcoes = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                produtos = JsonSerializer.Deserialize<List<Produto>>(conteudo, opcoes);
            }
            return View(produtos);
        }*/

        public async Task<IActionResult> ListarEncomendas()
        {
            var resposta = await _httpClient.GetAsync("encomenda");

            resposta.EnsureSuccessStatusCode();

            var conteudo = await resposta.Content.ReadAsStringAsync();

            List<Encomenda> encomendas = new List<Encomenda>();

            if (resposta.Content.Headers.ContentType != null &&
            resposta.Content.Headers.ContentType.MediaType == "application/json")
            {
                var opcoes = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }
                ;
                encomendas = JsonSerializer.Deserialize<List<Encomenda>>(conteudo, opcoes);
            }
            return View(encomendas);
        }

        public async Task<IActionResult> ListarEstafetas()
        {
            var resposta = await _httpClient.GetAsync("estafeta");

            resposta.EnsureSuccessStatusCode();

            var conteudo = await resposta.Content.ReadAsStringAsync();

            List<Estafeta> estafetas = new List<Estafeta>();

            if (resposta.Content.Headers.ContentType != null &&
            resposta.Content.Headers.ContentType.MediaType == "application/json")
            {
                var opcoes = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }
                ;
                estafetas = JsonSerializer.Deserialize<List<Estafeta>>(conteudo, opcoes);
            }
            return View(estafetas);
        }

        public async Task<IActionResult> ListarItens()
        {
            var resposta = await _httpClient.GetAsync("item");

            resposta.EnsureSuccessStatusCode();

            var conteudo = await resposta.Content.ReadAsStringAsync();

            List<Item> itens = new List<Item>();

            if (resposta.Content.Headers.ContentType != null &&
            resposta.Content.Headers.ContentType.MediaType == "application/json")
            {
                var opcoes = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }
                ;
                itens = JsonSerializer.Deserialize<List<Item>>(conteudo, opcoes);
            }
            return View(itens);
        }

        public async Task<IActionResult> ListarPedidos()
        {
            var resposta = await _httpClient.GetAsync("pedido");

            resposta.EnsureSuccessStatusCode();

            var conteudo = await resposta.Content.ReadAsStringAsync();

            List<Pedido> pedidos = new List<Pedido>();

            if (resposta.Content.Headers.ContentType != null &&
            resposta.Content.Headers.ContentType.MediaType == "application/json")
            {
                var opcoes = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }
                ;
                pedidos = JsonSerializer.Deserialize<List<Pedido>>(conteudo, opcoes);
            }
            return View(pedidos);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
