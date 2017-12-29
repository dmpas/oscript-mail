/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using ScriptEngine.Machine.Contexts;
using ScriptEngine.Machine;
using ScriptEngine.HostedScript.Library.Binary;

namespace OneScript.InternetMail
{
	/// <summary>
	/// Вложение в почтовое сообщение представляет собой двоичные данные.
	/// </summary>
	[ContextClass("ИнтернетПочтовоеВложение", "InternetMailAttachment")]
	public class InternetMailAttachment : AutoContext<InternetMailAttachment>
	{
        /// <summary>
        /// Пустое почтовое вложение
        /// </summary>
        public InternetMailAttachment()
		{
			EncodingMode = InternetMailAttachmentEncodingMode.Mime;
			Data = ValueFactory.Create();
		}

		/// <summary>
		/// Почтовое вложение на основании BinaryDataContext
		/// </summary>
		public InternetMailAttachment(BinaryDataContext binaryData, string fileName = "")
		{
			EncodingMode = InternetMailAttachmentEncodingMode.Mime;
			Data = binaryData;
			FileName = fileName;
		}

		/// <summary>
		/// Почтовое вложение на основании файла
		/// </summary>
		public InternetMailAttachment(string fileName)
		{
			EncodingMode = InternetMailAttachmentEncodingMode.Mime;
			Data = new BinaryDataContext(fileName);
			FileName = System.IO.Path.GetFileName(fileName);
		}

		/// <summary>
		/// Содержит данные почтового вложения.
		/// </summary>
		[ContextProperty("Данные", "Data")]
		public IValue Data { get; set; }

		/// <summary>
		/// Идентификатор вложения.
		/// </summary>
		[ContextProperty("Идентификатор", "CID")]
		public string CID { get; set; }

		/// <summary>
		/// Наименование вложения в сообщении. Используется в пользовательском интерфейсе.
		/// </summary>
		[ContextProperty("Имя", "Name")]
		public string Name { get; set; }

		/// <summary>
		/// Имя файла вложения.
		/// </summary>
		[ContextProperty("ИмяФайла", "FileName")]
		public string FileName { get; }

		/// <summary>
		/// Содержит кодировку для наименования вложения.
		/// Если кодировка не указана, будет использоваться значение кодировки из свойства Кодировка, объекта ИнтернетПочтовоеСообщение.
		/// </summary>
		[ContextProperty("Кодировка", "Encoding")]
		public string Encoding { get; set; }

		/// <summary>
		/// Содержит способ кодирования вложений сообщения. По умолчанию используется метод MIME.
		/// </summary>
		[ContextProperty("СпособКодирования", "EncodingMode")]
		public InternetMailAttachmentEncodingMode EncodingMode { get; set; }

		/// <summary>
		/// Содержит MIME тип вложения.
		/// Если свойство при отправке сообщения не заполнено, то производится попытка автоматически определить MIME тип вложения.
		/// Если автоматически определить тип не получилось, для такого вложения используется тип "application/octet-stream".
		/// </summary>
		[ContextProperty("ТипСодержимого", "MIMEType")]
		public string MimeType { get; set; }
	}
}
