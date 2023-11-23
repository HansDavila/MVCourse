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









    }
}