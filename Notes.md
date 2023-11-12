# Desription

### EF core commands
~~~
1) SCAFFOLD dotnet ef dbcontext scaffold "Host=routeam.ru;port=25532;Database=prosystem_orders_goods;Username=postgres;Password=e8tsC6YtFbvH5EYp" Npgsql.EntityFrameworkCore.PostgreSQL -o Db/Entity --context-dir Db -c ProAptekaProductsContext --context-namespace Db
2) dotnet ef migrations add AddOrderSupplierAnswersTables --context ProMarketContext -o Db/Migrations
3) dotnet ef database update --context ProMarketContext
~~~
