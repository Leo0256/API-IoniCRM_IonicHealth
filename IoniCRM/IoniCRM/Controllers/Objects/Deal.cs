using System;

namespace IoniCRM.Controllers.Objects
{
    public class Deal
    {
        private int pk_deal;
        public string nome;
        public int estagio;
        public int d_status;
        public double valor;

        public DateTime? abertura;
        public DateTime? fechamento;
        public int probabilidade;
        public string descr;

        public Cliente cliente;

        public Deal()
        {
            pk_deal = 0;
            nome = string.Empty;
            estagio = 0;
            d_status = 0;
            valor = 0.0;
            abertura = DateTime.Now;
            fechamento = DateTime.Now;
            probabilidade = 0;
            descr = string.Empty;
            cliente = new();
        }

        public Deal(int pk_deal, string nome, int estagio, int d_status, double valor, 
            DateTime? abertura, DateTime? fechamento, int prob, string descr, Cliente cliente)
        {
            this.pk_deal = pk_deal;
            this.nome = nome;
            this.estagio = estagio;
            this.d_status = d_status;
            this.valor = valor;
            this.abertura = abertura;
            this.fechamento = fechamento;
            probabilidade = prob;
            this.descr = descr;
            this.cliente = cliente;
        }

        public int getId() => pk_deal;
    }
}
