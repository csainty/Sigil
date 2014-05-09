using Sigil;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SigilTests
{
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public partial class CheckFinite
    {
        [Fact]
        public void Simple()
        {
            var e1 = Emit<Action<double>>.NewDynamicMethod("E1");
            e1.LoadArgument(0);
            e1.CheckFinite();
            e1.Pop();
            e1.Return();

            var d1 = e1.CreateDelegate();

            d1(123);

            try
            {
                d1(double.PositiveInfinity);
                Assert.True(false, "Should have thrown");
            }
            catch (ArithmeticException e)
            {
                Assert.NotNull(e);
            }
        }
    }
}
