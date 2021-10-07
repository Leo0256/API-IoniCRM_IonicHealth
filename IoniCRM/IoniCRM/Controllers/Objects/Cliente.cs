using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace IoniCRM.Controllers
{
    public class Cliente : EqualityComparer<Cliente>
    {
        private int pk_cliente;
        private string emp;
        public string nome;
        public string cpf_cnpj;
        public string razaoSocial;
        public string categoria;
        public string descr;

        public List<string[]> info;
        public List<string[]> contato;

        public List<Cliente> funcionarios;

        public Cliente(int pk_cliente, string emp, string nome) 
        {
            this.pk_cliente = pk_cliente;
            this.emp = emp;
            this.nome = nome;
            funcionarios = new();
        }

        public Cliente(
            int pk_cliente, string emp, string nome, string cpf_cnpj, string razaoSocial, 
            string categoria, string descr, List<string[]> info, List<string[]> contato
            )
        {
            this.pk_cliente = pk_cliente;
            this.emp = emp;
            this.nome = nome;
            this.cpf_cnpj = cpf_cnpj;
            this.razaoSocial = razaoSocial;
            this.categoria = categoria;
            this.descr = descr;
            this.info = info;
            this.contato = contato;
            funcionarios = new();
        }

        public int GetPk_Cliente() => pk_cliente;
        public void SetPk_Cliente(int pk_cliente) => this.pk_cliente = pk_cliente;

        public string GetEmp() => emp;
        public void SetEmp(string emp) => this.emp = emp;

        public void AddFuncionario(Cliente cliente) => funcionarios.Add(cliente);

        

        public override bool Equals(Cliente x, Cliente y) =>
            x.pk_cliente == y.pk_cliente;

        public override int GetHashCode([DisallowNull] Cliente obj) =>
            throw new NotImplementedException();
        
    }
}
