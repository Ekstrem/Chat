using Chat.Domain.Abstraction;
using Chat.Domain.Tests.Models;
using System;

namespace Chat.Domain.Tests
{
    public static class QuestionBuilder
    {
        public static Question DefaultUserRequest()
        {
            return new Question
            {
                UserId = Guid.NewGuid(),
                UserLogin = "Иванов Иван",
                SessionId = (int)DateTime.Now.Ticks,
                Text = "Тестовое сообщение",
                Platform = Platform.Android,
                Application = Application.IncommingCall,
                Type = MessageType.Voice
            };
        }
    }
}
