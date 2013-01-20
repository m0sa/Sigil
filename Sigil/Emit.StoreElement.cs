﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Sigil.Impl;

namespace Sigil
{
    public partial class Emit<DelegateType>
    {
        public void StoreElement()
        {
            var onStack = Stack.Top(3);

            if (onStack == null)
            {
                throw new SigilException("StoreElement expects three parameters to be on the stack", Stack);
            }

            var value = onStack[0];
            var index = onStack[1];
            var arr = onStack[2];

            if (arr.IsPointer || arr.IsReference || !arr.Type.IsArray || arr.Type.GetArrayRank() != 1)
            {
                throw new SigilException("StoreElement expects a rank one array, found " + arr, Stack);
            }

            if (index != TypeOnStack.Get<int>() && index != TypeOnStack.Get<NativeInt>())
            {
                throw new SigilException("StoreElement expects an index of type int or native int, found " + index, Stack);
            }

            var elemType = arr.Type.GetElementType();

            if (!elemType.IsAssignableFrom(value))
            {
                throw new SigilException("StoreElement expects a value assignable to " + elemType + ", found " + value, Stack);
            }

            OpCode? instr = null;

            if (elemType.IsPointer)
            {
                instr = OpCodes.Stelem_I;
            }

            if (!elemType.IsValueType)
            {
                instr = OpCodes.Stelem_Ref;
            }

            if (elemType == typeof(sbyte) || elemType == typeof(byte))
            {
                instr = OpCodes.Stelem_I1;
            }

            if (elemType == typeof(short) || elemType == typeof(ushort))
            {
                instr = OpCodes.Stelem_I2;
            }

            if (elemType == typeof(int) || elemType == typeof(uint))
            {
                instr = OpCodes.Stelem_I4;
            }

            if (elemType == typeof(long) || elemType == typeof(ulong))
            {
                instr = OpCodes.Stelem_I8;
            }

            if (elemType == typeof(float))
            {
                instr = OpCodes.Stelem_R4;
            }

            if (elemType == typeof(double))
            {
                instr = OpCodes.Stelem_R8;
            }

            if (!instr.HasValue)
            {
                throw new Exception("Couldn't infer proper Stelem* opcode from " + elemType);
            }

            UpdateState(instr.Value, pop: 3);
        }
    }
}
