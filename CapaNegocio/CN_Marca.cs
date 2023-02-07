using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Marca
    {

        //Objeto de capa de datos que tien elos metodos que interactuan directamenete con la base de datos
        private CD_Marca objCapaDato = new CD_Marca();

        //Se obtiene la lista con las categorias
        public List<Marca> Lista()
        {
            return objCapaDato.Listar();
        }


        public int Registrar(Marca obj, out string Mensaje)
        {
            //Mensaje pasado por referencia
            Mensaje = string.Empty;

            //Se verifica que no esten los campos vacios
            if (string.IsNullOrEmpty(obj.Descripcion) || string.IsNullOrWhiteSpace(obj.Descripcion))
            {
                Mensaje = "La descripcion de la marca no puede ser vacio";
            }


            //EN caso de que no haya problemas
            if (string.IsNullOrEmpty(Mensaje))
            {

                return objCapaDato.Registrar(obj, out Mensaje);
            }
            else
            {
                return 0;
            }

        }

        public bool Editar(Marca obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            //Se verifica que no esten los campos vacios
            if (string.IsNullOrEmpty(obj.Descripcion) || string.IsNullOrWhiteSpace(obj.Descripcion))
            {
                Mensaje = "La descripcion de la marca no puede ser vacio";
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

        //Se elimina el elemento
        public bool Eliminar(int id, out string Mensaje)
        {
            return objCapaDato.Eliminar(id, out Mensaje);
        }
    }
}
