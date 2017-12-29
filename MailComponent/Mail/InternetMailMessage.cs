/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using ScriptEngine.Machine.Contexts;
using ScriptEngine.Machine;
using ScriptEngine.HostedScript.Library;
using ScriptEngine.HostedScript.Library.Binary;
using System.IO;
using MimeKit;

namespace OneScript.InternetMail
{
	/// <summary>
	/// Почтовое сообщение (письмо).
	/// </summary>
	[ContextClass("ИнтернетПочтовоеСообщение", "InternetMailMessage")]
	public class InternetMailMessage : AutoContext<InternetMailMessage>
	{

		private readonly HeaderList headers = new HeaderList();

		public InternetMailMessage()
		{
			DeliveryReceiptAddresses = new InternetMailAddresses();
			ReadReceiptAddresses = new InternetMailAddresses();
			Attachments = new InternetMailAttachments();
			Cc = new InternetMailAddresses();
			ReplyTo = new InternetMailAddresses();
			To = new InternetMailAddresses();
			Bcc = new InternetMailAddresses();

			NonAsciiSymbolsEncodingMode = InternetMailMessageNonAsciiSymbolsEncodingMode.Mime;

			Texts = new InternetMailTexts();
			Uid = new ArrayImpl();
		}

		private static HeaderList GenerateHeadersList(MailKit.IMessageSummary headers)
		{
			var headerList = new HeaderList();
			foreach (var CcAddress in headers.Envelope.Cc)
			{
				headerList.Add("CC", CcAddress.ToString());
			}
			foreach (var CcAddress in headers.Envelope.Bcc)
			{
				headerList.Add("BCC", CcAddress.ToString());
			}
			foreach (var CcAddress in headers.Envelope.From)
			{
				headerList.Add("From", CcAddress.ToString());
			}
			foreach (var CcAddress in headers.Envelope.Sender)
			{
				headerList.Add("Sender", CcAddress.ToString());
			}
			headerList.Add("Subject", headers.Envelope.Subject ?? "");

			if (headers.Envelope.Date != null)
			{
				headerList.Add("Date", MimeKit.Utils.DateUtils.FormatDate(headers.Date));
			}
			return headerList;
		}

		public InternetMailMessage(HeaderList headers) : this()
		{
			using (var sw = new MemoryStream())
			{
				headers.WriteTo(sw);
				sw.Position = 0;

				using (var tr = new StreamReader(sw))
				{
					Header = tr.ReadToEnd();
				}
			}

			foreach (var header in headers)
			{
				SetField(header.Id.ToHeaderName(), ValueFactory.Create(header.Value));
			}

			DateReceived = DateTime.Now;
		}

		public InternetMailMessage(MailKit.IMessageSummary headers) : this(GenerateHeadersList(headers))
		{
		}

		public InternetMailMessage(MimeMessage nativeMessage, string identifier) : this(nativeMessage.Headers)
		{
            
            Uid.Add(ValueFactory.Create(identifier));
			if (nativeMessage.Body is TextPart)
			{
				Texts.Add(new InternetMailText(nativeMessage.Body as TextPart));
			}
			else if (nativeMessage.Body is Multipart)
			{
                
				var body = nativeMessage.Body as Multipart;
				foreach (var part in body)
				{
					if (part is TextPart)
					{
						var tpart = part as TextPart;
						Texts.Add(new InternetMailText(tpart));
					}
					else if (part is MessageDeliveryStatus)
					{
						// TODO: MessageDeliveryStatus
					}
					else if (part is MessagePart)
					{
						// Письмо во вложении
						// TODO: MessagePart
					}
					else
					{
						// Console.Write("Unchecked type: ");
						// Console.WriteLine(part.GetType());
					}
				}
			}

			foreach (var attachment in nativeMessage.Attachments)
			{
				var part = (MimePart)attachment;
				var fileName = part.FileName;
				var stream = new MemoryStream();
                
				part.ContentObject.DecodeTo(stream);
				BinaryDataContext bin = new BinaryDataContext(stream.ToArray());
				Attachments.Add(bin, fileName);
			}
		}

		/// <summary>
		/// Коллекция адресов, на которые будет высылаться уведомление о доставке.
		/// </summary>
		[ContextProperty("АдресаУведомленияОДоставке", "DeliveryReceiptAddresses")]
		public InternetMailAddresses DeliveryReceiptAddresses { get; }

