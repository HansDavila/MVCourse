using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Reporte
    {

        //Objeto de capa de datos que tiene los metodos que interactuan directamenete con la base de datos
        private CD_Reporte objCapaDato = new CD_Reporte();


        public List<Reporte> Ventas(string fechainicio, string fechafin, string idTransaccion)
        {
            return objCapaDato.Ventas(fechainicio, fechafin, idTransaccion);
        }

        //Se obtiene la lista con los usuarios
        public Dashboard VerDashboard()
        {
            return objCapaDato.VerDashboard();
        }
    }
}
