using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Recursos
    {
        //Metodo que genera una clave aleatoria de solo 6 digitos
        public static string GenerarClave()
        {
            //La N significa puro caracter alfanumericos
            string clave = Guid.NewGuid().ToString("N").Substring(0,6);
            return clave;
        }

        //Encriptacion de TEXTO en SHA256
        public static string ConvertirSha256(string texto)
        {
            StringBuilder Sb = new StringBuilder();

            //USAR LA REFERENCIA DE "System.Security.Cryptography
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach(byte b in result)
                {
                    Sb.Append(b.ToString("x2"));
                }

                return Sb.ToString();
            }
        }

        public static bool EnviarCorreo(string correo, string asunto, string mensaje)
        {
            bool resultado = false;

            try
            {
                MailMessage mail = new MailMessage();
                mail.To.Add(correo);
                mail.From = new MailAddress("deluxen123@gmail.com");
                mail.Subject = asunto;
                mail.Body = mensaje;
                mail.IsBodyHtml = true;

                //Configurando el servidor cliente que mandara el mail de arriba
                var smtp = new SmtpClient()
                {
                    Credentials = new NetworkCredential("deluxen123@gmail.com", "xjiaseigvekibmiu"),
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl= true
                };

                smtp.Send(mail);
                resultado = true;
            }catch(Exception ex)
            {
                resultado = false;
            }

            return resultado;
        }


        //Metodo que convierte la imagen a base65
        public static string ConvertirBase64(string ruta, out bool conversion)
        {
            string textoBase64 = string.Empty;
            conversion = true;

            try
            {
                byte[] bytes = File.ReadAllBytes(ruta);
                textoBase64 = Convert.ToBase64String(bytes);

            }catch(Exception ex)
            {
                conversion = false;
            }

            return textoBase64;
        }
    }
}
