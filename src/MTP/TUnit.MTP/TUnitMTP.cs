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

        public static IEnumerable<SerializableTestData> MethodDataSourceData()
        {
            yield return new SerializableTestData { Name = "First", Value = 1 };
            yield return new SerializableTestData { Name = "Second", Value = 2 };
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
    }
}
