using System.Threading.Tasks;
using MultipleBotFramework.Db.Entity;
using MultipleBotFramework.Enums;
using MultipleBotFramework.Models;

namespace MultipleBotFramework.Services.Interfaces;

public interface IBotsManagerService
{
    public Task<MyTelegramBotClient?> GetBotClientById(long botId);
    public Task<BotEntity?> GetBotById(long botId);
    public Task<BotEntity> RenewBotInfo(long botId);
    public Task<BotEntity> CreateBot(string token, string? comment = null, BotStatus? status = null);
    public Task<BotEntity> UpdateBot(long id, string? token, string? comment = null, BotStatus? status = null);
    public Task DeleteBot(long id);
}