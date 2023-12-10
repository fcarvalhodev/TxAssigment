using StackExchange.Redis;


namespace TxAssignmentInfra.Connectors
{
    public static class RedisConnectorHelper
    {
        private static Lazy<ConnectionMultiplexer> lazyConnection;

        static RedisConnectorHelper()
        {
            lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect("redis-12378.c56.east-us.azure.cloud.redislabs.com:12378,password=WVDdCALTgKG7vjsry9DUYuiUz9Lae0Lm");
            });
        }

        public static ConnectionMultiplexer Connection => lazyConnection.Value;
    }
}