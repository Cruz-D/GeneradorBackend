
using System.Text;
using System.Collections.Generic; // Ensure List<T> is available  

public class ModelGenerator
{
    public string GenerateModel(string tableName, List<ColumnInfo> columns)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"public class {tableName}");
        sb.AppendLine("{");
        foreach (var column in columns)
        {
            var csharpType = MapToCSharpType(column.DataType); // Replace SqlTypeMapper with a local method  
            sb.AppendLine($"    public {csharpType} {column.Name} {{ get; set; }}");
        }
        sb.AppendLine("}");
        return sb.ToString();
    }

    private string MapToCSharpType(string sqlType)
    {
        return sqlType.ToLower() switch
        {
            "int" => "int",
            "bigint" => "long",
            "smallint" => "short",
            "tinyint" => "byte",
            "decimal" => "decimal",
            "numeric" => "decimal",
            "float" => "double",
            "real" => "float",
            "money" => "decimal",
            "smallmoney" => "decimal",
            "bit" => "bool",
            "char" => "string",
            "varchar" => "string",
            "text" => "string",
            "nchar" => "string",
            "nvarchar" => "string",
            "ntext" => "string",
            "date" => "DateTime",
            "datetime" => "DateTime",
            "datetime2" => "DateTime",
            "smalldatetime" => "DateTime",
            "time" => "TimeSpan",
            "timestamp" => "byte[]",
            "uniqueidentifier" => "Guid",
            "binary" => "byte[]",
            "varbinary" => "byte[]",
            "image" => "byte[]",
            _ => "object" // Default to object for unknown types  
        };
    }
}
