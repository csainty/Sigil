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
    public partial class Throw
    {
        [Fact]
        public void Simple()
        {
            var e1 = Emit<Action>.NewDynamicMethod();
            e1.LoadConstant("Hello!");
            e1.NewObject<Exception, string>();
            e1.Throw();

            var d1 = e1.CreateDelegate();

            try
            {
                d1();
                Assert.True(false, "Expected exception was not thrown");
            }
            catch (Exception e)
            {
                Assert.Equal("Hello!", e.Message);
            }
        }
    }
}
