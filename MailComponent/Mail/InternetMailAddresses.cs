/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using ScriptEngine.Machine.Contexts;
using ScriptEngine.Machine;
using OneScript.Contexts;
using OneScript.Exceptions;
using System.Collections.Generic;
using System.Collections;
using OneScript.Execution;
using OneScript.Values;

namespace OneScript.InternetMail
{
	/// <summary>
	/// Представляет собой коллекцию объектов типа ИнтернетПочтовыйАдрес.
	/// </summary>
	[ContextClass("ИнтернетПочтовыеАдреса", "InternetMailAddresses")]
	public class InternetMailAddresses : AutoContext<InternetMailAddresses>, ICollectionContext<InternetMailAddress>, IEnumerable<InternetMailAddress>
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

		/// <summary>
		/// Добавляет адрес в коллекцию.
		/// </summary>
		/// <param name="address">Почтовый адрес.</param>
		/// <returns></returns>
		[ContextMethod("Добавить", "Add")]
		public InternetMailAddress Add(string address)
		{
			var newAddress = new InternetMailAddress();
			newAddress.Address = address;
			_data.Add(newAddress);

			return newAddress;
		}

		/// <summary>
		/// Получает количество элементов в коллекции почтовых адресов.
		/// </summary>
		/// <returns></returns>
		[ContextMethod("Количество", "Count")]
		public int Count()
		{
			return _data.Count;
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
		public InternetMailAddress Get(int index)
		{
			return _data[index];
		}

		/// <summary>
		/// Удаляет элемент из коллекции.
		/// </summary>
		/// <param name="element">Индекс элемента коллекции или элемент коллекции.</param>
		/// <exception cref="RuntimeException"></exception>
		[ContextMethod("Удалить", "Delete")]
		public void Delete(IValue element)
		{
			if (element is BslNumericValue)
				_data.RemoveAt((int)element.AsNumber());

			else if (element is InternetMailAddress)
				_data.Remove(element as InternetMailAddress);

			throw RuntimeException.InvalidArgumentType(nameof(element));
		}

        int ICollectionContext<InternetMailAddress>.Count(IBslProcess process) {
			return Count();
        }
    }
}
