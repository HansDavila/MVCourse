using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Categoria
    {

        public List<Categoria> Listar()
        {
            List<Categoria> lista = new List<Categoria>();

            try
            {
                //Con la conexion a la base de datos hacemos lo siguiente
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    //Query para obtener campos de la categoria
                    string query = "SELECT IdCategoria, Descripcion, Activo FROM CATEGORIA";

                    //Objeto donde se guarda la query y conexion para usarla
                    SqlCommand cmd = new SqlCommand(query, oconexion);
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
                                new Categoria()
                                {
                                    //Los campos se sacan de la capa de entidades

                                    //Para obtener la info se especifica el campo que se quiere obtener en el reader
                                    IdCategoria = Convert.ToInt32(reader["IdCategoria"]),
                                    Descripcion = reader["Descripcion"].ToString(),
                                    Activo = Convert.ToBoolean(reader["Activo"])
                                }
                            );
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //Crea una lista vacia
                lista = new List<Categoria>();
            }

            //Regresa lista
            return lista;
        }


        //Metodo INSERT
        public int Registrar(Categoria obj, out string Mensaje)
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
                    SqlCommand cmd = new SqlCommand("sp_RegistrarCategoria", oconexion);
                    cmd.Parameters.AddWithValue("Descripcion", obj.Descripcion);
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
        public bool Editar(Categoria obj, out string Mensaje)
        {
            //Variable que nos indica si la categoria fue actualizado
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    //Se crea comando con store procedure
                    SqlCommand cmd = new SqlCommand("sp_EditarCategoria", oconexion);
                    cmd.Parameters.AddWithValue("IdCategoria", obj.IdCategoria);
                    cmd.Parameters.AddWithValue("Descripcion", obj.Descripcion);
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


        //METODO DELETE
        public bool Eliminar(int id, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {//Se crea comando con store procedure
                    SqlCommand cmd = new SqlCommand("sp_EliminarCategoria", oconexion);
                    cmd.Parameters.AddWithValue("IdCategoria", id);
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
