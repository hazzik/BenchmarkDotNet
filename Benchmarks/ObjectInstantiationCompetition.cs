using System;
using System.Reflection.Emit;
using BenchmarkDotNet;

namespace Benchmarks
{
    public class TestClass
    {
    }

    public class ObjectInstantiationCompetition : BenchmarkCompetition
    {
        private readonly object[] _empty = new object[0];
        private const int IterationCount = 10000001;

        [BenchmarkMethod]
        public TestClass InvokeConstructor()
        {
            TestClass o = default(TestClass);
            for (int i = 0; i < IterationCount; i++)
            {
                o = (TestClass) typeof (TestClass).GetConstructor(Type.EmptyTypes).Invoke(_empty);
            }
            return o;
        }

        [BenchmarkMethod]
        public TestClass InvokeCachedConstructor()
        {
            var constructor = typeof (TestClass).GetConstructor(Type.EmptyTypes);
            TestClass o = default(TestClass);
            for (int i = 0; i < IterationCount; i++)
            {
                o = (TestClass) constructor.Invoke(_empty);
            }
            return o;
        }

        [BenchmarkMethod]
        public TestClass InvokeCachedConstructorDelegate()
        {
            Type type = typeof(TestClass);
            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor == null) throw new MissingMethodException("There is no constructor without defined parameters for this object");
            var dynamic = new DynamicMethod(string.Empty,
                        type,
                        Type.EmptyTypes,
                        type);
            var il = dynamic.GetILGenerator();

            il.DeclareLocal(type);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);

            var func = (Func<TestClass>)dynamic.CreateDelegate(typeof(Func<TestClass>));

            TestClass o = default(TestClass);
            for (int i = 0; i < IterationCount; i++)
            {
                o = func();
            }
            return o;
        }

        [BenchmarkMethod]
        public TestClass ActivatorCreateInstance()
        {
            TestClass o = default(TestClass);
            for (int i = 0; i < IterationCount; i++)
            {
                o = Activator.CreateInstance<TestClass>();
            }
            return o;
        }

        [BenchmarkMethod]
        public TestClass New()
        {
            TestClass o = default(TestClass);
            for (int i = 0; i < IterationCount; i++)
            {
                o = new TestClass();
            }
            return o;
        }
    }
}