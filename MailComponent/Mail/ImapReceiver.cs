/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using ScriptEngine.Machine;
using MailKit.Net.Imap;
using MailKit;
using MailKit.Search;
using System.Linq;
using System.Collections.Generic;
using ScriptEngine.HostedScript.Library;

namespace OneScript.InternetMail
{
	public class ImapReceiver : IMailReceiver, IMailSender, IDisposable
	{
		private readonly ImapClient client = new ImapClient();
		private InternetMailProfile _profile;
		private string _currentMailbox = "";
		private string _mailboxDelimiterCharacter = "/";
		private IMailFolder _currentFolder = null;

		public void SetCurrentMailbox(string mailbox)
		{
			_currentMailbox = mailbox;
			UpdateCurrentFolder();
		}

		public void SetDelimiterCharacter(string delimiter)
		{
			_mailboxDelimiterCharacter = delimiter;
			UpdateCurrentFolder();
		}

		private void CloseCurrentFolder()
		{
			if (_currentFolder != null)
			{
				_currentFolder.Close(false);
				_currentFolder = null;
			}
		}

		private void UpdateCurrentFolder()
		{
			CloseCurrentFolder();

			if (string.IsNullOrEmpty(_currentMailbox))
				_currentFolder = client.Inbox;
			else
				_currentFolder =  client.GetFolder(_currentMailbox);

			_currentFolder.Open(FolderAccess.ReadWrite);
		}

		public void ClearDeletedMessages()
		{
			_currentFolder.Expunge();
		}

		public void CreateMailbox(string name)
		{
			_currentFolder.Create(name, true);
		}

		public void DeleteMailbox(string name)
		{
			var folderToDelete = client.GetFolder(name);
			folderToDelete.Delete();
		}

		// Альмаматерь добавляет префикс к порядковым номерам. Сделаем же так и мы.
		private static string ID_PREFIX = "imap-";

		private string UniqueIdToInternalId(UniqueId id)
		{
			return string.Format("{0}{1}", ID_PREFIX, id.Id);
		}

		private UniqueId InternalIdToUniqueId(string id)
		{
			if (id.StartsWith(ID_PREFIX, StringComparison.Ordinal))
				return new UniqueId(0, UInt32.Parse(id.Substring(ID_PREFIX.Length)));

			return UniqueId.Invalid;
		}

		private IList<UniqueId> GetMessagesList(ArrayImpl ids)
		{
			var result = new List<UniqueId>();
			if (ids == null)
			{
				result.AddRange(_currentFolder
				                .Fetch(0, -1, MessageSummaryItems.UniqueId)
				                .Select((IMessageSummary arg) => arg.UniqueId));
			}
			else
			{
				// Получим список идентификаторов писем с учётом возможных вариантов входящих данных
				var Uids = new List<UniqueId>();
				foreach (var data in ids)
				{
					if (data.DataType == DataType.String)
					{
						// Идентификатор сообщения
						Uids.Add(InternalIdToUniqueId(data.AsString()));
					}
					else if (data.DataType == DataType.Number)
					{
						// Передан порядковый номер в текущем ящике
						var index = (int)data.AsNumber();
						var letterData = _currentFolder.Fetch(index, index, MessageSummaryItems.UniqueId);
						foreach (var oneData in letterData) 
							Uids.Add(oneData.UniqueId);
					}
					else if (data is InternetMailMessage)
					{
						// ИнтернетПочтовоеСообщение
						foreach (var id in (data as InternetMailMessage).Uid)
						{
							Uids.Add(InternalIdToUniqueId(id.AsString()));
						}
					}
					else if (data is ArrayImpl)
					{
						// Массив идентификаторов
						foreach (var id in (data as ArrayImpl))
						{
							Uids.Add(InternalIdToUniqueId(id.AsString()));
						}
					}
				}
				result.AddRange(Uids);
			}

			return result;
		}

		public void DeleteMessages(ArrayImpl dataToDelete)
		{
			var messages = GetMessagesList(dataToDelete);
			_currentFolder.AddFlags(messages, MessageFlags.Deleted, silent: true);
		}

