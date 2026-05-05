```C#
public class PersonExample
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    public string LastName { get; set; }

    public Identification Identification { get; set; }
}

public class PersonFlat
{
    [Alias(order: 0)]
    public int Id { get; set; }

    [Alias(order: 2)] // We have specifically changed the order in the query, for example
    public string FirstName { get; set; }

    [Alias(order: 1)]
    public string MiddleName { get; set; }

    [Alias(order: 3)]
    public string LastName { get; set; }

    [Gedaq.Common.Attributes.IgnoreProperty]
    public Identification Identification { get; set; }
}

public class Identification
{
    [Alias(order: 0)]
    public int Id { get; set; }

    [Alias(order: 1)]
    public string TypeName { get; set; }
}

    [Query(
query: @"
SELECT 
    p.id,
    p.firstname,
    p.middlename,
    p.lastname,
~StartInner::Identification:id~
    i.id,
    i.typename
~EndInner::Identification~
FROM person p
LEFT JOIN identification i ON i.id = p.identification_id
LEFT JOIN country c ON c.id = i.country_id
WHERE p.id != @id
",
methodName: "Query1",
queryMapTypes: [typeof(Person)],
methodType: Gedaq.Common.Enums.MethodType.Async | Gedaq.Common.Enums.MethodType.Sync),
Parametr(parametrType: typeof(int), parametrName: "id", dbType: System.Data.DbType.Int32)]
  [Query(
methodName: "Query2",
queryMapTypes: [typeof(Person), typeof(Identification)],
methodType: Gedaq.Common.Enums.MethodType.Async | Gedaq.Common.Enums.MethodType.Sync),
DynamicParametr()]

        [QueryBatch(
            batchName: "BatchQueries",
            queryType: QueryType.Read,
            methodType: Gedaq.Common.Enums.MethodType.Async | Gedaq.Common.Enums.MethodType.Sync),\
            Gedaq.DbConnection.Attributes.BatchPart(
            methodName: "Query1",
            position: 2),
Gedaq.DbConnection.Attributes.BatchPart(
            methodName: "Query2",
            position: 1)]
public async Task SomeMethod(DbConnection connection)
{
    var parametr = new SqlParameter();
    parametr.SqlDbType = System.Data.SqlDbType.Int;
    parametr.ParameterName = "id";

    var persons1 = new List<PersonFlatExample>();
    var persons2 = new List<PersonExample>();
    BatchQueries(
      connection,
      paramersQuery1Batch: [parametr],
      dynamicQuery1Batch: @"
SELECT 
    p.id as banana43,
    p.middlename as banana65,
    p.firstname,
    p.lastname,
    i.id as unexpect16,
    i.typename
FROM person p
LEFT JOIN identification i ON i.id = p.identification_id
LEFT JOIN country c ON c.id = i.country_id
WHERE p.id != @id
",
      idBatch2: 5,
      query1mapDelegate: (person1, ident) =>
      {
          person1.Identification = ident;
          persons1.Add(person1);
      },
      query2mapDelegate: (person2) => { persons2.Add(person2); });
}

```

As you can see, we specified that we want to generate a batch query named "BatchQueries" from queries "Query1" and "Query1," and we specified the order of the queries within the batch.

The generated method accepts all the necessary parameters to execute both queries. Currently, if one of the queries in the batch is multi-maping, they are all executed through a delegate.
Thus, if at some stage you need to execute existing queries in one batch, you do not need to touch the existing ones - you only need to generate a batch, indicating which queries it consists of.
