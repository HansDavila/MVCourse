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
    public class CD_Carrito
    {

        //Metodo INSERT
        public bool ExisteCarrito(int idcliente, int idproducto)
        {            
            bool resultado = true;            

            try
            {
                //Se crea conexion y apartir de aqui...
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    //Aqui se crea el comando con el store procedure y los parametros se pasan
                    SqlCommand cmd = new SqlCommand("sp_ExisteCarrito", oconexion);
                    cmd.Parameters.AddWithValue("IdCliente", idcliente);
                    cmd.Parameters.AddWithValue("IdProducto", idproducto);                    
                    cmd.Parameters.AddWithValue("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;                    
                    cmd.CommandType = CommandType.StoredProcedure;

                    //Se abre conexion
                    oconexion.Open();

                    //Se ejecuta el comando (store procedure) que inserta el usuario en la base de datos
                    cmd.ExecuteNonQuery();

                    //El store procedure regresa un id y un mensaje en caso de que sirja un error
                    resultado = Convert.ToBoolean(cmd.Parameters["Resultado"].Value);                    
                }
            }
            catch (Exception ex)
            {
                resultado = false;
            }

            return resultado;

        }

        //Metodo INSERT
        public bool OperacionCarrito(int idcliente, int idproducto, bool sumar, out string Mensaje)
        {            
            bool resultado = true;

            //Mensaje se inicializa vacio
            Mensaje = string.Empty;

            try
            {
                //Se crea conexion y apartir de aqui...
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    //Aqui se crea el comando con el store procedure y los parametros se pasan
                    SqlCommand cmd = new SqlCommand("sp_OperacionCarrito", oconexion);
                    cmd.Parameters.AddWithValue("IdCliente", idcliente);
                    cmd.Parameters.AddWithValue("IdProducto", idproducto);
                    cmd.Parameters.AddWithValue("Sumar", sumar);
                    
                    cmd.Parameters.AddWithValue("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    cmd.CommandType = CommandType.StoredProcedure;

                    //Se abre conexion
                    oconexion.Open();

                    //Se ejecuta el comando (store procedure) que inserta el usuario en la base de datos
                    cmd.ExecuteNonQuery();

                    //El store procedure regresa un id y un mensaje en caso de que sirja un error
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

        //SELECT COUNT(*) from carrito WHERE IdCliente = 1

        //METODO DELETE
        public int CantidadEnCarrito(int idcliente)
        {
            int resultado = 0;
            

            try
            {
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    //Se crea comando con el DELETE que se va a eliminar
                    SqlCommand cmd = new SqlCommand("SELECT COUNT(*) from carrito WHERE IdCliente = @idcliente", oconexion);
                    cmd.Parameters.AddWithValue("@idcliente", idcliente);
                    cmd.CommandType = CommandType.Text;
                    oconexion.Open();

                    //Si el todal de filas aceptadas es mayor a 0 devuelve true, en caso contrario devuelve false
                    resultado = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                resultado = 0;
                
            }

            return resultado;
        }
    }
}
