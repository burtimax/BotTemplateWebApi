cd /src/MultipleBotFramework
dotnet ef migrations add ReplaceChatFields --context BotDbContext -o Db/BotDb/Migrations

dotnet ef migrations remove --context BotDbContext 


cd /src/MultipleBotFramework
dotnet ef migrations add AddFieldsToNotifications --context BroadcastDbContext -o Db/BroadcastDb/Migrations

dotnet ef migrations add Init --context ReferralDbContext -o Db/ReferralDb/Migrations
