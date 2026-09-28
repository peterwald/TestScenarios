namespace NUnit.MTP
{
    [Serializable]
    public class SerializableTestData
    {
        public string? Name { get; set; }
        public int Value { get; set; }
    }

    [TestFixture]
    public class NUnitMTP
    {
        [Test]
        public void SimpleTest()
        {
        }

        [Test]
        public void StreamsTestHostOutput()
        {
            Stream standardOutput = Console.OpenStandardOutput();
            Stream standardError = Console.OpenStandardError();

            WriteTestHostLine(standardOutput, "[NUnit] stdout 1/3: test started");
            Thread.Sleep(TimeSpan.FromSeconds(1));
            WriteTestHostLine(standardError, "[NUnit] stderr 2/3: test is still running");
            Thread.Sleep(TimeSpan.FromSeconds(1));
            WriteTestHostLine(standardOutput, "[NUnit] stdout 3/3: test is completing");
        }

        [Test]
        [TestCase("one")]
        [TestCase("two")]
        public void ExpandedParameterizedTest(string s)
        {
        }

        [Test(Description = "Expanded Parameterized Test With Display Name")]
        [TestCase("one", TestName = "Value one")]
        [TestCase("two", TestName = "Value two")]
        public void ExpandedParameterizedTestWithDisplayName(string s)
        {
        }

        private static IEnumerable<TestCaseData> TestCaseSourceData()
        {
            yield return new TestCaseData(new SerializableTestData { Name = "First", Value = 1 });
            yield return new TestCaseData(new SerializableTestData { Name = "Second", Value = 2 });
        }

        private static IEnumerable<TestCaseData> TestCaseSourceDataWithDisplayName ()
        {
            yield return new TestCaseData(new SerializableTestData { Name = "First", Value = 1 }) { TestName = "Serializable Test Data 1" };
            yield return new TestCaseData(new SerializableTestData { Name = "Second", Value = 2 }) { TestName = "Serializable Test Data 2" };
        }

        [Test]
        [TestCaseSource(nameof(TestCaseSourceData))]
        public void ExpandedTestCaseSourceTest(SerializableTestData data)
        {
        }

        [Test(Description = "Expanded Test Case Source Test With Display Name")]
        [TestCaseSource(nameof(TestCaseSourceDataWithDisplayName))]
        public void ExpandedTestCaseSourceTestWithDisplayName(SerializableTestData data)
        {
        }

        private static void WriteTestHostLine(Stream stream, string message)
        {
            // Bypass framework capture so the host process emits each line immediately.
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(message + Environment.NewLine);
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
        }
    }
}
