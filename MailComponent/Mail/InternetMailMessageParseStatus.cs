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
	/// Содержит виды статусов разбора почтового сообщения:
	/// - ОбнаруженыОшибки - ошибки в структуре письма при разборе были обнаружены ошибки. Объект ИнтернетПочтовоеСообщение может содержать ошибки.
	/// - ОшибокНеОбнаружено - при разборе письма ошибки обнаружены не были.
	/// </summary>
	[EnumerationType("СтатусРазбораПочтовогоСообщения", "InternetMailMessageParseStatus")]
	public enum InternetMailMessageParseStatus
	{
		/// <summary>
		/// При разборе почтового сообщения обнаружены ошибки. Некоторые поля сообщения могут быть не заполнены.
		/// </summary>
		[EnumValue("ОбнаруженыОшибки", "ErrorsDetected")]
		ErrorsDetected,

		/// <summary>
		/// При разборе почтового соощения ошибок обнаружено не было.
		/// </summary>
		[EnumValue("ОшибокНеОбнаружено", "ErrorsNotDetected")]
		ErrorsNotDetected
	}
}
