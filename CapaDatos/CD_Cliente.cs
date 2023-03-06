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
    public class CD_Cliente
    {

        //Metodo INSERT
        public int Registrar(Cliente obj, out string Mensaje)
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
                    SqlCommand cmd = new SqlCommand("sp_RegistrarCliente", oconexion);
                    cmd.Parameters.AddWithValue("Nombres", obj.Nombres);
                    cmd.Parameters.AddWithValue("Apellidos", obj.Apellidos);
                    cmd.Parameters.AddWithValue("Correo", obj.Correo);
                    cmd.Parameters.AddWithValue("Clave", obj.Clave);                    
                    cmd.Parameters.AddWithValue("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    //Se abre conexion
                    oconexion.Open();

                    //Se ejecuta el comando (store procedure) que inserta el Cliente en la base de datos
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

        public List<Cliente> Listar()
        {
            List<Cliente> lista = new List<Cliente>();

            try
            {
                //Con la conexion a la base de datos hacemos lo siguiente
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    //Query para obtener campos del Cliente
                    string query = "SELECT IdCliente, Nombres, Apellidos, Correo, Clave, Reestablecer  FROM Cliente";

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
                            //En la lista crea un nuevo Cliente con los campos capturados por el reader
                            lista.Add(
                                new Cliente()
                                {
                                    //Los campos se sacan de la capa de entidades

                                    //Para obtener la info se especifica el campo que se quiere obtener en el reader
                                    IdCliente = Convert.ToInt32(reader["IdCliente"]),
                                    Nombres = reader["Nombres"].ToString(),
                                    Apellidos = reader["Apellidos"].ToString(),
                                    Correo = reader["Correo"].ToString(),
                                    Clave = reader["Clave"].ToString(),
                                    Reestablecer = Convert.ToBoolean(reader["Reestablecer"]),
                                    
                                }
                            );
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //Crea una lista vacia
                lista = new List<Cliente>();
            }

            //Regresa lista
            return lista;
        }

        //METODO CAMBIAR CONTRASEÑA
        public bool CambiarClave(int idcliente, string nuevaclave, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    //Se crea comando con el UPDATE con la nueva clave al cliente
                    SqlCommand cmd = new SqlCommand("UPDATE CLIENTE SET Clave = @nuevaclave, Reestablecer = 0 WHERE IdCliente = @id", oconexion);
                    cmd.Parameters.AddWithValue("@id", idcliente);
                    cmd.Parameters.AddWithValue("@nuevaclave", nuevaclave);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    //Si el todal de filas aceptadas es mayor a 0 devuelve true, en caso contrario devuelve false
                    resultado = cmd.ExecuteNonQuery() > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }

        //METODO PARA REESTABLECER CONSTRASEÑA
        public bool ReestablecerClave(int idcliente, string clave, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    //Se crea comando con el UPDATE con la nueva clave al usuario
                    SqlCommand cmd = new SqlCommand("UPDATE ClIENTE SET Clave = @clave, Reestablecer = 1 WHERE IdCliente = @id", oconexion);
                    cmd.Parameters.AddWithValue("@id", idcliente);
                    cmd.Parameters.AddWithValue("@clave", clave);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    //Si el todal de filas aceptadas es mayor a 0 devuelve true, en caso contrario devuelve false
                    resultado = cmd.ExecuteNonQuery() > 0 ? true : false;
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
