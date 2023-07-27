using ChainFlow.Enums;

namespace ChainFlow.Documentables
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DocumentFlowBehaviorAttribute : Attribute
    {
        public DocumentFlowBehaviorAttribute(DocumentFlowBehavior behavior)
        {
            Behavior = behavior;
        }

        public DocumentFlowBehavior Behavior { get; }
    }
}
