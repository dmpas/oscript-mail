/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace OneScript.InternetMail
{
	public class SmtpSender : IMailSender, IDisposable
	{

		private readonly SmtpClient _client = new SmtpClient();

		public void Logoff()
		{
			if (_client.IsConnected)
				_client.Disconnect(true);
		}

		public void Logon(InternetMailProfile profile)
		{
			SecureSocketOptions options = SecureSocketOptions.Auto;
			_client.Timeout = profile.Timeout * 1000;
			_client.ServerCertificateValidationCallback = (s, c, h, e) => true;
			_client.Connect(profile.SmtpServerAddress, profile.GetSmtpPort(), options);

			if (profile.SmtpUser != "")
				_client.Authenticate(profile.SmtpUser, profile.SmtpPassword);
		}

		public void Send(InternetMailMessage message, InternetMailTextProcessing processText)
		{
			var messageToSend = message.CreateNativeMessage(processText);
			_client.Send(messageToSend);
		}

		public void Dispose()
		{
			Logoff();
		}

	}
}
