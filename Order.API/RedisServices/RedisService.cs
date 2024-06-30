using StackExchange.Redis;

namespace Order.API.RedisServices
{
    public class RedisService
    {
        private readonly ConnectionMultiplexer _connectionMultiplexer;

        public RedisService(IConfiguration configuration)
        {
            //stackexchange mutlaka singleton olarak kullanılmalı redis öneri budur
            var host = configuration.GetSection("RedisSettings")["Host"];
            var port = configuration.GetSection("RedisSettings")["Port"];

            var config = $"{host}:{port}";

            _connectionMultiplexer = ConnectionMultiplexer.Connect(config);
        }

        //connection multiplexer'ı return etmek için gerekli metot bu işlem redis instrumentation için yapıldı.
        public ConnectionMultiplexer GetConnectionMultiplexer => _connectionMultiplexer; //sadece get'i olan bir property'i gibi davranır.

        //redis birden fazla db getiriyor bundan dolayı bu işlem gerçekleştiriyoruz db'ler 0,1,2,3 gibi gelir
        public IDatabase GetDb(int db)
        {
            return _connectionMultiplexer.GetDatabase(db);
        }

    }
}
