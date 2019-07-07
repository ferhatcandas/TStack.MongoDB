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
        [Fact]
        public void Add_Record_To_Collection()
        {
            Person person = new Person("ferhat", "candas", DateTime.Now.AddYears(-15), 2000.52);
            List<PersonAddress> personAddresses = new List<PersonAddress>()
            {
                new PersonAddress("Fatih mah","a","Istanbul"),
                new PersonAddress("Kıvanc mah","b","Izmir"),
                new PersonAddress("Kemaliye mah","c","Trabzon"),
            };

            var personDetail = new PersonDetail("candasferhat61@gmail.com", "905379106194");
            person.Addresses = personAddresses;
            //person.PersonDetail = personDetail;
            //personRepository.Insert(person,x=>x.Name == "test2");
            personRepository.Insert(person,x=>x.Name == "test");


            //addressRepository.Insert(personAddresses);
            //personDetailRepository.Insert(new PersonDetail("5cdb4fd332ef854458a18270", "candasferhat61@gmail.com", "905379106194"));
        }
        //[Fact]
        public void MapperTest()
        {
            var person = personRepository.First(x => x.Name == "test" || x.Name == "test2");


            //person.Include<ExMapper,PersonAddress>(x=>x.RuleName == "test");

            //person.Include(manager);
            //person.Include<PersonMapper<Person,PersonAddress>();
            //person.Include(personMapper);
        }
    }
}
