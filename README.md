# Overview
This library is designed for generic architecture based on mongodb. **It can be used relationally** because it has mapping feature. So, can make  relational queries with **defined rules**.

# Covarage
 - Generic Repository
 - CRUD
 - Relational Mapping
 - Cluster Mode (Not yet)

# Installation

[Nuget Package](https://www.nuget.org/packages/TStack.MongoDB/)
#### Package Manager
```PM
Install-Package TStack.MongoDB -Version 1.0.0
```
#### .NET CLI
```PM
dotnet add package TStack.MongoDB --version 1.0.0
```
#### PackageReference
```PM
<PackageReference Include="TStack.MongoDB" Version="1.0.0" />
```
#### Paket CLI
```PM
paket add TStack.MongoDB --version 1.0.0
# Usage

First must define connection to access mongodb engine.

For an example :
```csharp
public class TestConnection : MongoConnection
{
    public TestConnection():base("localhost",27017,"testRepo")
    {

    }
}
```

Your enitity objects must be inherit from "MongoEntity" class.

For an example :

```csharp
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
  
    [BsonDateTimeOptions(Kind = DateTimeKind.Local)]//on insert save datetime on your local datetime otherwise universal
    public DateTime BirthDate { get; set; }
    public double Salary { get; set; }
    [BsonIgnore]//for relation must be add
    public PersonDetail PersonDetail { get; set; }
    [BsonIgnore]//for relation must be add
    public List<PersonAddress> Addresses { get; set; }
}

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

public class PersonDetail : MongoEntity
{
    public PersonDetail()
    {

    }
    public PersonDetail(string email, string phone)
    {
        Email = email;
        Phone = phone;
    }
    public string PersonId { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}
// base class
public class MongoEntity : IMongoEntity
{
    /// <summary>
    /// id in string format
    /// </summary>
    [BsonElement(Order = 0)]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    /// <summary>
    /// id in objectId format
    /// </summary>
    public ObjectId ObjectId => ObjectId.Parse(Id);
}
```

Now, define repository to access methods to use.

***### Without mapping***
```csharp
public class PersonRepository : MongoRepositoryBase<Person, TestConnection>
{

}
```

***### With Mapping***

Before define map class
```csharp
public class PersonMapper : Mapper<Person>
{
    public PersonMapper() 
    {
        //example usage
        Rule("PersonDetail").Key(primaryKey => primaryKey.Id).WithOne(field => field.PersonDetail, relationKey => relationKey.PersonId);
        //example usage
        Rule("PersonAddresses").Key("Id").WithCollection(field => field.Addresses).RelationKey("PersonId");
    }
}

```
By the way you must fill **RelationKey("")** function, its required for init your rule.

Now define your repository like this.
```csharp
public class PersonRepository : MongoRepositoryBase<Person, TestConnection, PersonMapper>
{

}
```
## Fundamentals
Let's use mapped repository with rule.

For an example :
```csharp
PersonRepository personRepository = new PersonRepository();

Person person = new Person("ferhat", "candas", DateTime.Now.AddYears(-15), 2000.52);

List<PersonAddress> personAddresses = new List<PersonAddress>()
{
    new PersonAddress("Fatih mah","besikduzu apt.","Trabzon"),
    new PersonAddress("Laik sokak","Çağrı apt.","Izmir"),
    new PersonAddress("Yilmaz sokak.","esenyurt apt.","Tokat"),
};

var personDetail = new PersonDetail("candasferhat61@gmail.com", "90537*******");

person.Addresses = personAddresses;

person.PersonDetail = personDetail;

personRepository.Insert(person, rule => rule.Name == "PersonDetail" || rule.Name == "PersonAddresses");
```
That's it you have create three collection on mongodb related with rules.

You can get data like this.

```csharp
PersonRepository personRepository = new PersonRepository();
//example recordId
string recordId = "5d23a3036f4bca70448cf6de";
var personData = personRepository.First(person => person.Id == recordId, rule => rule.Name == "PersonDetail" || rule.Name == "PersonAddresses");
```
personData includes related objects on rule.

# Author

Ferhat Candaş - Software Developer
 - Mail : candasferhat61@gmail.com
 - LinkedIn : https://www.linkedin.com/in/ferhatcandas


 