using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TStack.MongoDB.Entity;
using Xunit;

namespace TStack.MongoDB.Tests.Entity
{
    public class Person : MongoEntity
    {
        public Person()
        {

        }
        public Person(string name, string surname, DateTime birthdate, double salary)
        {
            Name = name;
            Surname = surname;
            BirthDate = birthdate;
            Salary = salary;
        }
        public string Name { get; set; }
        public string Surname { get; set; }
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime BirthDate { get; set; }
        public double Salary { get; set; }
        [BsonIgnore]
        public PersonDetail PersonDetail { get; set; }
        [BsonIgnore]
        public List<PersonAddress> Addresses { get; set; }
    }
}
