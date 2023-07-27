using ChainFlow.Documentables;

namespace ChainFlowUnitTest
{
    public class DocumentChainFlowBuilderTest
    {
        private readonly DocumentChainFlowBuilder _sut;

        public DocumentChainFlowBuilderTest()
        {
            _sut = new ();
        }

        [Fact]
        public void ToString_WhenNoFlowsAreResolved_ReturnsEmptyString()
        {
            
        }

        [Fact]
        public void ToString_WhenSingleFlowIsResolved_ReturnsFlowString()
        {

        }

        [Fact]
        public void ToString_WhenMultipleFlowsAreResolved_ReturnsFlowString()
        {

        }
    }
}
