using System;
using System.IO;
using NUnit.Framework;
using VirtualMemory;

namespace VirtualMemoryTests
{
    /// <summary>
    /// Проверка MemoryList
    /// </summary>
    public class MemoryListTests
    {
        private MemoryList _mlEmpty;
        private MemoryList _mlSample;

        /// <summary>
        /// Инициализация тестовых объектов
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _mlEmpty = new MemoryList(0) {Locked = true};
            _mlSample = new MemoryList(1);

            var rnd = new Random();

            for (var i = 0; i < MemoryList.Length; i++)
            {
                if (rnd.Next() % 2 == 0)
                {
                    _mlSample[i] = rnd.Next();
                }
            }

            _mlSample.Locked = true;
        }

        /// <summary>
        /// Проверка на запись в поток
        /// </summary>
        [Test]
        public void WriteToStream()
        {
            using var memoryStream = new MemoryStream();
            _mlEmpty.Dump(memoryStream);
            Assert.NotZero(memoryStream.Length);
        }

        /// <summary>
        /// Проверка на загрузку из потока
        /// </summary>
        [Test]
        public void LoadFromStream()
        {
            using var memoryStream = new MemoryStream();
            _mlSample.Dump(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);
            var mlTest = new MemoryList(1, memoryStream);

            Assert.AreEqual(_mlSample.Modified, mlTest.Modified);
            Assert.AreEqual(_mlSample.Locked, mlTest.Locked);

            for (var i = 0; i < MemoryList.Length; i++)
            {
                Assert.AreEqual(_mlSample[i], mlTest[i]);
            }
        }

        /// <summary>
        /// Проверка на изменение времени
        /// </summary>
        [Test]
        public void TimestampCheck()
        {
            var mlTest = new MemoryList(0);
            var ts = mlTest.Timestamp;
            mlTest[0] = 1;
            Assert.Greater(mlTest.Timestamp, ts);
        }

        /// <summary>
        /// Проверка флага блокировки записи
        /// </summary>
        [Test]
        public void CheckLocked()
        {
            var mlTest = new MemoryList(0) {Locked = false, [0] = 1};
            Assert.AreEqual(1, mlTest[0]);
            mlTest.Locked = true;
            Assert.Catch<MemoryListLockedException>(() => mlTest[0] = 0);
        }

        /// <summary>
        /// Проверка флага модификации
        /// </summary>
        [Test]
        public void CheckModified()
        {
            var mlTest = new MemoryList(0);
            Assert.IsFalse(mlTest.Modified);
            mlTest[0] = 1;
            Assert.IsTrue(mlTest.Modified);
        }
}
}