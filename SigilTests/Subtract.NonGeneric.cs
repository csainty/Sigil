
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SigilTests
{
    public partial class Subtract
    {
        [Fact]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(double), new [] { typeof(double), typeof(double) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Subtract();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<double, double, double>>();

            Assert.Equal(3.14 - 1.59, d1(3.14, 1.59));
        }

        [Fact]
        public void OverflowNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(double), new [] { typeof(double), typeof(double) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.SubtractOverflow();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<double, double, double>>();

            Assert.Equal(3.14 - 1.59, d1(3.14, 1.59));
        }

        [Fact]
        public void UnsignedOverflowNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(double), new [] { typeof(double), typeof(double) }, "E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedSubtractOverflow();
            e1.Return();

            var d1 = e1.CreateDelegate<Func<double, double, double>>();

            Assert.Equal(3.14 - 1.59, d1(3.14, 1.59));
        }
    }
}
