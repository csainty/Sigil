using Sigil;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SigilTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class IsInstance
    {
        [Fact]
        public void NotElided()
        {
            var e1 = Emit<Func<string, string>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.IsInstance<string>();
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate(out instrs, OptimizationOptions.None);

            Assert.Equal("hello", d1("hello"));

            Assert.True(instrs.Contains("isinst"));
        }

        [Fact]
        public void Elided()
        {
            var e1 = Emit<Func<string, string>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.IsInstance<string>();
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate(out instrs);

            Assert.Equal("hello", d1("hello"));

            Assert.False(instrs.Contains("isinst"));
        }

        [Fact]
        public void Simple()
        {
            var e1 = Emit<Func<object, string>>.NewDynamicMethod();
            e1.LoadArgument(0);
            e1.IsInstance<string>();
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate(out instrs);

            Assert.Equal(null, d1(123));
            Assert.Equal("hello", d1("hello"));

            Assert.True(instrs.Contains("isinst"));
        }
    }
}
