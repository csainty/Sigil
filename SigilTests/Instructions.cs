﻿using Sigil;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SigilTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class Instructions
    {
        [Fact]
        public void Simple()
        {
            var emitter = Emit<Func<int, string>>.NewDynamicMethod("E1");
            emitter
                .LoadArgument(0)
                .LoadArgument(0)
                .Add()
                .LoadArgument(0)
                .Multiply()
                .Box<int>()
                .CallVirtual(typeof(object).GetMethod("ToString", Type.EmptyTypes))
                .Return();

            var unoptimized = emitter.Instructions();
            string optimized;

            var d1 = emitter.CreateDelegate(out optimized);

            Assert.Equal(((2 + 2) * 2).ToString(), d1(2));

            Assert.Equal("ldarg.0\r\nldarg.0\r\nadd\r\nldarg.0\r\nmul\r\nbox System.Int32\r\ncallvirt System.String ToString()\r\nret", unoptimized);
            Assert.Equal("ldarg.0\r\nldarg.0\r\nadd\r\nldarg.0\r\nmul\r\nbox System.Int32\r\ntail.callvirt System.String ToString()\r\nret\r\n", optimized);
        }
    }
}
