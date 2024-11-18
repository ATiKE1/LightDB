using System.Collections;
using System.Data;

namespace LightDB
{
    public class LightSqlData
    {
        public ArrayList ColumnsNames { get; private set; }

        public ArrayList DynamicDataList { get; private set; }

        public int RowsCount => DynamicDataList.Count;

        public int ColumnsCount => ColumnsNames.Count;

        public LightSqlData() { }

        public LightSqlData(ArrayList columnsNames, ArrayList dynamicDataList)
        {
            ColumnsNames = columnsNames;
            DynamicDataList = dynamicDataList;
        } 

        public void SetValues(ArrayList columnsNames, ArrayList dynamicDataList)
        {
            ColumnsNames = columnsNames;
            DynamicDataList = dynamicDataList;
        }

        public DataTable ToDataTable()
        {
            DataTable dataTable = new DataTable();

            if (ColumnsNames == null)
                return dataTable;


            foreach (var column in ColumnsNames)            
                dataTable.Columns.Add(new DataColumn(column.ToString()));                        

            for (int i = 0; i < DynamicDataList.Count; i++)
            {
                DataRow dataRow = dataTable.NewRow();
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    dataRow[j] = (DynamicDataList[i] as ArrayList)[j];
                }

                dataTable.Rows.Add(dataRow);
            }            

            return dataTable;
        }
    }
}
