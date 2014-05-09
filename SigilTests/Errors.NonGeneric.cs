
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SigilTests
{
    public partial class Errors
    {
        [Fact]
        public void NotDelegateNonGeneric()
        {
            try
            {
                var emit = Emit.NewDynamicMethod(typeof(void), Type.EmptyTypes);
                emit.CreateDelegate(typeof(string));

                Assert.True(false, "Expected exception was not thrown");
            }
            catch (ArgumentException e)
            {
                Assert.Equal("delegateType must be a delegate, found System.String", e.Message);
            }
        }

        [Fact]
        public void WrongReturnTypeNonGeneric()
        {
            try
            {
                var emit = Emit.NewDynamicMethod(typeof(string), Type.EmptyTypes);

                emit.LoadConstant("hello world");
                emit.Return();

                emit.CreateDelegate(typeof(Func<int>));

                Assert.True(false, "Expected exception was not thrown");
            }
            catch (ArgumentException e)
            {
                Assert.Equal("Expected delegateType to return System.String, found System.Int32", e.Message);
            }
        }

        [Fact]
        public void WrongParameterTypesNonGeneric()
        {
            {
                try
                {
                    var emit = Emit.NewDynamicMethod(typeof(void), new[] { typeof(int), typeof(string) });

                    emit.Return();

                    emit.CreateDelegate(typeof(Action<int, int>));

                    Assert.True(false, "Expected exception was not thrown");
                }
                catch (ArgumentException e)
                {
                    Assert.Equal("Expected delegateType's parameter at index 1 to be a System.String, found System.Int32", e.Message);
                }
            }

            {
                try
                {
                    var emit = Emit.NewDynamicMethod(typeof(void), new[] { typeof(int) });

                    emit.Return();

                    emit.CreateDelegate(typeof(Action<int, int>));

                    Assert.True(false, "Expected exception was not thrown");
                }
                catch (ArgumentException e)
                {
                    Assert.Equal("Expected delegateType to take 1 parameters, found 2", e.Message);
                }
            }
        }
    }
}
