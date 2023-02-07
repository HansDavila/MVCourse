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


        public ActionResult CambiarClave()
        {
            return View();
        }

        public ActionResult Reestablecer()
        {
            return View();
        }

        
    }
}