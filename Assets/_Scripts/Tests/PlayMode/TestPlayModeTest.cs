using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Potat.Tests.PlayMode
{
    public class TestPlayModeTest
    { 
        [UnityTest]
        public IEnumerator Foo()
        {
            Assert.AreEqual(true, true);
            yield return null;
        }
    }
}
