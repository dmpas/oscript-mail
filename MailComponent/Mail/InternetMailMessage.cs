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
		}

		[ContextProperty("АдресаУведомленияОДоставке", "DeliveryReceiptAddresses")]
		public InternetMailAddresses DeliveryReceiptAddresses { get; }

		[ContextProperty("АдресаУведомленияОПрочтении", "ReadReceiptAddresses")]
		public InternetMailAddresses ReadReceiptAddresses { get; }

		[ContextProperty("Важность", "Importance")]
		public InternetMailMessageImportance Importance { get; set; }

		[ContextProperty("Вложения", "Attachments")]
		public InternetMailAttachments Attachments { get; }

		[ContextProperty("ДатаОтправления", "PostingDate")]
		public DateTime PostingDate { get; private set; }

		[ContextProperty("ДатаПолучения", "DateReceived")]
		public DateTime DateReceived { get; private set; }

		[ContextProperty("Заголовок", "Header")]
		public string Header { get; private set; }

		[ContextProperty("Идентификатор", "Uid")]
		public ArrayImpl Uid { get; }

		[ContextProperty("ИдентификаторСообщения", "MessageId")]
		public string MessageId { get; private set; }

		[ContextProperty("ИмяОтправителя", "SenderName")]
		public string SenderName { get; set; }

		[ContextProperty("Категории", "Categories")]
		public string Categories { get; set; }

		[ContextProperty("Кодировка", "Encoding")]
		public string Encoding { get; set; }

		[ContextProperty("Копии", "Cc")]
		public InternetMailAddresses Cc { get; }

		[ContextProperty("ОбратныйАдрес", "ReplyTo")]
		public InternetMailAddresses ReplyTo { get; }

		[ContextProperty("Организация", "Organization")]
		public string Organization { get; }

		[ContextProperty("Отправитель", "Sender")]
		public IValue Sender { get; set; }

		[ContextProperty("Получатели", "To")]
		public InternetMailAddresses To { get; }

		[ContextProperty("Размер", "Size")]
		public int Size { get; }

		[ContextProperty("СлепыеКопии", "Bcc")]
		public InternetMailAddresses Bcc { get; }

		[ContextProperty("СмещениеДатыОтправления", "PostingDateOffset")]
		public decimal PostingDateOffset { get; private set; }

		[ContextProperty("СпособКодированияНеASCIIСимволов", "NonAsciiSymbolsEncodingMode")]
		public InternetMailMessageNonAsciiSymbolsEncodingMode NonAsciiSymbolsEncodingMode { get; set; }

		[ContextProperty("СтатусРазбора", "ParseStatus")]
		public InternetMailMessageParseStatus ParseStatus { get; }

		[ContextProperty("Тексты", "Texts")]
		public InternetMailTexts Texts { get; }

		[ContextProperty("Тема", "Subject")]
		public string Subject { get; set; } = "";

		[Obsolete]
		[ContextProperty("Theme")]
		public string ObsoleteTheme { get => Subject;
			set => Subject = value;
		}

		[ContextProperty("УведомитьОДоставке", "RequestDeliveryReceipt")]
		public bool RequestDeliveryReceipt { get; set; }

		[ContextProperty("УведомитьОПрочтении", "RequestReadReceipt")]
		public bool RequestReadReceipt { get; }

		[ContextProperty("Частичное", "Partial")]
		public bool Partial { get; }

		[ContextMethod("ОбработатьТексты", "ProcessTexts")]
		public void ProcessTexts()
		{
		}

		[ContextMethod("ПолучитьИсходныеДанные", "GetSourceData")]
		public BinaryDataContext GetSourceData()
		{
			return null;
		}

		[ContextMethod("ПолучитьИсходныйТекст", "GetSourceText")]
		public string GetSourceText(IValue encoding)
		{
			return "";
		}

		[ContextMethod("ПолучитьПолеЗаголовка", "GetField")]
		public IValue GetField(string fieldName, IValue type)
		{
			var value = headers[fieldName];
			return ValueFactory.Create(value);
		}

		[ContextMethod("УстановитьИсходныеДанные", "SetSourceData")]
		public void SetSourceData(BinaryDataContext data)
		{
		}

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
