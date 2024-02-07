using CapaEntidad;
using CapaNegocio;
using ClosedXML.Excel;
using iTextSharp.text.pdf;
using iTextSharp.text;
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
using static CapaPresentacionAdmin.Controllers.HomeController;

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

        [HttpPost]
        public ActionResult ExportarCategoriasPDF()
        {
            // Obtener los datos de las categorías usando tu lógica de negocio
            List<Categoria> listaCategorias = new CN_Categoria().Lista();

            // Configurar el documento PDF
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (Document document = new Document(PageSize.A4, 50, 50, 25, 25))
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    writer.PageEvent = new ITextEvents(); // Asegúrate de tener esta clase para eventos

                    document.Open();

                    // Agregar logo de la empresa
                    string pathLogo = Server.MapPath("~/Content/Images/logo_store.png");
                    Image logo = Image.GetInstance(pathLogo);
                    logo.ScalePercent(20);
                    logo.SetAbsolutePosition(document.PageSize.Width - logo.ScaledWidth - 10,
                                             document.PageSize.Height - logo.ScaledHeight - 10);
                    document.Add(logo);

                    // Agregar el nombre de la tienda
                    Paragraph tiendaNombre = new Paragraph("Cariño Floral",
                                                            new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD));
                    tiendaNombre.Alignment = Element.ALIGN_CENTER;
                    document.Add(tiendaNombre);

                    // Agregar metadatos y título al documento
                    document.AddTitle("Lista de Categorías");
                    Paragraph titulo = new Paragraph("Lista de Categorías",
                                                     new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
                    titulo.Alignment = Element.ALIGN_CENTER;
                    titulo.SpacingBefore = 20;
                    titulo.SpacingAfter = 30;
                    document.Add(titulo);

                    // Estilos de cabecera y celdas
                    Font headerFont = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE);
                    BaseColor headerBackgroundColor = BaseColor.BLACK;
                    Font cellFont = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL, BaseColor.BLACK);

                    // Crear tabla para categorías
                    PdfPTable table = new PdfPTable(new float[] { 1, 4, 1 }); // 3 columnas
                    table.WidthPercentage = 100;

                    // Cabeceras de la tabla
                    string[] headers = { "ID", "Descripción", "Activo" };
                    foreach (var headerTitle in headers)
                    {
                        PdfPCell header = new PdfPCell(new Phrase(headerTitle, headerFont))
                        {
                            BackgroundColor = headerBackgroundColor,
                            HorizontalAlignment = PdfPCell.ALIGN_CENTER
                        };
                        table.AddCell(header);
                    }

                    // Datos de las categorías
                    foreach (var categoria in listaCategorias)
                    {
                        table.AddCell(new PdfPCell(new Phrase(categoria.IdCategoria.ToString(), cellFont))
                        { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(categoria.Descripcion, cellFont))
                        { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(categoria.Activo ? "Sí" : "No", cellFont))
                        { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                    }

                    document.Add(table);

                    document.Close();
                    writer.Close();
                }

                // Convertir el MemoryStream a un array de bytes y enviar el archivo PDF al usuario
                byte[] content = memoryStream.ToArray();
                return File(content, "application/pdf", "ListaCategorias.pdf");
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

        [HttpPost]
        public ActionResult ExportarMarcasPDF()
        {
            // Obtener los datos de las marcas usando tu lógica de negocio
            List<Marca> listaMarcas = new CN_Marca().Lista();

            // Configurar el documento PDF
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (Document document = new Document(PageSize.A4, 50, 50, 25, 25))
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                    // Asumiendo que tienes una clase ITextEvents para manejar eventos del PDF
                    writer.PageEvent = new ITextEvents();

                    document.Open();

                    // Agregar logo de la empresa y demás elementos al documento                    
                    string pathLogo = Server.MapPath("~/Content/Images/logo_store.png"); // Asegúrate que la ruta sea correcta
                    Image logo = Image.GetInstance(pathLogo);
                    float scalePercent = 20; // ajusta este valor según la necesidad para escalar el tamaño del logo
                    float posX = document.PageSize.Width - logo.ScaledWidth * (scalePercent / 100) - 10; // Asegura que el logo esté a la derecha
                    float posY = document.PageSize.Height - logo.ScaledHeight * (scalePercent / 100) - 10; // Asegura que el logo esté en la parte superior
                    logo.ScalePercent(scalePercent); // escala el logo
                    logo.SetAbsolutePosition(posX, posY);
                    document.Add(logo);

                    // Agregar el nombre de la tienda
                    Paragraph tiendaNombre = new Paragraph("Cariño Floral", new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD));
                    tiendaNombre.Alignment = Element.ALIGN_CENTER;
                    document.Add(tiendaNombre);

                    // Agregar metadatos y título al documento
                    document.AddTitle("Lista de Usuarios");
                    Paragraph titulo = new Paragraph("Lista de Marcas", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
                    titulo.Alignment = Element.ALIGN_CENTER;
                    titulo.SpacingBefore = 20; // Espacio antes del título
                    titulo.SpacingAfter = 30; // Aumenta este valor para agregar más espacio después del título
                    document.Add(titulo);


                    // Configurar el estilo de las celdas de datos
                    Font cellFont = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL, BaseColor.BLACK);

                    // Más código para agregar la tabla y el contenido...
                    // Configurar el estilo de las cabeceras de la tabla
                    Font headerFont = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE);
                    BaseColor headerBackgroundColor = BaseColor.BLACK;                  

                    // Crear y configurar la tabla para los detalles de las marcas
                    PdfPTable table = new PdfPTable(new float[] { 1, 4, 1 }); // 3 columnas
                    table.WidthPercentage = 100;

                    // Agregar las cabeceras de la tabla
                    string[] headers = { "ID Marca", "Descripción", "Activo" };
                    foreach (string headerTitle in headers)
                    {
                        PdfPCell header = new PdfPCell(new Phrase(headerTitle, headerFont));
                        header.BackgroundColor = headerBackgroundColor;
                        header.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                        table.AddCell(header);
                    }

                    // Agregar los datos de las marcas a la tabla
                    foreach (Marca marca in listaMarcas)
                    {
                        table.AddCell(new PdfPCell(new Phrase(marca.IdMarca.ToString(), cellFont)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(marca.Descripcion, cellFont)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(marca.Activo ? "Sí" : "No", cellFont)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                    }

                    document.Add(table);

                    document.Close();
                    writer.Close();
                }

                // Convertir el MemoryStream a un array de bytes y enviar el archivo PDF al usuario
                byte[] content = memoryStream.ToArray();
                return File(content, "application/pdf", "ListaMarcas.pdf");
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


        [HttpPost]
        public ActionResult ExportarProductosPDF()
        {
            // Obtener los datos de los productos usando tu lógica de negocio
            List<Producto> listaProductos = new CN_Producto().Lista();

            // Configurar el documento PDF
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (Document document = new Document(PageSize.A4, 50, 50, 25, 25))
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    writer.PageEvent = new ITextEvents(); // Asegúrate de tener esta clase para eventos

                    document.Open();

                    // Agregar logo de la empresa
                    string pathLogo = Server.MapPath("~/Content/Images/logo_store.png");
                    Image logo = Image.GetInstance(pathLogo);
                    logo.ScalePercent(20);
                    logo.SetAbsolutePosition(document.PageSize.Width - logo.ScaledWidth - 10,
                                             document.PageSize.Height - logo.ScaledHeight - 10);
                    document.Add(logo);

                    // Agregar el nombre de la tienda
                    Paragraph tiendaNombre = new Paragraph("Cariño Floral",
                                                            new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD));
                    tiendaNombre.Alignment = Element.ALIGN_CENTER;
                    document.Add(tiendaNombre);

                    // Agregar metadatos y título al documento
                    document.AddTitle("Lista de Productos");
                    Paragraph titulo = new Paragraph("Lista de Productos",
                                                     new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
                    titulo.Alignment = Element.ALIGN_CENTER;
                    titulo.SpacingBefore = 20;
                    titulo.SpacingAfter = 30;
                    document.Add(titulo);

                    // Estilos de cabecera y celdas
                    Font headerFont = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE);
                    BaseColor headerBackgroundColor = BaseColor.BLACK;
                    Font cellFont = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL, BaseColor.BLACK);

                    // Crear tabla para productos
                    PdfPTable table = new PdfPTable(new float[] { 2, 3, 2, 2, 1, 1, 1 }); // 7 columnas
                    table.WidthPercentage = 100;

                    // Cabeceras de la tabla
                    string[] headers = { "Nombre", "Descripción", "Marca", "Categoría", "Precio", "Stock", "Activo" };
                    foreach (var headerTitle in headers)
                    {
                        PdfPCell header = new PdfPCell(new Phrase(headerTitle, headerFont))
                        {
                            BackgroundColor = headerBackgroundColor,
                            HorizontalAlignment = PdfPCell.ALIGN_CENTER
                        };
                        table.AddCell(header);
                    }

                    // Datos de los productos
                    foreach (var producto in listaProductos)
                    {
                        table.AddCell(new PdfPCell(new Phrase(producto.Nombre, cellFont)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(producto.Descripcion, cellFont)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(producto.oMarca.Descripcion, cellFont)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(producto.oCategoria.Descripcion, cellFont)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(producto.Precio.ToString("C", new CultureInfo("es-MX")), cellFont)) { HorizontalAlignment = PdfPCell.ALIGN_RIGHT });
                        table.AddCell(new PdfPCell(new Phrase(producto.Stock.ToString(), cellFont)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(producto.Activo ? "Sí" : "No", cellFont)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                    }

                    document.Add(table);

                    document.Close();
                    writer.Close();
                }

                // Convertir el MemoryStream a un array de bytes y enviar el archivo PDF al usuario
                byte[] content = memoryStream.ToArray();
                return File(content, "application/pdf", "ListaProductos.pdf");
            }
        }

        #endregion
    }
}