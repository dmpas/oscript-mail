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
using MimeKit;

namespace OneScript.InternetMail
{
    [ContextClass("ИнтернетПочтовоеСообщение", "InternetMailMessage")]
    public class InternetMailMessage : AutoContext<InternetMailMessage>
    {
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
        public string Uid { get; set; }

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
        public int PostingDateOffset { get; }

        [ContextProperty("СпособКодированияНеASCIIСимволов", "NonAsciiSymbolsEncodingMode")]
        public InternetMailMessageNonAsciiSymbolsEncodingMode NonAsciiSymbolsEncodingMode { get; set; }

        [ContextProperty("СтатусРазбора", "ParseStatus")]
        public InternetMailMessageParseStatus ParseStatus { get; }

        [ContextProperty("Тексты", "Texts")]
        public InternetMailTexts Texts { get; }

        [ContextProperty("Тема", "Theme")]
        public string Theme { get; set; }

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
            return ValueFactory.Create();
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
        }

		public MimeMessage CreateNativeMessage(InternetMailTextProcessing processText = InternetMailTextProcessing.Process)
		{
			if (processText == InternetMailTextProcessing.Process)
				ProcessTexts();

			var messageToSend = new MimeMessage();

			if (Sender.DataType == DataType.String)
				messageToSend.From.Add(new MailboxAddress("", Sender.AsString()));
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

			messageToSend.Subject = Theme;

			if (Texts.Count() == 1)
			{
				messageToSend.Body = Texts.Get(0).CreateTextPart();
			}
			else {
				var body = new Multipart();
				foreach (var text in Texts)
				{
					var part = text.CreateTextPart();
					body.Add(part);
				}
				messageToSend.Body = body;
			}

			return messageToSend;
		}

        [ScriptConstructor]
        public static IRuntimeContextInstance Constructor()
        {
            return new InternetMailMessage();
        }
    }
}
