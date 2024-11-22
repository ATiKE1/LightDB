using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Collections.Generic;

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

        public LightSqlData<TEntity> ExecuteSqlCommand<TEntity>(string sql_command)
        {
            ArrayList dynamicResultList = new ArrayList();
            LightSqlData<TEntity> lightSqlData = new LightSqlData<TEntity>();

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
                {                    
                    columnsNames.Add(reader.GetName(i));
                }

                lightSqlData.SetValues(columnsNames, dynamicResultList);
            }

            reader.Close();

            return lightSqlData;
        }

        public LightSqlData<object> ExecuteSqlCommand(string sql_command)
        {
            ArrayList dynamicResultList = new ArrayList();
            LightSqlData<object> lightSqlData = new LightSqlData<object>();

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

        public void Commit<TEntity>(IEnumerable<LightEntity<TEntity>> entities, string tableName)
        {
            var changedEntities = new List<LightEntity<TEntity>>();
            var deletedEntities = new List<LightEntity<TEntity>>();
            foreach (var entity in entities)
            {
                if (entity.IsChanged && !entity.IsDeleted)
                    changedEntities.Add(entity);

                if (entity.IsDeleted)
                    deletedEntities.Add(entity);
            }

            Update(changedEntities, tableName);
            Delete(deletedEntities, tableName);
        }

        private void Delete<TEntity>(IEnumerable<LightEntity<TEntity>> entities, string tableName)
        {
            var entitiesProperties = entities.Select(e => e.GetChangedProperties());

            foreach (var entityProperty in entitiesProperties)
            {
                var unchangedProperties = entityProperty.UnchangedProperties;

                var conditions = $"{unchangedProperties.ElementAt(0).Key} = '{unchangedProperties.ElementAt(0).Value}'";
                for (int i = 1; i < entityProperty.UnchangedProperties.Count; i++)
                {
                    conditions += $" AND {unchangedProperties.ElementAt(i).Key} = '{unchangedProperties.ElementAt(i).Value}'";
                }

                string sql = $"DELETE FROM {tableName} WHERE {conditions}";

                ExecuteSqlCommand(sql);
            }
        }

        private void Update<TEntity>(List<LightEntity<TEntity>> entities, string tableName)
        {
            var entitiesProperties = entities.Select(e => e.GetChangedProperties());

            foreach (var entityProperty in entitiesProperties)
            {
                StringBuilder properties = new StringBuilder();
                foreach (var property in entityProperty.ChangedProperties)
                {
                    properties.Append($"{property.Key} = '{property.Value}',");
                }

                if (properties.Length > 0)
                    properties.Remove(properties.Length - 1, 1);

                var unchangedProperties = entityProperty.UnchangedProperties;

                var conditions = $"{unchangedProperties.ElementAt(0).Key} = '{unchangedProperties.ElementAt(0).Value}'";
                for (int i = 1; i < entityProperty.UnchangedProperties.Count; i++)
                {
                    conditions += $" AND {unchangedProperties.ElementAt(i).Key} = '{unchangedProperties.ElementAt(i).Value}'";
                }

                if (!string.IsNullOrWhiteSpace(properties.ToString()) && !string.IsNullOrWhiteSpace(conditions))
                {
                    var updateSql = $"UPDATE {tableName} SET {properties} WHERE {conditions}";

                    ExecuteSqlCommand(updateSql);
                }
            }
        }
    }
}
