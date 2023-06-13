Constructors:

```C#

public QueryBatchAttribute(
    string batchName,
    QueryType queryType,
    MethodType methodType,
    AccessModifier accessModifier = AccessModifier.AsContainingClass,
    AsyncResult asyncResultType = AsyncResult.ValueTask
)

```
Parametrs:<br>
`batchName`: name of the generated method<br>
`methodType`: type of generated method(sync/async, flags enum)<br>
`queryType`: type of generated method typr(read/nonquery/scalar, flags enum)<br>
`accessModifier`: Access Modifier of Generated Methods.<br>
`asyncResultType`: The type of the generated Task/ValueTask method.<br>

```C#

public BatchPartAttribute(string methodName, string batchName, int position)

```
Parametrs:<br>
`methodName`: name of query, query must be in same class/struct as `QueryBatch`<br>
`batchName`: the name of the batch of which the query is a part<br>
`position`: the position of the request in the batch<br>

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
            "Query1",
            typeof(Person),
            MethodType.Async | MethodType.Sync,
            generate: false
            )]
public void Query1()
{
}

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
            "Query2",
            typeof(Person),
            MethodType.Async | MethodType.Sync,
            generate: false
            )]
public void Query2()
{
}

[QueryBatch(
    "BatchGetData",
    Gedaq.Common.Enums.QueryType.Read | Gedaq.Common.Enums.QueryType.Scalar,
    Gedaq.Common.Enums.MethodType.Sync
    ),
 BatchPart("Query2", 1),
 BatchPart("Query1", 2)
]
public async Task SomeBatchMethod(DbConnection connection)
{
    var persons = connection.BatchGetData().Select(sel => sel.ToList()).ToList();
    //persons[0] is result of Query2;
    //persons[1] is result of Query1;
    
    var personsAsync = await connection.BatchGetData().Select(sel => await sel.ToListAsync()).ToListAsync();
    //persons[0] is result of Query2;
    //persons[1] is result of Query1;
    
    var id = connection.ScalarBatchGetData();//return first person id from Query2
    var idAsync = await connection.ScalarBatchGetDataAsync();//return first person id from Query2
}
```
