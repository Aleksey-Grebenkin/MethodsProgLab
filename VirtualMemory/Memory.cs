using System;
using System.IO;
using System.Collections;

namespace VirtualMemory
{
    public class Memory
    {
        private const ushort BufferSize = 5;

        private MemoryList[] buffer = new MemoryList[BufferSize];

        private FileStream fstream;

        public Memory(ulong size, string filename = "vm.bin")
        {
            // тут должна быть проверка на существующую виртуальную память
            // если ее нет, создать
            fstream = new FileStream(filename, FileMode.OpenOrCreate);
        }

        public ulong GetAddress(uint index)
        {
            return index * BufferSize + 2;
        }
        
        public MemoryList this[uint index]
        {
            get
            {
                uint minId = 0;
                DateTime minTime = DateTime.MaxValue;
                foreach (var list in buffer)
                {
                    if (list.Id == index)
                        return list;
                    if (list.Timestamp < minTime)
                    {
                        minTime = list.Timestamp;
                        minId = list.Id;
                    }
                }
                //сохраняем страницу, которую дольше всего не использовли
                //и подгружаем новую страницу на ее место
                fstream.Seek((long)GetAddress(minId), SeekOrigin.Begin);
                buffer[minId].Dump(fstream);
                fstream.Seek((long)GetAddress(index), SeekOrigin.Begin);
                buffer[minId] = new MemoryList(index, fstream);
                return buffer[minId];
            }
            set
            {
                if (value.Locked)
                    throw new MemoryLockedException("Запись заблокированна");
                fstream.Seek((long)GetAddress(index), SeekOrigin.Begin);
                value.Dump(fstream);
                // реализовать запись в память
                // с проверкой на блокировку
            }
        }
    }

    public class MemoryLockedException : Exception
    {
        public MemoryLockedException(string message) : base(message) { }
    }
}