		/// <summary>
		/// Коллекция адресов, на которые будет высылаться уведомление о прочтении.
		/// </summary>
		[ContextProperty("АдресаУведомленияОПрочтении", "ReadReceiptAddresses")]
		public InternetMailAddresses ReadReceiptAddresses { get; }

		/// <summary>
		/// Содержит важность почтового сообщения.
		/// </summary>
		[ContextProperty("Важность", "Importance")]
		public InternetMailMessageImportance Importance { get; set; }

		/// <summary>
		/// Содержит коллекцию объектов ИнтернетПочтовоеВложение.
		/// </summary>
		[ContextProperty("Вложения", "Attachments")]
		public InternetMailAttachments Attachments { get; }

		/// <summary>
		/// Дата отправления письма. Значение представлено датой, локальной для почтового клиента отправителя.
		/// </summary>
		[ContextProperty("ДатаОтправления", "PostingDate")]
		public DateTime PostingDate { get; private set; }

		/// <summary>
		/// Дата получения письма. Имеет смысл только для полученных писем.
		/// </summary>
		[ContextProperty("ДатаПолучения", "DateReceived")]
		public DateTime DateReceived { get; private set; }

		/// <summary>
		/// Содержит заголовок сообщения в том виде, в котором письмо хранится на сервере.
		/// </summary>
		[ContextProperty("Заголовок", "Header")]
		public string Header { get; private set; }

		/// <summary>
		/// Содержит строку, идентифицирующую сообщение.
		/// Данный идентификатор сообщения уникален в пределах почтового ящика и остается неизменным на протяжении
		/// всего времени существования этого сообщения в почтовом ящике на сервере.
		/// После объединения частичных сообщений, результирующее сообщение может содержать несколько идентификаторов,
		/// поэтому свойство имеет тип Массив.
		/// </summary>
		[ContextProperty("Идентификатор", "Uid")]
		public ArrayImpl Uid { get; }

		/// <summary>
		/// Уникальный идентификатор письма.
		/// </summary>
		[ContextProperty("ИдентификаторСообщения", "MessageId")]
		public string MessageId { get; private set; }

		/// <summary>
		/// Содержит имя отправителя письма.
		/// </summary>
		[ContextProperty("ИмяОтправителя", "SenderName")]
		public string SenderName { get; set; }

		/// <summary>
		/// Указываются категории, к которым относится сообщение. Категория может быть как одна, так и несколько, разделенных запятыми. Например:
		/// <code>
		/// Сообщение = Новый ИнтернетПочтовоеСообщение;
		/// Сообщение.Категории = "Личное, Поздравления, Подарки";
		/// </code>
		/// </summary>
		[ContextProperty("Категории", "Categories")]
		public string Categories { get; set; }

		/// <summary>
		/// Содержит кодировку всего сообщения, включая тему, отображаемые имена, тексты и наименования вложений,
		/// если для этих частей сообщения кодировка не указана.
		/// По умолчанию используется UTF8.
		/// </summary>
		[ContextProperty("Кодировка", "Encoding")]
		public string Encoding { get; set; }

		/// <summary>
		/// Содержит коллекцию объектов ИнтернетПочтовыйАдрес.
		/// </summary>
		[ContextProperty("Копии", "Cc")]
		public InternetMailAddresses Cc { get; }

		/// <summary>
		/// Содержит коллекцию объектов ИнтернетПочтовыйАдрес.
		/// Используется в случае, когда адресант хочет получить ответ от адресата на другой адрес,
		/// отличный от того, с которого отправляется письмо.
		/// </summary>
		[ContextProperty("ОбратныйАдрес", "ReplyTo")]
		public InternetMailAddresses ReplyTo { get; }

		/// <summary>
		/// Поле заголовка. Может заполняться отправителем почтового сообщения.
		/// </summary>
		[ContextProperty("Организация", "Organization")]
		public string Organization { get; }

		/// <summary>
		/// Отправитель сообщения.
		/// </summary>
		[ContextProperty("Отправитель", "Sender")]
		public IValue Sender { get; set; }

