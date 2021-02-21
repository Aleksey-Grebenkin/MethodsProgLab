using System;
using System.IO;

namespace VirtualMemory
{
    public class MemoryList
    {
        private const uint Size = 512;

        // модифицировалась ли страница
        public bool Modified;

        // свободна ли для записи
        public bool Locked;

        // количество обращений
        public uint Counter;

        // время записи
        public uint Timestamp;

        // номер страиницы
        public readonly uint Id;

        // значения
        public int[] Values;

        public MemoryList(uint id, Stream streamReader)
        {
            var binReader = new BinaryReader(streamReader);
            Id = id;
            Values = new int[Size / sizeof(int)];

            try
            {
                Modified = binReader.ReadBoolean();
                Locked = binReader.ReadBoolean();
                Counter = (uint) binReader.ReadUInt32();
                Timestamp = binReader.ReadUInt32();
            }
            catch (Exception e)
            {
                Modified = false;
                Locked = false;
                Counter = (uint) 0;
                Timestamp = 0;
            }
        }
    }
}