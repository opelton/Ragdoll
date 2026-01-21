using System;
using UnityEngine;

namespace Potato.Core
{
    [Serializable]
    public class Foo
    {
        public int foo;
        public int bar;
        public float baz;
        public string placeholder = "artie fufkin";
    }

    [CreateAssetMenu(menuName = "ScriptableObjects/DataVariables/Foo")]
    public class DataBagVariableTest : DataVariable<Foo> { }

    [Serializable]
    public class DataBagReference : DataReference<DataBagVariableTest, Foo>
    {
        public DataBagReference() : base() { }
        public DataBagReference(Foo value) : base(value) { }
        public DataBagReference(DataBagVariableTest referenceData) : base(referenceData) { }

        public static implicit operator DataBagReference(Foo value) => new(value);
        public static implicit operator DataBagReference(DataBagVariableTest referenceData) => new(referenceData);
    }
}