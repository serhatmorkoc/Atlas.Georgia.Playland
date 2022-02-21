using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
namespace Playland.Database
{
    public class SQLDbFactory : IDisposable
    {
        protected virtual void Dispose(bool disposed)
        {

        }


        public string CommandTextPrefix = "tsp_";
        private int commandTimeOut = 5;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private string connectionString = string.Empty;

        public SQLDbFactory(int typeId = 0)
        {
            connectionString = ConfigurationManager.ConnectionStrings["BatumDbConnectionString"].ConnectionString;
            if (typeId == 1)
                connectionString = ConfigurationManager.ConnectionStrings["TiflisDbConnectionString"].ConnectionString;
            if (typeId == 2)
                connectionString = ConfigurationManager.ConnectionStrings["KutaisiDbConnectionString"].ConnectionString;
            if (typeId == 3)
                connectionString = ConfigurationManager.ConnectionStrings["LegendDbConnectionString"].ConnectionString;
        }

        private DataSet ExecuteDataSet(string commandText, List<SqlParameter> parameters)
        {
            DataSet dataSet = null;
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection();
                connection.ConnectionString = connectionString;
                SqlCommand command = new SqlCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.Connection = connection;
                command.CommandTimeout = commandTimeOut * 60;
                command.CommandText = CommandTextPrefix + commandText;
                if (parameters != null && parameters.Count != 0)
                {
                    foreach (SqlParameter item in parameters)
                    {
                        command.Parameters.Add(item);
                    }
                }
                connection.Open();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
                dataSet = new DataSet();
                dataAdapter.Fill(dataSet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                connection.Close();
            }
            return dataSet;
        }

        public DataSet GetDataSet(string commandText, List<SqlParameter> parameters)
        {
            return this.ExecuteDataSet(commandText, parameters);
        }

        public DataTable GetDataTable(string commandText, List<SqlParameter> parameters)
        {
            DataSet dataSet = this.ExecuteDataSet(commandText, parameters);

            if (dataSet != null && dataSet.Tables.Count > 0)
            {
                return dataSet.Tables[0];
            }
            return null;
        }

        public List<DataRow> GetDataRows(string commandText, List<SqlParameter> parameters)
        {
            DataTable dataTable = GetDataTable(commandText, parameters);
            if (dataTable != null)
            {
                return dataTable.Rows.OfType<DataRow>().ToList();
            }

            return null;
        }

    }
}
