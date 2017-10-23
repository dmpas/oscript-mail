/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using ScriptEngine.Machine.Contexts;
using ScriptEngine.Machine;
using System.Collections.Generic;
using System.Collections;
using ScriptEngine.HostedScript.Library.Binary;

namespace OneScript.InternetMail
{
	/// <summary>
	/// Коллекция почтовых вложений.
	/// </summary>
	[ContextClass("ИнтернетПочтовыеВложения", "InternetMailAttachments")]
	public class InternetMailAttachments : AutoContext<InternetMailAttachments>, ICollectionContext, IEnumerable<InternetMailAttachment>
	{
		public InternetMailAttachments()
		{
		}

		private readonly List<InternetMailAttachment> _data = new List<InternetMailAttachment>();

		/// <summary>
		/// Получает размер коллекции почтовых вложений.
		/// </summary>
		/// <returns></returns>
		[ContextMethod("Количество", "Count")]
		public int Count()
		{
			return _data.Count;
		}

		public IEnumerator<InternetMailAttachment> GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		public CollectionEnumerator GetManagedIterator()
		{
			return new CollectionEnumerator(GetEnumerator());
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public InternetMailAttachment Add(string filePath, string attachmentName = "")
		{
			var attachment = new InternetMailAttachment(filePath);
			attachment.Name = attachmentName;
			_data.Add(attachment);
			return attachment;
		}

		public InternetMailAttachment Add(BinaryDataContext data, string attachmentName = "")
		{
			var attachment = new InternetMailAttachment();
			attachment.Data = data;
			attachment.Name = attachmentName;
			_data.Add(attachment);
			return attachment;
		}

		public InternetMailAttachment Add(InternetMailMessage data, string attachmentName = "")
		{
			var attachment = new InternetMailAttachment();
			attachment.Data = data;
			attachment.Name = attachmentName;
			_data.Add(attachment);
			return attachment;
		}


		/// <summary>
		/// Добавляет вложение
		/// </summary>
		/// <param name="source">Адрес файла-вложения.</param>
		/// <param name="attachmentName">Имя вложения, которое отображают почтовые клиенты.</param>
		/// <returns></returns>
		/// <exception cref="RuntimeException"></exception>
		[ContextMethod("Добавить", "Add")]
		public InternetMailAttachment Add(IValue source, string attachmentName = "")
		{
			if (source.DataType == DataType.String)
				return Add(source.AsString(), attachmentName);
			else if (source is BinaryDataContext)
				return Add(source as BinaryDataContext, attachmentName);
			else if (source is InternetMailMessage)
				return Add(source as InternetMailMessage, attachmentName);

			throw RuntimeException.InvalidArgumentType(nameof(source));
		}

		/// <summary>
		/// Удаляет все элементы коллекции.
		/// </summary>
		[ContextMethod("Очистить", "Clear")]
		public void Clear()
		{
			_data.Clear();
		}

		/// <summary>
		/// Получает значение по индексу. Работает аналогично оператору [].
		/// </summary>
		/// <param name="index">Индекс элемента коллекции.</param>
		/// <returns></returns>
		[ContextMethod("Получить", "Get")]
		public InternetMailAttachment Get(int index)
		{
			return _data[index];
		}

		/// <summary>
		/// Удаляет почтовое вложение из коллекции.
		/// </summary>
		/// <param name="element">Индекс элемента коллекции или элемент коллекции.</param>
		/// <exception cref="RuntimeException"></exception>
		[ContextMethod("Удалить", "Delete")]
		public void Delete(IValue element)
		{
			if (element.DataType == DataType.Number)
				_data.RemoveAt((int)element.AsNumber());

			else if (element is InternetMailAttachment)
				_data.Remove(element as InternetMailAttachment);

			throw RuntimeException.InvalidArgumentType(nameof(element));
		}
	}
}
