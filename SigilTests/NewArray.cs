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
    public partial class NewArray
    {
        [Fact]
        public void Simple()
        {
            var e1 = Emit<Func<int[]>>.NewDynamicMethod();
            e1.LoadConstant(128);
            e1.NewArray<int>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            var x = d1();

            Assert.Equal(128, x.Length);
        }
    }
}
