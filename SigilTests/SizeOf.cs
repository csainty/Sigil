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
    public partial class SizeOf
    {
        [Fact]
        public void Simple()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod("E1");
            e1.SizeOf<int>();
            e1.Return();

            var d1 = e1.CreateDelegate();

            Assert.Equal(sizeof(int), d1());
        }
    }
}
