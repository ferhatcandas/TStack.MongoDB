using System;
using System.Collections.Generic;
using System.Text;
using TStack.MongoDB.Map;
using TStack.MongoDB.Repository;
using TStack.MongoDB.Tests.Entity;
using TStack.MongoDB.Tests.Maps;
using TStack.MongoDB.Tests.Repository;
using Xunit;
using System.Linq;
using System.Linq.Expressions;

namespace TStack.MongoDB.Tests.Tests
{
    public class Tests
    {
        private PersonRepository personRepository = new PersonRepository();
        private AddressRepository addressRepository = new AddressRepository();
        private PersonDetailRepository personDetailRepository = new PersonDetailRepository();
        public void Add_Record_To_Collection()
        {
            //Person person = new Person("ferhat", "candas", DateTime.Now.AddYears(-15), 2000.52);
            //List<PersonAddress> personAddresses = new List<PersonAddress>()
            //{
            //    new PersonAddress(person.Id,"Fatih mah","a","Istanbul"),
            //    new PersonAddress(person.Id,"Kıvanc mah","b","Izmir"),
            //    new PersonAddress(person.Id,"Kemaliye mah","c","Trabzon"),
            //};
            //personRepository.Insert(person);
            //addressRepository.Insert(personAddresses);
            personDetailRepository.Insert(new PersonDetail("5cdb4fd332ef854458a18270", "candasferhat61@gmail.com", "905379106194"));
        }
        [Fact]
        public void MapperTest()
        {
            var person = personRepository.First(x => x.Id == "5cdb4fd332ef854458a18270", x => x.RuleName == "test" || x.RuleName == "test2");


            //person.Include<ExMapper,PersonAddress>(x=>x.RuleName == "test");

            //person.Include(manager);
            //person.Include<PersonMapper<Person,PersonAddress>();
            //person.Include(personMapper);
        }
    }
}
