## Database setup

In **appsettings.json** modify the connection string:

`"DefaultConnection": "Server=**********\\SQLEXPRESS;Database=ProjectShop;Trusted_Connection=True;MultipleActiveResultSets=true"`

Powershell (add a new migration):

```

$ add-migration migrationName
$ update-database

```

If necessary, remove the latest migration with:

`$ remove-migration`

Login credentials:

```

username: aa@aa.aa
password: P@$$w0rd

```

