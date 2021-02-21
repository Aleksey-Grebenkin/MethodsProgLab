using System;
using System.Collections;

namespace VirtualMemory
{
    public class Memory
    {
        private const ushort BufferSize = 5;

        private MemoryList[] buffer = new MemoryList[BufferSize];
        
        public Memory(ulong size, string filename = "vm.bin")
        {
            // тут должна быть проверка на существующую виртуальную память
            // если ее нет, создать
        }

        public ulong GetAddress(int index)
        {
            // получть абсолютный путь по индексу
            return 0;
        }
        
        public MemoryList this[int index]
        {
            get
            {
                // реализовать чтение из памяти
                return null;
            }
            set
            {
                // реализовать запись в память
                // с проверкой на блокировку
            }
        }
        
        
    }
}