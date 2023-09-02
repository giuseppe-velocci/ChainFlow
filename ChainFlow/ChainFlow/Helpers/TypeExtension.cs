namespace ChainFlow.Helpers
{
    internal static class TypeExtension
    {
        public static string GetFullName(this Type type) =>
            type.IsGenericType ?
                $"{(type.Name.Split('`').First())}<\\{(string.Join(',', type.GetGenericArguments().Select(x => x.GetFullName())))}\\>" :
                type.Name;

        public static string GetFullName(this Type type, string nameSuffix) =>
            $"{GetFullName(type)}{nameSuffix}";
    }
}
