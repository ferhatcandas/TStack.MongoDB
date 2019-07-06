using System;
using System.Collections.Generic;
using System.Text;
using TStack.MongoDB.Map;
using TStack.MongoDB.Tests.Connection;
using TStack.MongoDB.Tests.Entity;

namespace TStack.MongoDB.Tests.Maps
{
    public class JMapper : Mapper<Person>
    {
        public JMapper() 
        {
            Rule().Name("test2").Key(x=>x.Id).RelationKey("PersonId").WithOne(x => x.PersonDetail);
            Rule().Name("test").Key("Id").RelationKey("PersonId").WithCollection(x => x.Addresses);
        }
    }
}
