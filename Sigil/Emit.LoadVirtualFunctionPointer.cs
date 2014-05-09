using Sigil.Impl;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Pops an object reference off the stack, and pushes a pointer to the given method's implementation on that object.
        /// 
        /// For static or non-virtual functions, use LoadFunctionPointer
        /// </summary>
        public Emit<DelegateType> LoadVirtualFunctionPointer(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            if (method.IsStatic)
            {
                throw new ArgumentException("Only non-static methods can be passed to LoadVirtualFunctionPointer, found " + method);
            }

            var parameters = method.GetParameters();

            var declaring = method.DeclaringType;

            if (declaring.IsValueType)
            {
                declaring = declaring.MakePointerType();
            }

            var transitions = 
                new[]
                {
                    new StackTransition(new [] { declaring }, new [] { typeof(NativeIntType) })
                };

            UpdateState(OpCodes.Ldvirtftn, method, LinqAlternative.Select(parameters, p => p.ParameterType).AsEnumerable(), Wrap(transitions, "LoadVirtualFunctionPointer"));

            return this;
        }
    }
}
