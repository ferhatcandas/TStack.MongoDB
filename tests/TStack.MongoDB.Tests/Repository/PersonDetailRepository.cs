using System;
using System.Collections.Generic;
using System.Text;
using TStack.MongoDB.Repository;
using TStack.MongoDB.Tests.Connection;
using TStack.MongoDB.Tests.Entity;
using TStack.MongoDB.Tests.Maps;

namespace TStack.MongoDB.Tests.Repository
{
    public class PersonDetailRepository : MongoRepositoryBase<PersonDetail, TestConnection>
    {

    }
}
