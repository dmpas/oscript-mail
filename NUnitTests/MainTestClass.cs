/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/
using NUnit.Framework;
using OneScript.InternetMail;

// Используется NUnit 3.6

namespace NUnitTests
{
	[TestFixture]
	public class MainTestClass
	{

		private EngineHelpWrapper host;

		[OneTimeSetUp]
		public void Initialize()
		{
			host = new EngineHelpWrapper();
			host.StartEngine();
		}

		[Test]
		public void Test_CreateProfile()
		{
			var profile = new InternetMailProfile();
			profile.User = "User";
			profile.Password = "Password";
		}

		[Test]
		public void TestAsExternalObjects()
		{
			host.RunTestScript("NUnitTests.Tests.external.os");
		}
	}
}
