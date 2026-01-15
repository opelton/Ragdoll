using UnityEngine;
using UnityEngine.TestTools;
using Potato.Core;
using NUnit.Framework;

namespace Potato.Tests.EditMode
{
    public class IndirectVariableTests
    {
        void InstantiatesToZero<D, T>() where T : DataVariableBase
        {
            var dataVar = ScriptableObject.CreateInstance<T>();
            Assert.AreEqual(default(D), dataVar.ValueObject);
            Assert.AreEqual(default(D), dataVar.InitialValueObject); 
        }

        void SetValue<D, T>(D newValue) where T : DataVariableBase
        {
            var dataVar = ScriptableObject.CreateInstance<T>();
            dataVar.SetValue(newValue);
            dataVar.SetInitialValue(newValue);
            Assert.AreEqual(newValue, (D)dataVar.ValueObject);
            Assert.AreEqual(newValue, (D)dataVar.InitialValueObject);
        }

        void ResetValue<D, T>(D testValue) where T : DataVariableBase
        {
            var dataVar = ScriptableObject.CreateInstance<T>();
            dataVar.SetValue(default(D));
            dataVar.SetInitialValue(testValue);

            dataVar.ResetValue();

            Assert.AreEqual((D)dataVar.InitialValueObject, (D)dataVar.ValueObject);
        }

        // issue a warning and prevent the value from changing
        void ConstGuard<D, T>(D testValue) where T : DataVariableBase
        {
            var dataVar = ScriptableObject.CreateInstance<T>();
            dataVar.SetReadonly(true);

            LogAssert.Expect(LogType.Warning, $"Attempted to modify const DataVariable '{dataVar.name}'. Value unchanged.");

            dataVar.SetValueProperty(testValue);
            Assert.AreEqual(dataVar.InitialValueObject, dataVar.ValueObject);
        }

        // when setting const to true, auto-update the runtime value to the const one
        void ConstAppliesInitialValue<D, T>(D testValue) where T : DataVariableBase
        {
            var dataVar = ScriptableObject.CreateInstance<T>();
            dataVar.SetInitialValue(testValue);
            dataVar.SetReadonly(true);

            Assert.AreEqual(testValue, dataVar.ValueObject);
            Assert.AreEqual(testValue, dataVar.InitialValueObject);
        }

        // blank initialization
        [Test] public void InstantiatesToZero_IntVariable() => InstantiatesToZero<int, IntVariable>();
        [Test] public void InstantiatesToZero_FloatVariable() => InstantiatesToZero<float, FloatVariable>();
        [Test] public void InstantiatesToZero_BoolVariable() => InstantiatesToZero<bool, BoolVariable>();
        [Test] public void InstantiatesToZero_StringVariable() => InstantiatesToZero<string, StringVariable>();
        [Test] public void InstantiatesToZero_GameObmectVariable() => InstantiatesToZero<GameObject, GameObjectVariable>();
        [Test] public void InstantiatesToZero_TransformVariable() => InstantiatesToZero<Transform, TransformVariable>();

        // setters
        [Test] public void SetValueTest_IntVariable() => SetValue<int, IntVariable>(11);
        [Test] public void SetValueTest_FloatVariable() => SetValue<float, FloatVariable>(11f);
        [Test] public void SetValueTest_BoolVariable() => SetValue<bool, BoolVariable>(true);
        [Test] public void SetValueTest_StringVariable() => SetValue<string, StringVariable>("test!");

        // reset
        [Test] public void ResetValueTest_IntVariable() => ResetValue<int, IntVariable>(11);
        [Test] public void ResetValueTest_FloatVariable() => ResetValue<float, FloatVariable>(11f);
        [Test] public void ResetValueTest_BoolVariable() => ResetValue<bool, BoolVariable>(true);
        [Test] public void ResetValueTest_StringVariable() => ResetValue<string, StringVariable>("test!");

        // const guard
        [Test] public void ConstGuardTest_IntVariable() => ConstGuard<int, IntVariable>(11);
        [Test] public void ConstGuardTest_FloatVariable() => ConstGuard<float, FloatVariable>(11f);
        [Test] public void ConstGuardTest_BoolVariable() => ConstGuard<bool, BoolVariable>(true);
        [Test] public void ConstGuardTest_StringVariable() => ConstGuard<string, StringVariable>("test!");

        // const update
        [Test] public void ConstDefaultingTest_IntVariable() => ConstAppliesInitialValue<int, IntVariable>(11);
        [Test] public void ConstDefaultingTest_FloatVariable() => ConstAppliesInitialValue<float, FloatVariable>(11f);
        [Test] public void ConstDefaultingTest_BoolVariable() => ConstAppliesInitialValue<bool, BoolVariable>(true);
        [Test] public void ConstDefaultingTest_StringVariable() => ConstAppliesInitialValue<string, StringVariable>("test!");
    }
}