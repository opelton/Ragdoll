using UnityEngine;
using UnityEngine.TestTools;
using Potato.Core;
using NUnit.Framework;
using System.Collections;

namespace Potato.Tests.PlayMode
{
    public class RuntimeSetMemberTests
    {
        GameObjectSet CreateSet() => ScriptableObject.CreateInstance<GameObjectSet>();

        GameObjectSetMember CreateMember(string name = "Test member")
        {
            GameObject obj = new(name);
            obj.SetActive(false);
            return obj.AddComponent<GameObjectSetMember>();
        }

        [UnityTest]
        public IEnumerator SingleAdd()
        {
            var set = CreateSet();
            var member = CreateMember();
            
            member.runtimeSet = set;
            member.gameObject.SetActive(true);
            Assert.AreEqual(1, set.Count);

            Object.DestroyImmediate(set);
            yield return null;
        }

        [UnityTest]
        public IEnumerator SingleRemove()
        {
            var set = CreateSet();
            var member = CreateMember();
            
            member.runtimeSet = set;
            member.gameObject.SetActive(true);
            member.gameObject.SetActive(false);
            Assert.AreEqual(0, set.Count);

            Object.DestroyImmediate(set);
            yield return null;
        }

        [UnityTest]
        public IEnumerator CorrectValue()
        {
            var set = CreateSet();
            var member = CreateMember();
            var obj = member.gameObject;
            
            member.runtimeSet = set;
            member.gameObject.SetActive(true);

            Assert.AreEqual(obj, set.Items[0]);

            Object.DestroyImmediate(set);
            yield return null;
        }

        [UnityTest]
        public IEnumerator AddRemoveAdd()
        {
            var set = CreateSet();
            var member = CreateMember();
            
            member.runtimeSet = set;
            member.gameObject.SetActive(true);
            member.gameObject.SetActive(false);
            member.gameObject.SetActive(true);

            Object.DestroyImmediate(set);
            yield return null;
        }
    }
}