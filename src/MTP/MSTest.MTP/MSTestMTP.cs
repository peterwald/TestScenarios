namespace MSTest.MTP
{
    [Serializable]
    public class SerializableTestData
    {
        public string? Name { get; set; }
        public int Value { get; set; }
    }

    [TestClass]
    public sealed class MSTestMTP
    {
        [TestMethod]
        public void SimpleTest()
        {
        }

        [TestMethod]
        [DataRow("one")]
        [DataRow("two")]
        public void ExpandedParameterizedTest(string s)
        {
        }

        [TestMethod(DisplayName = "Expanded Parameterized Test With Display Name")]
        [DataRow("one", DisplayName = "Value one")]
        [DataRow("two", DisplayName = "Value two")]
        public void ExpandedParameterizedTestWithDisplayName(string s)
        {
        }

        [TestMethod(UnfoldingStrategy = TestDataSourceUnfoldingStrategy.Fold)]
        [DataRow("one")]
        [DataRow("two")]
        public void NonExpandedParameterizedTest(string s)
        {
        }

        [TestMethod(DisplayName = "Non Expanded Parameterized Test With Display Name", UnfoldingStrategy = TestDataSourceUnfoldingStrategy.Fold)]
        [DataRow("one", DisplayName = "Value one")]
        [DataRow("two", DisplayName = "Value two")]
        public void NonExpandedParameterizedTestWithDisplayName(string s)
        {
        }

        public static IEnumerable<object[]> DynamicTestData
        {
            get
            {
                yield return new object[] { new SerializableTestData { Name = "First", Value = 1 } };
                yield return new object[] { new SerializableTestData { Name = "Second", Value = 2 } };
            }
        }

        public static IEnumerable<TestDataRow<SerializableTestData>> DynamicTestDataWithdDisplayName
        {
            get
            {
                yield return new TestDataRow<SerializableTestData>(new SerializableTestData { Name = "First", Value = 1 }) { DisplayName = "Serializable Test Data 1" };
                yield return new TestDataRow<SerializableTestData>(new SerializableTestData { Name = "Second", Value = 2 }) { DisplayName = "Serializable Test Data 2" };
            }
        }

        [TestMethod]
        [DynamicData(nameof(DynamicTestData))]
        public void ExpandedDynamicDataTest(SerializableTestData data)
        {
        }

        [TestMethod(DisplayName = "Expanded Dynamic Data Test With Display Name")]
        [DynamicData(nameof(DynamicTestDataWithdDisplayName))]
        public void ExpandedDynamicDataTestWithDisplayName(SerializableTestData data)
        {
        }

        [TestMethod(UnfoldingStrategy = TestDataSourceUnfoldingStrategy.Fold)]
        [DynamicData(nameof(DynamicTestData))]
        public void NonExpandedDynamicDataTest(SerializableTestData data)
        {
        }

        [TestMethod(DisplayName = "Non Expanded Dynamic Data Test With Display Name", UnfoldingStrategy = TestDataSourceUnfoldingStrategy.Fold)]
        [DynamicData(nameof(DynamicTestDataWithdDisplayName))]
        public void NonExpandedDynamicDataTestWithDisplayName(SerializableTestData data)
        {
        }
    }
}
