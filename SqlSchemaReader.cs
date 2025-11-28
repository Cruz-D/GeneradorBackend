using Microsoft.Data.SqlClient;


public class SqlSchemaReader
{
    private readonly string _connectionString;

    public SqlSchemaReader(string connectionString)
    {
        _connectionString = connectionString;
    }

    public List<TableInfo> GetTables()
    {
        var tables = new List<TableInfo>();
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var cmd = new SqlCommand(
            "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'", connection);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            tables.Add(new TableInfo { Name = reader.GetString(0) });
        }
        return tables;
    }

    public List<ColumnInfo> GetColumns(string tableName)
    {
        var columns = new List<ColumnInfo>();
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var cmd = new SqlCommand(
            @"SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName", connection);
        cmd.Parameters.AddWithValue("@TableName", tableName);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            columns.Add(new ColumnInfo
            {
                Name = reader.GetString(0),
                DataType = reader.GetString(1)
            });
        }
        return columns;
    }
}

public class TableInfo
{
    public string Name { get; set; }
}

public class ColumnInfo
{
    public string Name { get; set; }
    public string DataType { get; set; }
}

