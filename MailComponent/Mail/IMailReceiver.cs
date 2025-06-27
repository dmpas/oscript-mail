/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using OneScript.StandardLibrary.Collections;

namespace OneScript.InternetMail
{
	public interface IMailReceiver
	{

		void Logon(InternetMailProfile profile);
		void Logoff();

		ArrayImpl GetHeaders(StructureImpl filter);
		ArrayImpl GetIdentifiers(ArrayImpl identifiers, StructureImpl filter);
		int GetMessageCount();
		void DeleteMessages(ArrayImpl dataToDelete);

		ArrayImpl Get(bool deleteMessages, ArrayImpl ids, bool markAsRead);

		// IMAP Only

		ArrayImpl GetMailboxes();
		ArrayImpl GetMailboxesBySubscription();
		void SubscribeToMailbox(string name);
		void UnsubscribeFromMailbox(string name);
		void UndeleteMessages(ArrayImpl deletedData);
		void ClearDeletedMessages();
		void RenameMailbox(string name, string newName);
		void CreateMailbox(string name);
		void DeleteMailbox(string name);
	}
}
