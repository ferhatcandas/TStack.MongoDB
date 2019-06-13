using System;
using System.Collections.Generic;
using System.Text;
using TStack.MongoDB.Connection;

namespace TStack.MongoDB.Map
{
    public class MongoRule<TContext>
        where TContext : MongoConnection, new()
    {
        string PrimaryKey { get; set; }
    }
    public class MongoRuler<TContext>
        where TContext : MongoConnection, new()
    {

    }
}
