using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CapaEntidad;
using System.Data.SqlClient;
using System.Data;

namespace CapaDatos
{
    public  class CD_Usuarios
    {
        public List<Usuario> Listar()
        {
            List<Usuario> lista = new List<Usuario>();

            try
            {
                //Con la conexion a la base de datos hacemos lo siguiente
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    //Query para obtener campos del usuario
                    string query = "SELECT IdUsuario, Nombres, Apellidos, Correo, Clave, Reestablecer, Activo  FROM USUARIO";

                    //Objeto donde se guarda la query y conexion para usarla
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    //Se declara que el comando es de tipo texto (no store procedure u otro)
                    cmd.CommandType = CommandType.Text;

                    //Se abre la conexion
                    oconexion.Open();

                    //Se crea un reader que va a ejecutar el comando
                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        //Mientras lea
                        while(reader.Read())
                        {
                            //En la lista crea un nuevo Usuario con los campos capturados por el reader
                            lista.Add(
                                new Usuario()
                                {
                                    //Los campos se sacan de la capa de entidades
                                    
                                    //Para obtener la info se especifica el campo que se quiere obtener en el reader
                                    IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                                    Nombres = reader["Nombres"].ToString(),
                                    Apellidos = reader["Apellidos"].ToString(),
                                    Correo = reader["Correo"].ToString(),
                                    Clave = reader["Clave"].ToString(),
                                    Reestablecer = Convert.ToBoolean(reader["Reestablecer"]),
                                    Activo = Convert.ToBoolean(reader["Activo"])
                                }
                            );
                        }
                    }
                }
                
            }catch (Exception ex)
            {
                //Crea una lista vacia
                lista = new List<Usuario>();
            }

            //Regresa lista
            return lista;
        }


        //Metodo INSERT
        public int Registrar(Usuario obj, out string Mensaje)
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
                    SqlCommand cmd = new SqlCommand("sp_Registrarusuario", oconexion);
                    cmd.Parameters.AddWithValue("Nombres", obj.Nombres);
                    cmd.Parameters.AddWithValue("Apellidos", obj.Apellidos);
                    cmd.Parameters.AddWithValue("Correo", obj.Correo);
                    cmd.Parameters.AddWithValue("Clave", obj.Clave);
                    cmd.Parameters.AddWithValue("Activo", obj.Activo);
                    cmd.Parameters.AddWithValue("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType= CommandType.StoredProcedure;

                    //Se abre conexion
                    oconexion.Open();
                    
                    //Se ejecuta el comando (store procedure) que inserta el usuario en la base de datos
                    cmd.ExecuteNonQuery();

                    //El store procedure regresa un id y un mensaje en caso de que sirja un error
                    idAutogenerado = Convert.ToInt32(cmd.Parameters["Resultado"].Value);
                    Mensaje = cmd.Parameters["Mensaje"].Value.ToString();
                }
            }catch(Exception ex)
            {
                idAutogenerado = 0;
                Mensaje = ex.Message;
            }

            return idAutogenerado;

        }

        //METODO UPDATE
        public bool Editar(Usuario obj, out string Mensaje)
        {
            //Variable que nos indica si el usuario fue actualizado
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    //Se crea comando con store procedure
                    SqlCommand cmd = new SqlCommand("sp_EditarUsuario", oconexion);
                    cmd.Parameters.AddWithValue("IdUsuario", obj.IdUsuario);
                    cmd.Parameters.AddWithValue("Nombres", obj.Nombres);
                    cmd.Parameters.AddWithValue("Apellidos", obj.Apellidos);
                    cmd.Parameters.AddWithValue("Correo", obj.Correo);
                    cmd.Parameters.AddWithValue("Activo", obj.Activo);
                    cmd.Parameters.AddWithValue("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
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
                {
                    //Se crea comando con el DELETE que se va a eliminar
                    SqlCommand cmd = new SqlCommand("DELETE TOP (1) FROM USUARIO WHERE IdUsuario = @id", oconexion);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    //Si el todal de filas aceptadas es mayor a 0 devuelve true, en caso contrario devuelve false
                    resultado = cmd.ExecuteNonQuery() > 0 ? true : false; 
                }
            }catch(Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }

        //METODO CAMBIAR CONTRASEÑA
        public bool CambiarClave(int idusuario,string nuevaclave, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    //Se crea comando con el UPDATE con la nueva clave al usuario
                    SqlCommand cmd = new SqlCommand("UPDATE USUARIO SET Clave = @nuevaclave, Reestablecer = 0 WHERE IdUsuario = @id", oconexion);
                    cmd.Parameters.AddWithValue("@id", idusuario);
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
        public bool ReestablecerClave(int idusuario, string clave, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    //Se crea comando con el UPDATE con la nueva clave al usuario
                    SqlCommand cmd = new SqlCommand("UPDATE USUARIO SET Clave = @clave, Reestablecer = 1 WHERE IdUsuario = @id", oconexion);
                    cmd.Parameters.AddWithValue("@id", idusuario);
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
