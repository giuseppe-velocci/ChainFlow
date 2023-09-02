using ChainFlow.Helpers;

namespace ChainFlow.TestKit
{
    public class ChainFlowStack
    {
        public string Name { get; }

        public ChainFlowStack(Type flowType)
        {
            Name = flowType.GetFullName();
        }

        public ChainFlowStack(Type flowType, string nameSuffix)
        {
            Name = flowType.GetFullName(nameSuffix);
        }
    }
}