using AutoFixture;
using ChainFlow.Models;
using Moq;
using System.Reflection;

namespace ChainFlow.TestKit.Internals
{
    internal class MoqChainFlowDefaultValueProvider : LookupOrFallbackDefaultValueProvider
    {
        private readonly Type _fixtureWrapperGenericInfo;

        public MoqChainFlowDefaultValueProvider()
        {
            _fixtureWrapperGenericInfo = typeof(FixtureWrapper<>).GetGenericTypeDefinition();
            Register(typeof(bool), GetDefaultBool);
            Register(typeof(OperationResult<>), GetDefaultOperationResult);
        }

        private object GetDefaultOperationResult(Type type, Mock mock)
        {
            var genericArgument = type.GetGenericArguments()[0];
            var genericTypeDefinition = type.GetGenericTypeDefinition();
            var fullType = genericTypeDefinition.MakeGenericType(genericArgument);
            var operationResultCreateWithSuccessMethod = fullType.GetMethod("CreateWithSuccess");
            var instance = Activator.CreateInstance(fullType, true);

            var genericArgumentInstance = FromFixture(genericArgument);
            return operationResultCreateWithSuccessMethod!
                .Invoke(instance, new object[] { genericArgumentInstance, string.Empty })!;
        }

        private object GetDefaultBool(Type type, Mock mock) => true;

        private object FromFixture(Type typeArgument) 
        {
            Type fullType = _fixtureWrapperGenericInfo.MakeGenericType(typeArgument);
            var fixture = Activator.CreateInstance(fullType);
            MethodInfo concreteMethod = fullType!.GetMethod("Create")!;
            return concreteMethod!.Invoke(fixture, null)!;
        }
    }

    class FixtureWrapper<T>
    {
        private readonly Fixture _fixture;

        public FixtureWrapper()
        {
            _fixture = new Fixture();
        }

        public T Create()
        {
            return _fixture.Create<T>();
        }
    }
}
