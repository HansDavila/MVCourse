using CapaEntidad;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CapaPresentacionAdmin.Controllers
{
    public class AccesoController : Controller
    {
        // GET: Acceso
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string correo, string clave)
        {
            Usuario oUsuario = new Usuario();

            oUsuario = new CN_Usuarios().Lista().Where(u=> u.Correo == correo && u.Clave == CN_Recursos.ConvertirSha256(clave)).FirstOrDefault();

            if(oUsuario == null)
            {
                //Solo comparte la informacion con la misma vista
                ViewBag.Error = "correo o contraseña no correcta";
                return View();
            }
            else
            {

                if (oUsuario.Reestablecer)
                {
                    //Con TempData podemos almacenar valores de forma temporal y usarla en otras vistas, en este caso en la de cambiarClave
                    TempData["IdUsuario"] = oUsuario.IdUsuario;
                    return RedirectToAction("CambiarClave");
                }
                ViewBag.Error = null;
                return RedirectToAction("Index", "Home");
            }



         
        }

        [HttpGet]
        public ActionResult CambiarClave()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CambiarClave(string idusuario, string claveactual, string nuevaclave, string confirmarclave)
        {
            
            Usuario oUsuario = new Usuario();
            oUsuario = new CN_Usuarios().Lista().Where(u => u.IdUsuario == int.Parse(idusuario)).FirstOrDefault();

            if(oUsuario.Clave != CN_Recursos.ConvertirSha256(claveactual))
            {
                TempData["IdUsuario"] = idusuario;
                ViewData["vclave"] = "";
                ViewBag.Error = "La contraseña actual no es correcta";
                return View();
            }
            else if(nuevaclave != confirmarclave)
            {
                TempData["IdUsuario"] = idusuario;
                ViewData["vclave"] = claveactual;
                ViewBag.Error = "Las constraseñas no coinciden";
                return View();
            }

            ViewData["vclave"] = "";

            nuevaclave = CN_Recursos.ConvertirSha256(nuevaclave);

            string mensaje = string.Empty;

            bool respuesta = new CN_Usuarios().CambiarClave(int.Parse(idusuario), nuevaclave, out mensaje);

            if(respuesta)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["IdUsuario"] = idusuario;
                ViewBag.Error = mensaje;
                return View();
            }

            return View();
        }

        public ActionResult Reestablecer()
        {
            return View();
        }

        
    }
}