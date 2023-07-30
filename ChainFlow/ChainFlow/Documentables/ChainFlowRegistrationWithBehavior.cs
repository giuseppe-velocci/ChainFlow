using ChainFlow.Enums;
using ChainFlow.Internals;

namespace ChainFlow.Documentables
{
    record class ChainFlowRegistrationWithBehavior : ChainFlowRegistration
    {
        public ChainFlowRegistrationWithBehavior(ChainFlowRegistration original, DocumentFlowBehaviorAttribute? behavior) : base(original)
        {
            Behavior = behavior?.Behavior ?? DocumentFlowBehavior.Standard;
        }

        public DocumentFlowBehavior Behavior { get; }
    }
}
