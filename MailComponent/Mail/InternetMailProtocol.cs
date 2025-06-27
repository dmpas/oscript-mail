/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using OneScript.Contexts.Enums;

namespace OneScript.InternetMail
{
	/// <summary>
	/// Определяет набор допустимых типов протоколов, используемых Интернет почтой.
	/// </summary>
	[EnumerationType("ПротоколИнтернетПочты", "InternetMailProtocol")]
	public enum InternetMailProtocol
	{

		/// <summary>
		/// Соответствует IMAP протоколу.
		/// </summary>
		[EnumValue("IMAP", "Имап")]
		Imap,

		/// <summary>
		/// Соответствует POP3 протоколу.
		/// </summary>
		[EnumValue("POP3", "ПОП3")]
		Pop3,

		/// <summary>
		/// Соответствует SMTP протоколу.
		/// </summary>
		[EnumValue("SMTP", "СМТП")]
		Smtp

	}
}
