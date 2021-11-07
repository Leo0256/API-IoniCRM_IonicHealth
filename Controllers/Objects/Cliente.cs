using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace IoniCRM.Controllers.Objects
{
    public class Cliente : EqualityComparer<Cliente>
    {
        private int pk_cliente;
        private string emp;
        public string nome;
        public string cpf_cnpj;
        private string crm;
        public string img;
        public string razaoSocial;
        public string categoria;
        public string descr;

        public string[] websites;
        public string[] enderecos;
        
        public List<string[]> contatos;
        public List<Cliente> funcionarios;

        public Cliente()
        {
            pk_cliente = 0;
            emp = string.Empty;
            nome = string.Empty;
        }

        public Cliente(int pk_cliente, string emp, string nome, string img) 
        {
            this.pk_cliente = pk_cliente;
            this.emp = emp;
            this.nome = nome;
            this.img = img;
            funcionarios = new();
        }

        public Cliente(
            int pk_cliente, string emp, string nome, string cpf_cnpj, string crm, string img, string razaoSocial, 
            string categoria, string descr, string[] websites, string[] enderecos, List<string[]> contatos
            )
        {
            this.pk_cliente = pk_cliente;
            this.emp = emp;
            this.nome = nome;
            this.cpf_cnpj = cpf_cnpj;
            this.crm = crm;
            this.img = img;
            this.razaoSocial = razaoSocial;
            this.categoria = categoria;
            this.descr = descr;
            this.websites = websites;
            this.enderecos = enderecos;
            this.contatos = contatos;
            funcionarios = new();
        }

        public int GetPk_Cliente() => pk_cliente;
        public void SetPk_Cliente(int pk_cliente) => this.pk_cliente = pk_cliente;

        public string GetEmp() => emp;
        public void SetEmp(string emp) => this.emp = emp;

        public void SetCRM(string crm) => this.crm = crm;
        public string GetCRM() => crm;

        public void AddFuncionario(Cliente cliente) => funcionarios.Add(cliente);

        public override bool Equals(Cliente x, Cliente y) =>
            x.pk_cliente == y.pk_cliente;

        public override int GetHashCode([DisallowNull] Cliente obj) =>
            throw new NotImplementedException();
        
    }
}
