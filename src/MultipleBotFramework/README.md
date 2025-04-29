# MultipleBotFramework

Фреймворк для создания Telegram ботов с поддержкой множества функций и возможностей.

## Описание

MultipleBotFramework - это мощный фреймворк для разработки Telegram ботов на .NET 8.0. Он предоставляет готовую инфраструктуру для создания масштабируемых и функциональных ботов с поддержкой множества пользователей, ролей и прав доступа.

## Основные возможности

- 🔐 **Многопользовательский режим**
  - Система ролей и прав доступа
  - Управление пользователями
  - Claims-based авторизация

- 🌐 **Локализация**
  - Встроенная поддержка русского языка
  - Возможность добавления новых языков
  - Ресурсы для локализации

- 📊 **Работа с данными**
  - Entity Framework Core 8.0
  - PostgreSQL
  - Repository pattern
  - Миграции базы данных

- ⏰ **Планировщик задач**
  - Quartz.NET интеграция
  - Планирование периодических задач
  - Управление задачами

- 🛠 **Архитектура**
  - Модульная структура
  - Dependency Injection
  - Middleware поддержка
  - Расширяемая архитектура

## Технологический стек

- .NET 8.0
- Entity Framework Core 8.0.8
- PostgreSQL
- Telegram.BotAPI 8.3.0
- Quartz.NET 3.13.1
- Microsoft.AspNetCore.Mvc.Core
- Newtonsoft.Json

## Установка

1. Установите пакет через NuGet:
```bash
dotnet add package Burtimax.TelegramBot.MultipleBotFramework
```

2. Добавьте необходимые сервисы в `Program.cs`:
```csharp
builder.Services.AddMultipleBotFramework(options => {
    // Настройка фреймворка
});
```

## Структура проекта

```
MultipleBotFramework/
├── Attributes/         # Атрибуты для валидации и авторизации
├── Base/              # Базовые классы и интерфейсы
├── BotHandlers/       # Обработчики сообщений бота
├── Constants/         # Константы и конфигурация
├── Db/               # Контекст базы данных и миграции
├── Dispatcher/       # Диспетчер сообщений
├── Dto/              # Объекты передачи данных
├── Enums/            # Перечисления
├── Exceptions/       # Пользовательские исключения
├── Extensions/       # Расширения для существующих классов
├── Filters/          # Фильтры для обработки запросов
├── Middleware/       # Промежуточное ПО
├── Models/           # Модели данных
├── Options/          # Настройки приложения
├── Quartz/           # Планировщик задач
├── Repository/       # Репозитории для работы с данными
├── Resources/        # Ресурсы (локализация)
├── Services/         # Бизнес-логика
└── Utils/            # Вспомогательные утилиты
```

## Использование

### Базовая настройка бота

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMultipleBotFramework(options => {
            options.BotToken = "YOUR_BOT_TOKEN";
            options.DatabaseConnection = "YOUR_CONNECTION_STRING";
        });
    }
}
```

### Создание обработчика сообщений

```csharp
public class MessageHandler : IBotMessageHandler
{
    public async Task HandleMessage(Message message)
    {
        // Обработка сообщения
    }
}
```

### Настройка планировщика задач

```csharp
services.AddQuartz(q => {
    q.UseMicrosoftDependencyInjectionJobFactory();
    // Настройка задач
});
```

## Локализация

Фреймворк поддерживает локализацию через ресурсы. Ресурсы находятся в директории `Resources/Localization/`.

## Безопасность

- Встроенная система ролей и прав
- Claims-based авторизация
- Валидация входных данных
- Безопасное хранение конфиденциальных данных

## Лицензия

MIT License

## Поддержка

При возникновении вопросов или проблем, создайте issue в репозитории проекта.

## Авторы

- Burtimax
