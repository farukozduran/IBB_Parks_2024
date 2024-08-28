using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace IBB.Nesine.Services.Helpers
{
    public static class DataTableHelper
    {
        public static DataTable ToDataTable<T>(this List<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new();

            // Define the table columns based on the properties of T
            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }

            // Populate the table rows with data from the list
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }
                table.Rows.Add(row);
            }

            return table;
        }

    }
}
