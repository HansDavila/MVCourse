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
    public class CD_Producto
    {

        public List<Producto> Listar()
        {
            List<Producto> lista = new List<Producto>();

            try
            {
                //Con la conexion a la base de datos hacemos lo siguiente
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    //Query para obtener campos de la Producto
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("SELECT P.IdProducto, P.Nombre, P.Descripcion,");
                    sb.AppendLine("M.IdMarca, M.Descripcion[DesMarca],");
                    sb.AppendLine("C.IdCategoria, C.Descripcion[DesCategoria],");
                    sb.AppendLine("P.Precio, P.Stock, P.RutaImagen, P.NombreImagen, P.Activo");
                    sb.AppendLine("FROM PRODUCTO AS P");
                    sb.AppendLine("INNER JOIN MARCA AS M");
                    sb.AppendLine("ON P.IdMarca = M.IdMarca");
                    sb.AppendLine("INNER JOIN CATEGORIA AS C");
                    sb.AppendLine("ON P.IdCategoria = C.IdCategoria");

                    //Objeto donde se guarda la query y conexion para usarla
                    SqlCommand cmd = new SqlCommand(sb.ToString(), oconexion);
                    //Se declara que el comando es de tipo texto (no store procedure u otro)
                    cmd.CommandType = CommandType.Text;

                    //Se abre la conexion
                    oconexion.Open();

                    //Se crea un reader que va a ejecutar el comando
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        //Mientras lea
                        while (reader.Read())
                        {
                            //En la lista crea una nueva categoria  con los campos capturados por el reader
                            lista.Add(
                                new Producto()
                                {
                                    //Los campos se sacan de la capa de entidades

                                    //Para obtener la info se especifica el campo que se quiere obtener en el reader
                                    IdProducto = Convert.ToInt32(reader["IdProducto"]),
                                    Nombre = reader["Nombre"].ToString(),
                                    Descripcion = reader["Descripcion"].ToString(),
                                    oMarca = new Marca() { IdMarca = Convert.ToInt32(reader["IdMarca"]), Descripcion = reader["DesMarca"].ToString() },
                                    oCategoria = new Categoria() { IdCategoria = Convert.ToInt32(reader["IdCategoria"]), Descripcion = reader["DesCategoria"].ToString() },
                                    Precio = Convert.ToDecimal(reader["Precio"],new CultureInfo("es-MX")),
                                    Stock = Convert.ToInt32(reader["Stock"]),
                                    RutaImagen = reader["RutaImagen"].ToString(),
                                    NombreImagen = reader["NombreImagen"].ToString(),
                                    Activo = Convert.ToBoolean(reader["Activo"]),
                                }
                            );
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //Crea una lista vacia
                lista = new List<Producto>();
            }

            //Regresa lista
            return lista;
        }

        //Metodo INSERT
        public int Registrar(Producto obj, out string Mensaje)
        {
            //Id autogenerado que se autogenerara
            int idAutogenerado = 0;

            //Mensaje se inicializa vacio
            Mensaje = string.Empty;

            try
            {
                //Se crea conexion y apartir de aqui...
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    //Aqui se crea el comando con el store procedure y los parametros se pasan
                    SqlCommand cmd = new SqlCommand("sp_RegistrarProducto", oconexion);
                    cmd.Parameters.AddWithValue("Nombre", obj.Nombre);
                    cmd.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("IdMarca", obj.oMarca.IdMarca);
                    cmd.Parameters.AddWithValue("IdCategoria", obj.oCategoria.IdCategoria);
                    cmd.Parameters.AddWithValue("Precio", obj.Precio);
                    cmd.Parameters.AddWithValue("Stock", obj.Stock);
                    cmd.Parameters.AddWithValue("Activo", obj.Activo);
                    cmd.Parameters.AddWithValue("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    //Se abre conexion
                    oconexion.Open();

                    //Se ejecuta el comando (store procedure) que inserta la categoria en la base de datos
                    cmd.ExecuteNonQuery();

                    //El store procedure regresa un id y un mensaje en caso de que sirja un error
                    idAutogenerado = Convert.ToInt32(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                idAutogenerado = 0;
                Mensaje = ex.Message;
            }

            return idAutogenerado;

        }

        //METODO UPDATE
        public bool Editar(Producto obj, out string Mensaje)
        {
            //Variable que nos indica si la categoria fue actualizado
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    //Se crea comando con store procedure
                    SqlCommand cmd = new SqlCommand("sp_EditarProducto", oconexion);
                    cmd.Parameters.AddWithValue("IdProducto", obj.IdProducto);
                    cmd.Parameters.AddWithValue("Nombre", obj.Nombre);
                    cmd.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                    cmd.Parameters.AddWithValue("IdMarca", obj.oMarca.IdMarca);
                    cmd.Parameters.AddWithValue("IdCategoria", obj.oCategoria.IdCategoria);
                    cmd.Parameters.AddWithValue("Precio", obj.Precio);
                    cmd.Parameters.AddWithValue("Stock", obj.Stock);
                    cmd.Parameters.AddWithValue("Activo", obj.Activo);
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    //Se ejecuta el store procedure que actualiza el campo del Id con la informacion pasada
                    cmd.ExecuteNonQuery();

                    //Se obtiene el 1 (osea true) del store procedure
                    resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;

        }

        public bool GuardarDatosImagen(Producto obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    string query = "UPDATE Producto SET RutaImagen = @rutaImagen, NombreImagen = @nombreImagen WHERE IdProducto = @IdProducto";

                    //Se crea comando con store procedure
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@rutaImagen", obj.RutaImagen);
                    cmd.Parameters.AddWithValue("@nombreImagen", obj.NombreImagen);
                    cmd.Parameters.AddWithValue("@IdProducto", obj.IdProducto);
                    cmd.Parameters.AddWithValue("Activo", obj.Activo);
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    //Se ejecuta el store procedure que actualiza el campo del Id con la informacion pasada
                    cmd.ExecuteNonQuery();

                    if(cmd.ExecuteNonQuery() > 0)
                    {
                        resultado = true;
                    }
                    else
                    {
                        Mensaje = "No se pudo actualizar la imagen";
                    }
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }


        //METODO DELETE
        public bool Eliminar(int id, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {//Se crea comando con store procedure
                    SqlCommand cmd = new SqlCommand("sp_EliminarProducto", oconexion);
                    cmd.Parameters.AddWithValue("IdProducto", id);
                    cmd.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    oconexion.Open();

                    //Se ejecuta el store procedure que actualiza el campo del Id con la informacion pasada
                    cmd.ExecuteNonQuery();

                    //Se obtiene el 1 (osea true) del store procedure
                    resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }
    }
}
