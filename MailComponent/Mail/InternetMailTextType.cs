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
	/// Определяет набор допустимых типов текста почтового сообщения.
	/// </summary>
	[EnumerationType("ТипТекстаПочтовогоСообщения", "InternetMailTextType")]
	public enum InternetMailTextType
	{
		/// <summary>
		/// Текст почтового сообщения в формате HTML.
		/// При использовании текстов почтовых сообщений данного типа следует учитывать особенности их реализации на платформе Android.
		/// При оформлении тела письма не рекомендуется использовать:
		/// 
		/// - каскадные таблицы стилей,
		/// - встроенные таблицы,
		/// - встроенные изображения,
		/// - шрифты разных размеров.
		///
		/// Поведение может различаться при работе с разными клиентами и на различных версиях операционной системы.
		/// Рекомендуется проверять результат на устройствах конечных пользователей.
		/// </summary>
		[EnumItem("HTML", "ГиперТекст")]
		Html,

		/// <summary>
		/// Простой текст почтового сообщения. Отображается "как есть".
		/// </summary>
		[EnumItem("ПростойТекст", "PlainText")]
		PlainText,

		/// <summary>
		/// Текст почтового сообщения в формате Rich Text.
		/// </summary>
		[EnumItem("РазмеченныйТекст", "RichText")]
		RichText
	}
}
