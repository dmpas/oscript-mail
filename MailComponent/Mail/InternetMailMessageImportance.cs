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
	[EnumerationType("ВажностьИнтернетПочтовогоСообщения", "InternetMailMessageImportance")]
	public enum InternetMailMessageImportance
	{
		[EnumItem("Высокая", "High")]
		High,

		[EnumItem("Наивысшая", "Highest")]
		Highest,

		[EnumItem("Наименьшая", "Lowest")]
		Lowest,

		[EnumItem("Низкая", "Low")]
		Low,

		[EnumItem("Обычная", "Normal")]
		Normal
	}
}
