using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace TStack.MongoDB.Connection
{
    public class MongoConnection : MongoClientSettings
    {
        public MongoConnection(string host, int port, string database, string username = "", string password = "", string replicaSetName = "", TimeSpan? socketTimeOut = null, TimeSpan? connectTimeOut = null, TimeSpan? serverSelectionTimeOut = null)
        {
            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException(nameof(host));
            if (string.IsNullOrEmpty(database))
                throw new ArgumentNullException(nameof(database));
            DatabaseName = database;
            MongoCredential mongoCredential = null;
            if (!string.IsNullOrEmpty(username))
                mongoCredential = MongoCredential.CreateCredential(database, username, password);
            Server = new MongoServerAddress(host, port);
            SocketTimeout = socketTimeOut ?? TimeOut();
            ConnectTimeout = connectTimeOut ?? TimeOut();
            ServerSelectionTimeout = serverSelectionTimeOut ?? TimeOut();
            Credential = mongoCredential;
            if (!string.IsNullOrEmpty(replicaSetName))
                ReplicaSetName = replicaSetName;
        }
        public string DatabaseName { get; private set; }
        private TimeSpan TimeOut() => new TimeSpan(0, 0, 30);
    }
}