		/// <summary>
		/// Содержит коллекцию объектов ИнтернетПочтовыйАдрес.
		/// </summary>
		[ContextProperty("Получатели", "To")]
		public InternetMailAddresses To { get; }

		/// <summary>
		/// Размер сообщения. Может не совпадать с реальным размером сообщения.
		/// </summary>
		[ContextProperty("Размер", "Size")]
		public int Size { get; }

		/// <summary>
		/// Содержит коллекцию объектов ИнтернетПочтовыйАдрес.
		/// </summary>
		[ContextProperty("СлепыеКопии", "Bcc")]
		public InternetMailAddresses Bcc { get; }

		/// <summary>
		/// Смещение даты отправления от универсального времени (UTC) в секундах. Для часовых поясов, отстающих от UTC, значение отрицательное.
		/// Пример приведения даты отправления к дате в часовом поясе сеанса:
		/// ДатаОтправленияВЗонеОтправителя = Сообщение.ДатаОтправления; 
		/// <code>
		/// СмещениеОтправителя = Сообщение.СмещениеДатыОтправления;
		/// 
		/// // Дата отправления сообщения, приведенная к UTC
		/// ДатаОтправлениеUTC = ДатаОтправленияВЗонеОтправителя - СмещениеОтправителя;
		/// 
		/// ЧасовойПояс = ЧасовойПоясСеанса(); 
		/// 
		/// // Смещение времени получателя относительно UTC на дату отправки письма с учетом
		/// // смещения летнего времени
		/// СмещениеПолучателя = СмещениеСтандартногоВремени(ЧасовойПояс, ДатаОтправлениеUTC) + 
		/// СмещениеЛетнегоВремени(ЧасовойПояс, ДатаОтправлениеUTC);
		/// 
		/// // Дата отправления, приведенная к дате получателя. Смещение рассчитано на момент
		/// // отправления сообщения
		/// ЛокальнаяДатаОтправления = ДатаОтправленияВЗонеОтправителя + (СмещениеПолучателя – СмещениеОтправителя);
		/// 
		/// Пример приведения даты отправления к дате в часовом поясе компьютера.
		/// ДатаОтправленияВЗонеОтправителя = Сообщение.ДатаОтправления; 
		/// СмещениеОтправителя = Сообщение.СмещениеДатыОтправления;
		/// 
		/// // Дата отправления сообщения, приведенная к UTC
		/// ДатаОтправлениеUTC = ДатаОтправленияВЗонеОтправителя - СмещениеОтправителя;
		/// 
		/// ЧасовойПояс = ЧасовойПояс(); 
		/// 
		/// // Смещение времени получателя относительно UTC на дату отправки письма с учетом
		/// // смещения летнего времени
		/// СмещениеПолучателя = СмещениеСтандартногоВремени(ЧасовойПояс, ДатаОтправлениеUTC) + 
		/// СмещениеЛетнегоВремени(ЧасовойПояс, ДатаОтправлениеUTC);
		/// 
		/// // Дата отправления, приведенная к дате получателя. Смещение рассчитано на момент
		/// // отправления     сообщения
		/// ЛокальнаяДатаОтправления = ДатаОтправленияВЗонеОтправителя + (СмещениеПолучателя – СмещениеОтправителя);
		/// </code>
		/// </summary>
		[ContextProperty("СмещениеДатыОтправления", "PostingDateOffset")]
		public decimal PostingDateOffset { get; private set; }

		/// <summary>
		/// Содержит способ кодирования не ASCII символов сообщения. При создании объекта значение свойства MIME.
		/// </summary>
		[ContextProperty("СпособКодированияНеASCIIСимволов", "NonAsciiSymbolsEncodingMode")]
		public InternetMailMessageNonAsciiSymbolsEncodingMode NonAsciiSymbolsEncodingMode { get; set; }

		/// <summary>
		/// Содержит статус разбора исходного текста почтового сообщения.
		/// Если свойство имеет значение ОбнаруженыОшибки, то это означает, что во время разбора в структуре сообщения
		/// были найдены ошибки и объект ИнтернетПочтовоеСообщение сформирован по данным, которые удалось успешно распознать.
		/// </summary>
		[ContextProperty("СтатусРазбора", "ParseStatus")]
		public InternetMailMessageParseStatus ParseStatus { get; }

