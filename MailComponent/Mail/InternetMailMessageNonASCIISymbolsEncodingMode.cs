/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using ScriptEngine;

namespace OneScript.InternetMail
{
	/// <summary>
	/// Содержит варианты способов кодирования не ASCII символов в почтовом сообщении.
	/// </summary>
	[EnumerationType("СпособКодированияНеASCIIСимволовИнтернетПочтовогоСообщения", "InternetMailMessageNonASCIISymbolsEncodingMode")]
	public enum InternetMailMessageNonAsciiSymbolsEncodingMode
	{
		/// <summary>
		/// Кодируются методом MIME.
		/// </summary>
		[EnumItem("MIME", "МИМЕ")]
		Mime,

		/// <summary>
		/// Кодируются методом Quoted-Printable.
		/// </summary>
		[EnumItem("QuotedPrintable", "Кодировать")]
		QuotedPrintable,

		/// <summary>
		/// Все поля заголовков, содержащие не ASCII символы кодироваться не будут.
		/// Все символы будут конвертироваться в кодировку, заданною свойством Кодировка.
		/// </summary>
		[EnumItem("БезКодирования", "None")]
		None
	}
}
