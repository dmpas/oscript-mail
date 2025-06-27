/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using ScriptEngine.Machine.Contexts;
using OneScript.Contexts;

namespace OneScript.InternetMail
{
	/// <summary>
	/// Набор свойств для соединения с сервером.
	/// </summary>
	[ContextClass("ИнтернетПочтовыйПрофиль", "InternetMailProfile")]
	public class InternetMailProfile : AutoContext<InternetMailProfile>
	{
		public InternetMailProfile()
		{
			Timeout = 30;
		}

		/// <summary>
		/// Способ аутентификации, когда подключение к SMTP серверу происходит после успешного подключения к POP3 серверу.
		/// Значение по умолчанию - Ложь.
		/// </summary>
		[ContextProperty("POP3ПередSMTP", "POP3BeforeSMTP")]
		public bool Pop3BeforeSmtp { get; set; }

		/// <summary>
		/// Содержит хост имя IMAP сервера.
		/// </summary>
		[ContextProperty("АдресСервераIMAP", "IMAPServerAddress")]
		public string ImapServerAddress { get; set; }

		/// <summary>
		/// Адрес POP3 сервера.
		/// </summary>
		[ContextProperty("АдресСервераPOP3", "POP3ServerAddress")]
		public string Pop3ServerAddress { get; set; }

		/// <summary>
		/// Адрес SMTP сервера.
		/// </summary>
		[ContextProperty("АдресСервераSMTP", "SmtpServerAddress")]
		public string SmtpServerAddress { get; set; }

		/// <summary>
		/// Указывает необходимость использования SSL соединения для протокола IMAP.
		/// </summary>
		[ContextProperty("ИспользоватьSSLIMAP", "IMAPUseSSL")]
		public bool ImapUseSsl { get; set; }

		/// <summary>
		/// Использовать SSL-соединение для протокола POP3.
		/// </summary>
		[ContextProperty("ИспользоватьSSLPOP3", "POP3UseSSL")]
		public bool Pop3UseSsl { get; set; }

		/// <summary>
		/// Использовать SSL соединение для протокола SMTP.
		/// </summary>
		[ContextProperty("ИспользоватьSSLSMTP", "SMTPUseSSL")]
		public bool SmtpUseSsl { get; set; }

		/// <summary>
		/// Пароль доступа к почтовому ящику. Поддерживаются только символы, входящие в кодовую таблицу US-ASCII.
		/// </summary>
		[ContextProperty("Пароль", "Password")]
		public string Password { get; set; }

		/// <summary>
		/// Содержит пароль пользователя IMAP-сервера. Поддерживаются только символы, входящие в кодовую таблицу US-ASCII.
		/// </summary>
		[ContextProperty("ПарольIMAP", "IMAPPassword")]
		public string ImapPassword { get; set; }

		/// <summary>
		/// Содержит пароль пользователя для аутентификации на SMTP сервере. Поддерживаются только символы, входящие в кодовую таблицу US-ASCII.
		/// </summary>
		[ContextProperty("ПарольSMTP", "SMTPPassword")]
		public string SmtpPassword { get; set; }

		/// <summary>
		/// Логин пользователя – часть «пользователь» почтового адреса. Поддерживаются только символы, входящие в кодовую таблицу US-ASCII.
		/// </summary>
		[ContextProperty("Пользователь", "User")]
		public string User { get; set; }

		/// <summary>
		/// Содержит имя пользователя IMAP сервера. Поддерживаются только символы, входящие в кодовую таблицу US-ASCII.
		/// </summary>
		[ContextProperty("ПользовательIMAP", "IMAPUser")]
		public string ImapUser { get; set; }

		/// <summary>
		/// Содержит логин пользователя для аутентификации на SMTP-сервере. Поддерживаются только символы, входящие в кодовую таблицу US-ASCII.
		/// </summary>
		[ContextProperty("ПользовательSMTP", "SMTPUser")]
		public string SmtpUser { get; set; }

		/// <summary>
		/// Содержит порт IMAP-сервера.
		/// 0 - использовать 993 порт для SSL-соединений и 143 - для незащищенных.
		/// Значение по умолчанию: 0.
		/// </summary>
		[ContextProperty("ПортIMAP", "IMAPPort")]
		public int ImapPort { get; set; }

		/// <summary>
		/// Порт протокола POP3. 
		/// 0 - означает использовать 995 порт для SSL соединений и 110 - для незащищенных.
		/// Значение по умолчанию: 0.
		/// </summary>
		[ContextProperty("ПортPOP3", "POP3Port")]
		public int Pop3Port { get; set; }

		/// <summary>
		/// Порт протокола SMTP. 
		/// 0 - означает использовать 465 порт для SSL соединений и 25 - для незащищенных.
		/// Значение по умолчанию: 0.
		/// </summary>
		[ContextProperty("ПортSMTP", "SMTPPort")]
		public int SmtpPort { get; set; }

		/// <summary>
		/// Определяет время ожидания удачного исполнения операции в секундах.
		/// Значение по умолчанию: 30 секунд.
		/// </summary>
		[ContextProperty("Таймаут", "Timeout")]
		public int Timeout { get; set; }

		/// <summary>
		/// Указывает на использование для протокола IMAP только защищенных способов аутентификации из поддерживаемых сервером (на данный момент CRAM-MD5).
		/// Ложь - разрешить использовать незащищенные способы аутентификации, если сервер поддерживает только их.
		/// </summary>
		[ContextProperty("ТолькоЗащищеннаяАутентификацияIMAP", "IMAPSecureAuthenticationOnly")]
		public bool ImapSecureAuthenticationOnly { get; set; }

		/// <summary>
		/// Указывает на использование для протокола POP3 только защищенных способов аутентификации из поддерживаемых сервером (на данный момент CRAM-MD5).
		/// Ложь - разрешить использовать незащищенные способы аутентификации, если сервер поддерживает только их.
		/// </summary>
		[ContextProperty("ТолькоЗащищеннаяАутентификацияPOP3", "POP3SecureAuthenticationOnly")]
		public bool Pop3SecureAuthenticationOnly { get; set; }

		/// <summary>
		/// Указывает на использование для протокола SMTP только защищенных способов аутентификации из поддерживаемых сервером (на данный момент CRAM-MD5).
		/// Ложь - разрешить использовать незащищенные способы аутентификации, если сервер поддерживает только их.
		/// </summary>
		[ContextProperty("ТолькоЗащищеннаяАутентификацияSMTP", "SMTPSecureAuthenticationOnly")]
		public bool SmtpSecureAuthenticationOnly { get; set; }

		public int GetSmtpPort()
		{
			if (SmtpPort != 0)
				return SmtpPort;

			if (!SmtpSecureAuthenticationOnly && !SmtpUseSsl)
				return 25;

			return 465; // 587?
		}

		public int GetPop3Port()
		{
			if (Pop3Port != 0)
				return Pop3Port;

			if (Pop3UseSsl)
				return 995;

			return 110;
		}

		public int GetImapPort()
		{
			if (ImapPort != 0)
				return ImapPort;

			if (ImapUseSsl)
				return 993;

			return 143;
		}

		[ScriptConstructor]
		public static IRuntimeContextInstance Constructor()
		{
			return new InternetMailProfile();
		}
	}
}
