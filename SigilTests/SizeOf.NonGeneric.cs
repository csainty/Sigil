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
    public partial class SizeOf
    {
        [Fact]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "E1");
            e1.SizeOf<int>();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<int>>();

            Assert.Equal(sizeof(int), d1());
        }
    }
}
