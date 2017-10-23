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
	/// Определяет набор действий над текстами почтового сообщения перед отправкой.
	/// </summary>
	[EnumerationType("ОбработкаТекстаИнтернетПочтовогоСообщения", "InternetMailTextProcessing")]
	public enum InternetMailTextProcessing
	{
		/// <summary>
		/// Обработка текстов почтового сообщения перед отправкой не требуется. Текст посылается без внутренней обработки.
		/// Также отключает загрузку картинок при отправке HTML-документов.
		/// </summary>
		[EnumItem("НеОбрабатывать", "DontProcess")]
		DontProcess,

		/// <summary>
		/// Тексты и картинки отправляемых HTML-документов загружаются и добавляются к письму как вложения, с соответствующей правкой этих HTML-документов.
		/// </summary>
		[EnumItem("Обрабатывать", "Process")]
		Process
	}
}
