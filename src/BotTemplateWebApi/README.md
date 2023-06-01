

```bash
dotnet ef migrations add Init --context=AppDbContext -o=Db/Contexts/App/Migrations
dotnet ef database update
```