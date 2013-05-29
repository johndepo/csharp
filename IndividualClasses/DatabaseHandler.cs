using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.Sql;
using System.Data;

namespace TestProj
{
    internal class DatabaseHandler
    {
        private string connString = Settings.ConnectionString;
        private SqlCommand sqlCommand;
        private SqlDataReader sqlDataReader;
        private SqlConnection conn;
        private DataTable t;
        internal bool connectionStatus;

        internal DatabaseHandler()
        {
            Connect();
        }

        #region Connection Handlers
        internal void Connect()
        {
            try
            {
                conn = new SqlConnection(connString);
                conn.Open();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        internal void Disconnect()
        {
            conn.Close();
        }

        internal string CheckStatus()
        {
            return conn.State.ToString();
        }
        #endregion

        #region Stored Procedures, Overloaded
        internal void RunStoredProcedure(string spName)
        {
            SqlDataReader r;
            try
            {
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                r = cmd.ExecuteReader();
                string line;

                while ((line = r.ReadLine()) != null)
                {
                    //do stuff
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                r.Close();
            }
        }

        internal void RunStoredProcedure(string spName, string paramName, DateTime paramDate)
        {
            SqlDataReader r;
            try
            {
                SqlCommand cmd = new SqlCommand(spName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter(paramName, paramDate));
                r = cmd.ExecuteReader();

                while ((line = r.ReadLine()) != null)
                {
                    //do stuff
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                r.Close();
            }
        }
        #endregion

        #region Inserts
        private void WriteToDatabase(string query, int count)
        {
            int rows;
            try
            {
                this.sqlCommand = new SqlCommand(query, conn);
                rows = sqlCommand.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                throw ex;
            }
        }

        private void BulkInsert(string destinationTable, DataTable dt)
        {
            try
            {
                SqlBulkCopy bulk = new SqlBulkCopy(conn);
                bulk.DestinationTableName = destinationTable;
                bulk.WriteToServer(dt);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}