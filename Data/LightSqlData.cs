using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

        public ObservableCollection<TEntity> GetObservableCollection() => new ObservableCollection<TEntity>(Entities.Select(e => e.Entity)); 

        public void SetValues(ArrayList columnsNames, ArrayList dynamicDataList)
        {
            ColumnsNames = columnsNames;
            RawData = dynamicDataList;

            foreach (ArrayList entity in dynamicDataList)
            {
                var entityToAdd = CastToEntity(entity);
                entityToAdd.SetOld();

                Entities.Add(entityToAdd);
            }
        }

        public IEnumerable<LightEntity<TEntity>> GetNewEntities()
        {
            var addedEntitiesList = new List<LightEntity<TEntity>>();
            for (int i = 0; i < RowsCount; i++)
            {
                var entity = Entities[i].GetBackendEntity();

                bool isEntityFound = false;
                foreach (var rawDataElement in RawData)
                {
                    var rawDataEntity = CastToEntity(rawDataElement as ArrayList).Entity;

                    if (rawDataEntity.Equals(entity))
                    {
                        isEntityFound = true;
                        break;
                    }
                }

                if (!isEntityFound)
                {
                    Entities.ElementAt(i).SetOld();
                    addedEntitiesList.Add(Entities.ElementAt(i));
                }
            }

            return addedEntitiesList;
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
