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
	[ContextClass("ИнтернетПочтовыеАдреса", "InternetMailAddresses")]
	public class InternetMailAddresses : AutoContext<InternetMailAddresses>, ICollectionContext, IEnumerable<InternetMailAddress>
	{

		private readonly List<InternetMailAddress> _data = new List<InternetMailAddress>();

		public IEnumerator<InternetMailAddress> GetEnumerator()
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
		public InternetMailAddress Add(string address)
		{
			var newAddress = new InternetMailAddress();
			newAddress.Address = address;
			_data.Add(newAddress);

			return newAddress;
		}

		[ContextMethod("Количество", "Count")]
		public int Count()
		{
			return _data.Count;
		}

		[ContextMethod("Очистить", "Clear")]
		public void Clear()
		{
			_data.Clear();
		}

		[ContextMethod("Получить", "Get")]
		public InternetMailAddress Get(int index)
		{
			return _data[index];
		}

		[ContextMethod("Удалить", "Delete")]
		public void Delete(IValue element)
		{
			if (element.DataType == DataType.Number)
				_data.RemoveAt((int)element.AsNumber());

			else if (element is InternetMailAddress)
				_data.Remove(element as InternetMailAddress);

			throw RuntimeException.InvalidArgumentType(nameof(element));
		}
	}
}
