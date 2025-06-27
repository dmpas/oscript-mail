/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using OneScript.Contexts.Enums;

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
		[EnumValue("Высокая", "High")]
		High,

		/// <summary>
		/// Наивысшая важность сообщения.
		/// </summary>
		[EnumValue("Наивысшая", "Highest")]
		Highest,

		/// <summary>
		/// Наименьшая важность сообщения.
		/// </summary>
		[EnumValue("Наименьшая", "Lowest")]
		Lowest,

		/// <summary>
		/// Низкая важность сообщения.
		/// </summary>
		[EnumValue("Низкая", "Low")]
		Low,

		/// <summary>
		/// Обычная важность сообщения.
		/// </summary>
		[EnumValue("Обычная", "Normal")]
		Normal
	}
}
