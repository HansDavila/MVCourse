using CapaEntidad;
using CapaNegocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace CapaPresentacionTIenda.Controllers
{
    public class AccesoController : Controller
    {
        // GET: Acceso
        public ActionResult Indexes()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Indexes(string correo, string clave)
        {
            Cliente oCliente = null;

            oCliente = new CN_Cliente().Lista().Where(item => item.Correo == correo && item.Clave == CN_Recursos.ConvertirSha256(clave)).FirstOrDefault();

            //Si no se encontro un cliente...
            if(oCliente == null)
            {
                ViewBag.Error = "Correo o Contraseña no son correctas";
                return View();
            }
            else
            {
                //Si reestablecer esta activo redirreccionar a otra vista donde pondra su nueva ocntraseña
                if (oCliente.Reestablecer)
                {
                    TempData["IdCliente"] = oCliente.IdCliente;
                    return RedirectToAction("CambiarClave", "Acceso");
                }
                else
                {
                    //Crear una autenticacion con el correo
                    FormsAuthentication.SetAuthCookie(oCliente.Correo, false);
                    Session["Cliente"] = oCliente;

                    ViewBag.Error = null;
                    return RedirectToAction("index", "Tienda");
                }
            }
            
        }

        [HttpGet]
        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrar(Cliente objeto)
        {
            int resultado;
            string mensaje = string.Empty;

            //Si el Campo es nulo recibir un string vacio, si no obtener la propeidad del objeto
            ViewData["Nombres"] = string.IsNullOrEmpty(objeto.Nombres) ? "" : objeto.Nombres;
            ViewData["Apellidos"] = string.IsNullOrEmpty(objeto.Apellidos) ? "" : objeto.Apellidos;
            ViewData["Correo"] = string.IsNullOrEmpty(objeto.Correo) ? "" : objeto.Correo;

            if(objeto.Clave != objeto.ConfirmarClave)
            {
                ViewBag.Error = "Las constraseñas no coinciden";
                return View();
            }

            resultado = new CN_Cliente().Registrar(objeto, out mensaje);

            if(resultado > 0)
            {
                ViewBag.Error = null;
                return RedirectToAction("Indexes", "Acceso");
            }
            else
            {
                ViewBag.Error = mensaje;
                return View();
            }

            
        }

        public ActionResult Reestablecer()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Reestablecer(string correo)
        {
            Cliente oCliente = new Cliente();

            oCliente = new CN_Cliente().Lista().Where(item => item.Correo == correo).FirstOrDefault();

            //si no encontro usuario...
            if (oCliente == null)
            {
                ViewBag.Error = "No se encontro un usuario relacionado a ese correo";
                return View();
            }


            string mensaje = string.Empty;
            bool respuesta = new CN_Cliente().ReestablecerClave(oCliente.IdCliente, correo, out mensaje);

            if (respuesta)
            {
                ViewBag.Error = null;
                return RedirectToAction("Indexes");
            }
            else
            {
                ViewBag.Error = mensaje;
                return View();
            }
        }

        public ActionResult CambiarClave()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CambiarClave(string idcliente, string claveactual, string nuevaclave, string confirmaclave)
        {
            Cliente oCliente = new Cliente();
            oCliente = new CN_Cliente().Lista().Where(u => u.IdCliente == int.Parse(idcliente)).FirstOrDefault();

            if (oCliente.Clave != CN_Recursos.ConvertirSha256(claveactual))
            {
                TempData["IdCliente"] = idcliente;
                ViewData["vclave"] = "";
                ViewBag.Error = "La contraseña actual no es correcta";
                return View();
            }
            else if (nuevaclave != confirmaclave)
            {
                TempData["IdUsuario"] = idcliente;
                ViewData["vclave"] = claveactual;
                ViewBag.Error = "Las constraseñas no coinciden";
                return View();
            }

            ViewData["vclave"] = "";

            nuevaclave = CN_Recursos.ConvertirSha256(nuevaclave);

            string mensaje = string.Empty;

            bool respuesta = new CN_Cliente().CambiarClave(int.Parse(idcliente), nuevaclave, out mensaje);

            if (respuesta)
            {
                return RedirectToAction("Indexes");
            }
            else
            {
                TempData["IdUsuario"] = idcliente;
                ViewBag.Error = mensaje;
                return View();
            }
            
        }

        public ActionResult CerrarSesion()
        {
            //Se cierre la cesion del usuario (La que se abrio desde el index)
            Session["Cliente"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Indexes");
        }


    }
}