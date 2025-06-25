/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using ScriptEngine.Machine.Contexts;
using OneScript.Contexts;
using OneScript.Exceptions;
using OneScript.StandardLibrary.Collections;

namespace OneScript.InternetMail
{
	/// <summary>
	/// Предназначен для доступа к почтовым серверам для отправки и получения сообщений (писем).
	/// Использует наиболее распространенные Интернет протоколы SMTP, POP3 и IMAP. 
	/// Не требует установленного почтового клиента и, если почтовый клиент все же установлен, работает с ним(-и) параллельно.
	/// Для протоколов IMAP, POP3 и SMTP поддерживается работа по SSL, но без возможности проверки сертификата клиента или сервера.
	/// Важно! Если почтовый сервер не поддерживает работу с идентификаторами, то в качестве идентификаторов используются ИдентификаторСообщения писем.
	/// </summary>
	[ContextClass("ИнтернетПочта", "InternetMail")]
	public class InternetMail : AutoContext<InternetMail>, IDisposable
	{
		private InternetMailProfile _profile;

		private readonly SmtpSender smtpClient = new SmtpSender();
		private IMailReceiver receiver;
		private string _currentMailbox = "";
		private string _mailboxDelimiterCharacter = "";

		public InternetMail()
		{
		}

		/// <summary>
		/// Содержит разделитель, используемый для разделения папок в иерархии папок.
		/// Разделитель настраивается на сервере IMAP. 
		/// На клиенте свойство указывает, какой разделитель нужно использовать при адресации иерархии папок.
		/// </summary>
		[ContextProperty("СимволРазделитель", "DelimiterChar")]
		public string DelimiterChar
		{
			get
			{
				return _mailboxDelimiterCharacter;
			}
			set
			{
				_mailboxDelimiterCharacter = value;
				(receiver as ImapReceiver)?.SetDelimiterCharacter(_mailboxDelimiterCharacter);
			}
		}

		/// <summary>
		/// Содержит текущий почтовый ящик на сервере, с которым выполняется работа.
		/// Данное свойство влияет на методы ПолучитьЗаголовки, ПолучитьИдентификаторы, УдалитьСообщения, ПолучитьКоличествоСообщений, Послать
		/// для случая, когда используется протокол отправки IMAP. 
		/// Если свойство не задано, то действие будет производиться с IMAP почтовым ящиком "Входящие" ("Inbox").
		/// </summary>
		[ContextProperty("ТекущийПочтовыйЯщик", "CurrentMailbox")]
		public string CurrentMailbox
		{
			get
			{
				return _currentMailbox;
			}
			set
			{
				_currentMailbox = value;
				(receiver as ImapReceiver)?.SetCurrentMailbox(_currentMailbox);
			}
		}

		#region SMTP

		private void LogonSmtp()
		{
			smtpClient.Logon(_profile);
		}

		private void LogoffSmtp()
		{
			smtpClient.Logoff();
		}

		#endregion

		/// <summary>
		/// Осуществляет подключение к почтовому серверу для получения/посылки почты.
		/// </summary>
		/// <param name="profile">Профиль пользователя для подключения к почтовому серверу.</param>
		/// <param name="receiveMailProtocol">Задаёт, какой тип соединения нужно использовать при подключении к Интернет почте. 
		/// Если требуется использовать IMAP, то нужно указать соответствующий тип.
		/// Значение по умолчанию: POP3.</param>
		/// <exception cref="RuntimeException"></exception>
		[ContextMethod("Подключиться", "Logon")]
		public void Logon(InternetMailProfile profile, InternetMailProtocol receiveMailProtocol = InternetMailProtocol.Pop3)
		{
			_profile = profile;

			if (!string.IsNullOrEmpty(_profile.SmtpServerAddress) && !_profile.Pop3BeforeSmtp)
			{
				LogonSmtp();
			}

			switch (receiveMailProtocol)
			{
				case InternetMailProtocol.Imap:

					receiver = new ImapReceiver();
					if (!string.IsNullOrEmpty(_profile.ImapServerAddress))
						receiver.Logon(_profile);

					break;

				case InternetMailProtocol.Pop3:

					receiver = new Pop3Receiver();
					if (!string.IsNullOrEmpty(_profile.Pop3ServerAddress))
						receiver.Logon(_profile);

					break;

				case InternetMailProtocol.Smtp:

					throw new RuntimeException("Недопустимо указывать SMTP в качестве протокола получения почты!");
			}

			if (!string.IsNullOrEmpty(_profile.SmtpServerAddress) && _profile.Pop3BeforeSmtp)
				LogonSmtp();

		}

