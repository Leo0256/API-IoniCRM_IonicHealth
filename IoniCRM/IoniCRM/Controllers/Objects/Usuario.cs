
using Newtonsoft.Json;

namespace IoniCRM.Controllers.Objects
{
    public class Usuario
    {
        [JsonProperty]
        private int pk_usuario;
        [JsonProperty]
        private int nivel;

        public string nome;
        private string email;
        public string img;
        public string cargo;

        public string theme = string.Empty;
        
        public Usuario(int pk_usuario, int nivel, string theme)
        {
            this.pk_usuario = pk_usuario;
            this.nivel = nivel;
            this.theme = theme;
        }

        public Usuario()
        {
            pk_usuario = 0;
            nivel = 0;
            nome = string.Empty;
            email = string.Empty;
            img = "https://raw.githubusercontent.com/Leo0256/API-IoniCRM_IonicHealth/sistema/IoniCRM/IoniCRM/wwwroot/images/logo-icon-1.png";
            cargo = string.Empty;
            theme = "dark";
        }

        [JsonConstructor]
        public Usuario(int pk_usuario, int nivel, string nome, string email, string img, string cargo, string theme)
        {
            this.pk_usuario = pk_usuario;
            this.nivel = nivel;
            this.nome = nome;
            this.email = email;
            this.img = img;
            this.cargo = cargo;
            this.theme = theme;
        }

        public int GetPk_Usuario() => pk_usuario;
        public int GetNivel() => nivel;
        public string GetEmail() => email;
    }
}
