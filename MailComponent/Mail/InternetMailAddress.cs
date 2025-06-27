/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using ScriptEngine.Machine.Contexts;
using OneScript.Contexts;
using System.Net.Mail;
using MimeKit;

namespace OneScript.InternetMail
{
	/// <summary>
	/// Адрес для обмена почтовыми сообщениями.
	/// </summary>
	[ContextClass("ИнтернетПочтовыйАдрес", "InternetMailAddress")]
	public class InternetMailAddress : AutoContext<InternetMailAddress>
	{
		/// <summary>
		/// Полный адрес электронной почты в формате "пользователь@сервер".
		/// </summary>
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

		/// <summary>
		/// Содержит кодировку для отображаемых имен.
		/// Если кодировка не указана, будет использоваться значение кодировки из свойства Кодировка, объекта ИнтернетПочтовоеСообщение.
		/// </summary>
		[ContextProperty("Кодировка", "Encoding")]
		public string Encoding { get; set; }

		/// <summary>
		/// Содержит представление почтового адреса.
		/// Произвольный текст, сопоставляемый почтовому адресу, указанному в свойстве Адрес.
		/// Используется почтовыми клиентами при разборе сообщения в качестве отображаемого имени получателя, отправителя и т.д.
		/// </summary>
		[ContextProperty("ОтображаемоеИмя", "DisplayName")]
		public string DisplayName { get; set; }

		/// <summary>
		/// Часть "пользователь" почтового адреса.
		/// </summary>
		[ContextProperty("Пользователь", "User")]
		public string User { get; set; }

		/// <summary>
		/// Часть "сервер" почтового адреса.
		/// </summary>
		[ContextProperty("Сервер", "Server")]
		public string Server { get; set; }

		public MailboxAddress GetInternalObject()
		{
			return new MailboxAddress(System.Text.Encoding.GetEncoding(Encoding ?? "UTF-8"), DisplayName, Address);
		}
	}
}
