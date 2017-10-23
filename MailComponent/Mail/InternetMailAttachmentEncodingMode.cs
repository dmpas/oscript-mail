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
	/// Описывает способ кодирования вложений почтового сообщения.
	/// </summary>
	[EnumerationType("СпособКодированияИнтернетПочтовогоСообщения", "InternetMailAttachmentEncodingMode")]
	public enum InternetMailAttachmentEncodingMode
	{

		/// <summary>
		/// Кодирование вложений способом MIME.
		/// </summary>
		[EnumItem("MIME", "МИМЕ")]
		Mime,

		/// <summary>
		/// Кодирование вложений способом UUEncode.
		/// </summary>
		[EnumItem("UUEncode", "УУЕнкоде")]
		Uuencode

	}
}
