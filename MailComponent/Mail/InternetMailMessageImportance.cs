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
	/// Содержит варианты важности почтового сообщения.
	/// </summary>
	[EnumerationType("ВажностьИнтернетПочтовогоСообщения", "InternetMailMessageImportance")]
	public enum InternetMailMessageImportance
	{
		/// <summary>
		/// Высокая важность сообщения.
		/// </summary>
		[EnumItem("Высокая", "High")]
		High,

		/// <summary>
		/// Наивысшая важность сообщения.
		/// </summary>
		[EnumItem("Наивысшая", "Highest")]
		Highest,

		/// <summary>
		/// Наименьшая важность сообщения.
		/// </summary>
		[EnumItem("Наименьшая", "Lowest")]
		Lowest,

		/// <summary>
		/// Низкая важность сообщения.
		/// </summary>
		[EnumItem("Низкая", "Low")]
		Low,

		/// <summary>
		/// Обычная важность сообщения.
		/// </summary>
		[EnumItem("Обычная", "Normal")]
		Normal
	}
}
