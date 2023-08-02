using Moq;

namespace ChainFlow.TestKit.Internals
{
    internal class TestInstanceFactory
    {
        private readonly Type _mockTypeDefinition;
        private readonly IDictionary<int, object> _container = new Dictionary<int, object>();
        private readonly LookupOrFallbackDefaultValueProvider _defaultValueProvider;

        public TestInstanceFactory(LookupOrFallbackDefaultValueProvider defaultValueProvider)
        {
            _mockTypeDefinition = typeof(Mock<>).GetGenericTypeDefinition();
            _defaultValueProvider = defaultValueProvider;
        }

        public object CreateInstance(Type type)
        {
            var ctor = type.GetConstructors().First();
            var ctorParams = ctor.GetParameters();
            var ctorParamTypes = ctorParams.Select(p => p.ParameterType).ToList();

            List<object> paramInstances = new();
            ctorParamTypes.ForEach(paramType =>
            {
                object? instance = null;
                if (paramType.IsInterface)
                {
                    instance = CreateMockFromInterfaceType(paramType);
                }
                else
                {
                    instance = CreateInstance(paramType);
                }

                if (!_container.TryGetValue(paramType.GetHashCode(), out object? _))
                {
                    _container[paramType.GetHashCode()] = instance!;
                }
                paramInstances.Add(_container[paramType.GetHashCode()]);
            });

            return ctor.Invoke(paramInstances.Select(x => (x as Mock)?.Object ?? x).ToArray());
        }

        private object? CreateMockFromInterfaceType(Type? interfaceType)
        {
            Type? fullType = _mockTypeDefinition.MakeGenericType(interfaceType!);
            object? instance = Activator.CreateInstance(fullType);
            fullType.GetProperty("DefaultValueProvider")!.SetValue(instance, _defaultValueProvider);
            return instance;
        }

        public Mock<IMockedDependency> GetMock<IMockedDependency>() where IMockedDependency : class
            => (Mock<IMockedDependency>)_container[typeof(IMockedDependency).GetHashCode()];

    }
}
