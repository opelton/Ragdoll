using UnityEngine;
using UnityEngine.TestTools;
using Potato.Core;
using NUnit.Framework;

namespace Potato.Tests.EditMode
{
    public class DataReferenceTests
    {
        void InstantiatesToZero<D, T>() where T : DataReferenceBase, new()
        {
            T dataRef = new();
            Assert.IsTrue(dataRef.UseConstant);
            Assert.AreEqual(default(D), dataRef.GetValue());
            Assert.IsNull(dataRef.GetReference());
        }

        // dataRef should return the value it's assigned
        void ConstValueAssignment<D, T>(D testValue) where T : DataReferenceBase, new()
        {
            T dataRef = new();
            dataRef.SetValue(testValue);
            Assert.AreEqual(testValue, (D)dataRef.GetValue());
        }

        // just setting the reference should make dataRef's value equal dataVar's value
        void ValueLink_OneWay<D, T, R>(D testValue)
            where T : DataReferenceBase, new()
            where R : DataVariableBase, new()
        {
            T dataRef = new();
            R dataVar = ScriptableObject.CreateInstance<R>();
            dataVar.SetValue(testValue);
            dataRef.SetReference(dataVar);
            Assert.AreEqual((D)dataVar.GetValue(), (D)dataRef.GetValue());
            Assert.AreEqual(testValue, (D)dataRef.GetValue());
        }

        // changes should be mirrored
        void ValueLink_OneWay_Update<D, T, R>(D initialTestValue, D endTestValue)
            where T : DataReferenceBase, new()
            where R : DataVariableBase, new()
        {
            T dataRef = new();
            R dataVar = ScriptableObject.CreateInstance<R>();
            dataVar.SetValue(initialTestValue);
            dataRef.SetReference(dataVar);

            Assert.AreEqual(initialTestValue, (D)dataRef.GetValue());
            dataVar.SetValue(endTestValue);
            Assert.AreEqual(endTestValue, (D)dataRef.GetValue());
        }

        // multiple partners
        void ValueLink_MultiPartner_Update<D, T, R>(D initialTestValue, D endTestValue)
            where T : DataReferenceBase, new()
            where R : DataVariableBase, new()
        {
            T alice = new();
            T bob = new();
            R sharedData = ScriptableObject.CreateInstance<R>();

            // alice and bob should both get the initial value
            sharedData.SetValue(initialTestValue);
            alice.SetReference(sharedData);
            bob.SetReference(sharedData);
            Assert.AreEqual(initialTestValue, (D)alice.GetValue());
            Assert.AreEqual(initialTestValue, (D)bob.GetValue());

            // alice's change should reflect on everyone
            alice.SetValue(endTestValue);
            Assert.AreEqual(endTestValue, (D)alice.GetValue());
            Assert.AreEqual(endTestValue, (D)bob.GetValue());
            Assert.AreEqual(endTestValue, (D)sharedData.GetValue());

            // bob isn't ready to let the issue rest yet
            bob.SetValue(initialTestValue);
            Assert.AreEqual(initialTestValue, (D)alice.GetValue());
            Assert.AreEqual(initialTestValue, (D)bob.GetValue());
            Assert.AreEqual(initialTestValue, (D)sharedData.GetValue());

            // a neutral party has to step in
            sharedData.SetValue(endTestValue);
            Assert.AreEqual(endTestValue, (D)alice.GetValue());
            Assert.AreEqual(endTestValue, (D)bob.GetValue());
            Assert.AreEqual(endTestValue, (D)sharedData.GetValue());
        }

