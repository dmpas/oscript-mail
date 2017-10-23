/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using ScriptEngine.Machine.Contexts;
using ScriptEngine.HostedScript.Library.Binary;
using MimeKit;

namespace OneScript.InternetMail
{
	/// <summary>
	/// Текстовые данные письма.
	/// </summary>
	[ContextClass("ИнтернетТекстПочтовогоСообщения", "InternetMailText")]
	public class InternetMailText : AutoContext<InternetMailText>
	{
		public InternetMailText()
		{
		}

		public InternetMailText(TextPart nativeTextPart)
		{
			if (nativeTextPart.IsPlain)
				TextType = InternetMailTextType.PlainText;
			else if (nativeTextPart.IsHtml)
				TextType = InternetMailTextType.Html;
			else if (nativeTextPart.IsRichText)
				TextType = InternetMailTextType.RichText;

			Text = nativeTextPart.Text;
		}

		public InternetMailText(string text, InternetMailTextType type)
		{
			Text = text;
			TextType = type;
		}

		/// <summary>
		/// Содержит текстовые данные сообщения.
		/// Данные были только раскодированы, но работы над кодировкой текстов не производилось.
		/// </summary>
		[ContextProperty("Данные", "Data")]
		public BinaryDataContext Data
		{
			get
			{
				var encoding = GetEncoding();
				return new BinaryDataContext(encoding.GetBytes(Text));
			}
		}

		/// <summary>
		/// Содержит кодировку текстовых данных сообщения.
		/// Если кодировка не указана, будет использоваться значение кодировки из свойства Кодировка.
		/// </summary>
		[ContextProperty("Кодировка", "Encoding")]
		public string Encoding { get; set; }

		/// <summary>
		/// Текстовые данные сообщения.
		/// </summary>
		[ContextProperty("Текст", "Text")]
		public string Text { get; set; }

		/// <summary>
		/// Тип текстовых данных письма.
		/// </summary>
		[ContextProperty("ТипТекста", "TextType")]
		public InternetMailTextType TextType { get; set; }


		private System.Text.Encoding GetEncoding()
		{
			if (Encoding.Length == 0)
				return new System.Text.UTF8Encoding(false);

			return System.Text.Encoding.GetEncoding(Encoding);
		}

		private MimeKit.Text.TextFormat GetMimeTextFormat()
		{
			if (TextType == InternetMailTextType.Html)
				return MimeKit.Text.TextFormat.Html;

			if (TextType == InternetMailTextType.RichText)
				return MimeKit.Text.TextFormat.RichText;

			return MimeKit.Text.TextFormat.Plain;
		}

		public TextPart CreateTextPart()
		{
			return new TextPart(GetMimeTextFormat()) { Text = Text };
		}

		public override string ToString()
		{
			return Text;
		}
	}
}
