using CapaDatos;
using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CN_Ubicacion
    {
        private CD_Ubicacion objCapaDato = new CD_Ubicacion();

        public List<Departamento> ObtenerDepartamento()
        {
            return objCapaDato.ObtenerDepartamento();
        }

        public List<Provincia> ObtenerProvincia(string IdDepartamento)
        {
            return objCapaDato.ObtenerProvincia(IdDepartamento);
        }

        public List<Distrito> ObtenerDistrito(string IdDepartamento, string IdProvincia)
        {
            return objCapaDato.ObtenerDistrito(IdDepartamento,IdProvincia);
        }

        public List<Estado> ObtenerEstado()
        {
            return objCapaDato.obtenerEstado();
        }

        public List<Municipio> ObtenerMunicipio(string IdEstado)
        {
            return objCapaDato.ObtenerMunicipio(IdEstado);
        }

        public List<Colonia> ObtenerColonia(string IdEstado, string IdMunicipio)
        {
            return objCapaDato.ObtenerColonia(IdEstado, IdMunicipio);
        }
    }
}
