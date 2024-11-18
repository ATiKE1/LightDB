using System;
using System.Data;
using System.Collections;
using System.Data.SqlClient;

namespace LightDB
{
    public class LightDB
    {
        private SqlConnection _connection = null;
        private string _connectionString = "Server=(localdb)\\mssqllocaldb;Database=master;Trusted_Connection=True;";

        private bool Init()
        {
            _connection = new SqlConnection(_connectionString);
            try
            {
                _connection.Open();
                return true;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }

        public bool Close()
        {
            try
            {
                _connection.Close();
                return true;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }

        public LightDB(string connectionString)
        {
            _connectionString = connectionString;

            if (!this.Init())
                throw new NotImplementedException("Error access to DB. Check your connection string.");

        }

        public LightSqlData ExecuteSqlCommand(string sql_command)
        {
            ArrayList dynamicResultList = new ArrayList();
            LightSqlData lightSqlData = new LightSqlData();

            SqlCommand command = _connection.CreateCommand();
            command.CommandText = sql_command;

            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                DataTable schema = reader.GetSchemaTable();

                while (reader.Read())
                {
                    ArrayList currentRow = new ArrayList();

                    for (int i = 0; i < schema.Rows.Count; i++)
                    {
                        object data = reader.GetValue(i);
                        currentRow.Add(data);
                    }

                    dynamicResultList.Add(currentRow);

                }

                var columnsNames = new ArrayList();                
                for (int i = 0; i < reader.GetSchemaTable().Rows.Count; i++)
                    columnsNames.Add(reader.GetName(i));

                lightSqlData.SetValues(columnsNames, dynamicResultList);
            }           

            reader.Close();

            return lightSqlData;
        }
    }
}
