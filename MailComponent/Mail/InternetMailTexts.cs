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
	[ContextClass("ИнтернетТекстыПочтовогоСообщения", "InternetMailTexts")]
	public class InternetMailTexts : AutoContext<InternetMailTexts>, ICollectionContext, IEnumerable<InternetMailText>
	{
		private readonly List<InternetMailText> _data = new List<InternetMailText>();

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

		[ContextMethod("Очистить", "Clear")]
		public void Clear()
		{
			_data.Clear();
		}

		[ContextMethod("Получить", "Get")]
		public InternetMailText Get(int index)
		{
			return _data[index];
		}

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
