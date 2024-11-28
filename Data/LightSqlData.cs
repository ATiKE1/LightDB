using System.Data;
using System.Reflection;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace LightDatabase
{
    public class LightSqlData<TEntity>
    {
        private ArrayList RawData = new ArrayList();

        public ArrayList ColumnsNames { get; private set; }

        public ObservableCollection<LightEntity<TEntity>> Entities = new ObservableCollection<LightEntity<TEntity>>();

        public int RowsCount => Entities.Count;

        public int ColumnsCount => ColumnsNames.Count;

        public LightSqlData() { }

        public LightSqlData(ArrayList columnsNames, ArrayList dynamicDataList) => SetEntitiesFromDatabase(columnsNames, dynamicDataList);

        internal void SetEntitiesFromDatabase(ArrayList columnsNames, ArrayList dynamicDataList)
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

        internal LightEntity<TEntity> CastToEntity(ArrayList entity)
        {
            LightEntity<TEntity> tempEntity = new LightEntity<TEntity>();

            var properties = typeof(TEntity).GetRuntimeProperties();

            object[] args = new object[properties.Count()];

            for (int i = 0; i < properties.Count(); i++)
                args[i] = entity[i];

            tempEntity.SetEntity((TEntity)Activator.CreateInstance(typeof(TEntity), args), (TEntity)Activator.CreateInstance(typeof(TEntity), args));

            return tempEntity;
        }

        internal ArrayList GetRawData() => RawData;

        public ObservableCollection<TEntity> ToObservableCollection() => new ObservableCollection<TEntity>(Entities.Select(e => e.Entity));

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

        public void AddEntity(TEntity entity)
        {
            var lightEntity = new LightEntity<TEntity>();
            lightEntity.SetEntity(entity, entity);
            Entities.Add(lightEntity);
        }

        public void RemoveEntity(TEntity entity)
        {
            var lightEntity = Entities.FirstOrDefault(e => e.Entity.Equals(entity));
            if (lightEntity != null)
                lightEntity.Delete();
        }
    }
}