		/// <summary>
		/// Содержит коллекцию объектов ИнтернетТекстПочтовогоСообщения.
		/// Может содержать одно или несколько текстовых частей сообщения или не содержать ни одной текстовой части.
		/// </summary>
		[ContextProperty("Тексты", "Texts")]
		public InternetMailTexts Texts { get; }

		/// <summary>
		/// Тема сообщения.
		/// </summary>
		[ContextProperty("Тема", "Subject")]
		public string Subject { get; set; } = "";

		[Obsolete]
		[ContextProperty("Theme")]
		public string ObsoleteTheme { get => Subject;
			set => Subject = value;
		}

		/// <summary>
		/// Уведомить о доставке почтового сообщения. Значение по умолчанию - Ложь.
		/// Истина - уведомление необходимо.
		/// </summary>
		[ContextProperty("УведомитьОДоставке", "RequestDeliveryReceipt")]
		public bool RequestDeliveryReceipt { get; set; }

		/// <summary>
		/// Уведомить о прочтении почтового сообщения. Значение по умолчанию - Ложь.
		/// Истина - уведомление необходимо.
		/// </summary>
		[ContextProperty("УведомитьОПрочтении", "RequestReadReceipt")]
		public bool RequestReadReceipt { get; }

		/// <summary>
		/// Показывает, является ли сообщение частичным (в случае, если большое сообщение было разбито на части).
		/// Истина - сообщение частично.
		/// </summary>
		[ContextProperty("Частичное", "Partial")]
		public bool Partial { get; }

		/// <summary>
		/// Производит предварительную обработку текстов письма, с возможностью последующего изменения содержимого текстов.
		/// </summary>
		[ContextMethod("ОбработатьТексты", "ProcessTexts")]
		public void ProcessTexts()
		{
		}

		/// <summary>
		/// Возвращает исходные данные почтового сообщения.
		/// Если сообщение принято с сервера (с помощью метода Выбрать объекта ИнтернетПочта) и не было изменено,
		/// то возвращаемые методом данные соответствуют тем, которые были получены с сервера.
		/// Если объект ИнтернетПочтовоеСообщение создан программно или получен с сервера и был изменен,
		/// то данные генерируются на основе содержимого этого объекта, в том виде,
		/// в каком они готовились к передаче почтовому серверу при отправке.
		/// </summary>
		/// <returns></returns>
		[ContextMethod("ПолучитьИсходныеДанные", "GetSourceData")]
		public BinaryDataContext GetSourceData()
		{
			return null;
		}

		/// <summary>
		/// Возвращает исходный текст почтового сообщения.
		/// Если сообщение принято с сервера (с помощью метода Выбрать объекта ИнтернетПочта) и не было изменено,
		/// то возвращаемый методом текст соответствуют тексту, полученному с сервера
		/// Если объект ИнтернетПочтовоеСообщение создан программно или получен с сервера и был изменен,
		/// то текст генерируется на основе содержимого этого объекта.
		/// </summary>
		/// <param name="encoding"></param>
		/// <returns></returns>
		[ContextMethod("ПолучитьИсходныйТекст", "GetSourceText")]
		public string GetSourceText(IValue encoding)
		{
			return "";
		}

		/// <summary>
		/// Получает значение требуемого поля.
		/// </summary>
		/// <param name="fieldName">Имя получаемого поля из главного заголовка сообщения.
		/// Имя может состоять только из печатных символов набора ASCII (т.е. из символов с номерами от 33 до 126 включительно),
		/// за исключением символа "двоеточие" (":").
		/// Данный диапазон содержит все буквы английского алфавита (как строчные, так и заглавные) и несколько специальных символов.</param>
		/// <param name="type">Указывается тип возвращаемого значения поля. Значение переданного объекта остается без изменения.</param>
		/// <returns></returns>
		[ContextMethod("ПолучитьПолеЗаголовка", "GetField")]
		public IValue GetField(string fieldName, IValue type)
		{
			var value = headers[fieldName];
			return ValueFactory.Create(value);
		}

		/// <summary>
		/// Устанавливает исходные данные сообщения и инициализирует на основе них объект типа ИнтернетПочтовоеСообщение.
		/// В случае ошибки разбора указанных данных генерируетcя исключение.
		/// </summary>
		/// <param name="data">Исходные данные почтового сообщения.</param>
		[ContextMethod("УстановитьИсходныеДанные", "SetSourceData")]
		public void SetSourceData(BinaryDataContext data)
		{
		}

