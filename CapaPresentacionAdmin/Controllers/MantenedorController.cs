using CapaEntidad;
using CapaNegocio;
using ClosedXML.Excel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CapaPresentacionAdmin.Controllers
{
    //No va a poder acceder a ninguna de estas vistas si no esta autorizado
    [Authorize]
    public class MantenedorController : Controller
    {
        // GET: Mantenedor
        public ActionResult Categoria()
        {
            return View();
        }

        public ActionResult Marca()
        {
            return View();
        }

        public ActionResult Producto()
        {
            return View();
        }


        //---------------------------CATEGORIA--------------------------------
        #region categoria
        //Devuelve la lista de categorias en data con la informacion de la categoria en formato Json
        [HttpGet]
        public JsonResult ListarCategorias()
        {
            List<Categoria> oLista = new List<Categoria>();

            oLista = new CN_Categoria().Lista();

            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GuardarCategoria(Categoria objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            if (objeto.IdCategoria == 0)
            {
                resultado = new CN_Categoria().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CN_Categoria().Editar(objeto, out mensaje);
            }


            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarCategoria(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new CN_Categoria().Eliminar(id, out mensaje);



            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public FileResult ExportarCategorias()
        {
            List<Categoria> listaCategorias = new CN_Categoria().Lista();

            DataTable dt = new DataTable();
            dt.Columns.Add("Descripción", typeof(string));
            dt.Columns.Add("Activo", typeof(string));

            foreach (var categoria in listaCategorias)
            {
                dt.Rows.Add(categoria.Descripcion, categoria.Activo ? "Sí" : "No");
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("LISTA DE CATEGORIAS");

                // Establecer los encabezados manualmente
                ws.Cell("A2").Value = "Descripcion";
                ws.Cell("B2").Value = "Activo";


                // Estilo de los encabezados como en la segunda imagen
                var headerStyle = ws.Range("A2:B2").Style;
                headerStyle.Font.SetBold(true);
                headerStyle.Fill.SetBackgroundColor(XLColor.FromArgb(221, 235, 247)); // Color azul claro similar al de la segunda imagen
                headerStyle.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                headerStyle.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                // Aplicar bordes a las celdas de datos
                var dataRange = ws.Range("A3:B" + (listaCategorias.Count + 2).ToString());
                dataRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                // Agregar los datos del DataTable a partir de la fila 3
                int currentRow = 3;
                foreach (DataRow row in dt.Rows)
                {
                    ws.Cell(currentRow, 1).Value = row["Descripción"];
                    ws.Cell(currentRow, 2).Value = row["Activo"];

                    currentRow++;
                }

                // Ajustar el ancho de las columnas para que todo el contenido sea visible
                ws.Columns().AdjustToContents();

                // Ajustar el ancho de las columnas para que todo el contenido sea visible
                ws.Column("A").AdjustToContents();
                ws.Column("B").AdjustToContents();
                ws.Column("A").Width = 35;


                // Fusionar celdas para el título y aplicar estilo
                ws.Range("A1:B1").Merge().Value = "LISTA DE CATEGORIAS";
                ws.Cell("A1").Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Font.SetFontSize(20)
                    .Font.SetBold(true)
                    .Fill.SetBackgroundColor(XLColor.FromArgb(91, 155, 213)); // Color azul oscuro para el título

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    var fileName = "ListaCategorias_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }



        #endregion



        //---------------------------MARCA--------------------------------
        #region Marca
        //Devuelve la lista de categorias en data con la informacion de la categoria en formato Json
        [HttpGet]
        public JsonResult ListarMarcas()
        {
            List<Marca> oLista = new List<Marca>();

            oLista = new CN_Marca().Lista();

            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GuardarMarca(Marca objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            if (objeto.IdMarca == 0)
            {
                resultado = new CN_Marca().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CN_Marca().Editar(objeto, out mensaje);
            }


            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EliminarMarca(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new CN_Marca().Eliminar(id, out mensaje);



            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public FileResult ExportarMarcas()
        {
            List<Marca> listaMarcas = new CN_Marca().Lista();

            DataTable dt = new DataTable();
            dt.Columns.Add("Descripción", typeof(string));
            dt.Columns.Add("Activo", typeof(string));

            foreach (var marca in listaMarcas)
            {
                dt.Rows.Add(marca.Descripcion, marca.Activo ? "Sí" : "No");
            }

            dt.TableName = "Datos";
            

            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("LISTA DE MARCAS");

                // Establecer los encabezados manualmente
                ws.Cell("A2").Value = "Descripcion";
                ws.Cell("B2").Value = "Activo";
           

                // Estilo de los encabezados como en la segunda imagen
                var headerStyle = ws.Range("A2:B2").Style;
                headerStyle.Font.SetBold(true);
                headerStyle.Fill.SetBackgroundColor(XLColor.FromArgb(221, 235, 247)); // Color azul claro similar al de la segunda imagen
                headerStyle.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                headerStyle.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                // Aplicar bordes a las celdas de datos
                var dataRange = ws.Range("A3:B" + (listaMarcas.Count + 2).ToString());
                dataRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                // Agregar los datos del DataTable a partir de la fila 3
                int currentRow = 3;
                foreach (DataRow row in dt.Rows)
                {
                    ws.Cell(currentRow, 1).Value = row["Descripción"];
                    ws.Cell(currentRow, 2).Value = row["Activo"];
                    
                    currentRow++;
                }

                // Ajustar el ancho de las columnas para que todo el contenido sea visible
                ws.Columns().AdjustToContents();

                // Ajustar el ancho de las columnas para que todo el contenido sea visible
                ws.Column("A").AdjustToContents();
                ws.Column("B").AdjustToContents();
                ws.Column("A").Width = 35;


                // Fusionar celdas para el título y aplicar estilo
                ws.Range("A1:B1").Merge().Value = "LISTA DE MARCAS";
                ws.Cell("A1").Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Font.SetFontSize(20)
                    .Font.SetBold(true)
                    .Fill.SetBackgroundColor(XLColor.FromArgb(91, 155, 213)); // Color azul oscuro para el título

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    var fileName = "ListaMarcas_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        #endregion


        //---------------------------PRODUCTO--------------------------------
        #region Producto
        //Devuelve la lista de categorias en data con la informacion de la categoria en formato Json
        [HttpGet]
        public JsonResult ListarProducto()
        {
            List<Producto> oLista = new List<Producto>();

            oLista = new CN_Producto().Lista();

            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult GuardarProducto(string objeto, HttpPostedFileBase archivoImagen)
        {
            object resultado;
            string mensaje = string.Empty;
            bool operacion_exitosa = true;
            bool guardarImagen_exito = true;

            //Lo que hacemos aqui es convertir un objeto en texto a un objeto producto
            Producto oProducto = new Producto();
            oProducto = JsonConvert.DeserializeObject<Producto>(objeto);

            decimal precio;

            //El texto del precio me lo vas a convertir en decimal permitiendo que considere que los decimales son puntos y que la region es mexico
            //Para al final guardar el valor en precio
            //Si el parseo fue correcto retorna true, otherwise retorna false
            if(decimal.TryParse(oProducto.PrecioTexto, NumberStyles.AllowDecimalPoint, new CultureInfo("es-MX"), out precio))
            {
                oProducto.Precio = precio;
            }
            else
            {
                return Json(new {operacionExitosa= false, mensaje = "El formato del precio debe ser ##.##"}, JsonRequestBehavior.AllowGet);
            }


            if (oProducto.IdProducto == 0)
            {
                int IdProductoGenerado = new CN_Producto().Registrar(oProducto, out mensaje);

                if(IdProductoGenerado != 0)
                {
                    oProducto.IdProducto = IdProductoGenerado;
                }
                else
                {
                    operacion_exitosa = false;
                }
            }
            else
            {   //EDITAR
                operacion_exitosa = new CN_Producto().Editar(oProducto, out mensaje);
            }

            //LOGICA PARA GUARDAR IMAGEN
            if (operacion_exitosa)
            {
                if(archivoImagen != null)
                {
                    //guardamos la ruta en donde van a estar las fotos
                    string ruta_guardar = ConfigurationManager.AppSettings["ServidorFotos"];

                    //Obtener la extension del archivo 
                    string extension = Path.GetExtension(archivoImagen.FileName);

                    //Usamos el id para crear el nombre de la imagen por ejemplo tenemos:
                    //un producto con id 40 y su extension es .jpg, con la linea de abajo tendriamos
                    //40.jpg
                    string nombre_imagen = string.Concat(oProducto.IdProducto.ToString(), extension);

                    try
                    {
                        archivoImagen.SaveAs(Path.Combine(ruta_guardar, nombre_imagen));
                    }catch (Exception ex)
                    {
                        string msg = ex.Message;
                        guardarImagen_exito = false;
                    }

                    if (guardarImagen_exito)
                    {
                        oProducto.RutaImagen = ruta_guardar;
                        oProducto.NombreImagen = nombre_imagen;
                        bool answer = new CN_Producto().GuardarDatosImagen(oProducto, out mensaje);
                    }
                    else
                    {
                        mensaje = "Se guardo el proudcto pero hubo problemas con la imagen";
                    }
                }
            }


            return Json(new { operacionExitosa = operacion_exitosa, idGenerado = oProducto.IdProducto, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        //Metodo para devolver la cadena de caracteres de la imagen (en base 64) para ser pintado con javascript
        [HttpPost]
        public JsonResult ImagenProducto(int id)
        {
            bool conversion;
            Producto oProducto = new CN_Producto().Lista().Where(p => p.IdProducto == id).FirstOrDefault();

            string textoBase64 = CN_Recursos.ConvertirBase64(Path.Combine(oProducto.RutaImagen, oProducto.NombreImagen), out conversion);

            return Json(new
            {
                conversion = conversion,
                textoBase64 = textoBase64,
                extension = Path.GetExtension(oProducto.NombreImagen)    
            },
                JsonRequestBehavior.AllowGet
            );
        }

        [HttpPost]
        public JsonResult EliminarProducto(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new CN_Producto().Eliminar(id, out mensaje);



            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public FileResult ExportarProductos()
        {
            List<Producto> listaProductos = new CN_Producto().Lista();

            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("LISTA DE PRODUCTOS");

                // Establecer los encabezados manualmente
                ws.Cell("A2").Value = "Nombre";
                ws.Cell("B2").Value = "Descripcion";
                ws.Cell("C2").Value = "Marca";
                ws.Cell("D2").Value = "Categoria";
                ws.Cell("E2").Value = "Precio";
                ws.Cell("F2").Value = "Stock";
                ws.Cell("G2").Value = "Activo";

                // Aplicar estilo a los encabezados
                var headerStyle = ws.Range("A2:G2").Style;
                headerStyle.Font.SetBold(true);
                headerStyle.Fill.SetBackgroundColor(XLColor.FromArgb(221, 235, 247));
                headerStyle.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                headerStyle.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                // Agregar los datos del producto a partir de la fila 2
                int currentRow = 3;
                foreach (var producto in listaProductos)
                {
                    ws.Cell(currentRow, 1).Value = producto.Nombre;
                    ws.Cell(currentRow, 2).Value = producto.Descripcion;
                    ws.Cell(currentRow, 3).Value = producto.oMarca.Descripcion;
                    ws.Cell(currentRow, 4).Value = producto.oCategoria.Descripcion;
                    ws.Cell(currentRow, 5).Value = producto.Precio;
                    ws.Cell(currentRow, 6).Value = producto.Stock;
                    ws.Cell(currentRow, 7).Value = producto.Activo ? "Sí" : "No";
                    currentRow++;
                }

                // Ajustar el ancho de las columnas al contenido
                ws.Columns().AdjustToContents();


                // Fusionar celdas para el título y aplicar estilo
                ws.Range("A1:G1").Merge().Value = "LISTA DE PRODUCTOS";
                ws.Cell("A1").Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Font.SetFontSize(20)
                    .Font.SetBold(true)
                    .Fill.SetBackgroundColor(XLColor.FromArgb(91, 155, 213)); // Color azul oscuro para el título

                using (var stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    var fileName = "ListaProductos_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        #endregion
    }
}