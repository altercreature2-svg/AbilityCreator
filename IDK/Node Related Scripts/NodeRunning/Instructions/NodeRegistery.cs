using HarmonyLib;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDK.Node_Related_Scripts.NodeRunning.Instructions
{
    public class NodeRegistery
    {
        public struct Register
        {
            public int index;
        }
        public int MemoryAllocateSize { get; private set; }
        public int FreeMemory { get; private set; }
        public object[] memory;
        private int currentPointer;
        public NodeRegistery(int memoryAllocateSize = 1024)
        {
            this.currentPointer = 0;
            this.FreeMemory = memoryAllocateSize;
            this.MemoryAllocateSize = memoryAllocateSize;
            this.memory = new object[memoryAllocateSize];
        }
        public void AllocateMemory(int memorySize)
        {
            memory = memory.AddRangeToArray(new object[memorySize]);
        }
        public Register AllocateMemoryToRegister()
        {
            if (currentPointer == FreeMemory)
            {
                AllocateMemory(256);
            }
            FreeMemory -= 1;
            currentPointer += 1;
            Register register = new Register();
            register.index = currentPointer;
            return register;
        }
        public void StoreInRegister(Register register, object[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                memory[register.index] = values[i];
            }
        }
    }
}
