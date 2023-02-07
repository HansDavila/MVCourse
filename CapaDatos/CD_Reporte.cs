using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace CapaDatos
{
    public class CD_Reporte
    {



        public List<Reporte> Ventas(string fechainicio, string fechafin, string idTransaccion)
        {
            List<Reporte> lista = new List<Reporte>();

            try
            {
                //Con la conexion a la base de datos hacemos lo siguiente
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {                    
                    
                    //Objeto donde se guarda la query y conexion para usarla
                    SqlCommand cmd = new SqlCommand("sp_ReporteVentas", oconexion);
                    cmd.Parameters.AddWithValue("fechainicio", fechainicio);
                    cmd.Parameters.AddWithValue("fechafin", fechafin);
                    cmd.Parameters.AddWithValue("@idtransaccion", idTransaccion);

                    //Se declara que el comando es de tipo STORED PROCEDURE
                    cmd.CommandType = CommandType.StoredProcedure;

                    //Se abre la conexion
                    oconexion.Open();

                    //Se crea un reader que va a ejecutar el comando
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        //Mientras lea
                        while (reader.Read())
                        {
                            //En la lista crea un nuevo Usuario con los campos capturados por el reader
                            lista.Add(
                                new Reporte()
                                {
                                    //Los campos se sacan de la capa de entidades

                                    //Para obtener la info se especifica el campo que se quiere obtener en el reader
                                    FechaVenta = reader["FechaVenta"].ToString(),
                                    Cliente= reader["Cliente"].ToString(),
                                    Producto = reader["Producto"].ToString(),
                                    Precio = Convert.ToDecimal(reader["Precio"], new CultureInfo("es-MX")),                                    
                                    Cantidad = Convert.ToInt32(reader["Cantidad"]),
                                    Total = Convert.ToDecimal(reader["Total"], new CultureInfo("es-MX")),
                                    IdTransaccion = reader["IdTransaccion"].ToString()                                    
                                }
                            );
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //Crea una lista vacia
                lista = new List<Reporte>();
            }

            //Regresa lista
            return lista;
        }


        public Dashboard VerDashboard()
        {
            Dashboard objeto = new Dashboard();

            try
            {
                //Con la conexion a la base de datos hacemos lo siguiente
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    

                    //Objeto donde se guarda la query y conexion para usarla
                    SqlCommand cmd = new SqlCommand("sp_ReporteDashboard", oconexion);

                    //Se declara que el comando es de tipo Stored Procedure (no de tipo texto para una simple query)
                    cmd.CommandType = CommandType.StoredProcedure;

                    //Se abre la conexion
                    oconexion.Open();

                    //Se crea un reader que va a ejecutar el comando
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        //Mientras lea
                        while (reader.Read())
                        {
                            //Creamos un objeto de tipo Dashboard con los campos capturados por el reader
                            objeto = new Dashboard()
                            {
                                TotalCliente = Convert.ToInt32(reader["TotalCliente"]),
                                TotalVenta = Convert.ToInt32(reader["TotalVenta"]),
                                TotalProducto = Convert.ToInt32(reader["TotalProducto"]),
                            };

                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //Crea una lista vacia
                objeto = new Dashboard();
            }

            //Regresa lista
            return objeto;
        }
    }
}