        void Value_Unlink<D, T, R>(D initialTestValue, D endTestValue)
            where T : DataReferenceBase, new()
            where R : DataVariableBase, new()
        {
            T alice = new();
            T bob = new();
            R sharedData = ScriptableObject.CreateInstance<R>();

            // same initial data
            sharedData.SetValue(initialTestValue);
            alice.SetReference(sharedData);
            bob.SetReference(sharedData);

            // all the same
            Assert.AreEqual(initialTestValue, (D)alice.GetValue());
            Assert.AreEqual(initialTestValue, (D)bob.GetValue());

            // bob unsubs and set a new value
            bob.ClearReference();
            bob.SetValue(endTestValue);

            // alice should still be on her previous value, bob is on the new one
            Assert.AreEqual(initialTestValue, (D)alice.GetValue());
            Assert.AreEqual(endTestValue, (D)bob.GetValue());
        }
        
        // initialize
        [Test] public void InitializationTest_Bool() => InstantiatesToZero<bool, BoolReference>();
        [Test] public void InitializationTest_Float() => InstantiatesToZero<float, FloatReference>();
        [Test] public void InitializationTest_GameObject() => InstantiatesToZero<GameObject, GameObjectReference>();
        [Test] public void InitializationTest_Int() => InstantiatesToZero<int, IntReference>();
        [Test] public void InitializationTest_String() => InstantiatesToZero<string, StringReference>();
        [Test] public void InitializationTest_Transform() => InstantiatesToZero<Transform, TransformReference>();

        // set const value
        [Test] public void ValueAssignmentTest_Bool() => ConstValueAssignment<bool, BoolReference>(true);
        [Test] public void ValueAssignmentTest_Float() => ConstValueAssignment<float, FloatReference>(11f);
        [Test] public void ValueAssignmentTest_Int() => ConstValueAssignment<int, IntReference>(11);
        [Test] public void ValueAssignmentTest_String() => ConstValueAssignment<string, StringReference>("test!");

        // one-way link value
        [Test] public void ValueLink_OneWay_Bool() => ValueLink_OneWay<bool, BoolReference, BoolVariable>(true);
        [Test] public void ValueLink_OneWay_Float() => ValueLink_OneWay<float, FloatReference, FloatVariable>(11f);
        [Test] public void ValueLink_OneWay_Int() => ValueLink_OneWay<int, IntReference, IntVariable>(11);
        [Test] public void ValueLink_OneWay_String() => ValueLink_OneWay<string, StringReference, StringVariable>("test!");

        // one-way link value updating
        [Test] public void ValueLink_OneWay_Update_Bool() => ValueLink_OneWay_Update<bool, BoolReference, BoolVariable>(true, false);
        [Test] public void ValueLink_OneWay_Update_Float() => ValueLink_OneWay_Update<float, FloatReference, FloatVariable>(11f, 22f);
        [Test] public void ValueLink_OneWay_Update_Int() => ValueLink_OneWay_Update<int, IntReference, IntVariable>(11, 22);
        [Test] public void ValueLink_OneWay_Update_String() => ValueLink_OneWay_Update<string, StringReference, StringVariable>("test!", "goodbye!");

        // multiple data references should share and receive changes from any source
        [Test] public void ValueLink_MultiPartner_Update_Bool() => ValueLink_MultiPartner_Update<bool, BoolReference, BoolVariable>(true, false);
        [Test] public void ValueLink_MultiPartner_Update_Float() => ValueLink_MultiPartner_Update<float, FloatReference, FloatVariable>(11f, 22f);
        [Test] public void ValueLink_MultiPartner_Update_Int() => ValueLink_MultiPartner_Update<int, IntReference, IntVariable>(11, 22);
        [Test] public void ValueLink_MultiPartner_Update_String() => ValueLink_MultiPartner_Update<string, StringReference, StringVariable>("test!", "goodbye!");

        // stop sharing once a link is broken
        [Test] public void ValueUnlink_Bool() => Value_Unlink<bool, BoolReference, BoolVariable>(true, false);
        [Test] public void ValueUnlink_Float() => Value_Unlink<float, FloatReference, FloatVariable>(11f, 22f);
        [Test] public void ValueUnlink_Int() => Value_Unlink<int, IntReference, IntVariable>(11, 22);
        [Test] public void ValueUnlink_String() => Value_Unlink<string, StringReference, StringVariable>("test!", "goodbye!");
    }
}