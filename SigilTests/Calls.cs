﻿using Sigil;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SigilTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class Calls
    {
        [Fact]
        public void ValueTypeCallIndirect()
        {
            var hasValue = typeof(int?).GetProperty("HasValue");
            var getHasValue = hasValue.GetGetMethod();

            var e1 = Emit<Func<int?, bool>>.NewDynamicMethod();
            e1.LoadArgumentAddress(0);
            e1.Duplicate();
            e1.LoadVirtualFunctionPointer(getHasValue);
            e1.CallIndirect(getHasValue.CallingConvention, typeof(bool));
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.True(d1(1));
            Assert.False(d1(null));
        }

        [Fact]
        public void ValueTypeCallVirtual()
        {
            var hasValue = typeof(int?).GetProperty("HasValue");
            var getHasValue = hasValue.GetGetMethod();

            var e1 = Emit<Func<int?, bool>>.NewDynamicMethod();
            e1.LoadArgumentAddress(0);
            e1.CallVirtual(getHasValue);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.True(d1(1));
            Assert.False(d1(null));
        }

        [Fact]
        public void ValueTypeCall()
        {
            var hasValue = typeof(int?).GetProperty("HasValue");
            var getHasValue = hasValue.GetGetMethod();

            var e1 = Emit<Func<int?, bool>>.NewDynamicMethod();
            e1.LoadArgumentAddress(0);
            e1.Call(getHasValue);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.True(d1(1));
            Assert.False(d1(null));
        }

        [Fact]
        public void MultipleTailcalls()
        {
            var toString = typeof(object).GetMethod("ToString");

            var e1 = Emit<Func<int, string>>.NewDynamicMethod();
            var l1 = e1.DefineLabel("l1");
            var l2 = e1.DefineLabel("l2");
            var l3 = e1.DefineLabel("l3");

            e1.LoadArgument(0);
            e1.LoadConstant(0);
            e1.BranchIfEqual(l1);

            e1.LoadArgument(0);
            e1.LoadConstant(1);
            e1.BranchIfEqual(l2);

            e1.LoadArgument(0);
            e1.LoadConstant(2);
            e1.BranchIfEqual(l3);

            e1.LoadConstant("Foo");
            e1.Call(toString);
            e1.Return();

            e1.MarkLabel(l1);
            e1.LoadConstant(123);
            e1.Box<int>();
            e1.CallVirtual(toString);
            e1.Return();

            e1.MarkLabel(l2);
            e1.NewObject<object>();
            e1.Duplicate();
            e1.LoadVirtualFunctionPointer(toString);
            e1.CallIndirect<string>(toString.CallingConvention);
            e1.Return();

            e1.MarkLabel(l3);
            e1.LoadConstant("");
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate(out instrs);

            Assert.Equal("123", d1(0));
            Assert.Equal("System.Object", d1(1));
            Assert.Equal("", d1(2));
            Assert.Equal("System.String", d1(314));

            Assert.Equal("ldarg.0\r\nldc.i4.0\r\nbeq.s l1\r\nldarg.0\r\nldc.i4.1\r\nbeq.s l2\r\nldarg.0\r\nldc.i4.2\r\nbeq.s l3\r\nldstr 'Foo'\r\ntail.call System.String ToString()\r\nret\r\n\r\nl1:\r\nldc.i4.s 123\r\nbox System.Int32\r\ntail.callvirt System.String ToString()\r\nret\r\n\r\nl2:\r\nnewobj Void .ctor()\r\ndup\r\nldvirtftn System.String ToString()\r\ncalli Standard, HasThis System.String \r\nret\r\n\r\nl3:\r\nldstr ''\r\nret\r\n", instrs);
        }

        [Fact]
        public void PartialTypeMapping2()
        {
            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");
                var t = mod.DefineType("T");
                var dictOfT = typeof(IDictionary<,>).MakeGenericType(t, t);

                var e1 = Emit<Func<object, object, bool>>.BuildMethod(t, "E1", MethodAttributes.Public, CallingConventions.HasThis);
                e1.LoadArgument(1);
                e1.CastClass(dictOfT);
                e1.LoadArgument(2);
                e1.CallVirtual(typeof(object).GetMethod("Equals", new Type[] { typeof(object) }));
                e1.Return();

                e1.CreateMethod();

                var type = t.CreateType();

                var inst = type.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
                var e1Mtd = type.GetMethod("E1");

                Func<object, object, bool> d1 = (a, b) => (bool)e1Mtd.Invoke(inst, new object[] { a, b });

                dictOfT = typeof(Dictionary<,>).MakeGenericType(type, type);
                var cons = dictOfT.GetConstructor(Type.EmptyTypes);

                var dictInst = cons.Invoke(new object[0]);
                var listAdd = dictOfT.GetMethod("Add", new[] { type, type });
                listAdd.Invoke(dictInst, new object[] { inst, inst });

                var x = dictInst;

                Assert.True(d1(x, x));
                Assert.False(d1(x, new List<int> { 1, 2, 3 }));
            }
        }

        [Fact]
        public void PartialTypeMapping1()
        {
            {
                var asm = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Foo"), AssemblyBuilderAccess.Run);
                var mod = asm.DefineDynamicModule("Bar");
                var t = mod.DefineType("T");
                var listOfT = typeof(List<>).MakeGenericType(t);

                var e1 = Emit<Func<object, object, bool>>.BuildMethod(t, "E1", MethodAttributes.Public, CallingConventions.HasThis);
                e1.LoadArgument(1);
                e1.CastClass(listOfT);
                e1.LoadArgument(2);
                e1.CallVirtual(typeof(object).GetMethod("Equals", new Type[] { typeof(object) }));
                e1.Return();

                e1.CreateMethod();

                var type = t.CreateType();

                var inst = type.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
                var e1Mtd = type.GetMethod("E1");

                Func<object, object, bool> d1 = (a, b) => (bool)e1Mtd.Invoke(inst, new object[] { a, b });

                listOfT = typeof(List<>).MakeGenericType(type);
                var cons = listOfT.GetConstructor(Type.EmptyTypes);

                var listInst = cons.Invoke(new object[0]);
                var listAdd = listOfT.GetMethod("Add", new[] { type });
                listAdd.Invoke(listInst, new object[] { inst });

                var x = listInst;

                Assert.True(d1(x, x));
                Assert.False(d1(x, new List<int> { 1, 2, 3 }));
            }
        }

        public enum EnumParamsEnum
        {
            A,
            B
        }

        private static EnumParamsEnum? _EnumParamsMethod;
        public static void EnumParamsMethod(EnumParamsEnum foo)
        {
            _EnumParamsMethod = foo;
        }

        [Fact]
        public void EnumParams()
        {
            var e1 = Emit<Action<int>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.Call(typeof(Calls).GetMethod("EnumParamsMethod"));
            e1.Return();

            var d1 = e1.CreateDelegate();

            _EnumParamsMethod = null;
            d1((int)EnumParamsEnum.A);
            Assert.Equal(EnumParamsEnum.A, _EnumParamsMethod.Value);

            _EnumParamsMethod = null;
            d1((int)EnumParamsEnum.B);
            Assert.Equal(EnumParamsEnum.B, _EnumParamsMethod.Value);
        }

        private static bool DoesNothingWasCalled;
        public static void DoesNothing()
        {
            DoesNothingWasCalled = true;
        }

        [Fact]
        public void VoidStatic()
        {
            DoesNothingWasCalled = false;

            var mi = typeof(Calls).GetMethod("DoesNothing");

            var e1 = Emit<Action>.NewDynamicMethod("E1");
            e1.Call(mi);
            e1.Return();

            var del = e1.CreateDelegate();

            Assert.False(DoesNothingWasCalled);
            del();
            Assert.True(DoesNothingWasCalled);
        }

        class VoidInstanceClass
        {
            public int Go() { return 314159; }
        }

        [Fact]
        public void VoidInstance()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod("E1");
            e1.NewObject<VoidInstanceClass>();
            e1.Call(typeof(VoidInstanceClass).GetMethod("Go"));
            e1.Return();

            var del = e1.CreateDelegate();

            Assert.Equal(314159, del());
        }

        class StringInstanceClass
        {
            public string Go(int hello) { return hello.ToString(); }
        }

        [Fact]
        public void StringInstance()
        {
            var e1 = Emit<Func<string>>.NewDynamicMethod("E1");
            e1.NewObject<StringInstanceClass>();
            e1.LoadConstant(8675309);
            e1.Call(typeof(StringInstanceClass).GetMethod("Go"));
            e1.Return();

            var del = e1.CreateDelegate();

            Assert.Equal("8675309", del());
        }

        public static int MultiParamFunc(string a, int b, double c)
        {
            return int.Parse(a) + b + (int)c;
        }

        [Fact]
        public void MultiParam()
        {
            var func = typeof(Calls).GetMethod("MultiParamFunc");
            var e1 = Emit<Func<string, int, double, int>>.NewDynamicMethod();

            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.LoadArgument(2);
            e1.Call(func);
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(123 + 456 + 7, d1("123", 456, 7.89));
        }

        [Fact]
        public void DynamicRecursive()
        {
            var impl = Emit<Func<int,int>>.NewDynamicMethod("factorial");
            var lbl = impl.DefineLabel();
            impl.LoadArgument(0);
            impl.BranchIfTrue(lbl);
            impl.LoadConstant(0);
            impl.Return();
            impl.MarkLabel(lbl);
            impl.LoadArgument(0);
            impl.LoadArgument(0);
            impl.LoadConstant(1);
            impl.Subtract();
            impl.Call(impl);
            impl.Add();
            impl.Return();

            string instr;
            var del = impl.CreateDelegate(out instr);
            Assert.Equal(del(1), 1);
            Assert.Equal(del(2), 3);
            Assert.Equal(del(3), 6);
            Assert.False(instr.Contains("tail."));
        }

        [Fact]
        public void DynamicRecursiveTail()
        {
            var impl = Emit<Func<int, int, int>>.NewDynamicMethod("factorialImpl");
            var @else = impl.DefineLabel();

            impl.LoadArgument(0);     // if(x0 != 0) return x1
            impl.BranchIfTrue(@else);
            impl.LoadArgument(1);
            impl.Return();
            impl.MarkLabel(@else);
            impl.LoadArgument(0);     // return FactorialImpl(x0 - 1, x0 + x1)
            impl.LoadConstant(1);
            impl.Subtract();
            impl.LoadArgument(0);
            impl.LoadArgument(1);
            impl.Add();
            impl.Call(impl);          // <- tail call
            impl.Return();

            string instr;
            var del = impl.CreateDelegate(out instr);
            Assert.Equal(del(1, 0), 1);
            Assert.Equal(del(2, 0), 3);
            Assert.Equal(del(3, 0), 6);
            Assert.True(instr.Contains("tail."));
        }
    }
}
