﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using ExtensionMethods;

namespace UGoFor.API.DAL
{
    public class BaseDAL
    {
        public string ConnectionString { get; set; }

        public BaseDAL(string connection = "")
        {
            //stupid hack cause transforms arent working
            if (bool.Parse(ConfigurationManager.AppSettings["OnAzure"])) 
            { 
                this.ConnectionString = (string.IsNullOrEmpty(connection)) ? ConfigurationManager.ConnectionStrings["UGoForDBConnection"].ConnectionString : connection;
            }
            else
            {
                this.ConnectionString = System.IO.File.ReadAllText(@"C:\temp\connection.txt");
            }
        }

        public List<T> ExecuteSPReturnData<T>(string storedProcedureName, SqlParameter[] parameters = null) where T : IFromDataReader<T>
        {
            try
            {
                List<T> retset = null;
                T instance = Activator.CreateInstance<T>();

                using (var conn = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(storedProcedureName, conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    conn.Open();
                    if (parameters != null)
                    {
                        foreach (SqlParameter param in parameters)
                        {
                            command.Parameters.Add(param);
                        }
                    }
                    IDataReader dr = command.ExecuteReader();
                    retset = dr.Select<T>(instance.FromDataReader).ToList();
                }

                return retset;
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public void ExecuteSPNonReturnData(string storedProcedureName, SqlParameter[] parameters)
        {
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(storedProcedureName, conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    conn.Open();
                    foreach (SqlParameter param in parameters)
                    {
                        command.Parameters.Add(param);
                    }
                    command.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int ExecuteSPNonReturnData(string storedProcedureName, SqlParameter[] parameters, string outPutID)
        {
            int retval = -1;
            
            try
            {
                using (var conn = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand(storedProcedureName, conn)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    conn.Open();
                    foreach (SqlParameter param in parameters)
                    {
                        command.Parameters.Add(param);
                    }
                    command.Parameters.Add(outPutID, SqlDbType.Int).Direction = ParameterDirection.Output;
                    command.ExecuteNonQuery();
                    retval = Convert.ToInt32(command.Parameters[outPutID].Value);
                    conn.Close();
                    return retval;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        } 

        public DataTable ExecuteSPReturnDataTable(string storedProcedureName, SqlParameter[] parameters = null)
        {
            DataTable dt = null;
            try
            {
                using (SqlConnection sqlcon = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(storedProcedureName, sqlcon))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (parameters != null)
                        {
                            foreach (SqlParameter param in parameters)
                            {
                                cmd.Parameters.Add(param);
                            }
                        }
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            dt = new DataTable();
                            da.Fill(dt);
                        }
                    }
                }

            }
            catch (Exception e)
            {                
                throw e;
            }

            return dt;

        }

    }

    public interface IFromDataReader<T>
    {
        T FromDataReader(IDataReader dr);
    }
}