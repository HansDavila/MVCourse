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
    public class CD_Ubicacion
    {
        public List<Departamento> ObtenerDepartamento()
        {
            List<Departamento> lista = new List<Departamento>();

            try
            {
                //Con la conexion a la base de datos hacemos lo siguiente
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    //Query para obtener campos del usuario
                    string query = "SELECT * FROM DEPARTAMENTO";

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
                            //En la lista crea un nuevo Usuario con los campos capturados por el reader
                            lista.Add(
                                new Departamento()
                                {
                                    //Los campos se sacan de la capa de entidades

                                    //Para obtener la info se especifica el campo que se quiere obtener en el reader
                                    IdDepartamento = reader["IdDepartamento"].ToString(),
                                    Descripcion = reader["Descripcion"].ToString(),
                                    
                                }
                            );
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //Crea una lista vacia
                lista = new List<Departamento>();
            }

            //Regresa lista
            return lista;
        }

        public List<Provincia> ObtenerProvincia(string IdDepartamento)
        {
            List<Provincia> lista = new List<Provincia>();

            try
            {
                //Con la conexion a la base de datos hacemos lo siguiente
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    //Query para obtener campos del usuario
                    string query = "SELECT * FROM PROVINCIA WHERE IdDepartamento = @IdDepartamento";


                    //Objeto donde se guarda la query y conexion para usarla
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@IdDepartamento", IdDepartamento);
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
                            //En la lista crea un nuevo Usuario con los campos capturados por el reader
                            lista.Add(
                                new Provincia()
                                {
                                    //Los campos se sacan de la capa de entidades

                                    //Para obtener la info se especifica el campo que se quiere obtener en el reader
                                    IdProvincia = reader["IdProvincia"].ToString(),
                                    Descripcion = reader["Descripcion"].ToString(),

                                }
                            );
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //Crea una lista vacia
                lista = new List<Provincia>();
            }

            //Regresa lista
            return lista;
        }

        public List<Distrito> ObtenerDistrito(string IdDepartamento, string IdProvincia)
        {
            List<Distrito> lista = new List<Distrito>();

            try
            {
                //Con la conexion a la base de datos hacemos lo siguiente
                using (SqlConnection oconexion = new SqlConnection(Conexion.cn))
                {
                    //Query para obtener campos del usuario
                    string query = "SELECT * FROM DISTRITO WHERE IdProvincia = @IdProvincia AND IdDepartamento = @IdDepartamento";


                    //Objeto donde se guarda la query y conexion para usarla
                    SqlCommand cmd = new SqlCommand(query, oconexion);
                    cmd.Parameters.AddWithValue("@IdProvincia", IdProvincia);
                    cmd.Parameters.AddWithValue("@IdDepartamento", IdDepartamento);
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
                            //En la lista crea un nuevo Usuario con los campos capturados por el reader
                            lista.Add(
                                new Distrito()
                                {
                                    //Los campos se sacan de la capa de entidades

                                    //Para obtener la info se especifica el campo que se quiere obtener en el reader
                                    IdDistrito = reader["IdDistrito"].ToString(),
                                    Descripcion = reader["Descripcion"].ToString(),

                                }
                            );
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //Crea una lista vacia
                lista = new List<Distrito>();
            }

            //Regresa lista
            return lista;
        }
    }
}
