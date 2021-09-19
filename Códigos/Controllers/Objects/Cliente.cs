using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoniCRM.Controllers
{
    public class Cliente
    {
        private int pk_cliente;
        private int fk_emp;
        public String nome;
        public String apelido;
        public String cargo;
        public String descr;

        public Cliente(int pk_cliente, int fk_emp, string nome, string apelido, string cargo, string descr)
        {
            this.pk_cliente = pk_cliente;
            this.fk_emp = fk_emp;
            this.nome = nome;
            this.apelido = apelido;
            this.cargo = cargo;
            this.descr = descr;
        }

        public Cliente() { }

        public void SetPk_Cliente(int pk_cliente) => this.pk_cliente = pk_cliente;
        public int GetPk_Cliente() => this.pk_cliente;

        public void SetFk_Emp(int fk_emp) => this.fk_emp = fk_emp;
        public int GetFk_Emp() => this.fk_emp;

        public void SetNome(string nome) => this.nome = nome;
        public String GetNome() => this.nome;

        public void SetApelido(string apelido) => this.apelido = apelido;
        public String GetApelido() => this.apelido;

        public void SetCargo(string cargo) => this.cargo = cargo;
        public String GetCargo() => this.cargo;

        public void SetDescr(string descr) => this.descr = descr;
        public String GetDescr() => this.descr;
    }
}
