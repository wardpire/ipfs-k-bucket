﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Makaretu.Collections
{
    class Contact : IContact
    {
        public Contact(string id)
        {
            Id = Encoding.UTF8.GetBytes(id);
        }

        public Contact(int id)
        {
            Id = Encoding.UTF8.GetBytes(id.ToString(CultureInfo.InvariantCulture));
        }

        public byte[]Id { get; set; }

    }
    [TestClass]
    public class CollectionTest
    {
        [TestMethod]
        public void Add()
        {
            var bucket = new KBucket();
            var x = new Contact("1");
            bucket.Add(x);
            Assert.AreEqual(1, bucket.Count);
            Assert.IsTrue(bucket.Contains(x));
        }

        [TestMethod]
        public void AddDuplicate()
        {
            var bucket = new KBucket();
            var x = new Contact("1");
            bucket.Add(x);
            bucket.Add(x);
            Assert.AreEqual(1, bucket.Count);
            Assert.IsTrue(bucket.Contains(x));
        }

        [TestMethod]
        public void Count()
        {
            var bucket = new KBucket();
            Assert.AreEqual(0, bucket.Count);

            bucket.Add(new Contact("a"));
            bucket.Add(new Contact("a"));
            bucket.Add(new Contact("a"));
            bucket.Add(new Contact("b"));
            bucket.Add(new Contact("b"));
            bucket.Add(new Contact("c"));
            bucket.Add(new Contact("d"));
            bucket.Add(new Contact("c"));
            bucket.Add(new Contact("d"));
            bucket.Add(new Contact("e"));
            bucket.Add(new Contact("f"));
            bucket.Add(new Contact("a"));
            Assert.AreEqual(6, bucket.Count);
        }

        [TestMethod]
        public void Clear()
        {
            var bucket = new KBucket();
            Assert.AreEqual(0, bucket.Count);

            bucket.Add(new Contact("a"));
            bucket.Add(new Contact("b"));
            bucket.Add(new Contact("c"));
            Assert.AreEqual(3, bucket.Count);

            bucket.Clear();
            Assert.AreEqual(0, bucket.Count);
        }

        [TestMethod]
        public void Remove()
        {
            var bucket = new KBucket();
            Assert.AreEqual(0, bucket.Count);

            bucket.Add(new Contact("a"));
            bucket.Add(new Contact("b"));
            bucket.Add(new Contact("c"));
            Assert.AreEqual(3, bucket.Count);

            bucket.Remove(new Contact("b"));
            Assert.AreEqual(2, bucket.Count);
        }

        [TestMethod]
        public void CopyTo()
        {
            var bucket = new KBucket();
            Assert.AreEqual(0, bucket.Count);

            bucket.Add(new Contact("a"));
            bucket.Add(new Contact("b"));
            bucket.Add(new Contact("c"));
            Assert.AreEqual(3, bucket.Count);

            var array = new Contact[bucket.Count + 2];
            bucket.CopyTo(array, 1);
            Assert.IsNull(array[0]);
            Assert.IsNotNull(array[1]);
            Assert.IsNotNull(array[2]);
            Assert.IsNotNull(array[3]);
            Assert.IsNull(array[4]);
        }

        [TestMethod]
        public void Enumerate()
        {
            var bucket = new KBucket();
            var nContacts = 40;
            for (var i = 0; i < nContacts; ++i)
            {
                bucket.Add(new Contact(i));
            }
            Assert.AreEqual(nContacts, bucket.Count);

            int n = 0;
            foreach (var contact in bucket)
            {
                ++n;
            }
            Assert.AreEqual(n, nContacts);
        }

        [TestMethod]
        public void CanBeModified()
        {
            Assert.IsFalse(new KBucket().IsReadOnly);
        }

        [TestMethod]
        public async Task ThreadSafe()
        {
            var bucket = new KBucket();
            var nContacts = 1000;
            var nTasks = 100;
            var tasks = new Task[nTasks];

            for (var i = 0; i < nTasks; ++i)
            {
                tasks[i] = new Task(() => AddTask(bucket, i, nContacts));
                tasks[i].Start();
            }
            await Task.WhenAll(tasks);

            Assert.AreEqual(nTasks * nContacts, bucket.Count);
        }

        public void AddTask(KBucket bucket, int start, int count)
        {
            for (var i = 0; i < count; ++i)
            {
                bucket.Add(new Contact(start + i));
            }
        }
    }
}