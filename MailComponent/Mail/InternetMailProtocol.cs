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
	[EnumerationType("ПротоколИнтернетПочты", "InternetMailProtocol")]
	public enum InternetMailProtocol
	{

		[EnumItem("IMAP", "Имап")]
		Imap,

		[EnumItem("POP3", "ПОП3")]
		Pop3,

		[EnumItem("SMTP", "СМТП")]
		Smtp

	}
}
