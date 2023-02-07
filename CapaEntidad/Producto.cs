using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaEntidad
{
    public class Producto
    {
        public int IdProducto { get; set; }

        public string Nombre { get; set; }

        public string Descripcion { get; set; }

        public Marca oMarca { get; set; }

        public Categoria oCategoria { get; set; }

        public decimal Precio  { get; set; }

        public string PrecioTexto { get; set; }

        public int Stock  { get; set; }

        public string RutaImagen { get; set;}

        public string NombreImagen { get; set;}
        
        public bool Activo { get; set;}

        //Propiedad que va a guardar la imagen
        public string Base64 { get; set;}

        //Hace referencia a la extensio o tipo de imagen a la cual este asignada a la imagen producto
        public string Extension  { get; set; }


    }
}
