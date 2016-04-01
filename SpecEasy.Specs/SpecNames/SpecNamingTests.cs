using System.IO;
using System.Linq;
using System.Reflection;
using Examples;
using NUnit.Core;
using NUnit.Core.Filters;
using NUnit.Framework;
using Should;

namespace SpecEasy.Specs.SpecNames
{
    [TestFixture]
    public class SpecNamingTests
    {
        [Test]
        public void RunSpecsAndCheckNames()
        {
            CoreExtensions.Host.InitializeService();
            var pathToTestLibrary = Assembly.GetExecutingAssembly().Location;
            var testPackage = new TestPackage(pathToTestLibrary)
            {
                BasePath = Path.GetDirectoryName(pathToTestLibrary)
            };
            var builder = new TestSuiteBuilder();
            var suite = builder.Build(testPackage);
            var filter = new SimpleNameFilter("SpecEasy.Specs.SpecNames.SpecNamingSpec");
            var result = suite.Run(new NullListener(), filter);

            var allTests = result.FindAllTests();
            Assert.IsTrue(allTests.All(t => t.Name == SpecNamingSpec.ExpectedSpecName));
        }
    }

    internal class SpecNamingSpec : Spec
    {
        public static string ExpectedSpecName = "given a context\r\n  and a sub context\r\n  and another sub context\r\n  but yet another sub context\r\nwhen running the test\r\nthen the test passes\r\n";

        public void RunSpec()
        {
            When("running the test", () => { });

            Given("a context", () => { }).Verify(() =>
            Given("a sub context", () => { }).Verify(() =>
            And("another sub context", () => { }).Verify(() =>
            But("yet another sub context", () => { }).Verify(() =>
                Then("the test passes", () => { })))));
        }
    }

}
