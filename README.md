Ссылка на проект

Библиотека для создания Telegram ботов.
### 1) Настройка конфигурации.
Добавить в конфигурацию проекта разделы.

```json
// Конфигурация настроек бота. Обязательный раздел.
"Bot": {  
	"Name" : "BOT_NAME",  
	"TelegramToken" : "TOKEN",  
	"Webhook" : "https://ADDRESS",  
	"ResourcesFilePath" : "Resources.json",  
	"DbConnection" : "Host=127.0.0.1;Port=5432;Database=dbname;Username=postgres;Password=123",  
	"Password" : "123456",  
	"MediaDirectory": "/app/media",  
	"ExceptionDirectory": "/app/exception"  
},
// Логическая конфигурация бота. Необязательный раздел.
"BotOptions": {  
	"SaveUpdatesInDatabase" : true,  
	"SaveExceptionsInDatabase" : true,  
	"SaveExceptionsInDirectory" : true  
},
```

### 2) Подключить бота в Program.cs
Регистрируем конфигурации бота в DI.
Подключаем к контроллерам парсинг `json` с помощью `services.AddControllers().AddNewtonsoftJson();`;
Подключаем `services.AddHttpContextAccessor();`

```C#
//...
// Регистрируем конфигурации.
services.Configure<BotConfiguration>(builder.Configuration.GetSection("Bot"));
services.Configure<BotOptions>(builder.Configuration.GetSection("BotOptions"));
var botConfig = builder.Configuration.GetSection("Bot").Get<BotConfiguration>();
services.AddBot(botConfig); // Подключаем бота
services.AddControllers().AddNewtonsoftJson(); //Обязательно подключаем NewtonsoftJson
services.AddHttpContextAccessor();
// ...
```

В метод `AddBot` другим параметром необходимо передавать список `ClaimValue` разрешений приложения. Тогда бот сможет записать эти разрешения в БД и использовать при диспетчеризации запросов.

### 3) Настройка БД.
**Добавить расширение HSTORE в PostgreSQL.**
В случае, если возникает ошибка при работе программы бота. Возможно отсутствует необходимое расширение. Исполните скрипт в БД бота.

```SQL
-- Добавление расширения в БД для возможности сохранения Dictionary в EF Core.
CREATE EXTENSION IF NOT EXISTS hstore;
```

Установить поведение EfCore для работы c PostgreSQL. В файл `Program.cs` добавить строчку.

```c#
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
```
### 4) Добавление ресурсов бота.
Механизм ресурсов сэкономит и упростит работу с строковыми ресурсами бота.
Регистрация ресурсов происходит по аналогии с регистрацией конфигураций. Можно использовать этот метод расширения.

```C#
public static BotResources ConfigureBotResources(this IServiceCollection services, string resourcesFilePath)  
{  
	if (resourcesFilePath == null) throw new ArgumentNullException(nameof(resourcesFilePath));  
	  
	string json = File.ReadAllText(resourcesFilePath);  
	BotResourcesBuilder resourcesBuilder = new(json);  
	json = resourcesBuilder.Build();  
	  
	Stream jsonStream = StreamHelper.GenerateStreamFromString(json);  
	var resourcesConfigBuilder = new ConfigurationBuilder().AddJsonStream(jsonStream);  
	IConfiguration resourcesConfiguration = resourcesConfigBuilder.Build();  
	  
	services.Configure<BotResources>(resourcesConfiguration);  
	return resourcesConfiguration.Get<BotResources>();  
}
```

Который необходимо вызвать в `Program.cs`

```c#
BotResources botResources = services.ConfigureBotResources(botConfig.ResourcesFilePath);
```

Пример файла ресурсов `BotResources.json`
```json
{  
	"Hello" : "Привет",  
	"Common" : {  
		"DefaultAnswer" : "Ответ по умолчанию :)"  
	},  
	"Test" : 
	{  
		"Introduction" : "Привет, {{Test.Farewell}})",  
		"Farewell" : "Пока",  
		"Keyboard" : "[{{Test.Introduction}}] [{{Common.DefaultAnswer}}] [ ] \n [ ] ",  
		"Goodbye" : "До свидания {{Test.Farewell}}"  
	}  
}
```

Для `json` файла ресурсов делаем соответствующий класс ресурсов.

