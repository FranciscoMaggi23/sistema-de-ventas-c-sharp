﻿using CapaEntidad;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaDatos
{
    public class CD_Permiso
    {
        public List<Permiso> Listar(int idUsuario)
        {
            List<Permiso> lista = new List<Permiso>();

            using (SqlConnection oconexion = new SqlConnection(Conexion.cadena))
            {
                try
                {
                    //sirve para hacer saltos de linea
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("select p.IdRol, NombreMenu from PERMISO p \r\n");
                    query.AppendLine("inner join ROL r on r.IdRol= p.IdRol\r\n");
                    query.AppendLine("inner join USUARIO u on u.IdRol = r.IdRol\r\n");
                    query.AppendLine("where u.IdUsuario = @idUsuario");

                    SqlCommand cmd = new SqlCommand(query.ToString(), oconexion);
                    cmd.Parameters.AddWithValue("@idUsuario", idUsuario);
                    cmd.CommandType = CommandType.Text;

                    oconexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Permiso()
                            {
                                oRol = new Rol() { IdRol = Convert.ToInt32(dr["IdRol"]) },
                                NombreMenu = dr["NombreMenu"].ToString(),                             
                            });
                        }
                    }


                }
                catch (Exception ex)
                {

                    lista = new List<Permiso>();
                }
                return lista;
            }
        }
    }
}
