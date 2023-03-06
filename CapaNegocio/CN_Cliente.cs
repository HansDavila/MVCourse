using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Cliente
    {
        //Objeto de capa de datos que tien elos metodos que interactuan directamenete con la base de datos
        private CD_Cliente objCapaDato = new CD_Cliente();

        public int Registrar(Cliente obj, out string Mensaje)
        {
            //Mensaje pasado por referencia
            Mensaje = string.Empty;

            //Se verifica que no esten los campos vacios
            if (string.IsNullOrEmpty(obj.Nombres) || string.IsNullOrWhiteSpace(obj.Nombres))
            {
                Mensaje = "El nombre del cliente no puede ser vacio";
            }
            else if (string.IsNullOrEmpty(obj.Apellidos) || string.IsNullOrWhiteSpace(obj.Apellidos))
            {
                Mensaje = "El apellido del cliente no puede ser vacio";
            }
            else if (string.IsNullOrEmpty(obj.Correo) || string.IsNullOrWhiteSpace(obj.Correo))
            {
                Mensaje = "El correo del cliente no puede ser vacio";
            }

            //EN caso de que no haya problemas
            if (string.IsNullOrEmpty(Mensaje))
            {
                obj.Clave = CN_Recursos.ConvertirSha256(obj.Clave);
                return objCapaDato.Registrar(obj, out Mensaje);
               
            }
            else
            {
                return 0;
            }

        }


        //Se obtiene la lista con los Cliente
        public List<Cliente> Lista()
        {
            return objCapaDato.Listar();
        }

        //metodo para cambiar la contraseña
        public bool CambiarClave(int idcliente, string nuevaclave, out string Mensaje)
        {
            return objCapaDato.CambiarClave(idcliente, nuevaclave, out Mensaje);
        }


        public bool ReestablecerClave(int idcliente, string correo, out string Mensaje)
        {
            //Mensaje pasado por referencia
            Mensaje = string.Empty;

            //Generamos la clave con el convertidor
            string nuevaclave = CN_Recursos.GenerarClave();
            bool resultado = objCapaDato.ReestablecerClave(idcliente, CN_Recursos.ConvertirSha256(nuevaclave), out Mensaje);

            if (resultado)
            {
                string asunto = "Contraseña Reestablecida";
                string mensaje_correo = "<h3>Su cuenta fue reestablecida correctamente</h3> <br/> <p>Su contraseña para acceder ahora es: !clave!</p>";
                mensaje_correo = mensaje_correo.Replace("!clave!", nuevaclave);

                bool respuesta = CN_Recursos.EnviarCorreo(correo, asunto, mensaje_correo);

                if (respuesta)
                {
                    return true;

                }
                else
                {
                    Mensaje = "No se pudo enviar el correo";
                    return false;

                }
            }
            else
            {
                Mensaje = "No se pudo reestablecer la contraseña";
                return false;
            }



        }
    }
}
