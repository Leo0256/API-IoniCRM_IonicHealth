using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Text.Json;

namespace IoniCRM.Models
{
    public class Session : PageModel
    {
        public const string SessionKeyName = "_Usuario";
        public const string SessionKeyPermission = "_Permission";
        

        public void OnGet()
        {
            // testes
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyName)))
            {
                HttpContext.Session.SetInt32(SessionKeyName, 42);
                HttpContext.Session.SetInt32(SessionKeyPermission, 1);
            }

            var name = HttpContext.Session.GetString(SessionKeyName);
            var permission = HttpContext.Session.GetInt32(SessionKeyPermission);
        }
    }

    public static class SessionValues
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }

        public static int? GetPermission(this ISession session)
        {
            var value = session.GetInt32(Session.SessionKeyPermission);
            return value == null ? 0 : session.GetInt32(Session.SessionKeyPermission);
        }
    }
}
