using Dapper;
using CapexIdentity.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapexIdentity.Infraestructure
{
    public static class Operation
    {
        /* ------------------------------------------------------------------------------------
         * 
         * PMO360
         * Av. Nueva Tajamar 481 Of 1403 - Vitacura, Santiago
         * http://www.pmo360.cl
         * 
         * -----------------------------------------------------------------------------------
         * 
         * CLIENTE          : AMSA - ANTOFAGASTA MINERALS
         * PRODUCTO         : CAPEX
         * RESPONABILIDAD   : IDENTIDAD & ROL MANAGEMENT
         * TIPO             : LOGICA DE NEGOCIO
         * DESARROLLADO POR : PMO360
         * FECHA            : 2018
         * VERSION          : 0.0.1
         * PROPOSITO        :
         * 
         * 
         */

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="spName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>

        public static T GetRecord<T>(string spName, List<ParameterInfo> parameters)
        {
            T objRecord = default(T);
            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                DynamicParameters p = new DynamicParameters();
                foreach (var param in parameters)
                {
                    p.Add("@" + param.ParameterName, param.ParameterValue);
                }

                objRecord = SqlMapper.Query<T>(objConnection, spName, p, commandType: CommandType.StoredProcedure).FirstOrDefault();
                objConnection.Close();
            }
            return objRecord;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="spName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static List<T> GetRecords<T>(string spName, List<ParameterInfo> parameters)
        {
            List<T> recordList = new List<T>();
            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                DynamicParameters p = new DynamicParameters();
                foreach (var param in parameters)
                {
                    p.Add("@" + param.ParameterName, param.ParameterValue);
                }

                recordList = SqlMapper.Query<T>(objConnection, spName, p, commandType: CommandType.StoredProcedure).ToList();
                objConnection.Close();
            }
            return recordList;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="spName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int GetIntRecord<T>(string spName, List<ParameterInfo> parameters)
        {
            int intRecord = 0;
            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                DynamicParameters p = new DynamicParameters();
                foreach (var param in parameters)
                {
                    p.Add("@" + param.ParameterName, param.ParameterValue);
                }

                using (var reader = SqlMapper.ExecuteReader(objConnection, spName, p, commandType: CommandType.StoredProcedure))
                {
                    if (reader != null && reader.Read())
                    {
                        intRecord = Convert.ToInt32(reader[0].ToString());
                    }
                }
                objConnection.Close();
            }
            return intRecord;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int ExecuteQuery(string spName, List<ParameterInfo> parameters)
        {
            int success = 0;
            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                DynamicParameters p = new DynamicParameters();
                foreach (var param in parameters)
                {
                    p.Add("@" + param.ParameterName, param.ParameterValue);
                }
                success = SqlMapper.Execute(objConnection, spName, p, commandType: CommandType.StoredProcedure);
                objConnection.Close();
            }
            return success;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static int ExecuteQueryWithIntOutputParam(string spName, List<ParameterInfo> parameters)
        {
            int success = 0;
            using (SqlConnection objConnection = new SqlConnection(Utils.ConnectionString()))
            {
                objConnection.Open();
                DynamicParameters p = new DynamicParameters();
                foreach (var param in parameters)
                {
                    p.Add("@" + param.ParameterName, param.ParameterValue);
                }
                success = SqlMapper.Execute(objConnection, spName, p, commandType: CommandType.StoredProcedure);
                objConnection.Close();
            }
            return success;
        }
    }
}
