namespace Mission_Service.Config
{
    public class MongoDBConfiguration
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public int MaxConnectionPoolSize { get; set; }
        public int MinConnectionPoolSize { get; set; }
        public TimeSpan MaxConnectionIdleTime { get; set; }
        public TimeSpan ServerSelectionTimeout { get; set; }

        public MongoDBConfiguration() { }

        public MongoDBConfiguration(
            string connectionString,
            string databaseName,
            int maxConnectionPoolSize,
            int minConnectionPoolSize,
            TimeSpan maxConnectionIdleTime,
            TimeSpan serverSelectionTimeout
        )
        {
            ConnectionString = connectionString;
            DatabaseName = databaseName;
            MaxConnectionPoolSize = maxConnectionPoolSize;
            MinConnectionPoolSize = minConnectionPoolSize;
            MaxConnectionIdleTime = maxConnectionIdleTime;
            ServerSelectionTimeout = serverSelectionTimeout;
        }
    }
}
