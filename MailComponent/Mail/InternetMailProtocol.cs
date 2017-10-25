/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using ScriptEngine.Machine.Contexts;
using ScriptEngine;

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
		[EnumItem("IMAP", "Имап")]
		Imap,

		/// <summary>
		/// Соответствует POP3 протоколу.
		/// </summary>
		[EnumItem("POP3", "ПОП3")]
		Pop3,

		/// <summary>
		/// Соответствует SMTP протоколу.
		/// </summary>
		[EnumItem("SMTP", "СМТП")]
		Smtp

	}
}
