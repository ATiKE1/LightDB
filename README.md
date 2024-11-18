# Pure SQL library for CSharp
### Credits: Антон Винокуров, Орлов Никита, Тремасов Александр (ИС-301 2024г)
[Download link](http://192.168.222.27:3000/is1-26-vinokurovav/LightDB/raw/master/bin/Debug/LightDB.dll)

## How to usage?

Include library LightDB.dll to the your c# project (Links - Browse - Select LightDB.dll)

Next, Init connection to DB. Using connection string
```csharp
LightDB.LightDB lightDB = new LightDB.LightDB("Data Source=K21509N01\\SQLEXPRESS;Initial Catalog=testdb;Integrated Security=True");
```

Step 2. Make your query to DB
```csharp
LightDB.LightSqlData result = lightDB.ExecuteSqlCommand("SELECT * FROM users");
```

Step 3. Work with your response (Response is filled or empty ArrayList)
```csharp
foreach (ArrayList row in result.DynamicDataList)
{
    Console.WriteLine($"ID -> {row[0]} Name -> {row[1]} Surname -> {row[2]} Patronimyc -> {row[3]}");
}

/*
Output example:

ID -> 1 Name -> Hailee Surname -> Byk Patronimyc -> Deathridge
ID -> 2 Name -> Devonne Surname -> McCluney Patronimyc -> Marcoolyn
ID -> 3 Name -> Natal Surname -> McKennan Patronimyc -> Fetteplace
ID -> 4 Name -> Augy Surname -> Waterson Patronimyc -> Strank
ID -> 5 Name -> Jehu Surname -> Jinkinson Patronimyc -> Kubczak
ID -> 6 Name -> Bud Surname -> Riccetti Patronimyc -> Salzburger
ID -> 7 Name -> Corabel Surname -> Perview Patronimyc -> Orknay
ID -> 8 Name -> Wayland Surname -> Ateridge Patronimyc -> Andrieu
ID -> 9 Name -> Vlad Surname -> Richold Patronimyc -> Warrener
ID -> 10 Name -> Vivian Surname -> Lamke Patronimyc -> McQuin


*/

```

Step 4. Close connection
```csharp
lightDB.Close();
```

## You can create few query without closing connection


## Full Code
```csharp

// You can use this for Console Application
static void Main(string[] args)
{
    LightDB.LightDB lightDB = new LightDB.LightDB("Data Source=K21509N01\\SQLEXPRESS;Initial Catalog=testdb;Integrated Security=True");

    LightDB.LightSqlData result = lightDB.ExecuteSqlCommand("SELECT * FROM users");

    foreach (ArrayList row in result.DynamicDataList)
    {
        Console.WriteLine($"ID -> {row[0]} Name -> {row[1]} Surname -> {row[2]} Patronimyc -> {row[3]}");
    }

    lightDB.Close();

    Console.ReadLine();
}

// You can use this for WPF-project
public MainWindow()
{
    InitializeComponent();

    LightDB.LightDB lightDB = new LightDB.LightDB("Data Source=K21509N01\\SQLEXPRESS;Initial Catalog=testdb;Integrated Security=True");

    var result = lightDB.ExecuteSqlCommand("SELECT * FROM users");

    dtg.ItemsSource = result.ToDataTable().DefaultView;

    lightDB.Close();
}

```

![Example of using LightDB for WPF-project](https://i.imgur.com/4GdsejN.png)


## Difference between EntityFrameworkCore


```csharp
public MainWindow()
{
    InitializeComponent();

    LightDB.LightDB lightDB = new LightDB.LightDB("Data Source=K21509N01\\SQLEXPRESS;Initial Catalog=testdb;Integrated Security=True");

    var result = lightDB.ExecuteSqlCommand("SELECT * FROM users");

    dtg.ItemsSource = result.ToDataTable().DefaultView;

    lightDB.Close();
}
```

## VS

```csharp
public class User
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Surname { get; set; }

    public string Patronimyc { get; set; }
}

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=K21509N01\\SQLEXPRESS;Initial Catalog=testdb;Integrated Security=True");
    }
}

public MainWindow()
{
    InitializeComponent();

    ApplicationDbContext context = new ApplicationDbContext();

    dtg.ItemsSource = context.Users.ToList();
}

```

