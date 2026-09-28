using Xunit;

namespace XUnit.MTPv1
{
    [Serializable]
    public class SerializableTestData
    {
        public string? Name { get; set; }
        public int Value { get; set; }
    }

    public class XUnitMTPv1
    {
        [Fact]
        public void SimpleTest()
        {
        }

        [Fact]
        public void StreamsTestHostOutput()
        {
            Stream standardOutput = Console.OpenStandardOutput();
            Stream standardError = Console.OpenStandardError();

            WriteTestHostLine(standardOutput, "[xUnit MTP v1] stdout 1/3: test started");
            Thread.Sleep(TimeSpan.FromSeconds(1));
            WriteTestHostLine(standardError, "[xUnit MTP v1] stderr 2/3: test is still running");
            Thread.Sleep(TimeSpan.FromSeconds(1));
            WriteTestHostLine(standardOutput, "[xUnit MTP v1] stdout 3/3: test is completing");
        }

        [Theory]
        [InlineData("one")]
        [InlineData("two")]
        public void ExpandedParameterizedTest(string s)
        {
            _ = s;
        }

        [Theory(DisplayName = "Expanded Parameterized Test With Display Name")]
        [InlineData("one")]
        [InlineData("two")]
        public void ExpandedParameterizedTestWithDisplayName(string s)
        {
            _ = s;
        }

        public static IEnumerable<object[]> MemberDataSource
        {
            get
            {
                yield return new object[] { new SerializableTestData { Name = "First", Value = 1 } };
                yield return new object[] { new SerializableTestData { Name = "Second", Value = 2 } };
            }
        }

        [Theory]
        [MemberData(nameof(MemberDataSource))]
        public void ExpandedMemberDataTest(SerializableTestData data)
        {
            _ = data;
        }

        [Theory(DisplayName = "Expanded Member Data Test With Display Name")]
        [MemberData(nameof(MemberDataSource))]
        public void ExpandedMemberDataTestWithDisplayName(SerializableTestData data)
        {
            _ = data;
        }

        [Theory]
        [MemberData(nameof(MemberDataSource), DisableDiscoveryEnumeration = true)]
        public void NonExpandedMemberDataTest(SerializableTestData data)
        {
            _ = data;
        }

        [Theory(DisplayName = "Expanded Member Data Test With Display Name")]
        [MemberData(nameof(MemberDataSource), DisableDiscoveryEnumeration = true)]
        public void NonExpandedMemberDataTestWithDisplayName(SerializableTestData data)
        {
            _ = data;
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
