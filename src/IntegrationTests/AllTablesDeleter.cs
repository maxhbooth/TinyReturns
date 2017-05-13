using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;

namespace Dimensional.TinyReturns.IntegrationTests
{
    public class AllTablesDeleter
    {
        public class TableInfoDto
        {
            public TableInfoDto()
            {
            }

            public TableInfoDto(string schemaName, string tableName)
            {
                SchemaName = schemaName;
                TableName = tableName;
            }

            public string SchemaName { get; set; }
            public string TableName { get; set; }

            protected bool Equals(TableInfoDto other)
            {
                return string.Equals(SchemaName, other.SchemaName) && string.Equals(TableName, other.TableName);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((TableInfoDto)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((SchemaName != null ? SchemaName.GetHashCode() : 0) * 397) ^ (TableName != null ? TableName.GetHashCode() : 0);
                }
            }

        }

        public void DeleteAllDataFromTables(
            string connectionString,
            TableInfoDto[] tablesToSkip)
        {
            const string sql = @"
WITH fk_tables AS (
    SELECT    s1.name AS from_schema    
    ,        o1.Name AS from_table    
    ,        s2.name AS to_schema    
    ,        o2.Name AS to_table    
    FROM    sys.foreign_keys fk    
    INNER    JOIN sys.objects o1                                 ON        fk.parent_object_id = o1.OBJECT_ID    
    INNER    JOIN sys.schemas s1                                 ON        o1.schema_id = s1.schema_id    
    INNER    JOIN sys.objects o2                                 ON        fk.referenced_object_id = o2.OBJECT_ID    
    INNER    JOIN sys.schemas s2                                 ON        o2.schema_id = s2.schema_id    
    /*For the purposes of finding dependency hierarchy we're not worried about self-referencing tables*/
    WHERE    NOT    (    s1.name = s2.name                 
            AND        o1.name = o2.name)
)
,ordered_tables AS
(        SELECT    s.name AS schemaName
        ,        t.name AS tableName
        ,        0 AS LEVEL    
        FROM    (    SELECT    *                
                    FROM    sys.tables                 
                    WHERE    name <> 'sysdiagrams') t    
        INNER    JOIN sys.schemas s                                  ON        t.schema_id = s.schema_id    
        LEFT    OUTER JOIN fk_tables fk                               ON        s.name = fk.from_schema    
                                                                       AND   t.name = fk.from_table    
        WHERE    fk.from_schema IS NULL
        UNION    ALL
        SELECT    fk.from_schema
        ,        fk.from_table
        ,        ot.LEVEL + 1    
        FROM    fk_tables fk    
        INNER    JOIN ordered_tables ot                              ON        fk.to_schema = ot.schemaName    
                                                                       AND   fk.to_table = ot.tableName
)
SELECT    DISTINCT
        ot.schemaName AS SchemaName,
        ot.tableName AS TableName,
        ot.LEVEL AS Level
FROM    ordered_tables ot
INNER    JOIN (
        SELECT    schemaName,tableName,MAX(LEVEL) maxLevel        
        FROM    ordered_tables        
        GROUP    BY schemaName,tableName
        ) mx
ON        ot.schemaName = mx.schemaName
AND        ot.tableName = mx.tableName
AND        mx.maxLevel = ot.LEVEL
ORDER BY LEVEL DESC";

            IEnumerable<TableInfoDto> tableInfoDtos = null;

            SqlDatabaseHelper.ConnectionExecuteWithLog(
                connectionString,
                connection =>
                {
                    tableInfoDtos = connection.Query<TableInfoDto>(sql);
                },
                sql);

            var stringBuilder = new StringBuilder();

            foreach (var tableInfoDto in tableInfoDtos)
            {
                if (tablesToSkip.Any(skip => skip.Equals(tableInfoDto)))
                    continue;

                stringBuilder.AppendFormat("DELETE FROM [{0}].[{1}]", tableInfoDto.SchemaName, tableInfoDto.TableName);
            }

            SqlDatabaseHelper.ConnectionExecuteWithLog(
                connectionString,
                connection =>
                {
                    connection.Execute(stringBuilder.ToString());
                },
                stringBuilder.ToString());
        }
    }
}