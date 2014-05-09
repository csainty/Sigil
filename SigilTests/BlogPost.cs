﻿using Sigil;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SigilTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class BlogPost
    {
        [Fact]
        public void Block1()
        {
            var il = Emit<Func<int>>.NewDynamicMethod("AddOneAndTwo");
            il.LoadConstant(1);
            try
            {
                // Still missing that 2!
                il.Add();
                il.Return();
                var del = il.CreateDelegate();
                del();

                Assert.True(false, "Expected exception was not thrown");
            }
            catch (SigilVerificationException e)
            {
                Assert.Equal("Add expects 2 values on the stack", e.Message);
            }
        }

        [Fact]
        public void Block2()
        {
            var il = Emit<Func<string, Func<string, int>, string>>.NewDynamicMethod("E1");
            var invoke = typeof(Func<string, int>).GetMethod("Invoke");
            try
            {
                var notNull = il.DefineLabel("not_null");

                il.LoadArgument(0);
                il.LoadNull();
                il.UnsignedBranchIfNotEqual(notNull);
                il.LoadNull();
                il.Return();

                
                il.MarkLabel(notNull);
                il.LoadArgument(1);
                il.LoadArgument(0);
                il.CallVirtual(invoke);
                il.Return();

                il.CreateDelegate();

                Assert.True(false, "Expected exception was not thrown");
            }
            catch (SigilVerificationException e)
            {
                Assert.Equal("Return expected a System.String; found int", e.Message);
            }
        }
    }
}
