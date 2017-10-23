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

namespace OneScript.InternetMail
{
	/// <summary>
	/// Представляет собой коллекцию объектов типа ИнтернетТекстПочтовогоСообщения.
	/// </summary>
	[ContextClass("ИнтернетТекстыПочтовогоСообщения", "InternetMailTexts")]
	public class InternetMailTexts : AutoContext<InternetMailTexts>, ICollectionContext, IEnumerable<InternetMailText>
	{
		private readonly List<InternetMailText> _data = new List<InternetMailText>();

		/// <summary>
		/// Получает количество элементов в коллекции почтовых текстов.
		/// </summary>
		/// <returns></returns>
		[ContextMethod("Количество", "Count")]
		public int Count()
		{
			return _data.Count;
		}

		public IEnumerator<InternetMailText> GetEnumerator()
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

		/// <summary>
		/// Добавляет текст в коллекцию почтовых текстов.
		/// </summary>
		/// <param name="text">Добавляемый текст.</param>
		/// <param name="type">Указывает тип добавляемого текста.
		/// Значение по умолчанию: ПростойТекст.</param>
		/// <returns></returns>
		[ContextMethod("Добавить", "Add")]
		public InternetMailText Add(string text, InternetMailTextType type = InternetMailTextType.PlainText)
		{
			var newText = new InternetMailText(text, type);
			_data.Add(newText);

			return newText;
		}

		public void Add(InternetMailText text)
		{
			_data.Add(text);
		}

		/// <summary>
		/// Удаляет все тексты коллекции.
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
		public InternetMailText Get(int index)
		{
			return _data[index];
		}

		/// <summary>
		/// Удаляет текст из коллекции текстов.
		/// </summary>
		/// <param name="element">Индекс элемента коллекции или элемент коллекции.</param>
		/// <exception cref="RuntimeException"></exception>
		[ContextMethod("Удалить", "Delete")]
		public void Delete(IValue element)
		{
			if (element.DataType == DataType.Number)
				_data.RemoveAt((int)element.AsNumber());

			else if (element is InternetMailText)
				_data.Remove(element as InternetMailText);

			throw RuntimeException.InvalidArgumentType(nameof(element));
		}
	}
}