		/// <summary>
		/// Производит отключение от почтового сервера. После отключения посылка и получение сообщений с сервера не доступны.
		/// </summary>
		[ContextMethod("Отключиться", "Logoff")]
		public void Logoff()
		{
			LogoffSmtp();
			receiver?.Logoff();
		}

		/// <summary>
		/// Посылает сообщение типа ИнтернетПочтовоеСообщение.
		/// </summary>
		/// <param name="message">Почтовое сообщение.</param>
		/// <param name="processText">Обрабатывает тексты перед отправкой.
		/// Значение по умолчанию: Обрабатывать.</param>
		/// <param name="protocol">Определяет тип протокола отправки. Допустимые варианты IMAP или SMTP. 
		/// Указание POP3 приведет к возникновению исключения.
		/// Значение по умолчанию: SMTP.</param>
		/// <exception cref="RuntimeException"></exception>
		[ContextMethod("Послать", "Send")]
		public void Send(InternetMailMessage message,
		                 InternetMailTextProcessing processText = InternetMailTextProcessing.Process,
		                 InternetMailProtocol protocol = InternetMailProtocol.Smtp)
		{
			if (protocol == InternetMailProtocol.Pop3)
			{
				throw new RuntimeException("Недопустимо указывать POP3 в качестве протокола отправки почты!");
			}

			IMailSender sender = null;
			if (protocol == InternetMailProtocol.Smtp)
				sender = smtpClient;
			else if (protocol == InternetMailProtocol.Imap)
			{
				sender = receiver as ImapReceiver;
				if (sender == null)
					throw new RuntimeException("Соединение IMAP не установлено!");
			}

			sender?.Send(message, processText);
		}

		/// <summary>
		/// Формирует массив, содержащий объекты типа ИнтернетПочтовоеСообщение. Каждый объект содержит только следующие поля:
		/// - Заголовок,
		/// - Размер,
		/// - ИдентификаторСообщения,
		/// - Тема,
		/// - Отправитель,
		/// - ОбратныйАдрес,
		/// - Получатели,
		/// - СлепыеКопии,
		/// - ДатаОтправления.
		/// Выбираются заголовки всех сообщений, находящихся на сервере.
		/// </summary>
		/// <param name="filter">В качестве ключей структуры используются названия свойств письма, по которым осуществляется отбор. 
		/// Значения могут быть выражения следующих типов: Строка, Дата, Число, Булево.
		/// Тип значения определяется ключом. Например, если требуется отобрать письма до определённой даты, то в качестве значения необходимо использовать выражения типа Дата. 
		/// Если в качестве значения используется строка, то регистр не имеет значения. 
		/// Если заданы несколько ключей, то они применяются последовательно по логическому И.
		/// Если в структуре передано неподдерживаемое значение ключа, оно игнорируется, ошибки при этом не возникает.
		/// 
		/// - ОтправленОтвет (Answered) - Булево. Отобрать сообщения, у которых установлен флаг – Answered;
		/// - Недавние (Recent) - Булево. Отобрать сообщения, пришедшие в рамках текущей IMAP-сессии.
		/// - СлепыеКопии (Bcc) - Строка. Отобрать сообщения, которые имеют “строка” в поле Bcc;
		/// - Копии (Cc) - Строка. Отобрать сообщения, которые имеют “строка” в поле Cc;
		/// - Получатели (To) - Строка. Отобрать сообщения, которые имеют “строка” в поле To;
		/// - ДатаОтправления (PostDating) - Дата. Отобрать сообщения, у которых значение поле Date: равно “Дата”;
		/// - Отправитель (From) - Строка. Отобрать все сообщения у которых встречается “строка”в поле From;
		/// - ДоДатыОтправления (BeforeDateOfPosting) - Дата. Отобрать сообщения, у которых значение поле Date: перед “дата”;
		/// - ПослеДатыОтправления (AfterDateOfPosting) - Дата. Отобрать сообщения, у которых значение поля Date: после значения “Дата”;
		/// - Тема (Subject) - Строка. Отобрать сообщения, в заголовке которых встречается заданная строка;
		/// - Текст (Text) - Строка. Отобрать сообщения, в любых текстовых полях которого встречается заданная строка;
		/// - ТелоСообщения (Body) - Строка. Отобрать сообщения, в теле которых встречается строка – “строка”;
		/// - Удаленные (Deleted) - Булево. Отобрать сообщения, которые должны быть удалены или не должны быть удалены;
		/// - УстановленФлаг (Flagged) - Булево. Отобрать сообщения, которые помечены флагом или не помечены флагом;
		/// - Прочитанные (Seen) - Булево. Отобрать сообщения, которые были прочитаны или не прочитаны;
		/// - Новые (New) - Булево. Отобрать новые или старые сообщения.
		/// Пример: 
		/// <code>
		/// ПараметрыОтбораIMAP =НовыйСтруктура;
		/// ПараметрыОтбораIMAP.Вставить("Новые",Истина);
		/// ПараметрыОтбораIMAP.Вставить("Тема", "привет");
		/// Почта.ПолучитьЗаголовки(ПараметрыОтбораIMAP);
		/// </code> 
		/// Строковые значения критериев отбора отправляются в кодировке US-ASCII, если содержат только символы ANSI и в кодировке UTF-8 - в противном случае. Не все IMAP-серверы поддерживают UTF-8, поэтому сервер может выдать соответствующую ошибку.</param>
		/// <returns></returns>
		[ContextMethod("ПолучитьЗаголовки", "GetHeaders")]
		public ArrayImpl GetHeaders(StructureImpl filter = null)
		{
			return receiver?.GetHeaders(filter);
		}

