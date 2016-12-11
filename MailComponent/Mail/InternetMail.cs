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
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace OneScript.InternetMail
{
	[ContextClass("ИнтернетПочта", "InternetMail")]
	public class InternetMail : AutoContext<InternetMail>, IDisposable
	{
		private InternetMailProfile _profile;

		private SmtpClient smtpClient = new SmtpClient();
		private IMailReceiver receiver;
		private string _currentMailbox = "";
		private string _mailboxDelimiterCharacter = "";

		public InternetMail()
		{
		}

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
			SecureSocketOptions options = SecureSocketOptions.Auto;
			smtpClient.Timeout = _profile.Timeout * 1000;
			smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
			smtpClient.Connect(_profile.SmtpServerAddress, _profile.GetSmtpPort(), options);

			if (_profile.SmtpUser != "")
				smtpClient.Authenticate(_profile.SmtpUser, _profile.SmtpPassword);
		}

		private void LogoffSmtp()
		{
			if (smtpClient.IsConnected)
				smtpClient.Disconnect(true);
		}

		#endregion

		[ContextMethod("Подключиться", "Logon")]
		public void Logon(InternetMailProfile profile, InternetMailProtocol receiveMailProtocol = InternetMailProtocol.Pop3)
		{
			_profile = profile;

			if (!string.IsNullOrEmpty(_profile.SmtpServerAddress) && !_profile.Pop3BeforeSmtp)
				LogonSmtp();

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

		[ContextMethod("Отключиться", "Logoff")]
		public void Logoff()
		{
			LogoffSmtp();
			receiver?.Logoff();
		}

		[ContextMethod("Послать", "Send")]
		public void Send(InternetMailMessage message,
						 InternetMailTextProcessing processText = InternetMailTextProcessing.Process,
						 InternetMailProtocol protocol = InternetMailProtocol.Smtp)
		{
			if (protocol == InternetMailProtocol.Pop3)
			{
				throw new RuntimeException("Недопустимо указывать POP3 в качестве протокола отправки почты!");
			}

			var messageToSend = message.CreateNativeMessage(processText);

			if (protocol == InternetMailProtocol.Smtp)
			{
				smtpClient.Send(messageToSend);
			}
		}

		[ContextMethod("ПолучитьЗаголовки", "GetHeaders")]
		public ArrayImpl GetHeaders(StructureImpl filter = null)
		{
			return receiver?.GetHeaders(filter);
		}

		[ContextMethod("ПолучитьИдентификаторы", "GetIdentifiers")]
		public ArrayImpl GetIdentifiers(ArrayImpl identifiers = null, StructureImpl filter = null)
		{
			return receiver?.GetIdentifiers(identifiers, filter);
		}

		[ContextMethod("ПолучитьКоличествоСообщений", "GetMessageCount")]
		public int GetMessageCount()
		{
			return receiver?.GetMessageCount() ?? 0;
		}

		[ContextMethod("УдалитьСообщения", "DeleteMessages")]
		public void DeleteMessages(ArrayImpl dataToDelete)
		{
			receiver?.DeleteMessages(dataToDelete);
		}

		public ArrayImpl GetMailboxes()
		{
			return receiver?.GetMailboxes();
		}

		public ArrayImpl GetMailboxesBySubscription()
		{
			return receiver?.GetMailboxesBySubscription();
		}

		public void SubscribeToMailbox(string name)
		{
			receiver?.SubscribeToMailbox(name);
		}

		public void UnsubscribeFromMailbox(string name)
		{
			receiver?.UnsubscribeFromMailbox(name);
		}

		public void UndeleteMessages(ArrayImpl deletedData)
		{
			receiver?.UndeleteMessages(deletedData);
		}

		public void ClearDeletedMessages()
		{
			receiver?.ClearDeletedMessages();
		}

		public void RenameMailbox(string name, string newName)
		{
			receiver?.RenameMailbox(name, newName);
		}

		public void CreateMailbox(string name)
		{
			receiver?.CreateMailbox(name);
		}

		public void DeleteMailbox(string name)
		{
			receiver?.DeleteMailbox(name);
		}

		[ContextMethod("Выбрать", "Get")]
		public ArrayImpl Get(bool? deleteMessages = null, ArrayImpl ids = null, bool? markAsRead = null)
		{
			return receiver?.Get(deleteMessages ?? true, ids, markAsRead ?? true);
		}

		public void Dispose()
		{
			((IDisposable)smtpClient).Dispose();
		}

		[ScriptConstructor]
		public static IRuntimeContextInstance Constructor()
		{
			return new InternetMail();
		}
	}
}
