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
}

```

Static mapping:
Static mapping allows you to define nested entities using markup. Without such markup, you can only populate flat structures; otherwise, you need to use multi mapping.
```C#

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
    methodName: "StaticMapping",
    queryMapTypes: [typeof(Person)],
    methodType: Gedaq.Common.Enums.MethodType.Async | Gedaq.Common.Enums.MethodType.Sync),
    Parametr(parametrType: typeof(int), parametrName: "id", dbType: System.Data.DbType.Int32)]
public async Task SomeMethod(DbConnection connection)
{
    var persons = StaticMapping(connection, id: 3);
    var personsAsync = await StaticMappingAsync(connection, id: 3);
}
```

_____________

Multi mapping:
Model classes in example:
```C#

[AliasPrefix("person_")]
public class Person
{
    public int Id { get; set; }

    public string FirstName { get; set; }

    public string MiddleName { get; set; }

    public string LastName { get; set; }

    [Gedaq.Common.Attributes.IgnoreProperty]
    public Identification Identification { get; set; }
}

[AliasPrefix("ident_")]
public class Identification
{
    public int Id { get; set; }

    [Alias(alias: "custom_alias")]
    public string TypeName { get; set; }
}

```

Since we have multiple mappings, we need to specify how to differentiate columns with the same name.
The AliasPrefix attribute is used for this.
If the properties don't have specific names specified using the Alias ​​attribute, the lowercase name of the property is assumed as the column name.
It is also worth paying attention to the IgnoreProperty attribute, which explicitly indicate to the property generator that it does not need to be checked.

```C#

    [Query(
methodName: "MultiMapping",
queryMapTypes: [typeof(Person), typeof(Identification)],
methodType: Gedaq.Common.Enums.MethodType.Async | Gedaq.Common.Enums.MethodType.Sync),
Parametr(parametrType: typeof(int), parametrName: "id", dbType: System.Data.DbType.Int32)]
public async Task SomeMethod(DbConnection connection)
{
    var persons = new List<Person>;
    MultiMapping(connection, dynamicQuery: @"
SELECT 
    p.id as person_id,
    p.firstname as person_firstname,
    p.middlename as person_middlename,
    p.lastname as person_lastname,
    i.id as ident_id,
    i.typename as ident_typename
FROM person p
LEFT JOIN identification i ON i.id = p.identification_id
LEFT JOIN country c ON c.id = i.country_id
WHERE p.id != @id
",
id: 3,
mapDelegate: (person, ident) =>
{
    person.Identification = ident;
    persons.Add(person);
});
}

```

Multi-mapping forces us to define how different types interact with each other. You can map them into each other as in the example, or do something else — it's all up to you.
In the delegate, note that the types are in the order you defined them in queryMapTypes "mapDelegate: (person, ident)".

______

Queries have two mapping outputs: by name (Alias), as in the examples above, or by order.
Let's rewrite the last example for order mapping.

```C#

public class Person
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
methodName: "MultiMapping",
queryMapTypes: [typeof(Person), typeof(Identification)],
methodType: Gedaq.Common.Enums.MethodType.Async | Gedaq.Common.Enums.MethodType.Sync),
Parametr(parametrType: typeof(int), parametrName: "id", dbType: System.Data.DbType.Int32)]
public async Task SomeMethod(DbConnection connection)
{
    var persons = new List<Person>;
    MultiMapping(connection, dynamicQuery: @"
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
id: 3,
mapDelegate: (person, ident) =>
{
    person.Identification = ident;
    persons.Add(person);
});
}

```

Now we don't care what the properties are named, because the mapping will retrieve them by order, not by name, like this:
`reader.GetFieldValue<T>(order);`

This allows us to write any SQL queries as long as the order in the row matches the declared one.

___________

As you noted above, we can use both dynamic queries and pre-defined ones. But what if the parameters are also unknown in advance? Then we need to use the DynamicParameter attribute instead of the Parameter attribute.

```C#

public class Person
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
methodName: "MultiMapping",
queryMapTypes: [typeof(Person), typeof(Identification)],
methodType: Gedaq.Common.Enums.MethodType.Async | Gedaq.Common.Enums.MethodType.Sync),
DynamicParametr()]
public async Task SomeMethod(DbConnection connection)
{
    var parametr = new SqlParameter();
    parametr.SqlDbType = System.Data.SqlDbType.Int;
    parametr.ParameterName = "id";

    var persons = new List<Person>;
    MultiMapping(connection, dynamicQuery: @"
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
dynamicParametrs: [parametr],
mapDelegate: (person, ident) =>
{
    person.Identification = ident;
    persons.Add(person);
});
}

```

The new generated method takes abstract parameters `dynamicParametrs`.
