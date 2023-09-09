using System.Threading.Tasks;
using BotFramework.Db.Entity;
using BotFramework.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotFramework.BotCommands.Admin;

public class AdminCommandHelper
{
    public static async Task<bool> HasUserAccessToCommand(BotUser user, ITelegramBotClient botClient, ChatId chatId, BotConfiguration botConfig)
    {
        if (user.Claims.Contains(BotConstants.AdminClaims.LastPasswordClaim) == false)
        {
            await botClient.SendTextMessageAsync(chatId, "Команда доступна только администраторам");
            return false;
        }
        
        string password = user.Claims.Get(BotConstants.AdminClaims.LastPasswordClaim);
        
        if (string.Equals(password, botConfig.Password) == false)
        {
            await botClient.SendTextMessageAsync(chatId, $"Необходимо авторизоваться. Введите команду {AuthAdminCommand.AUTH}.");
            return false;
        }

        return true;
    }
}