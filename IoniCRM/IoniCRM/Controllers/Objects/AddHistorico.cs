using IoniCRM.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;

namespace IoniCRM.Controllers.Objects
{
    public class AddHistorico
    {
        public AddHistorico(ISession session, string mensagem)
        {
            NewHistorico(session, mensagem);
        }

        public static void NewHistorico(ISession session, string mensagem)
        {
            PostgreSQLConnection con = new();
            Regex regex = new("[\"\']");
            JObject hist = JObject.Parse("{" +
                "\"pk_usuario\":" + Session.GetUsuario(session).GetPk_Usuario() + "," +
                "\"data\":\"" + DateTime.Now + "\"," +
                "\"descr\":\"" + regex.Replace(mensagem, "´") + "\"" +
                "}");

            _ = con.ExecuteCmdAsync(string.Format(@"select addHistorico('{0}')", hist.ToString()));
            
        }
    }
}
