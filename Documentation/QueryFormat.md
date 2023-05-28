Constructors:

```C#

public QueryFormatAttribute(
  int position,
  string parametrName = null
  )

```
Parametrs:<br>
`parametrName`: parametr name(purely stylistically, instead of format0, format1.. your name will be used for the parameter of the generated method format0, myName,format1..)<br>
`position`: position in String.Format<br>

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
WHERE {0}
{1}
",
            "Query1",
            typeof(Person),
            Gedaq.Common.Enums.MethodType.Async | Gedaq.Common.Enums.MethodType.Sync
            ),
            QueryFormat(1, "order"),
            QueryFormat(0, "filter")
            ]
public async Task Query1()
{
    //same query but different conditions
    var list = await connection.Query1Async(filter: "p.id != 0 AND p.id != 1", order: "ORDER BY p.id ASC").ToListAsync();
    var list = await connection.Query1Async(filter: "p.id > 2", order: string.Empty).ToListAsync();
}


```

This attribute works also with BatchQuery attribute.
