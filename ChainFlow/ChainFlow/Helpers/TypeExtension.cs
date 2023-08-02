namespace ChainFlow.Helpers
{
    public static class TypeExtension
    {
        public static string GetFullName(this Type type) =>
            type.IsGenericType ?
                $"{(type.Name.Split('`').First())}<{(string.Join(',', type.GetGenericArguments().Select(x => x.Name)))}>" :
                type.Name;
    }
}
