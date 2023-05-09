The `~Reinterpret::NewName~` syntax allows you to interpret the column name that follows it as `NewName`.

Model classes in example:
```C#

public class Person
{
    public int ReinterpId { get; set; }

    public string Name1 { get; set; }

    public string Name2 { get; set; }

    public string FinalName { get; set; }

    public Identification Identification { get; set; }
}

public class Identification
{
    public int Id { get; set; }
    public string TypeName { get; set; }
    public Country Country { get; set; }
}

public class Country
{
    public int Id { get; set; }
    public string Name { get; set; }
}

```

Usage:

```C#

[Query(
            @"
SELECT 
~Reinterpret::ReinterpId~
    p.id,
~Reinterpret::Name1~
    p.firstname,
~StartInner::Identification:id~
    i.id,
~StartInner::Country:id~
    c.id,
    c.name,
~EndInner::Country~
    i.typename,
~EndInner::Identification~
~Reinterpret::Name2~
    p.middlename,
~Reinterpret::FinalName~
    p.lastname
FROM person p
LEFT JOIN identification i ON i.id = p.identification_id
LEFT JOIN country c ON c.id = i.country_id
ORDER BY p.id ASC
",
            "GetAllPerson",
            typeof(Person),
            Gedaq.Common.Enums.MethodType.Async | Gedaq.Common.Enums.MethodType.Sync
            )]
public async Task SomeMethod(DbConnection connection)
{
    var persons = connection.GetAllPerson().ToList();
    var personsAsync = await connection.GetAllPersonAsync().ToListAsync();
    
    var id = connection.ScalarGetAllPerson();//return int because id in Person class is int
    var idAsync = await connection.ScalarGetAllPersonAsync();//return int because id in Person class is int
    
    var personsCommand = CreateGetAllPersonCommand(prepare: false);
    var personsFromCommand = personsCommand.ExecuteGetAllPersonCommand().ToList();
}
```
