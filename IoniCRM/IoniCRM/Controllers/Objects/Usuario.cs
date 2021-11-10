using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoniCRM.Controllers.Objects
{
    public class Usuario
    {
        private int pk_usuario;
        private int nivel;
        public string nome;
        private string email;
        public string img;
        public string cargo;

        public Usuario(int pk_usuario, int nivel)
        {
            this.pk_usuario = pk_usuario;
            this.nivel = nivel;
        }

        public Usuario(int pk_usuario, int nivel, string nome, string email, string img, string cargo)
        {
            this.pk_usuario = pk_usuario;
            this.nivel = nivel;
            this.nome = nome;
            this.email = email;
            this.cargo = cargo;
        }

        public void SetPk_Usuario(int pk_usuario) => this.pk_usuario = pk_usuario;
        public int GetPk_Usuario() => this.pk_usuario;

        public void SetNivel(int nivel) => this.nivel = nivel;
        public int GetNivel() => this.nivel;

        public void SetNome(string nome) => this.nome = nome;
        public string GetNome() => this.nome;

        public void SetEmail(string email) => this.email = email;
        public string GetEmail() => this.email;

        public void SetSenha(string senha)
        {
            //Falta um meio de gerar um código hash para a senha
        }

        public void SetCargo(string cargo) => this.cargo = cargo;
        public string GetCargo() => this.cargo;
    }
}
