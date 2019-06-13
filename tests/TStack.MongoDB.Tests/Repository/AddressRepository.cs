using System;
using System.Collections.Generic;
using System.Text;
using TStack.MongoDB.Repository;
using TStack.MongoDB.Tests.Connection;
using TStack.MongoDB.Tests.Entity;

namespace TStack.MongoDB.Tests.Repository
{
    public class AddressRepository : MongoRepositoryBase<PersonAddress, TestConnection>
    {
    }
}
