using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using CapaEntidad;
using CapaNegocio;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace CapaPresentacionAdmin.Controllers
{
    //No va a poder acceder a ninguna de estas vistas si no esta autorizado
    [Authorize]

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Usuarios()
        {
            return View();
        }

        //Devuelve la lista de usuarios en data con la informaciond el usuario en formato Json
        [HttpGet]
        public JsonResult ListarUsuarios()
        {
            List<Usuario> oLista= new List<Usuario>();

            oLista = new CN_Usuarios().Lista();

            return Json(new {data = oLista } , JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardarUsuario(Usuario objeto)
        {
            object resultado;
            string mensaje = string.Empty;

            if (objeto.IdUsuario == 0)
            {
                resultado = new CN_Usuarios().Registrar(objeto, out mensaje);
            }
            else
            {
                resultado = new CN_Usuarios().Editar(objeto, out mensaje);
            }
            

            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult EliminarUsuario(int id)
        {
            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new CN_Usuarios().Eliminar(id, out mensaje);



            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ListaReporte(string fechainicio, string fechafin, string idtransaccion)
        {
            List<Reporte> oLista = new List<Reporte>();
            
            oLista = new CN_Reporte().Ventas(fechainicio,fechafin,idtransaccion);

            return Json(new { data = oLista }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult VistaDashboard()
        {
            Dashboard objeto = new CN_Reporte().VerDashboard();

            return Json(new { resultado = objeto}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public FileResult ExportarVenta(string fechainicio, string fechafin, string idtransaccion)
        {
            List<Reporte> oLista = new List<Reporte>();
            oLista = new CN_Reporte().Ventas(fechainicio, fechafin, idtransaccion);

            DataTable dt = new DataTable();

            //dt.Locale = new CultureInfo("es-MX");
            dt.Locale = new CultureInfo("es-MX");
            dt.Columns.Add("Fecha Venta" , typeof(string));
            dt.Columns.Add("Cliente" , typeof(string));
            dt.Columns.Add("Producto" , typeof(string));
            dt.Columns.Add("Precio" , typeof(decimal));
            dt.Columns.Add("Cantidad", typeof(int));
            dt.Columns.Add("Total", typeof(decimal));
            dt.Columns.Add("IdTransaccion", typeof(string));

            foreach(Reporte rp in oLista)
            {
                dt.Rows.Add(new object[]
                {
                    rp.FechaVenta,
                    rp.Cliente,
                    rp.Producto,
                    rp.Precio,
                    rp.Cantidad,
                    rp.Total,
                    rp.IdTransaccion
                });
            }

            dt.TableName = "Datos";

            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add(dt);
                ws.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Style.Font.Bold = true;

                // Aplicar un estilo de título más grande y negrita
                var tituloCell = ws.Cell(1, 1);
                tituloCell.Value = "REPORTE DE TRANSACCIONES";
                tituloCell.Style.Font.FontSize = 20;
                tituloCell.Style.Font.Bold = true;
                tituloCell.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                // Ajustar el ancho de las columnas antes de aplicar estilos
                ws.Columns().AdjustToContents();

                //// Aplicar estilos a los encabezados de las columnas
                //var rngHeaders = ws.Range(2, 1, 2, dt.Columns.Count); // Ajusta esto a la fila correcta si es necesario
                //rngHeaders.Style.Font.Bold = true;
                //rngHeaders.Style.Fill.BackgroundColor = XLColor.LightGray;
                //rngHeaders.Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    var fileName = "ReporteVenta" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }

        }

        [HttpPost]
        public ActionResult ExportarVentaPDF(string fechainicio, string fechafin, string idtransaccion)
        {
            // Obtener los datos del reporte usando tu lógica de negocio
            List<Reporte> reporteVentas = new CN_Reporte().Ventas(fechainicio, fechafin, idtransaccion);

            // Configurar el documento PDF
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (Document document = new Document(PageSize.A4, 50, 50, 25, 25))
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                    document.Open();

                    // Agregar logo de la empresa
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

                    // Agregar el rango de fechas del reporte
                    Paragraph fechaRango = new Paragraph($"Reporte desde {fechainicio} hasta {fechafin}", new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL));
                    fechaRango.Alignment = Element.ALIGN_CENTER;
                    fechaRango.SpacingBefore = 10;
                    document.Add(fechaRango);

                    // Agregar metadatos y título al documento
                    document.AddTitle("Reporte de Transacciones");
                    Paragraph titulo = new Paragraph("Reporte de Transacciones", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
                    titulo.Alignment = Element.ALIGN_CENTER;
                    titulo.SpacingBefore = 20; // Espacio antes del título
                    titulo.SpacingAfter = 30; // Aumenta este valor para agregar más espacio después del título
                    document.Add(titulo);

                    // Más código para agregar la tabla y el contenido...
                    // Configurar el estilo de las cabeceras de la tabla
                    Font headerFont = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD, BaseColor.WHITE);
                    BaseColor headerBackgroundColor = BaseColor.BLACK;

                    // Crear una tabla para los detalles del reporte de ventas
                    PdfPTable table = new PdfPTable(new float[] { 2, 2, 2, 1, 1, 1, 2 }); // 7 columnas
                    table.WidthPercentage = 100;

                    // Agregar las cabeceras de la tabla con estilo
                    string[] headers = new string[] { "Fecha Venta", "Cliente", "Producto", "Precio", "Cantidad", "Total", "IdTransaccion" };
                    foreach (var headerTitle in headers)
                    {
                        PdfPCell header = new PdfPCell(new Phrase(headerTitle, headerFont));
                        header.BackgroundColor = headerBackgroundColor;
                        header.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                        table.AddCell(header);
                    }

                    // Configurar el estilo de las celdas de datos
                    Font cellFont = new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL, BaseColor.BLACK);

                    decimal totalGeneral = 0;
                    // Agregar los datos de cada venta a la tabla
                    foreach (var venta in reporteVentas)
                    {
                        table.AddCell(new PdfPCell(new Phrase(venta.FechaVenta, cellFont)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(venta.Cliente, cellFont)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(venta.Producto, cellFont)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(venta.Precio.ToString("C", new CultureInfo("es-MX")), cellFont)) { HorizontalAlignment = PdfPCell.ALIGN_RIGHT });
                        table.AddCell(new PdfPCell(new Phrase(venta.Cantidad.ToString(), cellFont)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(venta.Total.ToString("C", new CultureInfo("es-MX")), cellFont)) { HorizontalAlignment = PdfPCell.ALIGN_RIGHT });
                        table.AddCell(new PdfPCell(new Phrase(venta.IdTransaccion, cellFont)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                        totalGeneral += venta.Total; // Acumulando el total general
                    }

                    // Agregar una fila para el total general al final de la tabla
                    Font totalFont = new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD, BaseColor.BLACK);
                    PdfPCell totalLabelCell = new PdfPCell(new Phrase("Total General", totalFont));
                    totalLabelCell.Colspan = 6; // Abarca 6 columnas
                    totalLabelCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    table.AddCell(totalLabelCell);

                    PdfPCell totalValueCell = new PdfPCell(new Phrase(totalGeneral.ToString("C", new CultureInfo("es-MX")), totalFont));
                    totalValueCell.HorizontalAlignment = PdfPCell.ALIGN_RIGHT;
                    table.AddCell(totalValueCell);

                    document.Add(table);

                    document.Close();
                    writer.Close();
                }

                // Convertir el MemoryStream a un array de bytes y enviar el archivo PDF al usuario
                byte[] content = memoryStream.ToArray();
                return File(content, "application/pdf", "ReporteVenta.pdf");
            }
        }




        [HttpPost]
        public FileResult ExportarUsuarios()
        {
            List<Usuario> oLista = new CN_Usuarios().Lista();

            DataTable dt = new DataTable();
            dt.Columns.Add("Nombres", typeof(string));
            dt.Columns.Add("Apellidos", typeof(string));
            dt.Columns.Add("Correo", typeof(string));
            dt.Columns.Add("Activo", typeof(bool));

            foreach (Usuario usuario in oLista)
            {
                dt.Rows.Add(usuario.Nombres, usuario.Apellidos, usuario.Correo, usuario.Activo);
            }

            dt.TableName = "Datos";

            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("LISTA DE USUARIOS");

                // Establecer los encabezados manualmente
                ws.Cell("A2").Value = "Nombres";
                ws.Cell("B2").Value = "Apellidos";
                ws.Cell("C2").Value = "Correo";
                ws.Cell("D2").Value = "Activo";

                // Estilo de los encabezados como en la segunda imagen
                var headerStyle = ws.Range("A2:D2").Style;
                headerStyle.Font.SetBold(true);
                headerStyle.Fill.SetBackgroundColor(XLColor.FromArgb(221, 235, 247)); // Color azul claro similar al de la segunda imagen
                headerStyle.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                headerStyle.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                // Aplicar bordes a las celdas de datos
                var dataRange = ws.Range("A3:D" + (oLista.Count + 2).ToString());
                dataRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);

                // Agregar los datos del DataTable a partir de la fila 3
                int currentRow = 3;
                foreach (DataRow row in dt.Rows)
                {
                    ws.Cell(currentRow, 1).Value = row["Nombres"];
                    ws.Cell(currentRow, 2).Value = row["Apellidos"];
                    ws.Cell(currentRow, 3).Value = row["Correo"];
                    ws.Cell(currentRow, 4).Value = row["Activo"];
                    currentRow++;
                }

                // Ajustar el ancho de las columnas para que todo el contenido sea visible
                ws.Columns().AdjustToContents();

                // Ajustar el ancho de las columnas para que todo el contenido sea visible
                ws.Column("A").AdjustToContents();
                ws.Column("B").AdjustToContents();
                ws.Column("C").AdjustToContents();
                // Asegurarse de que la columna 'Activo' tenga un ancho mínimo adecuado
                ws.Column("D").AdjustToContents();
                if (ws.Column("D").Width < 10) // Establece un ancho mínimo si el ajuste automático no es suficiente
                {
                    ws.Column("D").Width = 15;
                }

                // Fusionar celdas para el título y aplicar estilo
                ws.Range("A1:D1").Merge().Value = "LISTA DE USUARIOS";
                ws.Cell("A1").Style
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center)
                    .Font.SetFontSize(20)
                    .Font.SetBold(true)
                    .Fill.SetBackgroundColor(XLColor.FromArgb(91, 155, 213)); // Color azul oscuro para el título

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    var fileName = "ListaUsuarios_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }

        [HttpPost]
        public ActionResult ExportarUsuariosPDF()
        {
            // Obtener los datos de los usuarios usando tu lógica de negocio
            List<Usuario> listaUsuarios = new CN_Usuarios().Lista();

            // Configurar el documento PDF
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (Document document = new Document(PageSize.A4, 50, 50, 25, 25))
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                    // Registra el event handler para el pie de página aquí
                    writer.PageEvent = new ITextEvents();

                    document.Open();

                    // Agregar logo de la empresa
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
                    Paragraph titulo = new Paragraph("Lista de Usuarios", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
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

                    // Crear una tabla para los detalles de los usuarios
                    PdfPTable table = new PdfPTable(new float[] { 3, 3, 3, 1 }); // 4 columnas
                    table.WidthPercentage = 100;

                    // Agregar las cabeceras de la tabla
                    string[] headers = { "Nombres", "Apellidos", "Correo", "Activo" };
                    foreach (var headerTitle in headers)
                    {
                        PdfPCell header = new PdfPCell(new Phrase(headerTitle, headerFont));
                        header.BackgroundColor = headerBackgroundColor;
                        header.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
                        table.AddCell(header);
                    }

                    // Agregar los datos de los usuarios a la tabla
                    foreach (var usuario in listaUsuarios)
                    {
                        table.AddCell(new PdfPCell(new Phrase(usuario.Nombres, cellFont)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(usuario.Apellidos, cellFont)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(usuario.Correo, cellFont)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                        table.AddCell(new PdfPCell(new Phrase(usuario.Activo ? "Sí" : "No", cellFont)) { HorizontalAlignment = PdfPCell.ALIGN_CENTER });
                    }

                    document.Add(table);

                    document.Close();
                    writer.Close();
                }

                // Convertir el MemoryStream a un array de bytes y enviar el archivo PDF al usuario
                byte[] content = memoryStream.ToArray();
                return File(content, "application/pdf", "ListaUsuarios.pdf");
            }
        }



        // Clase que maneja eventos de iTextSharp, colócala fuera del método de la acción pero dentro de la misma clase del controlador
        public class ITextEvents : PdfPageEventHelper
        {
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);
                int pageNumber = writer.PageNumber;
                Paragraph footer = new Paragraph("Página " + pageNumber, FontFactory.GetFont(FontFactory.HELVETICA, 8));
                footer.Alignment = Element.ALIGN_CENTER;
                PdfPTable footerTbl = new PdfPTable(1);
                footerTbl.TotalWidth = 300;
                footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;
                PdfPCell cell = new PdfPCell(footer);
                cell.Border = 0;
                cell.PaddingLeft = 10;
                footerTbl.AddCell(cell);
                footerTbl.WriteSelectedRows(0, -1, 415, 30, writer.DirectContent);
            }
        }





    }
}