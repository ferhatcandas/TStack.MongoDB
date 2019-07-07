using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using TStack.MongoDB.Entity;

namespace TStack.MongoDB.Tests.Entity
{
    public class PersonAddress : MongoEntity
    {
        public PersonAddress()
        {

        }
        public PersonAddress(string street, string apartment, string city)
        {
            Street = street;
            Apartment = apartment;
            City = city;
        }
        public string PersonId { get; set; }
        public string Street { get; set; }
        public string Apartment { get; set; }
        public string City { get; set; }
    }
}
