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

        public Deal(int pk_deal, string nome, int estagio, int d_status, 
            double valor, DateTime abertura, DateTime fechamento, int prob, string descr, Cliente cliente)
        {
            this.pk_deal = pk_deal;
            this.nome = nome;
            this.estagio = estagio;
            this.d_status = d_status;
            this.valor = valor;
            this.abertura = abertura;
            this.fechamento = fechamento;
            this.probabilidade = prob;
            this.descr = descr;
            this.cliente = cliente;
        }

        public int getId() => pk_deal;
    }
}
