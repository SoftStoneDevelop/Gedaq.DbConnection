Constructors:

```C#

QueryAttribute(
string query,
string methodName,
Type queryMapType = null,
MethodType methodType = MethodType.Sync,
QueryType queryType = QueryType.Read,
bool generate = true
)

```
Parametrs:<br>
`query`: sql query<br>
`methodName`: name of the generated method<br>
`queryMapType`: Type of result mapping collection<br>
`methodType`: type of generated method(sync/async, flags enum)<br>
`queryType`: type of generated method typr(read/nonquery/scalar, flags enum)<br>
`generate`: The need to generate a method. In case requests are used only as part of a batch, it may not be necessary to generate single methods for a request<br>

Model classes in example:
```C#

public class Person
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    public string LastName { get; set; }

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
    p.id,
    p.firstname,
~StartInner::Identification:id~
    i.id,
~StartInner::Country:id~
    c.id,
    c.name,
~EndInner::Country~
    i.typename,
~EndInner::Identification~
    p.middlename,
    p.lastname
FROM person p
LEFT JOIN identification i ON i.id = p.identification_id
LEFT JOIN country c ON c.id = i.country_id
ORDER BY p.id ASC
",
            "GetAllPerson",
            typeof(Person),
            MethodType.Async | MethodType.Sync,
            QueryType.Read | QueryType.Scalar | QueryType.NonQuery
            )]
public async Task SomeMethod(DbConnection connection)
{
    var persons = connection.GetAllPerson().ToList();
    var personsAsync = await connection.GetAllPersonAsync().ToListAsync();
    
    var id = connection.ScalarGetAllPerson();//return int because id in Person class is int
    var idAsync = await connection.ScalarGetAllPersonAsync();//return int because id in Person class is int
    
    var rowsAffected = connection.NonQueryGetAllPerson();//return int because id in Person class is int
    var rowsAffectedAsync = await connection.NonQueryGetAllPersonAsync();//return int because id in Person class is int
    
    var personsCommand = CreateGetAllPersonCommand(prepare: false);
    var personsFromCommand = personsCommand.ExecuteGetAllPersonCommand().ToList();
}
```

`~StartInner::Identification:id~`:
    `StartInner` - a marker for the parser about the beginning of columns of the inner entity. Each must be closed with an `EndInner` marker. 
    `::Identification` tells the parser which complex property the column group is responsible for.
    `:id~` tells the parser which column is the null entity identifier. It doesn't have to be the primary key of the table.
`~EndInner::Identification~`: end of inner entity with name `Identification`.
