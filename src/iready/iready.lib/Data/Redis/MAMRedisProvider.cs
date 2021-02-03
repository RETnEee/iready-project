namespace iready.lib.Data.Redis
{
    public class RedisProvider
    {
        public static MAMRedisDistributedCache Build(List<ConnectionModel> connections)
        {
            if (null == connections ||
                0 == connections.Count)
            {
                throw new Exception("没有找到redis连接信息");
            }

            var mamCache = new MAMRedisDistributedCache();

            var masterServerName = connections.FirstOrDefault(f => !string.IsNullOrEmpty(f.ServiceName))?.ServiceName;
            var password = connections.FirstOrDefault(f => !string.IsNullOrEmpty(f.Password))?.Password;
            var db = int.Parse(connections.FirstOrDefault().Database);

            if (!string.IsNullOrEmpty(masterServerName))
            {
                var configOptions = new ConfigurationOptions
                {
                    ServiceName = masterServerName,
                    TieBreaker = "",
                    CommandMap = CommandMap.Sentinel,
                    DefaultVersion = new Version(3, 0),
                    AllowAdmin = true,
                    SyncTimeout = 1000,
                    ReconnectRetryPolicy = new ExponentialRetry(10000),
                    ConfigCheckSeconds = 10,
                };

                foreach (var connectItem in connections)
                {
                    configOptions.EndPoints.Add(connectItem.IP, int.Parse(connectItem.Port));
                }

                var _sentinelConn = ConnectionMultiplexer.Connect(configOptions);

                var subscriber = _sentinelConn.GetSubscriber();

                IServer masterServer = GetMasterServer(_sentinelConn);

                Tuple<List<string>, List<string>> masterServersAndSlaverServers = GetMasterServerAndSlavesServer(masterServer, masterServerName);

                var config = ConstructConfigOptions(masterServerName, masterServersAndSlaverServers.Item1, masterServersAndSlaverServers.Item2, password);

                mamCache.Set(db, config);
            }
            else
            {
                var redisConfiguration = new ConfigurationOptions
                {
                    AbortOnConnectFail = true,
                    AllowAdmin = false,
                    ConnectRetry = 5,
                    ConnectTimeout = 2000,
                    DefaultDatabase = 0,
                    KeepAlive = 20,
                    SyncTimeout = 30 * 1000,
                    Ssl = false,
                    Password = password
                };

                connections.ForEach(f => redisConfiguration.EndPoints.Add(f.IP, int.Parse(f.Port)));

                mamCache.Set(db, redisConfiguration);
            }

            return mamCache;
        }

        private static IServer GetMasterServer(ConnectionMultiplexer connectionMultiplexer)
        {
            IServer masterServer = null;
            var endpoints = connectionMultiplexer.GetEndPoints();
            foreach (var endpoint in endpoints)
            {
                var server = connectionMultiplexer.GetServer(endpoint);
                if (server.IsSlave || !server.IsConnected)
                    continue;
                masterServer = server;
                break;
            }
            return masterServer;
        }

        private static Tuple<List<string>, List<string>> GetMasterServerAndSlavesServer(IServer masterServer, string masterServerName)
        {
            List<string> redisMasters = new List<string>();

            var masterEndpoint = masterServer.SentinelGetMasterAddressByName(masterServerName);

            if (masterEndpoint is DnsEndPoint masterDnsEndPoint)
            {
                redisMasters.Add($"{masterDnsEndPoint.Host}:{masterDnsEndPoint.Port}");
            }
            else
            {
                if (masterEndpoint is IPEndPoint masterIpEndPoint)
                    redisMasters.Add($"{masterIpEndPoint.Address}:{masterIpEndPoint.Port}");
            }

            var sentinelSlaves = masterServer.SentinelSlaves(masterServerName);

            var ip = string.Empty;
            var port = string.Empty;
            var flags = string.Empty;

            var redisSlaves = new List<string>();

            foreach (var config in sentinelSlaves)
            {
                foreach (var kvp in config)
                {
                    if (kvp.Key == "ip")
                    {
                        ip = kvp.Value;
                    }
                    if (kvp.Key == "port")
                    {
                        port = kvp.Value;
                    }
                    if (kvp.Key == "flags")
                    {
                        flags = kvp.Value;
                    }
                }

                if (ip != null && port != null && !flags.Contains("s_down") && !flags.Contains("o_down"))
                {
                    redisSlaves.Add($"{ip}:{port}");
                }
            }

            return Tuple.Create(redisMasters, redisSlaves);
        }

        private static ConfigurationOptions ConstructConfigOptions(string masterName, List<string> masterServers, List<string> slavesServers, string password)
        {
            string endConnectionString;
            if (string.IsNullOrWhiteSpace(password))
            {
                endConnectionString =
                    string.Format("{1},{2}", masterName, string.Join(", ", masterServers), string.Join(", ", slavesServers));
            }
            else
            {
                endConnectionString =
                    string.Format("{0},{1},password={2}", string.Join(", ", masterServers), string.Join(", ", slavesServers), password);
            }

            var redisConfiguration = ConfigurationOptions.Parse(endConnectionString);
            redisConfiguration.AbortOnConnectFail = true;
            redisConfiguration.AllowAdmin = false;
            redisConfiguration.ConnectRetry = 5;
            redisConfiguration.ConnectTimeout = 2000;
            redisConfiguration.DefaultDatabase = 0;
            redisConfiguration.KeepAlive = 20;
            redisConfiguration.SyncTimeout = 30 * 1000;
            redisConfiguration.Ssl = false;

            return redisConfiguration;
        }
    }
}