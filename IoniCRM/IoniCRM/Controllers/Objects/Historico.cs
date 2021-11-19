using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoniCRM.Controllers.Objects
{
    public class Historico
    {
        private int pk_h;
        private int fk_usuario;
        public DateTime? data;
        public string descr;

        public Historico(int pk_h, int fk_usuario, DateTime? data, string descr)
        {
            this.pk_h = pk_h;
            this.fk_usuario = fk_usuario;
            this.data = data;
            this.descr = descr;
        }

        public int GetId() => pk_h;
        public int GetIdUsuario() => fk_usuario;
    }
}
