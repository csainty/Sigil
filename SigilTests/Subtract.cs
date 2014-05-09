using Sigil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SigilTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class Subtract
    {
        [Fact]
        public void Simple()
        {
            var e1 = Emit<Func<double, double, double>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.Subtract();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(3.14 - 1.59, d1(3.14, 1.59));
        }

        [Fact]
        public void Overflow()
        {
            var e1 = Emit<Func<double, double, double>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.SubtractOverflow();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(3.14 - 1.59, d1(3.14, 1.59));
        }

        [Fact]
        public void UnsignedOverflow()
        {
            var e1 = Emit<Func<double, double, double>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.LoadArgument(1);
            e1.UnsignedSubtractOverflow();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(3.14 - 1.59, d1(3.14, 1.59));
        }
    }
}
