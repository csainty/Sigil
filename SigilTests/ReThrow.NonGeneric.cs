﻿
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SigilTests
{
    public partial class ReThrow
    {
        [Fact]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(void), Type.EmptyTypes);
            var m = typeof(ReThrow).GetMethod("AlwaysThrows");

            var t = e1.BeginExceptionBlock();
            e1.Call(m);
            var c = e1.BeginCatchAllBlock(t);
            e1.Pop();
            e1.ReThrow();
            e1.EndCatchBlock(c);
            e1.EndExceptionBlock(t);

            e1.Return();

            var d1 = e1.CreateDelegate<Action>();

            try
            {
                d1();
                Assert.True(false, "Expected exception was not thrown");
            }
            catch (Exception e)
            {
                Assert.Equal("Hello World", e.Message);
            }
        }
    }
}
