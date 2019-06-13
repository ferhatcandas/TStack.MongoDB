using System;
using System.Collections.Generic;
using System.Text;
using TStack.MongoDB.Entity;

namespace TStack.MongoDB.Tests.Entity
{
    public class PersonDetail : MongoEntity
    {
        public PersonDetail()
        {

        }
        public PersonDetail(string personId,string email, string phone)
        {
            Email = email;
            Phone = phone;
            PersonId = personId;
        }
        public string PersonId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
