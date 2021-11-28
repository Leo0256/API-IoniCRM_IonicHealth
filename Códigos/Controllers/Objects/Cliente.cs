using System;
using System.Collections.Generic;

namespace IoniCRM.Controllers.Objects
{
    public class Cliente
    {
        private int pk_cliente;
        private string emp;
        public string nome;
        private string cpf_cnpj;
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
            cpf_cnpj = string.Empty;
            crm = string.Empty;
            img = "https://raw.githubusercontent.com/Leo0256/API-IoniCRM_IonicHealth/sistema/IoniCRM/IoniCRM/wwwroot/images/logo-icon-1.png";
            razaoSocial = string.Empty;
            categoria = string.Empty;
            descr = string.Empty;
            websites = Array.Empty<string>();
            enderecos = Array.Empty<string>();
            contatos = new();
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
        public string GetCpfCnpj() => cpf_cnpj;
        public string GetEmp() => emp;
        public string GetCRM() => crm;
        public void AddFuncionario(Cliente cliente) => funcionarios.Add(cliente);
    }
}
