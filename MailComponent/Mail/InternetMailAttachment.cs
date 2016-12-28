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
	[ContextClass("ИнтернетПочтовоеВложение", "InternetMailAttachment")]
	public class InternetMailAttachment : AutoContext<InternetMailAttachment>
	{
		public InternetMailAttachment()
		{
			EncodingMode = InternetMailAttachmentEncodingMode.Mime;
			Data = ValueFactory.Create();
		}

		public InternetMailAttachment(string fileName)
		{
			EncodingMode = InternetMailAttachmentEncodingMode.Mime;
			Data = new BinaryDataContext(fileName);
			FileName = System.IO.Path.GetFileName(fileName);
		}

		[ContextProperty("Данные", "Data")]
		public IValue Data { get; set; }

		[ContextProperty("Идентификатор", "CID")]
		public string CID { get; set; }

		[ContextProperty("Имя", "Name")]
		public string Name { get; set; }

		[ContextProperty("ИмяФайла", "FileName")]
		public string FileName { get; }

		[ContextProperty("Кодировка", "Encoding")]
		public string Encoding { get; set; }

		[ContextProperty("СпособКодирования", "EncodingMode")]
		public InternetMailAttachmentEncodingMode EncodingMode { get; set; }

		[ContextProperty("ТипСодержимого", "MIMEType")]
		public string MimeType { get; set; }
	}
}
