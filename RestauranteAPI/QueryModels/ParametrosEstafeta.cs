using RestauranteAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestauranteAPI.QueryModels
{
    public class ParametrosEstafeta : Parametros
    {
        public int? Id { get; set; }
        public string Nome { get; set; }
        public bool? Disponivel { get; set; }
    }
}
