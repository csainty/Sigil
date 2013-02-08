﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;
using System.Reflection.Emit;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        /// <summary>
        /// Expects a pointer, an initialization value, and a count on the stack.  Pops all three.
        /// 
        /// Writes the initialization value to count bytes at the passed pointer.
        /// </summary>
        public Emit<DelegateType> InitializeBlock(bool isVolatile = false, int? unaligned = null)
        {
            if (unaligned.HasValue && (unaligned != 1 && unaligned != 2 && unaligned != 4))
            {
                throw new ArgumentException("unaligned must be null, 1, 2, or 4");
            }

            if (!AllowsUnverifiableCIL)
            {
                throw new InvalidOperationException("InitializeBlock isn't verifiable");
            }

            var onStack = Stack.Top(3);

            if (onStack == null)
            {
                throw new SigilException("InitializeBlock expects three values to be on the stack", Stack);
            }

            var start = onStack[2];
            var init = onStack[1];
            var count = onStack[0];

            if (!start.IsPointer && !start.IsReference && start != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException("InitializeBlock expects the start value to be a pointer, reference, or native int; found " + start, Stack);
            }

            if (init != TypeOnStack.Get<int>() && init != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException("InitBlock expects the initial value to be an int or native int; found " + init, Stack);
            }

            if (count != TypeOnStack.Get<int>())
            {
                throw new SigilException("InitBlock expects the count to be an int; found " + count, Stack);
            }

            if(isVolatile)
            {
                UpdateState(OpCodes.Volatile);
            }

            if (unaligned.HasValue)
            {
                UpdateState(OpCodes.Unaligned, unaligned.Value);
            }

            UpdateState(OpCodes.Initblk, pop: 3);

            return this;
        }
    }
}
