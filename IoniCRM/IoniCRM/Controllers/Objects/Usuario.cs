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
        public String nome;
        private String email;
        private String hash_senha;
        public String cargo;

        public Usuario(int pk_usuario, int nivel, string nome, string email, string hash_senha, string cargo)
        {
            this.pk_usuario = pk_usuario;
            this.nivel = nivel;
            this.nome = nome;
            this.email = email;
            this.hash_senha = hash_senha;
            this.cargo = cargo;
        }

        public void SetPk_Usuario(int pk_usuario) => this.pk_usuario = pk_usuario;
        public int GetPk_Usuario() => this.pk_usuario;

        public void SetNivel(int nivel) => this.nivel = nivel;
        public int GetNivel() => this.nivel;

        public void SetNome(String nome) => this.nome = nome;
        public String GetNome() => this.nome;

        public void SetEmail(String email) => this.email = email;
        public String GetEmail() => this.email;

        public void SetSenha(String senha)
        {
            //Falta um meio de gerar um código hash para a senha
            this.hash_senha = senha;
        }
        public String GetHash_Senha() => this.hash_senha;

        public void SetCargo(String cargo) => this.cargo = cargo;
        public String GetCargo() => this.cargo;
    }
}
