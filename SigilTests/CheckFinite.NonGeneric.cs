
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SigilTests
{
    public partial class CheckFinite
    {
        [Fact]
        public void SimpleNonGeneric()
        {
            var e1 = Emit.NewDynamicMethod(typeof(void), new [] { typeof(double) }, "E1");
            e1.LoadArgument(0);
            e1.CheckFinite();
            e1.Pop();
            e1.Return();

            var d1 = e1.CreateDelegate<Action<double>>();

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
