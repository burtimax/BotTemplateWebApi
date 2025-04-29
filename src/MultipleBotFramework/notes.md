cd /src/MultipleBotFramework
dotnet ef migrations add ReplaceChatFields --context BotDbContext -o Db/BotDb/Migrations

dotnet ef migrations remove --context BotDbContext 


cd /src/MultipleBotFramework
dotnet ef migrations add AddBotNotification --context BroadcastDbContext -o Db/BroadcastDb/Migrations
