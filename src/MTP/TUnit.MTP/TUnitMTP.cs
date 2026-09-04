namespace TUnit.MTP
{
    [Serializable]
    public class SerializableTestData
    {
        public string? Name { get; set; }
        public int Value { get; set; }
    }

    public class TUnitMTP
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

            WriteTestHostLine(standardOutput, "[TUnit] stdout 1/3: test started");
            Thread.Sleep(TimeSpan.FromSeconds(1));
            WriteTestHostLine(standardError, "[TUnit] stderr 2/3: test is still running");
            Thread.Sleep(TimeSpan.FromSeconds(1));
            WriteTestHostLine(standardOutput, "[TUnit] stdout 3/3: test is completing");
        }

        [Test]
        [Arguments("one")]
        [Arguments("two")]
        public void ExpandedParameterizedTest(string s)
        {
        }

        [Test]
        [DisplayName("Expanded Parameterized Test With Display Name")]
        [Arguments("one")]
        [Arguments("two")]
        public void ExpandedParameterizedTestWithDisplayName(string s)
        {
        }        

        public static IEnumerable<Func<SerializableTestData>> MethodDataSourceData()
        {
            yield return () => new SerializableTestData { Name = "First", Value = 1 };
            yield return () => new SerializableTestData { Name = "Second", Value = 2 };
        }

        [Test]
        [MethodDataSource(nameof(MethodDataSourceData))]
        public void ExpandedMethodDataSourceTest(SerializableTestData data)
        {
        }

        [Test]
        [DisplayName("Expanded Method Data Source Test With Display Name")]
        [MethodDataSource(nameof(MethodDataSourceData))]
        public void ExpandedMethodDataSourceTestWithDisplayName(SerializableTestData data)
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
