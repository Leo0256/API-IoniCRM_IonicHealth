using IoniCRM.Models;
using IoniCRM.Controllers.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System;

namespace IoniCRM.Controllers
{
    public class EmailController : Controller
    {
        private PostgreSQLConnection pgsqlcon;

        public EmailController()
        {
            pgsqlcon = new();
        }

        public IActionResult Email()
        {
            if (Session.Empty(HttpContext.Session))
                return RedirectToAction("Login", "Login");

            ViewBag.Usuario = Session.GetUsuario(HttpContext.Session);
            return View();
        }

        public IActionResult SendEmail(string receiver, string subject, string message)
        {
            try
            {
                MailAddress senderEmail = new("senderEmail", "senderName");
                MailAddress receiverEmail = new(receiver, "receiverName");
                string pass = "password";

                SmtpClient smtp = new()
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail.Address, pass)
                };

                using MailMessage mailMessage = new(senderEmail, receiverEmail)
                {
                    Subject = subject,
                    Body = message
                };
                smtp.Send(mailMessage);
            }
            catch (Exception)
            {
                ViewBag.Erro = "Erro ao enviar o e-mail.";
            }

            return RedirectToAction("Clientes", "Listagem");
        }
    }
}