		/// <summary>
		/// Возвращает массив, содержащий идентификаторы всех или новых сообщений, находящихся в почтовом ящике на сервере.
		/// Внимание! Фильтрация заголовков работает только при работе по протоколу IMAP.
		/// При работе по протоколу POP3 отбор писем не выполняется - метод вернет полный массив сообщений, даже если установлены ПараметрыОтбора.
		/// </summary>
		/// <param name="identifiers">Для получения всех идентификаторов сообщений из почтового ящика необходимо передать пустой массив.
		/// Если же надо получить идентификаторы только новых сообщений, то необходимо передать массив, заполненный ранее полученными идентификаторами (свойство Идентификатор).</param>
		/// <param name="filter">В качестве ключей структуры используются названия свойств письма, по которым осуществляется отбор. 
		/// Значения могут быть выражения следующих типов: Строка, Дата, Число, Булево.
		/// Тип значения определяется ключом. Например, если требуется отобрать письма до определённой даты, то в качестве значения необходимо использовать выражения типа Дата. 
		/// Если в качестве значения используется строка, то регистр не имеет значения. 
		/// Если заданы несколько ключей, то они применяются последовательно по логическому И.
		/// 
		/// - ОтправленОтвет (Answered) - Булево. Отобрать сообщения, у которых установлен флаг – Answered;
		/// - Недавние (Recent) - Булево. Отобрать сообщения, пришедшие в рамках текущей IMAP-сессии.
		/// - СлепыеКопии (Bcc) - Строка. Отобрать сообщения, которые имеют “строка” в поле Bcc;
		/// - Копии (Cc) - Строка. Отобрать сообщения, которые имеют “строка” в поле Cc;
		/// - Получатели (To) - Строка. Отобрать сообщения, которые имеют “строка” в поле To;
		/// - ДатаОтправления (PostDating) - Дата. Отобрать сообщения, у которых значение поле Date: равно “Дата”;
		/// - Отправитель (From) - Строка. Отобрать все сообщения у которых встречается “строка”в поле From;
		/// - ДоДатыОтправления (BeforeDateOfPosting) - Дата. Отобрать сообщения, у которых значение поле Date: перед “дата”;
		/// - ПослеДатыОтправления (AfterDateOfPosting) - Дата. Отобрать сообщения, у которых значение поля Date: после значения “Дата”;
		/// - Тема (Subject) - Строка. Отобрать сообщения, в заголовке которых встречается заданная строка;
		/// - Текст (Text) - Строка. Отобрать сообщения, в любых текстовых полях которого встречается заданная строка;
		/// - ТелоСообщения (Body) - Строка. Отобрать сообщения, в теле которых встречается строка – “строка”;
		/// - Удаленные (Deleted) - Булево. Отобрать сообщения, которые должны быть удалены или не должны быть удалены;
		/// - УстановленФлаг (Flagged) - Булево. Отобрать сообщения, которые помечены флагом или не помечены флагом;
		/// - Прочитанные (Seen) - Булево. Отобрать сообщения, которые были прочитаны или не прочитаны;
		/// - Новые (New) - Булево. Отобрать новые или старые сообщения.
		/// Пример:
		/// <code>
		/// ПараметрыОтбораIMAP =НовыйСтруктура;
		/// ПараметрыОтбораIMAP.Вставить("Новые",Истина);
		/// ПараметрыОтбораIMAP.Вставить("Тема", "привет");
		/// Почта.ПолучитьЗаголовки(ПараметрыОтбораIMAP);
		/// </code>
		/// Строковые значения критериев отбора отправляются в кодировке US-ASCII, если содержат только символы ANSI и в кодировке UTF-8 - в противном случае.
		/// Не все IMAP-серверы поддерживают UTF-8, поэтому сервер может выдать соответствующую ошибку.</param>
		/// <returns></returns>
		[ContextMethod("ПолучитьИдентификаторы", "GetIdentifiers")]
		public ArrayImpl GetIdentifiers(ArrayImpl identifiers = null, StructureImpl filter = null)
		{
			return receiver?.GetIdentifiers(identifiers, filter);
		}

