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

        [Theory]
        [InlineData("one")]
        [InlineData("two")]
        public void ExpandedParameterizedTest(string s)
        {
        }

        [Theory(DisplayName = "Expanded Parameterized Test With Display Name")]
        [InlineData("one")]
        [InlineData("two")]
        public void ExpandedParameterizedTestWithDisplayName(string s)
        {
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
        }

        [Theory(DisplayName = "Expanded Member Data Test With Display Name")]
        [MemberData(nameof(MemberDataSource))]
        public void ExpandedMemberDataTestWithDisplayName(SerializableTestData data)
        {
        }

        [Theory]
        [MemberData(nameof(MemberDataSource), DisableDiscoveryEnumeration = true)]
        public void NonExpandedMemberDataTest(SerializableTestData data)
        {
        }

        [Theory(DisplayName = "Expanded Member Data Test With Display Name")]
        [MemberData(nameof(MemberDataSource), DisableDiscoveryEnumeration = true)]
        public void NonExpandedMemberDataTestWithDisplayName(SerializableTestData data)
        {
        }
    }
}
