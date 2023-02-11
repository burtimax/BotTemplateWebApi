// using BotFramework.Interfaces;
//
// namespace BotFramework.Base
// {
//     /// <summary>
//     /// Контроллер состояний бота.
//     /// </summary>
//     /// <remarks>Определяет Handler для текущего состояния</remarks>
//     public class BotStateController
//     {
//         public Bot Bot { get; set; }
//         private IBaseBotRepository _botRepository;
//
//         private BotUpdateProcessor updateProcessor;
//
//         protected BotStateController(Bot bot)
//         {
//             this.Bot = bot;
//             this._botRepository = new BotControllerAdditionalMethods();
//         }
//
//         protected BotProcessController(Bot bot, IBaseBotRepository botRepository):this(bot)
//         {
//             this._botRepository = botRepository;
//         }
//
//         
//
//         public virtual async Task ProcessUpdate(object sender, Update update)
//         {
//             try
//             {
//                 //Create BotContext concrete object
//                 BotContext dbBotContext = bot.GetNewBotContext();
//                 this._botRepository.Db = dbBotContext;
//
//                 //Get objects from Update
//                 User telegramUser = update.GetUser();
//                 Chat telegramChat = update.GetChat();
//
//                 await AddUserIfNeed(telegramUser);
//                 await AddChatIfNeed(telegramChat);
//                 await SaveMessageIfNeed(update);
//                 
//                 //Это мой костыль (нужен пока здесь) //ToDo потом убрать
//                 #region Костыль
//                 var user = await dbBotContext.User.FirstOrDefaultAsync(u => u.Id == telegramUser.Id);
//                 if (user != null)
//                 {
//                     user.Username = telegramUser.Username;
//                     dbBotContext.User.Update(user);
//                     await dbBotContext.SaveChangesAsync();
//                 }
//                 #endregion
//
//                 //Get Chat model from DataBase to process Update
//                 Db.DbBot.Chat chatModel = await _botRepository.GetChat(telegramChat.Id);
//                 var curState = bot.StateStorage.Get(chatModel.GetCurrentStateName());
//                 string curStateName = curState.ClassName;
//                 //---Process Update---
//                 //Find BotStateProcessor Class
//                 string processorClassName = $"{bot.BotNamespaceStatePrefix}{curStateName}.{curStateName}BotStateProcessor";
//                 Type processorType = Assembly.GetExecutingAssembly().GetType(processorClassName, true, true);
//
//                 //Collect Data for Processing
//                 UpdateBotModel data = new UpdateBotModel()
//                 {
//                     bot = this.bot.client,
//                     dbBot = dbBotContext,
//                     stateStorage = bot.StateStorage,
//                     commandStorage = bot.CommandStorage,
//                     state = curState,
//                     update = update,
//                     chat = await _botRepository.GetChat(telegramChat.Id),
//                     user = await _botRepository.GetUser(telegramUser.Id)
//                 };
//
//                 //Create Instance of Found BotStateProcessorClass
//                 var processor = Activator.CreateInstance(processorType, data);
//                 MethodInfo methodProcessor = processorType.GetMethod("ProcessUpdate");
//
//                 if (methodProcessor == null) throw new Exception($"Method [ProcessUpdate] don't exists in processor type {processorType.Name}!");
//
//                 //Invoke Process method and Get Hop Data
//                 Hop hop = await (methodProcessor.Invoke(processor, null) as Task<Hop>);
//
//                 if (hop == null) throw new Exception("Hop can't be NULL");
//
//                 //Get hop, get State, Update CurrentState in Db
//
//                 //Change Chat CurrentState by hop type
//                 chatModel.SetState(hop.PriorityNextStateName, hop.PriorityHopType);
//
//                 Db.DbBot.Chat c = new Db.DbBot.Chat()
//                 {
//                     Id = chatModel.Id,
//                     State= chatModel.State,
//                     StateData = hop.PriorityData != null ? hop.PriorityData : chatModel.StateData,
//                 };
//                 await _botRepository.SetChatData(c);
//
//                 if (hop.BlockSendAnswer == false)
//                 {
//                     OutboxMessage outboxMessage = new OutboxMessage(hop.PriorityIntroduction);
//                     if (hop.PriorityKeyboard != null)
//                     {
//                         outboxMessage.ReplyMarkup = hop.PriorityKeyboard;
//                     }
//
//                     outboxMessage.ParseMode = ParseMode.Html;
//                     await bot.client.SendOutboxMessageAsync(chatModel.Id, outboxMessage);
//                     //outboxMessage.SendOutboxMessage(BotApplication.client, chatModel.Id);
//                 }
//                 
//             }
//             catch (Exception exception)
//             {
//                 await bot.client.SendTextMessageAsync(new ChatId(AppConstants.SupportUserId), $"<b>ERROR</b>\n\n" +
//                                                                        $"<b>MESSAGE</b>\n{exception.Message}\n\n" +
//                                                                        $"<b>STACK TRACE</b>{exception.StackTrace}\n\n",
//                     ParseMode.Html);
//                 //in case of exception not save changes
//                 this._botRepository.Db = null;
//             }
//             finally
//             {
//                 //if wasn't exception save changes
//                 if(this._botRepository.Db != null)
//                 {
//                     await this._botRepository.Db.SaveChangesAsync();
//                     this._botRepository.Db = null;
//                 }
//             }
//
//         }
//
//
//         private async Task AddUserIfNeed(User user)
//         {
//             if (user == null) throw new Exception("User is NULL");
//
//             if (await _botRepository.IsUserInBot(user.Id) == false)
//             {
//                 await _botRepository.AddUserToBot(user);
//             }
//         }
//
//         private async Task AddChatIfNeed(Chat chat)
//         {
//             if (chat == null) throw new Exception("Chat is NULL");
//
//             if (await _botRepository.IsChatInBot(chat.Id) == false)
//             {
//                 await _botRepository.AddChatToBot(chat);
//             }
//         }
//
//         private async Task SaveMessageIfNeed(Update update)
//         {
//             if (update == null) return;
//
//             if (update.Type == UpdateType.Message)
//             {
//                 await _botRepository.SaveMessage(update.Message);
//             }
//         }
//     }
// }