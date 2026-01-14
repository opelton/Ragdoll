using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Potato.Core;

namespace Potato.Tests.EditMode
{
    public class IndirectVariableTests
    {
        // A Test behaves as an ordinary method
        [Test]
        public void IndirectVariableTestsSimplePasses()
        {
            var intVar = ScriptableObject.CreateInstance<IntVariable>();
            Assert.AreEqual(0, intVar.Value);
            Assert.AreEqual(0, intVar.InitialValue);
        }
    }
}