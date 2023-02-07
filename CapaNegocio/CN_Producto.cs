using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Producto
    {

        //Objeto de capa de datos que tien elos metodos que interactuan directamenete con la base de datos
        private CD_Producto objCapaDato = new CD_Producto();

        //Se obtiene la lista con las categorias
        public List<Producto> Lista()
        {
            return objCapaDato.Listar();
        }


        public int Registrar(Producto obj, out string Mensaje)
        {
            //Mensaje pasado por referencia
            Mensaje = string.Empty;

            //Se verifica que no esten los campos vacios
            if (string.IsNullOrEmpty(obj.Nombre) || string.IsNullOrWhiteSpace(obj.Nombre))
            {
                Mensaje = "El nombre del Producto no puede ser vacio";
            }            
            else if (string.IsNullOrEmpty(obj.Descripcion) || string.IsNullOrWhiteSpace(obj.Descripcion))
            {
                Mensaje = "La descripcion del Producto no puede ser vacio";
            }
            else if (obj.oMarca.IdMarca == 0)
            {
                Mensaje = "Debe seleccionar una marca";
            }
            else if (obj.oCategoria.IdCategoria == 0)
            {
                Mensaje = "Debe seleccionar una categoria";
            }
            else if (obj.oMarca.IdMarca == 0)
            {
                Mensaje = "Debe seleccionar una marca";
            }
            else if (obj.Precio == 0)
            {
                Mensaje = "Debe ingresar el precio del producto";
            }
            else if (obj.Stock == 0)
            {
                Mensaje = "Debe ingresar el stock del producto";
            }


            //EN caso de que no haya problemas
            if (string.IsNullOrEmpty(Mensaje))
            {
                //Devuelve Id del producto
                return objCapaDato.Registrar(obj, out Mensaje);
            }
            else
            {
                return 0;
            }

        }

        public bool Editar(Producto obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            //Se verifica que no esten los campos vacios
            //Se verifica que no esten los campos vacios
            if (string.IsNullOrEmpty(obj.Nombre) || string.IsNullOrWhiteSpace(obj.Nombre))
            {
                Mensaje = "El nombre del Producto no puede ser vacio";
            }
            else if (string.IsNullOrEmpty(obj.Descripcion) || string.IsNullOrWhiteSpace(obj.Descripcion))
            {
                Mensaje = "La descripcion del Producto no puede ser vacio";
            }
            else if (obj.oMarca.IdMarca == 0)
            {
                Mensaje = "Debe seleccionar una marca";
            }
            else if (obj.oCategoria.IdCategoria == 0)
            {
                Mensaje = "Debe seleccionar una categoria";
            }
            else if (obj.oMarca.IdMarca == 0)
            {
                Mensaje = "Debe seleccionar una marca";
            }
            else if (obj.Precio == 0)
            {
                Mensaje = "Debe ingresar el precio del producto";
            }
            else if (obj.Stock == 0)
            {
                Mensaje = "Debe ingresar el stock del producto";
            }



            if (string.IsNullOrEmpty(Mensaje))
            {

                //Se edita elemento
                return objCapaDato.Editar(obj, out Mensaje);
            }
            else
            {
                return false;
            }
        }


        public bool GuardarDatosImagen(Producto obj, out string Mensaje)
        {
            return objCapaDato.GuardarDatosImagen(obj, out Mensaje);
        }

        //Se elimina el elemento
        public bool Eliminar(int id, out string Mensaje)
        {
            return objCapaDato.Eliminar(id, out Mensaje);
        }
    }
}
