﻿/*----------------------------------------------------------
This Source Code Form is subject to the terms of the
Mozilla Public License, v.2.0. If a copy of the MPL
was not distributed with this file, You can obtain one
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using System;
using System.IO;
using OneScript.StandardLibrary;
using OneScript.StandardLibrary.Collections;
using ScriptEngine;
using ScriptEngine.Machine;
using ScriptEngine.HostedScript;
using ScriptEngine.HostedScript.Extensions;
using ScriptEngine.Hosting;
using OneScript.InternetMail;
using OneScript.Execution;

namespace NUnitTests
{
    public class EngineHelpWrapper : IHostApplication
    {
        public EngineHelpWrapper() {
        }

        private ScriptingEngine Engine { get; set; }

        private IBslProcess Process { get; set; }

        private IValue TestRunner { get; set; }

        public void StartEngine() {
            var builder = DefaultEngineBuilder.Create();
            builder.SetupConfiguration(providers => { });
            builder.SetDefaultOptions()
                .UseImports()
                .UseNativeRuntime()
                .UseFileSystemLibraries()
                ;

            Engine = builder.Build();

            // Регистрируем сборку по имени любого из стандартных классов движка
            Engine.AttachAssembly(System.Reflection.Assembly.GetAssembly(typeof(ArrayImpl)));

            // Тут можно указать любой класс из компоненты
            Engine.AttachExternalAssembly(System.Reflection.Assembly.GetAssembly(typeof(InternetMail)));
            
            Process = Engine.NewProcess();

            var hosted = new HostedScriptEngine(Engine);
            hosted.Initialize();

            var cs = Engine.GetCompilerService();
            var testrunnerSource = LoadCodeFromAssemblyResource("NUnitTests.Tests.testrunner.os");
            var testRunner = Engine.AttachedScriptsFactory.LoadFromString(cs, testrunnerSource, Process);

            TestRunner = (IValue)testRunner;

        }

        public void RunTestScript(string resourceName) {
            var source = LoadCodeFromAssemblyResource(resourceName);
            var test = Engine.AttachedScriptsFactory.LoadFromString(Engine.GetCompilerService(), source, Process);

            ArrayImpl testArray;
            {
                var methodIndex = test.GetMethodNumber("ПолучитьСписокТестов");
                test.CallAsFunction(methodIndex, new IValue[] { TestRunner }, out var ivTests, Process);
                testArray = ivTests as ArrayImpl;
            }

            foreach (var ivTestName in testArray) {
                string testName = ivTestName.ExplicitString();
                var methodIndex = test.GetMethodNumber(testName);
                if (methodIndex == -1) {
                    // Тест указан, но процедуры нет или она не экспортирована
                    continue;
                }

                test.CallAsProcedure(methodIndex, new IValue[] { }, Process);
            }
        }

        private static string LoadCodeFromAssemblyResource(string resourceName) {
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            using var resourceStream = asm.GetManifestResourceStream(resourceName) ??
                                       throw new NullReferenceException(resourceName);
            using var reader = new StreamReader(resourceStream);
            var codeSource = reader.ReadToEnd();
            return codeSource;
        }

        public void Echo(string str, MessageStatusEnum status = MessageStatusEnum.Ordinary) {
            Console.WriteLine(str);
        }

        public bool InputString(out string result, string prompt, int maxLen, bool multiline) {
            throw new NotImplementedException();
        }

        public string[] GetCommandLineArguments() {
            return new string[] { };
        }

        public bool InputString(out string result, int maxLen) {
            result = "";
            return false;
        }

        public void ShowExceptionInfo(Exception exc) {
        }
    }
}
