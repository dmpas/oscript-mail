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

namespace OneScript.InternetMail
{
    [ContextClass("ИнтернетПочтовыйПрофиль", "InternetMailProfile")]
    public class InternetMailProfile : AutoContext<InternetMailProfile>
    {
        public InternetMailProfile()
        {
            Timeout = 30;
        }

        [ContextProperty("POP3ПередSMTP", "POP3BeforeSMTP")]
        public bool Pop3BeforeSmtp { get; set; }

        [ContextProperty("АдресСервераIMAP", "IMAPServerAddress")]
        public string ImapServerAddress { get; set; }

        [ContextProperty("АдресСервераPOP3", "POP3ServerAddress")]
        public string Pop3ServerAddress { get; set; }

        [ContextProperty("АдресСервераSMTP", "SmtpServerAddress")]
        public string SmtpServerAddress { get; set; }

        [ContextProperty("ИспользоватьSSLIMAP", "IMAPUseSSL")]
        public bool ImapUseSsl { get; set; }

        [ContextProperty("ИспользоватьSSLPOP3", "POP3UseSSL")]
        public bool Pop3UseSsl { get; set; }

        [ContextProperty("ИспользоватьSSLSMTP", "SMTPUseSSL")]
        public bool SmtpUseSsl { get; set; }

        [ContextProperty("Пароль", "Password")]
        public string Password { get; set; }

        [ContextProperty("ПарольIMAP", "IMAPPassword")]
        public string ImapPassword { get; set; }

        [ContextProperty("ПарольSMTP", "SMTPPassword")]
        public string SmtpPassword { get; set; }

        [ContextProperty("Пользователь", "User")]
        public string User { get; set; }

        [ContextProperty("ПользовательIMAP", "IMAPUser")]
        public string ImapUser { get; set; }

        [ContextProperty("ПользовательSMTP", "SMTPUser")]
        public string SmtpUser { get; set; }

        [ContextProperty("ПортIMAP", "IMAPPort")]
        public int ImapPort { get; set; }

        [ContextProperty("ПортPOP3", "POP3Port")]
        public int Pop3Port { get; set; }

        [ContextProperty("ПортSMTP", "SMTPPort")]
        public int SmtpPort { get; set; }

        [ContextProperty("Таймаут", "Timeout")]
        public int Timeout { get; set; }

        [ContextProperty("ТолькоЗащищеннаяАутентификацияIMAP", "IMAPSecureAuthenticationOnly")]
        public bool ImapSecureAuthenticationOnly { get; set; }

        [ContextProperty("ТолькоЗащищеннаяАутентификацияPOP3", "POP3SecureAuthenticationOnly")]
        public bool Pop3SecureAuthenticationOnly { get; set; }

        [ContextProperty("ТолькоЗащищеннаяАутентификацияSMTP", "SMTPSecureAuthenticationOnly")]
        public bool SmtpSecureAuthenticationOnly { get; set; }

        public int GetSmtpPort()
        {
            if (SmtpPort != 0)
                return SmtpPort;
            
            if (!SmtpSecureAuthenticationOnly && !SmtpUseSsl)
                return 25;
            
            return 465; // 587?
        }

		public int GetPop3Port()
		{
			if (Pop3Port != 0)
				return Pop3Port;

			if (Pop3UseSsl)
				return 995;

			return 110;
		}

        [ScriptConstructor]
        public static IRuntimeContextInstance Constructor()
        {
            return new InternetMailProfile();
        }
    }
}