		/// <summary>
		/// Получает количество сообщений IMAP в текущем почтовом ящике (ТекущийПочтовыйЯщик) или количество сообщений в ящике POP3.
		/// </summary>
		/// <returns></returns>
		[ContextMethod("ПолучитьКоличествоСообщений", "GetMessageCount")]
		public int GetMessageCount()
		{
			return receiver?.GetMessageCount() ?? 0;
		}

		/// <summary>
		/// При работе с POP3 удаляет с сервера все сообщения, указанные либо объектами ИнтернетПочтовоеСообщение, либо идентификаторами, находящимися в массиве, принимаемом в качестве параметра.
		/// При работе с IMAP помечает как удаленные на сервере все сообщения, указанные одним из следующих способов:
		/// 
		/// - объектами ИнтернетПочтовоеСообщение,
		/// - идентификаторами, находящимися в массиве, принимаемом в качестве параметра,
		/// - порядковыми номерами сообщений в текущем почтовом ящике, заданном свойством ТекущийПочтовыйЯщик.
		/// Помеченные сообщения можно окончательно удалить методом ОчиститьУдаленныеСообщения или снять отметку удаления с помощью метода ОтменитьУдалениеСообщений.
		/// </summary>
		/// <param name="dataToDelete"></param>
		[ContextMethod("УдалитьСообщения", "DeleteMessages")]
		public void DeleteMessages(ArrayImpl dataToDelete)
		{
			receiver?.DeleteMessages(dataToDelete);
		}

		/// <summary>
		/// Получает массив имен всех почтовых ящиков на сервере для данной учётной записи.
		/// </summary>
		/// <returns></returns>
		[ContextMethod("ПолучитьПочтовыеЯщики", "GetMailBoxes")]
		public ArrayImpl GetMailboxes()
		{
			return receiver?.GetMailboxes();
		}

		/// <summary>
		/// Получить список всех почтовых ящиков, которые помечены как подписанные.
		/// </summary>
		/// <returns></returns>
		[ContextMethod("ПолучитьПочтовыеЯщикиПоПодписке", "GetMailBoxesBySubscription")]
		public ArrayImpl GetMailboxesBySubscription()
		{
			return receiver?.GetMailboxesBySubscription();
		}

		/// <summary>
		/// Подписать почтовый ящик IMAP для дальнейшей работы с ним.
		/// </summary>
		/// <param name="name">Имя IMAP почтового ящика.</param>
		[ContextMethod("ПодписатьсяНаПочтовыйЯщик", "SubscribeToMailbox")]
		public void SubscribeToMailbox(string name)
		{
			receiver?.SubscribeToMailbox(name);
		}

		/// <summary>
		/// Отозвать подписку на IMAP почтовый ящик.
		/// </summary>
		/// <param name="name">Имя IMAP почтового ящика.</param>
		[ContextMethod("ОтменитьПодпискуНаПочтовыйЯщик", "UnsubscribeFromMailbox")]
		public void UnsubscribeFromMailbox(string name)
		{
			receiver?.UnsubscribeFromMailbox(name);
		}

		/// <summary>
		/// Снимает пометку удаления для IMAP письма. Позволяет отменить действия метода УдалитьСообщения.
		/// </summary>
		/// <param name="deletedData">Cодержит либо заголовки сообщений, либо серверные идентификаторы сообщений,
		/// либо массив порядковых номеров почтовых сообщений, для которых необходимо отменить действие удаления с сервера.</param>
		[ContextMethod("ОтменитьУдалениеСообщений", "UndeleteMessages")]
		public void UndeleteMessages(ArrayImpl deletedData)
		{
			receiver?.UndeleteMessages(deletedData);
		}