		/// <summary>
		/// Добавляет пользовательское поле в главный заголовок почтового сообщения. Текстовые данные будут конвертироваться в кодировку, заданную свойством Кодировка.
		/// </summary>
		/// <param name="fieldName">Имя поля помещаемого в главный заголовок сообщения.
		/// Имя может состоять только из печатных символов набора ASCII (т.е. из символов диапазона от 33 до 126 включительно),
		/// за исключением символа "двоеточие" (":").
		/// Данный диапазон содержит все буквы английского алфавита (как строчные, так и прописные) и несколько специальных символов.</param>
		/// <param name="value">Значение добавляемого поля.</param>
		/// <param name="encodingMode">Способ кодирования поля в заголовке. Если параметр не задан, будет использоваться значение из свойства СпособКодированияНеASCIIСимволов.</param>
		[ContextMethod("УстановитьПолеЗаголовка", "SetField")]
		public void SetField(string fieldName, IValue value, InternetMailMessageNonAsciiSymbolsEncodingMode? encodingMode = null)
		{
			if (encodingMode == null)
				encodingMode = NonAsciiSymbolsEncodingMode;

			var stringValue = value.AsString();

			headers.Add(fieldName, stringValue);
			if (fieldName.Equals("BCC", StringComparison.InvariantCultureIgnoreCase))
				Bcc.Add(stringValue);

			else if (fieldName.Equals("CC", StringComparison.InvariantCultureIgnoreCase))
				Cc.Add(stringValue);

			else if (fieldName.Equals("Date", StringComparison.InvariantCultureIgnoreCase))
			{
				DateTimeOffset res;
				if (MimeKit.Utils.DateUtils.TryParse(stringValue, out res))
				{
					PostingDate = res.DateTime;
					PostingDateOffset = new Decimal(res.Offset.TotalSeconds);
				}
			}
			else if (fieldName.Equals("From", StringComparison.InvariantCultureIgnoreCase))
				Sender = value;

			else if (fieldName.Equals("Sender", StringComparison.InvariantCultureIgnoreCase))
				SenderName = stringValue;

			else if (fieldName.Equals("Subject", StringComparison.InvariantCultureIgnoreCase))
				Subject = stringValue;
		}

		public MimeMessage CreateNativeMessage(InternetMailTextProcessing processText = InternetMailTextProcessing.Process)
		{
			if (processText == InternetMailTextProcessing.Process)
				ProcessTexts();

			var messageToSend = new MimeMessage();

			if (Sender.DataType == DataType.String)
				messageToSend.From.Add(new MailboxAddress(SenderName, Sender.AsString()));
			else if (Sender is InternetMailAddress)
				messageToSend.From.Add((Sender as InternetMailAddress).GetInternalObject());
			else
				throw new RuntimeException("Неверный тип отправителя!");

			foreach (var recipient in To)
			{
				messageToSend.To.Add(recipient.GetInternalObject());
			}

			foreach (var replyTo in ReplyTo)
			{
				messageToSend.ReplyTo.Add(replyTo.GetInternalObject());
			}

			messageToSend.Subject = Subject;

			var body = new Multipart();
			foreach (var text in Texts)
			{
				var part = text.CreateTextPart();
				body.Add(part);
			}

			if (Attachments.Count() > 0) {
				foreach (InternetMailAttachment attachment in Attachments)
				{
					var mimeattachment = new MimePart()
					{
						ContentObject = new ContentObject(((BinaryDataContext) attachment.Data.AsObject()).OpenStreamForRead().GetUnderlyingStream(), ContentEncoding.Default),
						ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
						ContentTransferEncoding = ContentEncoding.Base64,
						FileName = attachment.FileName
					};
					if (String.IsNullOrEmpty(attachment.FileName))
					{
						mimeattachment.FileName = attachment.Name;
					}

					body.Add(mimeattachment);
				}
			}

			messageToSend.Body = body;

			return messageToSend;
		}

		[ScriptConstructor]
		public static IRuntimeContextInstance Constructor()
		{
			return new InternetMailMessage();
		}
	}
}
