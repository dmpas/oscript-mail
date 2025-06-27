/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using System.IO;
using ScriptEngine.Machine;
using ScriptEngine.HostedScript;
using OneScript.StandardLibrary;
using ScriptEngine.Hosting;
using OneScript.InternetMail;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace TestApp
{
	class MainClass : IHostApplication
	{

		public static HostedScriptEngine StartEngine()
		{
            var mainEngine = DefaultEngineBuilder.Create()
                .SetDefaultOptions()
                .SetupEnvironment(envSetup => {
                    envSetup
						.AddStandardLibrary()
						.AddAssembly(typeof(InternetMail).Assembly);
                })
                .Build();
            var engine = new HostedScriptEngine(mainEngine);
            engine.Initialize();

            return engine;
        }

        public static void InjectSettings(HostedScriptEngine engine)
		{

			var cfg = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.Build();

			string server = cfg.GetSection("AppSettings:server").Value;
			string userName = cfg.GetSection("AppSettings:user").Value;
			string password = cfg.GetSection("AppSettings:password").Value;
			string replyTo = cfg.GetSection("AppSettings:replyTo").Value?? String.Format("{0}@{1}", userName, server);
			string pop3server = cfg.GetSection("AppSettings:pop3server").Value ?? server;
			string imapserver = cfg.GetSection("AppSettings:imapserver").Value ?? server;

			int portSmtp;
			bool useSsl;
			int timeout;

			if (!Int32.TryParse(cfg.GetSection("AppSettings:portSmtp").Value, out portSmtp))
				portSmtp = 25;

			if (!Boolean.TryParse(cfg.GetSection("AppSettings:useSsl").Value, out useSsl))
				useSsl = true;

			if (!Int32.TryParse(cfg.GetSection("AppSettings:timeout").Value, out timeout))
				timeout = 30;

			engine.InjectGlobalProperty("Сервер", "Server", ValueFactory.Create(server), true);
			engine.InjectGlobalProperty("СерверPOP3", "ServerPOP3", ValueFactory.Create(pop3server), true);
			engine.InjectGlobalProperty("СерверIMAP", "ServerIMAP", ValueFactory.Create(imapserver), true);
			engine.InjectGlobalProperty("Пользователь", "User", ValueFactory.Create(userName), true);
			engine.InjectGlobalProperty("Пароль", "Password", ValueFactory.Create(password), true);
			engine.InjectGlobalProperty("ПортSMTP", "SMTPPort", ValueFactory.Create(portSmtp), true);
			engine.InjectGlobalProperty("Отправитель", "Sender", ValueFactory.Create(replyTo), true);
			engine.InjectGlobalProperty("ИспользоватьSSLSMTP", "UseSslSmtp", ValueFactory.Create(useSsl), true);
			engine.InjectGlobalProperty("ИспользоватьSSLPOP3", "UseSslPop3", ValueFactory.Create(useSsl), true);
			engine.InjectGlobalProperty("ИспользоватьSSLIMAP", "UseSslImap", ValueFactory.Create(useSsl), true);
			engine.InjectGlobalProperty("Таймаут", "Timeout", ValueFactory.Create(timeout), true);
		}

		public static string LoadFromAssemblyResource(string resourceName)
		{
			var asm = Assembly.GetExecutingAssembly();
            string codeSource;

			using (Stream s = asm.GetManifestResourceStream(resourceName))
			{
				using (StreamReader r = new StreamReader(s))
				{
					codeSource = r.ReadToEnd();
				}
			}

			return codeSource;
		}


		public static void Main(string[] args)
		{
			var engine = StartEngine();
			InjectSettings(engine);

			var script = engine.Loader.FromString(LoadFromAssemblyResource("TestApp.TestSendReceive.os"));
			var process = engine.CreateProcess(new MainClass(), script);

			var result = process.Start(); // Запускаем наш тестовый скрипт

			Console.WriteLine("Result = {0}", result);

			// ВАЖНО: движок перехватывает исключения, для отладки можно пользоваться только точками останова.
		}

		public void Echo(string str, MessageStatusEnum status = MessageStatusEnum.Ordinary)
		{
			Console.WriteLine(str);
		}

		public void ShowExceptionInfo(Exception exc)
		{
			Console.WriteLine(exc.ToString());
		}

		public bool InputString(out string result, int maxLen)
		{
			throw new NotImplementedException();
		}

		public string[] GetCommandLineArguments()
		{
			return new string[] { "1", "2", "3" }; // Здесь можно зашить список аргументов командной строки
		}

        void IHostApplication.Echo(string str, MessageStatusEnum status) {
            Console.WriteLine(str);
        }

        bool IHostApplication.InputString(out string result, string prompt, int maxLen, bool multiline) {
            throw new NotImplementedException();
        }
    }
}