		/// <summary>
		/// Удаляет из текущего почтового ящика IMAP все письма, которые были помечены для удаления.
		/// </summary>
		[ContextMethod("ОчиститьУдаленныеСообщения", "ClearDeletedMessages")]
		public void ClearDeletedMessages()
		{
			receiver?.ClearDeletedMessages();
		}

		/// <summary>
		/// Выполняет переименование почтового ящика.
		/// </summary>
		/// <param name="name">Имя почтового ящика, которое требуется переименовать.</param>
		/// <param name="newName">Новое имя, которое будет назначено почтовому ящику.</param>
		[ContextMethod("ПереименоватьПочтовыйЯщик", "RenameMailbox")]
		public void RenameMailbox(string name, string newName)
		{
			receiver?.RenameMailbox(name, newName);
		}

		/// <summary>
		/// Создает IMAP почтовый ящик по заданному имени. Ящик создаётся от корня. Имя может содержать иерархический путь к почтовому ящику. 
		/// Символ-разделитель иерархии почтовых ящиков, используемый текущим почтовым сервером, доступен с помощью свойства СимволРазделитель.
		/// </summary>
		/// <param name="name">Имя почтового ящика. Может содержать иерархию.
		/// В качестве разделителя используется символ, который доступен через свойство СимволРазделитель.
		/// В большинстве случаев это символ '/'.</param>
		[ContextMethod("СоздатьПочтовыйЯщик", "CreateMailbox")]
		public void CreateMailbox(string name)
		{
			receiver?.CreateMailbox(name);
		}

		/// <summary>
		/// При работе с POP3 удаляет с сервера все сообщения, указанные либо объектами ИнтернетПочтовоеСообщение, либо идентификаторами, находящимися в массиве, принимаемом в качестве параметра.
		/// При работе с IMAP помечает как удаленные на сервере все сообщения, указанные одним из следующих способов:
		/// 
		/// - объектами ИнтернетПочтовоеСообщение,
		/// - идентификаторами, находящимися в массиве, принимаемом в качестве параметра,
		/// - порядковыми номерами сообщений в текущем почтовом ящике, заданном свойством ТекущийПочтовыйЯщик.
		/// Помеченные сообщения можно окончательно удалить методом ОчиститьУдаленныеСообщения или снять отметку удаления с помощью метода ОтменитьУдалениеСообщений.
		/// </summary>
		/// <param name="name">Массив, содержащий либо заголовки сообщений, либо серверные идентификаторы сообщений, которые необходимо удалить с сервера.
		/// Для варианта работы с IMAP протоколом также допускается передать массив порядковых номеров сообщений (целые числа) в текущем почтовом ящике (ТекущийПочтовыйЯщик).
		/// </param>
		[ContextMethod("УдалитьПочтовыйЯщик", "DeleteMailbox")]
		public void DeleteMailbox(string name)
		{
			receiver?.DeleteMailbox(name);
		}

		/// <summary>
		/// Используется для получения сообщений с сервера.
		/// </summary>
		/// <param name="deleteMessages">Истина - удалять выбранные сообщения с сервера.
		/// Значение по умолчанию: Истина.</param>
		/// <param name="ids">Массив, содержащий либо заголовки сообщений, либо серверные идентификаторы сообщений, которые необходимо получить. 
		/// Для IMAP соединения массив может содержать порядковые номера сообщений в текущем почтовом ящике (см. ТекущийПочтовыйЯщик).
		/// Значение по умолчанию: Пустой массив.</param>
		/// <param name="markAsRead">Отмечать письма на сервере как прочтенные.
		/// Актуален только для IMAP, для протокола POP3 единственное допустимое значение - Истина.
		/// Значение по умолчанию: Истина.</param>
		/// <returns></returns>
		[ContextMethod("Выбрать", "Get")]
		public ArrayImpl Get(bool? deleteMessages = null, ArrayImpl ids = null, bool? markAsRead = null)
		{
			return receiver?.Get(deleteMessages ?? true, ids, markAsRead ?? true);
		}

		/// <inheritdoc />
		public void Dispose()
		{
			smtpClient.Dispose();
			(receiver as IDisposable)?.Dispose();
		}

		[ScriptConstructor]
		public static IRuntimeContextInstance Constructor()
		{
			return new InternetMail();
		}
	}
}
