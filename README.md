# Chat
Агрегат
## Объекты-значения
| Объект | Свойство | Тип | Описание |
|--------|----------|-----|----------|
|Root	| UserId	| Guid	| Идентификатор пользователя
|Root	| SessionId	| int	| Идентификатор сессии
|Message	| Type	| byte	| Тип сообщения (текст, речь, видео)
|Message	| Text	| string	| Текст обращения
|Message	| Platform	| byte	| Платформа с которой отправлено сообщение (Linux, Windows, Android).
|Message	| Application	| byte	| Приложение из которого отправлено сообщение (приложение, сайт, звонок)
|Message	| ContentId	| guid	| Идентификатор загруженного контента
|Actor	| Login	| string	| Имя пользователя
|Actor	| Type	| byte	| Тип пользователя (клиент, бот, оператор).
|Feedback	| Type	| byte	| Форма ответа (да/нет, баллы, свободная)
|Feedback	| Value	| byte	| Значение
|Feedback	| Text	| string	| Текст
## Бизнес-методы
SubscriberRequestQuestion	Абонент задал вопрос в чате	Root, Message, Actor	
BotRepliedToUser	Бот ответил пользователю	Message, UserInfo, Actor	
OperatorDequeueRequest	Оператор получил обращение на обработку	Actor	
OperatorRepliedToMessage	Оператор ответил на обращение	Message, Actor	
OperatorClarifiedQuestion	Оператор уточнил вопрос	Message, Actor	
SubscriberGaveFeedback	Абонент оценил работу	Feedback	
SubscriberDownloadMedia	Абонент загрузил медиа-контент	Root, Message, Actor	
OperatorDownloadMedia	Оператор загрузил медиаконтент	Message, Actor	
SessionEndingByTrigger	Сессия закончилась по триггеру	Message	
## Инварианты
Методы создания агрегата. Порядок вызова методов.
Проверки объектов значений
