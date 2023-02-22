Model classes in example:
```C#

public class PersonCustom
{
    public int Id { get; set; }

    public Name Name { get; set; }

    public Identification Identification { get; set; }
}

public class Name
{
    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    public string LastName { get; set; }
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
    p.id,
~StartInner::Name:?~
    p.firstname,
    p.middlename,
    p.lastname,
~EndInner::Name~
~StartInner::Identification:id~
    i.id,
~StartInner::Country:id~
    c.id,
    c.name,
~EndInner::Country~
    i.typename
~EndInner::Identification~
FROM person p
LEFT JOIN identification i ON i.id = p.identification_id
LEFT JOIN country c ON c.id = i.country_id
WHERE p.id != @id
ORDER BY p.id ASC
",
            "CustomMapping",
            typeof(PersonCustom),
            Gedaq.Common.Enums.MethodType.Async | Gedaq.Common.Enums.MethodType.Sync
            )]
[Parametr("CustomMapping", parametrType: typeof(int), parametrName: "id", dbType: System.Data.DbType.Int32)]
public async Task SomeMethod(DbConnection connection)
{
    var persons = connection.CustomMapping(3).ToList();
    var personsAsync = await connection.CustomMappingAsync(3).ToListAsync();
}
```

`PersonCustom.Name` is not real ref in database, it's just grouping in mapping.
That's why we can't check against the primary key `:?~`. Name will only be set to null if all of its properties are null.
