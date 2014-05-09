
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SigilTests
{
    public partial class IsInstance
    {
        [Fact]
        public void NotElidedNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(string), new [] { typeof(string) });
            e1.LoadArgument(0);
            e1.IsInstance<string>();
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate<Func<string, string>>(out instrs, Sigil.OptimizationOptions.None);

            Assert.Equal("hello", d1("hello"));

            Assert.True(instrs.Contains("isinst"));
        }

        [Fact]
        public void ElidedNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(string), new [] { typeof(string) });
            e1.LoadArgument(0);
            e1.IsInstance<string>();
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate<Func<string, string>>(out instrs);

            Assert.Equal("hello", d1("hello"));

            Assert.False(instrs.Contains("isinst"));
        }

        [Fact]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(string), new [] { typeof(object) });
            e1.LoadArgument(0);
            e1.IsInstance<string>();
            e1.Return();

            string instrs;
            var d1 = e1.CreateDelegate<Func<object, string>>(out instrs);

            Assert.Equal(null, d1(123));
            Assert.Equal("hello", d1("hello"));

            Assert.True(instrs.Contains("isinst"));
        }
    }
}
