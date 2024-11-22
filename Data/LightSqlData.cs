using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace LightDB
{
    public class LightSqlData<TEntity>
    {
        private ArrayList RawData = new ArrayList();

        public ArrayList ColumnsNames { get; private set; }

        public List<LightEntity<TEntity>> Entities = new List<LightEntity<TEntity>>();

        public int RowsCount => Entities.Count;

        public int ColumnsCount => ColumnsNames.Count;

        public LightSqlData() { }

        public LightSqlData(ArrayList columnsNames, ArrayList dynamicDataList)
        {
            SetValues(columnsNames, dynamicDataList);
        }

        public void SetValues(ArrayList columnsNames, ArrayList dynamicDataList)
        {
            ColumnsNames = columnsNames;
            RawData = dynamicDataList;

            foreach (ArrayList entity in dynamicDataList)
            {
                Entities.Add(CastToEntity(entity));
            }
        }        

        public LightEntity<TEntity> CastToEntity(ArrayList entity)
        {
            LightEntity<TEntity> tempEntity = new LightEntity<TEntity>();           

            var properties = typeof(TEntity).GetRuntimeProperties();

            object[] args = new object[properties.Count()];

            for (int i = 0; i < properties.Count(); i++)            
                args[i] = entity[i];                        

            tempEntity.SetEntity((TEntity)Activator.CreateInstance(typeof(TEntity), args), (TEntity)Activator.CreateInstance(typeof(TEntity), args));

            return tempEntity;
        }        

        public List<LightEntity<TEntity>> GetValues() => Entities;

        public ArrayList GetRawData() => RawData; 

        public DataTable ToDataTable()
        {
            DataTable dataTable = new DataTable();

            if (ColumnsNames == null)
                return dataTable;


            foreach (var column in ColumnsNames)
                dataTable.Columns.Add(new DataColumn(column.ToString()));

            for (int i = 0; i < RawData.Count; i++)
            {
                DataRow dataRow = dataTable.NewRow();
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    dataRow[j] = (RawData[i] as ArrayList)[j];
                }

                dataTable.Rows.Add(dataRow);
            }

            return dataTable;
        }
    }
}
