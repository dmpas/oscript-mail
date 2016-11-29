/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using ScriptEngine.Machine.Contexts;
using System.Net.Mail;
using MimeKit;

namespace OneScript.InternetMail
{
	[ContextClass("ИнтернетПочтовыйАдрес", "InternetMailAddress")]
	public class InternetMailAddress : AutoContext<InternetMailAddress>
	{
		[ContextProperty("Адрес", "Address")]
		public string Address
		{
			get
			{
				return String.Format("{0}@{1}", User, Server);
			}
			set
			{
				var address = new MailAddress(value);
				User = address.User;
				Server = address.Host;
			}
		}

		[ContextProperty("Кодировка", "Encoding")]
		public string Encoding { get; set; }

		[ContextProperty("ОтображаемоеИмя", "DisplayName")]
		public string DisplayName { get; set; }

		[ContextProperty("Пользователь", "User")]
		public string User { get; set; }

		[ContextProperty("Сервер", "Server")]
		public string Server { get; set; }

		public MailboxAddress GetInternalObject()
		{
			return new MailboxAddress(System.Text.Encoding.GetEncoding(Encoding ?? "UTF-8"), DisplayName, Address);
		}
	}
}
