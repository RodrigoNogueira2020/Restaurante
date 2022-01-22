using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestauranteAPI.QueryModels
{
    public class Parametros
    {
        private int tamanho = 10;
        private const int TAMANHO_MAXIMO = 100;

        public int Pagina { get; set; }
        public int Tamanho {
            get { return tamanho; }
            set 
            {
                if (value > 0 && value < TAMANHO_MAXIMO)
                    tamanho = value;
                else
                    tamanho = 1;
            }
        }
    }
}
