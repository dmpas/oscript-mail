using System;
namespace OneScript.InternetMail
{
	public class NotSupportedInSelectedProtocol : Exception
	{
		public NotSupportedInSelectedProtocol()
		{
		}

		public NotSupportedInSelectedProtocol(string message)
			: base(message)
		{
		}
	}
}
