﻿/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using System.IO;
using ScriptEngine.Machine.Contexts;
using ScriptEngine.HostedScript.Library;
using ScriptEngine.Machine;
using ScriptEngine.Environment;
using ScriptEngine.HostedScript;

namespace NUnitTests
{
	public class EngineHelpWrapper : IHostApplication
	{

		private HostedScriptEngine engine;

		public EngineHelpWrapper()
		{
		}

		public HostedScriptEngine Engine
		{
			get
			{
				return engine;
			}
		}

		public IValue TestRunner { get; private set; }

		public HostedScriptEngine StartEngine()
		{
			engine = new HostedScriptEngine();
			engine.Initialize();

			// Тут можно указать любой класс из компоненты
			engine.AttachAssembly(System.Reflection.Assembly.GetAssembly(typeof(OneScript.InternetMail.InternetMail)));

			// Подключаем тестовую оболочку
			engine.AttachAssembly(System.Reflection.Assembly.GetAssembly(typeof(EngineHelpWrapper)));

			var testrunnerSource = LoadFromAssemblyResource("NUnitTests.Tests.testrunner.os");
			var testrunnerModule = engine.GetCompilerService().CreateModule(testrunnerSource);

			engine.LoadUserScript(new ScriptEngine.UserAddedScript()
			{
				Type = ScriptEngine.UserAddedScriptType.Class,
				Module = testrunnerModule,
				Symbol = "TestRunner"
			});

			var testRunner = AttachedScriptsFactory.ScriptFactory("TestRunner", new IValue[] { });
			TestRunner = ValueFactory.Create(testRunner);

			return engine;
		}

		public void RunTestScript(string resourceName)
		{
			var source = LoadFromAssemblyResource(resourceName);
			var module = engine.GetCompilerService().CreateModule(source);

			engine.LoadUserScript(new ScriptEngine.UserAddedScript()
			{
				Type = ScriptEngine.UserAddedScriptType.Class,
				Module = module,
				Symbol = resourceName
			});

			var test = AttachedScriptsFactory.ScriptFactory(resourceName, new IValue[] { });
			ArrayImpl testArray;
			{
				int methodIndex = test.FindMethod("ПолучитьСписокТестов");

				{
					IValue ivTests;
					test.CallAsFunction(methodIndex, new IValue[] { TestRunner }, out ivTests);
					testArray = ivTests as ArrayImpl;
				}
			}

			foreach (var ivTestName in testArray)
			{
				string testName = ivTestName.AsString();
				int methodIndex = test.FindMethod(testName);
				if (methodIndex == -1)
				{
					// Тест указан, но процедуры нет или она не экспортирована
					continue;
				}

				test.CallAsProcedure(methodIndex, new IValue[] { });
			}
		}

		public ICodeSource LoadFromAssemblyResource(string resourceName)
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

			return engine.Loader.FromString(codeSource);
		}

		public void Echo(string str, MessageStatusEnum status = MessageStatusEnum.Ordinary)
		{
		}

		public string[] GetCommandLineArguments()
		{
			return new string[] { };
		}

		public bool InputString(out string result, int maxLen)
		{
			result = "";
			return false;
		}

		public void ShowExceptionInfo(Exception exc)
		{
		}
	}
}
