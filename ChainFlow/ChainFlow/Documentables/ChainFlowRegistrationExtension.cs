using ChainFlow.Internals;

namespace ChainFlow.Documentables
{
    internal static class ChainFlowRegistrationExtension
    {
        public static string GetDocumentFlowId(this ChainFlowRegistration registration)
        {
            return $"_{registration.LinkType.GetHashCode()}";
        }
    }
}
