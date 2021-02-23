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
        private const uint Size = 512;

        /// <summary>
        /// Флаг модификации
        /// </summary>
        public bool Modified => _modified;
        public bool _modified;

        /// <summary>
        /// Флаг блокировки записи
        /// </summary>
        public bool Locked;
        
        /// <summary>
        /// Время последнего изменения в файле
        /// </summary>
        public DateTime Timestamp => _timestamp;
        private DateTime _timestamp = DateTime.Now;

        /// <summary>
        /// Количество элементов в блоке, доступные для работы
        /// </summary>
        public const int Length = (int) Size / sizeof(int);

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
            _modified = false;
            Locked = false;
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
                _modified = binReader.ReadBoolean();
                Locked = binReader.ReadBoolean();

                for (var i = 0; i < Length; i++)
                {
                    _values[i] = binReader.ReadInt32();
                }
            }
            catch (Exception e)
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

                _modified = true;
                _timestamp = DateTime.Now;
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