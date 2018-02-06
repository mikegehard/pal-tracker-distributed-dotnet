using DatabaseSupport;

namespace TestSupport
{
    public class TestDatabaseSupport
    {
        private readonly IDataSourceConfig _dataSourceConfig;
        
        public TestDatabaseSupport(IDataSourceConfig dataSourceConfig)
        {
            _dataSourceConfig = dataSourceConfig;
        }


        public void ExecSql(string sql)
        {
            var connection = _dataSourceConfig.GetConnection();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
            connection.Close();
        }

        public void TruncateAllTables()
        {
            var dbName = _dataSourceConfig.GetConnection().Database;
            
            var tableNameSql = $@"set foreign_key_checks = 0;
                select table_name FROM information_schema.tables
                where table_schema='{dbName}' and table_name != 'schema_version';";

            var truncateSql = "";

            using (var connection = _dataSourceConfig.GetConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = tableNameSql;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var table = reader.GetString(0);
                                truncateSql += $"truncate {table};";
                            }

                            reader.NextResult();
                        }
                    }
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = truncateSql;
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }
}