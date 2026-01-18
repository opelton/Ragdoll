using UnityEngine;
using UnityEngine.TestTools;
using Potato.Core;
using NUnit.Framework;
using System.Text.RegularExpressions;
using System.Collections;

namespace Potato.Tests.EditMode
{
    public class ScriptableEnumTests
    {
        [Test]
        public void InstanceUniqueness()
        {
            var dir1 = ScriptableObject.CreateInstance<CardinalEnum>();
            var dir2 = ScriptableObject.CreateInstance<CardinalEnum>();

            Assert.AreNotSame(dir1, dir2);
            Assert.AreNotEqual(dir1, dir2);
        }

        [Test]
        public void SelfEquality()
        {
            var dir = ScriptableObject.CreateInstance<CardinalEnum>();
            var dirRef = dir;

            Assert.AreEqual(dir, dirRef);
        }

        [Test]
        public void DataVariableUsage()
        {
            var data = ScriptableObject.CreateInstance<ScriptableEnumVariable>();
            var alice = ScriptableObject.CreateInstance<CardinalEnum>();
            var bob = ScriptableObject.CreateInstance<CardinalEnum>();

            ScriptableEnumReference dataRef = alice;
            data.Value = bob;
            Assert.AreEqual(alice, dataRef.Value);
            Assert.AreNotEqual(data.Value, dataRef.Value);

            dataRef.SetReference(data);
            Assert.AreEqual(data.Value, dataRef.Value);
            Assert.AreEqual(bob, dataRef.Value);
        }
    }
}