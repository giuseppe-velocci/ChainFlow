using ChainFlow.Helpers;
using ChainFlow.Interfaces;
using ChainFlowUnitTest.TestHelpers;

namespace ChainFlowUnitTest.Helpers
{
    public class TypeExtensionTest
    {
        [Fact]
        public void GetFullName_WhenTypeIsNotGeneric_ReturnsName()
        {
            var type = typeof(FakeChainLink0);
            var expected = type.Name;
            Assert.Equal(expected, type.GetFullName());
        }

        [Fact]
        public void GetFullName_WhenTypeIsGeneric_ReturnsNameWithGenericTypes()
        {
            var type = typeof(IRouterDispatcher<string>);
            var expected = "IRouterDispatcher<String>";
            Assert.Equal(expected, type.GetFullName());
        }

        [Fact]
        public void GetFullName_WhenTypeHasManyGenericParamaters_ReturnsNameWithGenericTypes()
        {
            var type = typeof(ClassWithGenericParams<FakeChainLink1, bool>);
            var expected = "ClassWithGenericParams<FakeChainLink1,Boolean>";
            Assert.Equal(expected, type.GetFullName());
        }

        [Fact]
        public void GetFullName_WhenTypeHasGenericParamaterThatIsGeneric_ReturnsNameWithGenericTypes()
        {
            var type = typeof(ClassWithGenericParams<IEnumerable<FakeChainLink0>, bool>);
            var expected = "ClassWithGenericParams<IEnumerable<FakeChainLink0>,Boolean>";
            Assert.Equal(expected, type.GetFullName());
        }
    }

    class ClassWithGenericParams<T, V> { }
}
