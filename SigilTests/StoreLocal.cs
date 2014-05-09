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
    public partial class StoreLocal
    {
        [Fact]
        public void Int()
        {
            var e1 = Emit<Func<int>>.NewDynamicMethod("E1");

            var bar = e1.DeclareLocal<int>("bar");
            e1.LoadConstant(12);
            e1.LoadConstant(34);
            e1.Add();
            e1.StoreLocal(bar);
            e1.LoadLocal(bar);
            e1.Return();

            var del = e1.CreateDelegate();

            Assert.Equal(46, del());
        }
    }
}
