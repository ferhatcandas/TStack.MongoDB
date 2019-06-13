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
            AddRule(new Rule<Person>().Name("test2").Key("Id").RelationKey("PersonId").WithOne(x => x.PersonDetail));
            AddRule(new Rule<Person>().Name("test").Key("Id").RelationKey("PersonId").WithCollection(x => x.Addresses));

        }
    }
}
