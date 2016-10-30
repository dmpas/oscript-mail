/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using ScriptEngine.Machine.Contexts;
using ScriptEngine.Machine;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace OneScript.InternetMail
{
    [ContextClass("ИнтернетПочта", "InternetMail")]
	public class InternetMail : AutoContext<InternetMail>, IDisposable
    {
        private InternetMailProfile _profile;

		private SmtpClient smtpClient = new SmtpClient();

        public InternetMail()
        {
        }

        [ContextProperty("СимволРазделитель", "DelimiterChar")]
        public string DelimiterChar { get; set; }

        [ContextProperty("ТекущийПочтовыйЯщик", "CurrentMailbox")]
        public string CurrentMailbox { get; set; }

		private void LogonSmtp()
		{
			SecureSocketOptions options = SecureSocketOptions.Auto;
			smtpClient.Timeout = _profile.Timeout * 1000;
			smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
			smtpClient.Connect(_profile.SmtpServerAddress, _profile.GetSmtpPort(), options);

			if (_profile.SmtpUser != "")
				smtpClient.Authenticate(_profile.SmtpUser, _profile.SmtpPassword);
		}

        [ContextMethod("Подключиться", "Logon")]
        public void Logon(InternetMailProfile profile, InternetMailProtocol receiveMailProtocol = InternetMailProtocol.Pop3)
        {
            _profile = profile;

			if (_profile.SmtpServerAddress != "")
				LogonSmtp();
        }

        [ContextMethod("Послать", "Send")]
        public void Send(InternetMailMessage message,
                         InternetMailTextProcessing processText = InternetMailTextProcessing.Process,
                         InternetMailProtocol protocol = InternetMailProtocol.Smtp)
        {
            if (protocol == InternetMailProtocol.Pop3)
            {
                throw new RuntimeException("Недопустимо указывать POP3 в качестве протокола отправки почты!");
            }

			var messageToSend = message.CreateNativeMessage(processText);

            if (protocol == InternetMailProtocol.Smtp)
            {
				smtpClient.Send(messageToSend);
            }
        }

		public void Dispose()
		{
			((IDisposable)smtpClient).Dispose();
		}

        [ScriptConstructor]
        public static IRuntimeContextInstance Constructor()
        {
            return new InternetMail();
        }
	}
}
