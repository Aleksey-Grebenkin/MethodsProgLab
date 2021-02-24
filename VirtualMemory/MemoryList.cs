using System;
using System.IO;

namespace VirtualMemory
{
    /// <summary>
    /// Класс блока в виртуальной памяти
    /// </summary>
    public class MemoryList
    {
        /// <summary>
        /// Размер блока
        /// </summary>
        private const int Size = 512;

        /// <summary>
        /// Размер блока в байтах
        /// </summary>
        public const int BinarySize = 1 + 1 + Size;

        /// <summary>
        /// Флаг модификации
        /// </summary>
        public bool Modified { get; private set; }

        /// <summary>
        /// Флаг блокировки записи
        /// </summary>
        public bool Locked;
        
        /// <summary>
        /// Время последнего изменения в файле
        /// </summary>
        public DateTime Timestamp { get; private set; } = DateTime.Now;

        /// <summary>
        /// Количество элементов в блоке, доступные для работы
        /// </summary>
        public const int Length = Size / sizeof(int);

        /// <summary>
        /// Номер страницы
        /// </summary>
        public readonly uint Id;

        /// <summary>
        /// Элементы
        /// </summary>
        private readonly int[] _values;

        /// <summary>
        /// Создание пустого блока
        /// </summary>
        /// <param name="id">Номер страницы</param>
        public MemoryList(uint id)
        {
            Id = id;
            _values = new int[Size / sizeof(int)];
        }

        /// <summary>
        /// Загрузка из потока и создание блока
        /// </summary>
        /// <param name="id">Номер страницы</param>
        /// <param name="streamReader">Поток</param>
        /// <exception cref="ArgumentException">Чтение из потока недоступно</exception>
        /// <exception cref="ArgumentOutOfRangeException">Некорректный поток</exception>
        public MemoryList(uint id, Stream streamReader) : this(id)
        {
            if (!streamReader.CanRead)
            {
                throw new ArgumentException("can't read data from a stream");
            }

            var binReader = new BinaryReader(streamReader);

            try
            {
                Modified = binReader.ReadBoolean();
                Locked = binReader.ReadBoolean();

                for (var i = 0; i < Length; i++)
                {
                    _values[i] = binReader.ReadInt32();
                }
            }
            catch (Exception)
            {
                throw new ArgumentOutOfRangeException("invalid stream for reading block");
            }
        }

        /// <summary>
        /// Обращение к элементам в блоке
        /// </summary>
        /// <param name="index">Индекс</param>
        /// <exception cref="MemoryListLockedException">Блок недоступен для записи</exception>
        public int this[int index]
        {
            get => _values[index];
            set
            {
                if (Locked)
                {
                    throw new MemoryListLockedException("Блок недоступен для записи.");
                }

                Modified = true;
                Timestamp = DateTime.Now;
                _values[index] = value;
            }
        }

        /// <summary>
        /// Запись блока в поток
        /// </summary>
        /// <param name="streamWriter">Поток, куда нужно записать</param>
        /// <exception cref="ArgumentException">В поток нельзя записывать данные</exception>
        public void Dump(Stream streamWriter)
        {
            if (!streamWriter.CanWrite)
            {
                throw new ArgumentException("Нельзя записывать данные в поток");
            }

            var binWriter = new BinaryWriter(streamWriter);
            
            binWriter.Write(Modified);
            binWriter.Write(Locked);

            foreach (var value in _values)
            {
                binWriter.Write(value);
            }
        }
}

    /// <summary>
    /// Исключение, которое выдается, если блок недоступен для записи
    /// </summary>
    public class MemoryListLockedException : Exception
    {
        public MemoryListLockedException(string message) : base(message) {}
    }
}