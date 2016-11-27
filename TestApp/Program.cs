using System;
using System.IO;
using ScriptEngine.Machine;
using ScriptEngine.HostedScript;
using ScriptEngine.HostedScript.Library;
using System.Configuration;

namespace TestApp
{
	class MainClass : IHostApplication
	{

		public static HostedScriptEngine StartEngine()
		{
			var engine = new ScriptEngine.HostedScript.HostedScriptEngine();
			engine.Initialize();

			// Тут можно указать любой класс из компоненты
			engine.AttachAssembly(System.Reflection.Assembly.GetAssembly(typeof(OneScript.InternetMail.InternetMail)));

			return engine;
		}

		public static void InjectSettings(HostedScriptEngine engine)
		{
			string server = ConfigurationManager.AppSettings["server"];
			string userName = ConfigurationManager.AppSettings["userName"];
			string password = ConfigurationManager.AppSettings["password"];
			string replyTo = ConfigurationManager.AppSettings["replyTo"] ?? String.Format("{0}@{1}", userName, server);
			string pop3server = ConfigurationManager.AppSettings["pop3server"] ?? server;

			int portSmtp;
			bool useSsl;
			int timeout;

			if (!Int32.TryParse(ConfigurationManager.AppSettings["portSmtp"], out portSmtp))
				portSmtp = 25;

			if (!Boolean.TryParse(ConfigurationManager.AppSettings["useSsl"], out useSsl))
				useSsl = true;

			if (!Int32.TryParse(ConfigurationManager.AppSettings["timeout"], out timeout))
				timeout = 30;

			engine.InjectGlobalProperty("Сервер", ValueFactory.Create(server), true);
			engine.InjectGlobalProperty("СерверPOP3", ValueFactory.Create(pop3server), true);
			engine.InjectGlobalProperty("Пользователь", ValueFactory.Create(userName), true);
			engine.InjectGlobalProperty("Пароль", ValueFactory.Create(password) , true);
			engine.InjectGlobalProperty("ПортSMTP", ValueFactory.Create(portSmtp), true);
			engine.InjectGlobalProperty("Отправитель", ValueFactory.Create(replyTo), true);
			engine.InjectGlobalProperty("ИспользоватьSSLSMTP", ValueFactory.Create(useSsl), true);
			engine.InjectGlobalProperty("Таймаут", ValueFactory.Create(timeout), true);
		}

		public static string LoadFromAssemblyResource(string resourceName)
		{
			var asm = System.Reflection.Assembly.GetExecutingAssembly();
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
	}
}
