/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using ScriptEngine.Machine.Contexts;
using ScriptEngine.Machine;
using MailKit.Net.Smtp;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Security;
using MimeKit;
using ScriptEngine.HostedScript.Library;

namespace OneScript.InternetMail
{
	public class Pop3Receiver : IMailReceiver, IDisposable
	{

		private readonly Pop3Client client = new Pop3Client();
		private readonly string notSupportedMessage = "Операция невыполнима в текущем протоколе";

		public Pop3Receiver()
		{
		}

		public Pop3Receiver(string notSupportedMessage)
		{
			this.notSupportedMessage = notSupportedMessage;
		}


		public void Logon(InternetMailProfile profile)
		{
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

		public void ClearDeletedMessages()
		{
			throw new NotImplementedException();
		}

		public void DeleteMessages(ArrayImpl dataToDelete)
		{
			throw new NotImplementedException();
		}

		public ArrayImpl GetHeaders(StructureImpl filter)
		{
			var result = new ArrayImpl();
			var allHeaders = client.GetMessageHeaders(0, client.Count);

			foreach (var headers in allHeaders)
			{
				var mailMessage = new InternetMailMessage(headers);
				result.Add(mailMessage);
			}

			return result;
		}

		public ArrayImpl GetIdentifiers(ArrayImpl identifiers, StructureImpl filter)
		{
			var result = new ArrayImpl();
			var allUids = client.GetMessageUids();

			foreach (var uid in allUids)
			{
				result.Add(ValueFactory.Create(uid));
			}

			return result;
		}

		public int GetMessageCount()
		{
			return client.Count;
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
		#endregion

		public void Dispose()
		{
			Logoff();
		}
	}
}
