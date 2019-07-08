using System;
using System.Collections.Generic;
using System.Text;
using TStack.MongoDB.Map;
using TStack.MongoDB.Tests.Connection;
using TStack.MongoDB.Tests.Entity;

namespace TStack.MongoDB.Tests.Maps
{
    public class PersonMapper : Mapper<Person>
    {
        public PersonMapper() 
        {
            Rule("test2").Key(x => x.Id).WithOne(x => x.PersonDetail, relationKey => relationKey.PersonId);
            Rule("test").Key("Id").WithCollection(x => x.Addresses).RelationKey("PersonId");
        }
    }
}
