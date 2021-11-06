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
            nome = new(string.Empty);
            prioridade = 1;
            descr = new(string.Empty);
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
    }
}
