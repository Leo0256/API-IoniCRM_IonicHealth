using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoniCRM.Controllers.Objects
{
    public class Pipeline
    {
        private int pk_pipeline;
        public string nome;
        public int prioridade;
        public string descr;

        public List<Deal> deals;

        // remover depois?
        private Usuario usuario;

        public Pipeline() 
        {
            pk_pipeline = 0;
            nome = string.Empty;
            prioridade = 1;
            descr = string.Empty;
        }

        public Pipeline(int pk_pipeline, string nome, int prioridade, 
            string descr)
        {
            this.pk_pipeline = pk_pipeline;
            this.nome = nome;
            this.prioridade = prioridade;
            this.descr = descr;
        }

        public int GetId() => pk_pipeline;

        public int GetTotalDeals(int estagio)
        {
            int total = 0;
            foreach(Deal deal in deals)
            {
                if (deal.estagio == estagio)
                    total++;
            }
            return total;
        }
        public double GetTotalValor(int estagio)
        {
            double valor = 0;
            foreach(Deal deal in deals)
            {
                if(deal.estagio == estagio)
                    valor += deal.valor;
            }
            return valor;
        }

        public int[] GetListTotalDeals()
        {
            int[] total = new int[] { 0, 0, 0, 0, 0};
            for(int estagio = 0; estagio < 5; estagio++)
                foreach(Deal deal in deals)
                {
                    if (deal.estagio == estagio)
                        total[estagio] += 1;
                }

            return total;
        }

        public double[] GetListTotalValor()
        {
            double[] valor = new double[] { 0, 0, 0, 0, 0 };
            for (int estagio = 0; estagio < 5; estagio++)
                foreach (Deal deal in deals)
                {
                    if (deal.estagio == estagio)
                        valor[estagio] += deal.valor;
                }

            return valor;
        }
    }
}
