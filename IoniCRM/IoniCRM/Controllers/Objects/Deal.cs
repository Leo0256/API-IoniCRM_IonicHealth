using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoniCRM.Controllers.Objects
{
    public class Deal
    {
        private int pk_deal;
        public string nome;
        public int estagio;
        public int d_status;
        public double valor;

        public DateTime abertura;
        public DateTime fechamento;
        public int probabilidade;
        public string descr;

        public Cliente cliente;

        public Deal(int pk_usuario, int nivel)
        {
            
        }

    }
}
