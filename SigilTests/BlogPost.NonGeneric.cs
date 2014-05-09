
using Sigil;
using Sigil.NonGeneric;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SigilTests
{
    public partial class BlogPost
    {
        [Fact]
        public void Block1NonGeneric()
        {
            var il = Emit.NewDynamicMethod(typeof(int), Type.EmptyTypes, "AddOneAndTwo");
            il.LoadConstant(1);
            try
            {
                // Still missing that 2!
                il.Add();
                il.Return();
                var del = il.CreateDelegate<Func<int>>();
                del();

                Assert.True(false, "Expected exception was not thrown");
            }
            catch (SigilVerificationException e)
            {
                Assert.Equal("Add expects 2 values on the stack", e.Message);
            }
        }

        [Fact]
        public void Block2NonGeneric()
        {
            var il = Emit.NewDynamicMethod(typeof(string), new[] { typeof(string), typeof(Func<string, int>) }, "E1");
            var invoke = typeof(Func<string, int>).GetMethod("Invoke");
            try
            {
                var notNull = il.DefineLabel("not_null");

                il.LoadArgument(0);
                il.LoadNull();
                il.UnsignedBranchIfNotEqual(notNull);
                il.LoadNull();
                il.Return();


                il.MarkLabel(notNull);
                il.LoadArgument(1);
                il.LoadArgument(0);
                il.CallVirtual(invoke);
                il.Return();

                il.CreateDelegate<Func<string, Func<string, int>, string>>();

                Assert.True(false, "Expected exception was not thrown");
            }
            catch (SigilVerificationException e)
            {
                Assert.Equal("Return expected a System.String; found int", e.Message);
            }
        }
    }
}
