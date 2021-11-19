using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

using IoniCRM.Controllers.Objects;

namespace IoniCRM.Models
{
    public class SessionValues : PageModel
    {
        public const string SessionKeyUser = "_Usuario";
        public const string SessionKeyPermission = "_Permission";
    }

    public static class Session
    {
        public static bool Empty(this ISession session)
        {
            return session.GetString(SessionValues.SessionKeyUser) == default;
        }

        public static void SetUser(this ISession session, Usuario usuario)
        {
            session.SetString(SessionValues.SessionKeyUser, JsonConvert.SerializeObject(usuario));
            session.SetInt32(SessionValues.SessionKeyPermission, usuario.GetNivel());
        }

        public static Usuario GetUsuario(this ISession session)
        {
            return JsonConvert.DeserializeObject<Usuario>(session.GetString(SessionValues.SessionKeyUser));
        }

        public static int? GetPermission(this ISession session)
        {
            var value = session.GetInt32(SessionValues.SessionKeyPermission);
            return value == null ? 0 : session.GetInt32(SessionValues.SessionKeyPermission);
        }
    }
}
