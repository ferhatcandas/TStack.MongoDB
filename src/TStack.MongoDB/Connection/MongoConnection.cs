using System;

namespace TStack.MongoDB.Connection
{
    public class MongoConnection
    {
        public MongoConnection(string host, int port, string database, string username = "", string password = "", TimeSpan? socketTimeOut = null, TimeSpan? connectTimeOut = null, TimeSpan? serverSelectionTimeOut = null)
        {
            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException(nameof(host));
            if (string.IsNullOrEmpty(database))
                throw new ArgumentNullException(nameof(database));
            Host = host;
            Port = port;
            Username = username;
            Database = database;
            Password = password;
            SocketTimeout = socketTimeOut ?? TimeOut();
            ConnectTimeout = connectTimeOut ?? TimeOut();
            ServerSelectionTimeout = serverSelectionTimeOut ?? TimeOut();
        }
        public string Host { get; private set; }
        public string Database { get; private set; }
        public int Port { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public TimeSpan SocketTimeout { get; private set; }
        public TimeSpan ConnectTimeout { get; private set; }
        public TimeSpan ServerSelectionTimeout { get; private set; }
        private TimeSpan TimeOut() => new TimeSpan(0, 0, 30);

    }
}
