using System;

namespace MultipleBotFrameworkUpgrade.Exceptions
{
    public class NotFoundHandlerForCommandException : Exception
    {
        public NotFoundHandlerForCommandException(string? botCommand, string? assemblyName) 
            : base($"Нет обработчика для команды [{botCommand}] в сборке [{assemblyName}]. Возможно у пользователя нет разрешений для обработчиков или у пользователя неподходящая роль.")
        {
            
        }
    }
}