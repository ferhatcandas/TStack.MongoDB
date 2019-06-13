using System;
using System.Collections.Generic;
using System.Text;
using TStack.MongoDB.Connection;
using TStack.MongoDB.Entity;

namespace TStack.MongoDB.Tests.Connection
{
    public class TestConnection : MongoConnection
    {
        public TestConnection():base("localhost",27017,"testRepo")
        {

        }
    }
}
