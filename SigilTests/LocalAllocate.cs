﻿using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SigilTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class LocalAllocate
    {
        [Fact]
        public void Simple()
        {
            var e1 = Emit<Action<byte[]>>.NewDynamicMethod();
            var arg = e1.DeclareLocal(typeof(byte*));
            var stack = e1.DeclareLocal(typeof(byte*));
            
            e1.LoadArgument(0);
            e1.LoadConstant(0);
            e1.LoadElementAddress<byte>();
            e1.StoreLocal(arg);
            
            e1.LoadArgument(0);
            e1.LoadLength<byte>();
            e1.LocalAllocate();
            e1.StoreLocal(stack);

            e1.LoadLocal(stack);
            e1.LoadConstant(123);
            e1.LoadArgument(0);
            e1.LoadLength<byte>();
            e1.InitializeBlock();

            e1.LoadLocal(arg);
            e1.LoadLocal(stack);

            e1.LoadArgument(0);
            e1.LoadLength<byte>();

            e1.CopyBlock();

            e1.Return();

            var d1 = e1.CreateDelegate();

            var b = new byte[100];

            d1(b);

            for (var i = 0; i < b.Length; i++)
            {
                Assert.Equal(123, b[i]);
            }
        }
    }
}