		public ArrayImpl Get(bool deleteMessages, ArrayImpl ids, bool markAsRead)
		{
			var result = new ArrayImpl();
			var processedMessages = GetMessagesList(ids);

			foreach (var i in processedMessages)
			{
				var mimeMessage = _currentFolder.GetMessage(i);
				var iMessage = new InternetMailMessage(mimeMessage, UniqueIdToInternalId(i));
				result.Add(iMessage);
			}

			if (processedMessages.Count > 0)
			{
				if (deleteMessages)
				{
					_currentFolder.AddFlags(processedMessages, MessageFlags.Deleted, silent: true);
				}
				else if (markAsRead)
				{
					_currentFolder.AddFlags(processedMessages, MessageFlags.Seen, silent: true);
				}
			}

			return result;
		}

		private IList<UniqueId> SearchMessages(StructureImpl filter)
		{
			var imapFilter = new InternetMailImapSearchFilter(filter);
			var query = imapFilter.CreateSearchQuery();
			return _currentFolder.Search(query);
		}

		public ArrayImpl GetHeaders(StructureImpl filter)
		{

			var result = new ArrayImpl();

			var dataToFetch = MessageSummaryItems.Envelope
			                                     | MessageSummaryItems.Flags
			                                     | MessageSummaryItems.UniqueId
			                                     | MessageSummaryItems.InternalDate
												 | MessageSummaryItems.MessageSize
			;

			IList<IMessageSummary> allHeaders;

			if (filter == null)
				allHeaders = _currentFolder.Fetch(0, -1, dataToFetch);
			else
			{
				var ids = SearchMessages(filter);
				if (ids.Count == 0)
					return result;

				allHeaders = _currentFolder.Fetch(ids, dataToFetch);
			}

			foreach (var headers in allHeaders)
			{
				var mailMessage = new InternetMailMessage(headers);

				mailMessage.Uid.Add(ValueFactory.Create(UniqueIdToInternalId(headers.UniqueId)));
				result.Add(mailMessage);
			}

			return result;
		}

		public ArrayImpl GetIdentifiers(ArrayImpl identifiers, StructureImpl filter)
		{
			var result = new ArrayImpl();
			var allUids = GetHeaders(filter);

			foreach (var ivHeaderWithUid in allUids)
			{
				var headerWithUid = ivHeaderWithUid as InternetMailMessage;
				var Id = headerWithUid.Uid.Get(0);

				if (identifiers == null || identifiers.Find(Id).DataType == DataType.Undefined)
					result.Add(Id);
			}

			return result;
		}

		public ArrayImpl GetMailboxes(bool subscribedOnly)
		{
			var result = new ArrayImpl();

			var allFolders = client.GetFolders(null, subscribedOnly);
			foreach (var folder in allFolders)
			{
				result.Add(ValueFactory.Create(folder.FullName));
			}

			return result;
		}

		public ArrayImpl GetMailboxes()
		{
			return GetMailboxes(subscribedOnly: false);
		}

		public ArrayImpl GetMailboxesBySubscription()
		{
			return GetMailboxes(subscribedOnly: true);
		}

		public int GetMessageCount()
		{
			return _currentFolder.Count;
		}

		public void Logoff()
		{
			CloseCurrentFolder();
			if (client.IsConnected)
				client.Disconnect(true);
		}

		public void Logon(InternetMailProfile profile)
		{
			_profile = profile;

			client.Timeout = _profile.Timeout * 1000;
			client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
			client.Connect(profile.ImapServerAddress, profile.GetImapPort());

			if (!string.IsNullOrEmpty(profile.ImapUser))
				client.Authenticate(profile.ImapUser, profile.ImapPassword);

			UpdateCurrentFolder();
		}

		public void RenameMailbox(string name, string newName)
		{
			var oldFolder = client.GetFolder(name);
			oldFolder.Rename(oldFolder.ParentFolder, newName);
		}

		public void SubscribeToMailbox(string name)
		{
			var folder = client.GetFolder(name);
			folder.Subscribe();
		}

		public void UndeleteMessages(ArrayImpl deletedData)
		{
			var messages = GetMessagesList(deletedData);
			_currentFolder.RemoveFlags(messages, MessageFlags.Deleted, silent: true);
		}

		public void UnsubscribeFromMailbox(string name)
		{
			var folder = client.GetFolder(name);
			if (folder.IsSubscribed)
				folder.Unsubscribe();
		}

		public void Dispose()
		{
			Logoff();
		}

		public void Send(InternetMailMessage message, InternetMailTextProcessing processText)
		{
			var messageToSend = message.CreateNativeMessage(processText);
			_currentFolder.Append(messageToSend);
		}
	}
}
