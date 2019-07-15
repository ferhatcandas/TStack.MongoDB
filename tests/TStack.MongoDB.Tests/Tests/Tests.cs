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
using FluentAssertions;
using TStack.MongoDB.Tests.Connection;

namespace TStack.MongoDB.Tests.Tests
{
    public class Tests
    {
        [Fact]
        public void Insert_single_and_multiple_with_rule_success()
        {
            PersonRepository personRepository = new PersonRepository();

            Person person = new Person("ferhat", "candas", DateTime.Now.AddYears(-15), 2000.52);

            List<PersonAddress> personAddresses = new List<PersonAddress>()
            {
                new PersonAddress("Fatih mah","a","Istanbul"),
                new PersonAddress("Kıvanc mah","b","Izmir"),
                new PersonAddress("Kemaliye mah","c","Trabzon"),
            };

            var personDetail = new PersonDetail("candasferhat61@gmail.com", "905379106194");

            person.Addresses = personAddresses;

            person.PersonDetail = personDetail;

            personRepository.Insert(person, x => x.Name == "test" || x.Name == "test2");

            var personData = personRepository.First(x => x.Id == person.Id, x => x.Name == "test" || x.Name == "test2");

            personData.Name.Should().Be(person.Name);
            personData.Surname.Should().Be(person.Surname);
            personData.BirthDate.ToString("dd MM yyyy HH:mm:ss").Should().Be(person.BirthDate.ToString("dd MM yyyy HH:mm:ss"));
            personData.Salary.Should().Be(person.Salary);
            personData.Id.Should().Be(person.PersonDetail.PersonId);
            personData.PersonDetail.PersonId.Should().Be(person.PersonDetail.PersonId);
            personData.PersonDetail.Phone.Should().Be(person.PersonDetail.Phone);
            personData.PersonDetail.Email.Should().Be(person.PersonDetail.Email);
            personData.Addresses.ForEach(x =>
            {
                x.PersonId.Should().Be(person.Id);
            });
        }
        [Fact]
        public void Insert_single_and_multiple_with_generic_repository_and_rule_success()
        {
            var genericPerson = new MongoRepositoryBase<Person, TestConnection, PersonMapper>();

            Person person = new Person("ferhat", "candas", DateTime.Now.AddYears(-15), 2000.52);

            List<PersonAddress> personAddresses = new List<PersonAddress>()
            {
                new PersonAddress("Fatih mah","a","Istanbul"),
                new PersonAddress("Kıvanc mah","b","Izmir"),
                new PersonAddress("Kemaliye mah","c","Trabzon"),
            };


            var personDetail = new PersonDetail("candasferhat61@gmail.com", "905379106194");

            person.Addresses = personAddresses;

            person.PersonDetail = personDetail;

            genericPerson.Insert(person, x => x.Name == "test" || x.Name == "test2");

            var personData = genericPerson.First(x => x.Id == person.Id, x => x.Name == "test" || x.Name == "test2");

            personData.Name.Should().Be(person.Name);
            personData.Surname.Should().Be(person.Surname);
            personData.BirthDate.ToString("dd MM yyyy HH:mm:ss").Should().Be(person.BirthDate.ToString("dd MM yyyy HH:mm:ss"));
            personData.Salary.Should().Be(person.Salary);
            personData.Id.Should().Be(person.PersonDetail.PersonId);
            personData.PersonDetail.PersonId.Should().Be(person.PersonDetail.PersonId);
            personData.PersonDetail.Phone.Should().Be(person.PersonDetail.Phone);
            personData.PersonDetail.Email.Should().Be(person.PersonDetail.Email);
            personData.Addresses.ForEach(x =>
            {
                x.PersonId.Should().Be(person.Id);
            });

        }
    }
}
