/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using ScriptEngine.Machine;
using MailKit.Net.Pop3;
using System.Collections.Generic;
using ScriptEngine.HostedScript.Library;

namespace OneScript.InternetMail
{
	public class Pop3Receiver : IMailReceiver, IDisposable
	{

		private readonly Pop3Client client = new Pop3Client();
		private readonly string notSupportedMessage = "Операция невыполнима в текущем протоколе";
		private InternetMailProfile _profile;

		public Pop3Receiver()
		{
		}

		public Pop3Receiver(string notSupportedMessage)
		{
			this.notSupportedMessage = notSupportedMessage;
		}


		public void Logon(InternetMailProfile profile)
		{
			_profile = profile;

			client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
			client.Connect(profile.Pop3ServerAddress, profile.GetPop3Port());

			if (profile.User != "")
				client.Authenticate(profile.User, profile.Password);
		}

		public void Logoff()
		{
			if (client.IsConnected)
				client.Disconnect(true);
		}

		private void Relogon()
		{
			Logoff();
			Logon(_profile);
		}

		private IList<int> GetMessagesList(ArrayImpl ids)
		{
			var result = new List<int>();
			if (ids == null)
			{
				for (int i = 0; i < client.Count; i++)
					result.Add(i);
			}
			else
			{
				// Получим список идентификаторов писем с учётом возможных вариантов входящих данных
				var Uids = new List<string>();
				foreach (var data in ids)
				{
					if (data.DataType == DataType.String)
					{
						Uids.Add(data.AsString());
					}
					else if (data is InternetMailMessage)
					{
						foreach (var id in (data as InternetMailMessage).Uid)
						{
							Uids.Add(id.AsString());
						}
					}
					else if (data is ArrayImpl)
					{
						foreach (var id in (data as ArrayImpl))
						{
							Uids.Add(id.AsString());
						}
					}
				}

				for (int i = 0; i < client.Count; i++)
				{
					var uid = client.GetMessageUid(i);
					if (Uids.FindIndex((x) => x.Equals(uid, StringComparison.Ordinal)) != -1)
						result.Add(i);
				}
			}

			return result;
		}

		public void DeleteMessages(ArrayImpl dataToDelete)
		{
			var messages = GetMessagesList(dataToDelete);
			if (messages.Count > 0)
			{
				client.DeleteMessages(messages);
				Relogon(); // TODO: костыль. Подумать, почему падает без переподключения
			}
		}

		public ArrayImpl GetHeaders(StructureImpl filter)
		{
			var result = new ArrayImpl();

			if (client.Count > 0)
			{
				var allHeaders = client.GetMessageHeaders(0, client.Count);

				foreach (var headers in allHeaders)
				{
					var mailMessage = new InternetMailMessage(headers);
					result.Add(mailMessage);
				}
			}

			return result;
		}

		// В режиме Pop3 отбор filter игнорируется
		public ArrayImpl GetIdentifiers(ArrayImpl identifiers = null, StructureImpl filter = null)
		{
			var result = new ArrayImpl();
			var allUids = client.GetMessageUids();

			foreach (var uid in allUids)
			{
				var Id = ValueFactory.Create(uid);

				if (identifiers == null || identifiers.Find(Id).DataType != DataType.Undefined)
					result.Add(Id);
			}

			return result;
		}

		public int GetMessageCount()
		{
			return client.Count;
		}

		public ArrayImpl Get(bool deleteMessages, ArrayImpl ids, bool markAsRead)
		{
			if (markAsRead != true)
				throw RuntimeException.InvalidArgumentValue(); // TODO: Внятное сообщение

			var result = new ArrayImpl();
			var processedMessages = GetMessagesList(ids);

			foreach (var i in processedMessages)
			{
				var mimeMessage = client.GetMessage(i);
				var iMessage = new InternetMailMessage(mimeMessage, client.GetMessageUid(i));
				result.Add(iMessage);
			}

			if (deleteMessages && processedMessages.Count > 0)
			{
				client.DeleteMessages(processedMessages);
			}

			return result;
		}

		#region NotSupported in POP3

		public void RenameMailbox(string name, string newName)
		{
			throw new NotSupportedInSelectedProtocol(notSupportedMessage);
		}

		public void SubscribeToMailbox(string name)
		{
			throw new NotSupportedInSelectedProtocol(notSupportedMessage);
		}

		public void UndeleteMessages(ArrayImpl deletedData)
		{
			throw new NotImplementedException();
		}

		public void UnsubscribeFromMailbox(string name)
		{
			throw new NotSupportedInSelectedProtocol(notSupportedMessage);
		}
		public void CreateMailbox(string name)
		{
			throw new NotSupportedInSelectedProtocol(notSupportedMessage);
		}

		public void DeleteMailbox(string name)
		{
			throw new NotSupportedInSelectedProtocol(notSupportedMessage);
		}

		public ArrayImpl GetMailboxes()
		{
			throw new NotSupportedInSelectedProtocol(notSupportedMessage);
		}

		public ArrayImpl GetMailboxesBySubscription()
		{
			throw new NotSupportedInSelectedProtocol(notSupportedMessage);
		}

		public void ClearDeletedMessages()
		{
			throw new NotSupportedInSelectedProtocol(notSupportedMessage);
		}
		#endregion

		public void Dispose()
		{
			Logoff();
		}

	}
}
