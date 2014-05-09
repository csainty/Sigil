﻿using System;
using Sigil;
using Xunit;

namespace SigilTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Init
    {
        delegate string FooDelegate(object p);

        [Fact]
        public void EmitDelegateParsing()
        {
            var e1 = Emit<Action<int, string>>.NewDynamicMethod("E1");
            var e2 = Emit<Func<int>>.NewDynamicMethod("E2");
            var e3 = Emit<FooDelegate>.NewDynamicMethod("E3");

            Assert.NotNull(e1);
            Assert.NotNull(e2);
            Assert.NotNull(e3);

            try
            {
                Emit<string>.NewDynamicMethod("E4");
                Assert.True(false, "Shouldn't be able to emit non-delegate");
            }
            catch (ArgumentException e)
            {
                Assert.Equal("DelegateType must be a delegate, found System.String", e.Message);
            }
        }

        [Fact]
        public void EmitSingleReturn()
        {
            var e1 = Emit<Action>.NewDynamicMethod("E1");
            e1.Return();

            var del = e1.CreateDelegate();

            try
            {
                e1.LoadConstant(100);
                Assert.True(false, "Shouldn't be able to modify emit after a delegate has been created");
            }
            catch (InvalidOperationException e)
            {
                Assert.Equal(e.Message, "Cannot modify Emit after a delegate has been generated from it");
            }

            del();
        }
    }
}