```c#
public partial class BotResources  
{  
	public string Hello { get; set; }  
	public CommonResources Common { get; set; }  
	public TestResources Test { get; set; }  
}
```

Теперь можем использовать ресурсы в других классах.

```c#
public class BotState : BaseBotState
{
    protected BotResources R;
    
    public BotState(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        R = serviceProvider.GetRequiredService<IOptions<BotResources>>().Value;
    }

    public override Task HandleBotRequest(Update update)
    {
        return null;
    }
}
```

### 5) Регистрируем контроллер для Бота.
Необходимо определить контроллер, указать адрес, по которому будут обрабатываться запросы бота.
Контроллер необходимо наследовать от `BotDispatcherController`.
Просто скопируйте код.

```c#
[ApiController]  
public class MainBotDispatcherController : BotDispatcherController  
{  
	private readonly ITelegramBotClient _botClient;  
	private readonly IMapper _mapper;  
	private IBaseBotRepository _baseBotRepository;  
	  
	public MainBotDispatcherController(ITelegramBotClient botClient, IMapper mapper, IBaseBotRepository baseBotRepository,  
	IHttpContextAccessor contextAccessor)  
	: base(baseBotRepository, contextAccessor, Assembly.GetExecutingAssembly())  
	{  
	_botClient = botClient;  
	}  
	  
	[HttpPost("/")]  
	public override async Task<IActionResult> HandleBotRequest([FromBody] Update updateRequest)  
	{  
	return await base.HandleBotRequest(updateRequest);  
	}  
}
```

### 6) Управление ботом.
#### Базовые команды управления ботом.
- `/set {@user} {params claim}` - Установить разрешения для пользователя.
- `/reset {@user} {params claim}` - Сбросить разрешения для пользователя.
- `/user {@user || ID}` - Показать данные пользователя (активность, разрешения, состояние чата и т.д.)
- `/find {строка}` - Поиск пользователя (ИД, имя, ник и т.д.). Возвращает список.
- `/report {int N}` - Получить отчет по боту за последние N часов.
- `/auth {password}` - Получить доступ администратора.
- `/claims` - вывести список claims для пользователей бота с описанием доступности.
- `/commands` - вывести мануал команд для пользователя (с учетом его разрешений).
- `/block {params @user || ID}` - заблокировать пользователя. (Могут использовать модеры)
- `/unblock {params @user || ID}` - заблокировать пользователя.
- `/me` - показать меня, разрешения и т.д.
- `/msg` - показать данные по сообщению, команду надо вводит на ответ к какому-либо сообщению.

Для использования каждой команды у пользователя должно быть соответствующее разрешение. Разрешения может раздавать администратор бота.

Чтобы стать администратором бота, необходимо авторизоваться через команду `/auth {password}`, указать пароль из конфигурации. У админа есть только одно супер-разрешение, которое позволяет ему иметь доступ ко всему функционалу бота.
### Примечания

Пример файла `Program.cs`
```c#
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);  
  
var builder = WebApplication.CreateBuilder(args);  
IServiceCollection services = builder.Services;  
  
// Initialize  
  
// Register options  
services.AddLogging();  
services.Configure<ApplicationConfiguration>(builder.Configuration);  
services.Configure<BotConfiguration>(builder.Configuration.GetSection("Bot"));  
services.Configure<BotOptions>(builder.Configuration.GetSection("BotOptions"));  
var botConfig = builder.Configuration.GetSection("Bot").Get<BotConfiguration>();  
BotResources botResources = services.ConfigureBotResources(botConfig.ResourcesFilePath);  
services.AddBot(botConfig);  
  
// Add services to the container.  
services.AddMapster(Assembly.GetExecutingAssembly());  
services.AddControllers().AddNewtonsoftJson();  
services.AddHttpContextAccessor();  
  
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle  
services.AddEndpointsApiExplorer();  
services.AddSwaggerGen();  
  
var app = builder.Build();  
 
// Configure the HTTP request pipeline.  
if (app.Environment.IsDevelopment())  
{  
app.UseSwagger();  
app.UseSwaggerUI();  
}  
//app.UseHttpsRedirection();  
app.UseRouting();  
app.UseAuthorization();  
app.MapControllers();  
app.Run();
```

