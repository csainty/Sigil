﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SigilTests
{
    public partial class Add
    {
        [TestMethod]
        public unsafe void PointerToPointerNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int*), new[] { typeof(int), typeof(int*) });
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Add();
            e1.Return();

            var d1 = (PointerToPointerDelegate)e1.CreateDelegate(typeof(PointerToPointerDelegate));

            int* ptr1 = (int*)Marshal.AllocHGlobal(64);

            var ptr2 = d1(4, ptr1);

            Marshal.FreeHGlobal((IntPtr)ptr1);

            Assert.AreEqual(((int)ptr1) + 4, (int)ptr2);
        }

        [TestMethod]
        public unsafe void ByRefToByRefNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(void), new [] { Type.GetType("System.Int32&"), typeof(int), Type.GetType("System.Int32&") });
            e1.LoadArgument(2);
            e1.LoadArgument(1);
            e1.LoadArgument(0);
            e1.LoadIndirect<int>();
            e1.Add();
            e1.StoreIndirect<int>();
            e1.Return();

            var d1 = (ByRefToByRefDelegate)e1.CreateDelegate(typeof(ByRefToByRefDelegate));

            int a = 2;
            int c = 0;
            d1(ref a, 4, ref c);

            Assert.AreEqual(6, c);
        }

        [TestMethod]
        public void BlogPostNonGeneric()
        {
            {
                var method = new DynamicMethod("AddOneAndTwo", typeof(int), Type.EmptyTypes);
                var il = method.GetILGenerator();
                il.Emit(OpCodes.Ldc_I4, 1);
                il.Emit(OpCodes.Ldc_I4, 2);
                il.Emit(OpCodes.Add);
                il.Emit(OpCodes.Ret);

                var del = (Func<int>)method.CreateDelegate(typeof(Func<int>));

                Assert.AreEqual(3, del());
            }

#if !__MonoCS__ // Mono does validation at delegate creation that fails this test
            {
                var method = new DynamicMethod("AddOneAndTwo", typeof(int), Type.EmptyTypes);
                var il = method.GetILGenerator();
                il.Emit(OpCodes.Ldc_I4, 1);
                il.Emit(OpCodes.Add);
                il.Emit(OpCodes.Ret);

                var del = (Func<int>)method.CreateDelegate(typeof(Func<int>));

                try
                {
                    del();
                    Assert.Fail();
                }
                catch (Exception e)
                {
                    Assert.AreEqual("Common Language Runtime detected an invalid program.", e.Message);
                }
            }
#endif

            {

                try
                {
                    var il = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "AddOneAndTwo");
                    il.LoadConstant(1);
                    il.Add();
                    il.Return();

                    var del = (Func<int>)il.CreateDelegate(typeof(Func<int>));

                    del();
                    Assert.Fail();
                }
                catch (Sigil.SigilVerificationException e)
                {
                    Assert.AreEqual("Add expects 2 values on the stack", e.Message);
                }
            }
        }

        [TestMethod]
        public void IntIntNonGeneric()
        {
            var il = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes,"IntInt");
            il.LoadConstant(1);
            il.LoadConstant(2);
            il.Add();
            il.Return();

            var del = (Func<int>)il.CreateDelegate(typeof(Func<int>));
            Assert.AreEqual(3, del());
        }

        [TestMethod]
        public void IntNativeIntNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "E1");
            e1.LoadConstant(1);
            e1.Convert<IntPtr>();
            e1.LoadConstant(3);
            e1.Add();
            e1.Convert<int>();
            e1.Return();

            var d1 = (Func<int>)e1.CreateDelegate(typeof(Func<int>));

            Assert.AreEqual(4, d1());
        }

        [TestMethod]
        public void NativeIntIntNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "E1");
            e1.LoadConstant(1);
            e1.LoadConstant(3);
            e1.Convert<IntPtr>();
            e1.Add();
            e1.Convert<int>();
            e1.Return();

            var d1 = (Func<int>)e1.CreateDelegate(typeof(Func<int>));

            Assert.AreEqual(4, d1());
        }

        [TestMethod]
        public void NativeIntNativeIntNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "E1");
            e1.LoadConstant(1);
            e1.Convert<IntPtr>();
            e1.LoadConstant(3);
            e1.Convert<IntPtr>();
            e1.Add();
            e1.Convert<int>();
            e1.Return();

            var d1 = (Func<int>)e1.CreateDelegate(typeof(Func<int>));

            Assert.AreEqual(4, d1());
        }

        [TestMethod]
        public void IntPointerNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(int) }, "E1");
            e1.LoadArgumentAddress(0);
            e1.LoadConstant(2);
            e1.Add();
            e1.Convert<int>();
            e1.Return();

            var d1 = (Func<int, int>)e1.CreateDelegate(typeof(Func<int, int>));

            try
            {
                var x = d1(3);

                Assert.IsTrue(x != 0);
            }
            catch
            {
                Assert.Fail("ShouldBeLegal");
            }
        }

        [TestMethod]
        public void PointerIntNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(int) }, "E1");
            e1.LoadConstant(2);
            e1.LoadArgumentAddress(0);
            e1.Add();
            e1.Convert<int>();
            e1.Return();

            var d1 = (Func<int, int>)e1.CreateDelegate(typeof(Func<int, int>));

            try
            {
                var x = d1(3);

                Assert.IsTrue(x != 0);
            }
            catch
            {
                Assert.Fail("ShouldBeLegal");
            }
        }

        [TestMethod]
        public void PointerNativeIntNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(int) }, "E1");
            e1.LoadConstant(2);
            e1.Convert<IntPtr>();
            e1.LoadArgumentAddress(0);
            e1.Add();
            e1.Convert<int>();
            e1.Return();

            var d1 = (Func<int, int>)e1.CreateDelegate(typeof(Func<int, int>));

            try
            {
                var x = d1(3);

                Assert.IsTrue(x != 0);
            }
            catch
            {
                Assert.Fail("ShouldBeLegal");
            }
        }

        [TestMethod]
        public void LongLongNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(long), new [] { typeof(long), typeof(long) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Add();
            e1.Return();

            var d1 = (Func<long, long, long>)e1.CreateDelegate(typeof(Func<long, long, long>));

            Assert.AreEqual(2 * ((long)uint.MaxValue), d1(uint.MaxValue, uint.MaxValue));
        }

        [TestMethod]
        public void FloatFloatNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(float), new [] { typeof(float), typeof(float) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Add();
            e1.Return();

            var d1 = (Func<float, float, float>)e1.CreateDelegate(typeof(Func<float, float, float>));

            Assert.AreEqual(3.14f + 1.59f, d1(3.14f, 1.59f));
        }

        [TestMethod]
        public void DoubleDoubleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(double), new [] { typeof(double), typeof(double) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Add();
            e1.Return();

            var d1 = (Func<double, double, double>)e1.CreateDelegate(typeof(Func<double, double, double>));

            Assert.AreEqual(3.14 + 1.59, d1(3.14, 1.59));
        }

        [TestMethod]
        public void OverflowNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), new [] { typeof(int), typeof(int) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.AddOverflow();
            e1.Return();

            var d1 = (Func<int, int, int>)e1.CreateDelegate(typeof(Func<int, int, int>));

            try
            {
                d1(int.MaxValue, 10);
                Assert.Fail();
            }
            catch (OverflowException)
            {
            }
        }

        [TestMethod]
        public void UnsignedOverflowNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(uint), new[] { typeof(uint), typeof(uint) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedAddOverflow();
            e1.Return();

            var d1 = (Func<uint, uint, uint>)e1.CreateDelegate(typeof(Func<uint, uint, uint>));

            try
            {
                d1(uint.MaxValue, 10);
                Assert.Fail();
            }
            catch (OverflowException)
            {

            }
        }
    }
}
