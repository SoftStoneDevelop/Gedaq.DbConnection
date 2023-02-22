Constructors:

```C#

public ParametrAttribute(
  string methodName,
  string parametrName,
  Type parametrType,
  DbType dbType = DbType.Object,
  int size = -1,
  bool nullable = false,
  ParameterDirection direction = ParameterDirection.Input,
  string sourceColumn = "",
  bool sourceColumnNullMapping = false,
  DataRowVersion sourceVersion = DataRowVersion.Current,
  byte scale = 0, 
  byte precision = 0
  )

```
Parametrs:<br>
`methodName`: the name of query of which the parametr belongs<br>
`parametrName`: parametr name<br>
`parametrType`: parametr type<br>
The rest of the properties are DbParameter properties.

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
WHERE p.id != @id
ORDER BY p.id ASC
",
            "Query1",
            typeof(Person),
            MethodType.Async | MethodType.Sync,
            generate: false
            )]
[Parametr("Query1", parametrType: typeof(int), parametrName: "id")]
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
WHERE p.id != @id AND p.id != @id2
ORDER BY p.id ASC
",
            "Query2",
            typeof(Person),
            MethodType.Async | MethodType.Sync,
            generate: true
            )]
[Parametr("Query2", parametrType: typeof(int), parametrName: "id")]
[Parametr("Query2", parametrType: typeof(int), parametrName: "id2")]
public void Query2()
{
}

[QueryBatch("BatchGetData", Gedaq.Common.Enums.QueryType.Read | Gedaq.Common.Enums.QueryType.Scalar, Gedaq.Common.Enums.MethodType.Sync)]
[BatchPart("Query2", "BatchGetData", 1)]
[BatchPart("Query1", "BatchGetData", 2)]
public async Task SomeBatchMethod(DbConnection connection)
{
    var query2Result = connection.Query2(4, 15).ToList();
    var persons = connection.BatchGetData(4, 15, 20).Select(sel => sel.ToList()).ToList();
    //4 is parametr @id from Query2; 15 is parametr @id2 from Query2
    //20 is parametr @id from Query1
    //persons[0] is result of Query2;
    //persons[1] is result of Query1;
    
    var personsAsync = await connection.BatchGetData(4, 15, 20).Select(sel => sel.ToListAsync()).ToListAsync();
    //persons[0] is result of Query2;
    //persons[1] is result of Query1;
    
    var id = connection.ScalarBatchGetData(4, 15, 20);//return first person id from Query2
    var idAsync = await connection.ScalarBatchGetDataAsync(4, 15, 20);//return first person id from Query2
    
    DbBatch command = connection.CreateBatchGetDataBatch(prepare: false);//create batch
    command.SetBatchGetDataParametr(4, 15, 20);
    var personsFromBatch = command.ExecuteBatchGetDataBatch().Select(sel => sel.ToList()).ToList();
}
```
