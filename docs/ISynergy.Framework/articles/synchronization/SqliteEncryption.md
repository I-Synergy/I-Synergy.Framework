Sqlite Encryption
=================

Overview
--------

-   **Sqlite** doesn\'t support encrypting database files by default.
-   Instead, we need to use a modified version of Sqlite like
    [SEE](https://www.hwaci.com/sw/sqlite/see.html) ,
    [SQLCipher](https://www.zetetic.net/sqlcipher/) ,
    [SQLiteCrypt](http://www.sqlite-crypt.com/) , or
    [wxSQLite3](https://utelle.github.io/wxsqlite3) .
-   This article demonstrates using an unsupported, open-source build of
    **SQLCipher**, but the information also applies to other solutions
    since they generally follow the same pattern.

> ### Hint
> You will find more information about Sqlite Encryption with
**Microsoft.Data.Sqlite**
[Here](https://docs.microsoft.com/en-us/dotnet/standard/data/sqlite/encryption?tabs=netcore-cli)
.

> ### Hint
> You will find the sqlite encryption sample here : [Sqlite Encryption
Sample](https://github.com/I-Synergy/I-Synergy.Framework/blob/master/Samples/SqliteEncryption)

Tweak the nuget packages
------------------------

Basically, installing the packages needed to use Sqlite encryption is
pretty simple. Just override packages:

``` {.sourceCode .bash}
dotnet add package Microsoft.Data.Sqlite.Core
dotnet add package SQLitePCLRaw.bundle_e_sqlcipher
```

Your project file should be something like this:

``` {.sourceCode .xml}
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>netcoreapp3.1</TargetFramework>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="I-Synergy.Framework.Synchronization.Sqlite"  />
        <PackageReference Include="Microsoft.Data.Sqlite.Core"  />
        <PackageReference Include="SQLitePCLRaw.bundle_e_sqlcipher"  />
    </ItemGroup>
</Project>
```

Here is a screenshot of Visual Studio, after installing the packages:

![image](images/SqliteEncryption01.png)

-   As you can see, the `I-Synergy.Framework.Synchronization.Sqlite` is referencing the
    `Microsoft.Data.Sqlite` package that is referencing
    `Microsoft.Data.Sqlite.Core` and `SQLitePCLRaw.bundle_e_sqlite3`.
-   Because we made references at the root level of
    `Microsoft.Data.Sqlite.Core` and `SQLitePCLRaw.bundle_e_sqlcipher`,
    these two packages will be used in place of the
    `Microsoft.Data.Sqlite`\'s packages.

Code
----

The code is prett much the same code, just ensure you\'re filling a
**Password** in your **Sqlite** connection string:

``` {.sourceCode .csharp}
// connection string should be something like "Data Source=AdventureWorks.db;Password=..."
var sqliteConnectionString = configuration.GetConnectionString("SqliteConnection");
var clientProvider = new SqliteSyncProvider(sqliteConnectionString);

// You can use a SqliteConnectionStringBuilder() as well, like this:
//var builder = new SqliteConnectionStringBuilder();
//builder.DataSource = "AdventureWorks.db";
//builder.Password = "...";
```
