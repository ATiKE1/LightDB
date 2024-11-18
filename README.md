# Pure SQL library for CSharp
### Credits: Антон Винокуров, Орлов Никита (ИС-301 2024г)
[Download link](https://wdho.ru/ooO1)

## How to usage?

Include library LightDB.dll to the your c# project (Links - Browse - Select LightDB.dll)

Next, Init connection to DB. Using connection string
```
LightDB.LightDB lightDB = new LightDB.LightDB("Data Source=K21509N01\\SQLEXPRESS;Initial Catalog=testdb;Integrated Security=True");
```

Step 2. Make your query to DB
```
ArrayList result = lightDB.ExecuteSqlCommand("SELECT * FROM users");
```

Step 3. Work with your response (Response is filled or empty ArrayList)
```
foreach (ArrayList row in result)
{
    Console.WriteLine($"ID -> {row[0]} Name -> {row[1]} Surname -> {row[2]} Patronimyc -> {row[3]}");
}
```

Step 4. Close connection
```
lightDB.Close();
```

## You can create few query without closing connection


## Full Code
```
static void Main(string[] args)
{
    LightDB lightDB = new LightDB("Data Source=K21509N01\\SQLEXPRESS;Initial Catalog=testdb;Integrated Security=True");

    ArrayList result = lightDB.ExecuteSqlCommand("SELECT * FROM users");

    foreach (ArrayList row in result)
    {
        Console.WriteLine($"ID -> {row[0]} Name -> {row[1]} Surname -> {row[2]} Patronimyc -> {row[3]}");
    }

    lightDB.Close();

    Console.ReadLine();
}
```