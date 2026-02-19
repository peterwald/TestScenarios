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
    }
}
