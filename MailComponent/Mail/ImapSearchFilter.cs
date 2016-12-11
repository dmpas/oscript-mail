/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using ScriptEngine.Machine.Contexts;
using ScriptEngine.HostedScript.Library;
using MailKit.Search;

namespace OneScript.InternetMail
{
	// Класс-обёртка для упрощения обработки структуры отбора

	[ContextClass("ИнтернетПочтаОтборImap", "InternetMailImapSearchFilter")]
	public class InternetMailImapSearchFilter : AutoContext<InternetMailImapSearchFilter>
	{

		public InternetMailImapSearchFilter(StructureImpl filter)
		{
			foreach (var KV in filter)
			{
				var myPropertyIndex = this.FindProperty((KV as KeyAndValueImpl).Key.AsString());
				if (myPropertyIndex != -1)
				{
					SetPropValue(myPropertyIndex, KV.Value);
				}
			}
		}

		[ContextProperty("ОтправленОтвет", "Answered")]
		public bool? Answered { get; set; }

		[ContextProperty("Недавние", "Recent")]
		public bool? Recent { get; set; }

		[ContextProperty("СлепыеКопии", "Bcc")]
		public string Bcc { get; set; }

		[ContextProperty("Копии", "Cc")]
		public string Cc { get; set; }

		[ContextProperty("Получатели", "To")]
		public string To { get; set; }

		[ContextProperty("ДатаОтправления", "Date")]
		public DateTime? PostDating { get; set; }

		[ContextProperty("Отправитель", "From")]
		public string From { get; set; }

		[ContextProperty("ДоДатыОтправления", "BeforeDateOfPosting")]
		public DateTime? BeforeDateOfPosting { get; set; }

		[ContextProperty("ПослеДатыОтправления", "AfterDateOfPosting")]
		public DateTime? AfterDateOfPosting { get; set; }

		[ContextProperty("Тема", "Subject")]
		public string Subject { get; set; }

		[ContextProperty("Текст", "Text")]
		public string Text { get; set; }

		[ContextProperty("Тело", "Body")]
		public string Body { get; set; }

		[ContextProperty("Удаленные", "Deleted")]
		public bool? Deleted { get; set; }

		[ContextProperty("УстановленФлаг", "Flagged")]
		public bool? Flagged { get; set; }

		[ContextProperty("Прочитанные", "Seen")]
		public bool? Seen { get; set; }

		[ContextProperty("Новые", "New")]
		public bool? New { get; set; }

		private static SearchQuery ApplyBool(SearchQuery query, bool? value, SearchQuery ifTrue, SearchQuery ifFalse)
		{
			if (value == true)
				return query.And(ifTrue);

			if (value == false)
				return query.And(ifFalse);

			return query;
		}

		public SearchQuery CreateSearchQuery()
		{
			var query = new SearchQuery();

			query = ApplyBool(query, Answered, SearchQuery.Answered, SearchQuery.NotAnswered);
			query = ApplyBool(query, Recent, SearchQuery.Recent, SearchQuery.NotRecent);
			query = ApplyBool(query, Deleted, SearchQuery.Deleted, SearchQuery.NotDeleted);
			query = ApplyBool(query, Flagged, SearchQuery.Flagged, SearchQuery.NotFlagged);
			query = ApplyBool(query, Seen, SearchQuery.Seen, SearchQuery.NotSeen);
			query = ApplyBool(query, New, SearchQuery.New, SearchQuery.Not(SearchQuery.New)); // TODO: Not(New) ??

			if (Bcc != null)
				query = query.And(SearchQuery.BccContains(Bcc));

			if (Cc != null)
				query = query.And(SearchQuery.CcContains(Cc));

			if (To != null)
				query = query.And(SearchQuery.ToContains(To));

			if (From != null)
				query = query.And(SearchQuery.ToContains(From));

			if (Subject != null)
				query = query.And(SearchQuery.ToContains(Subject));

			if (Text != null)
				query = query.And(SearchQuery.ToContains(Text));

			if (Body != null)
				query = query.And(SearchQuery.ToContains(Body));

			if (PostDating != null)
				query = query.And(SearchQuery.SentOn((DateTime)PostDating));

			if (BeforeDateOfPosting != null)
				query = query.And(SearchQuery.SentBefore((DateTime)BeforeDateOfPosting));

			if (AfterDateOfPosting != null)
				query = query.And(SearchQuery.SentAfter((DateTime)AfterDateOfPosting));

			return query;
		}

	}
}
