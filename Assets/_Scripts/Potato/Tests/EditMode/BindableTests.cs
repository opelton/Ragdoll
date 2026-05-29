using Potato.Core;
using NUnit.Framework;

namespace Potato.Tests.EditMode
{
    public class BindableTests
    {
        int sentinel = 0;

        void EventSentinel(int value)
        {
            sentinel = value;
        }

        [Test]
        public void InstantiatesToZero()
        {
            Bindable<int> testBindable = new();
            Assert.AreEqual(0, testBindable.Value);
        }

        [Test]
        public void ConstructorInitialized()
        {
            Bindable<int> testBindable = new(11);
            Assert.AreEqual(11, testBindable.Value);
        }

        [Test]
        public void SurvivesNullEvent()
        {
            Bindable<int> testBindable = new(11);
            Assert.DoesNotThrow(() =>
            {
                testBindable.Value = 15;
            });
            Assert.AreEqual(15, testBindable.Value);
        }

        [Test]
        public void FiresEvent()
        {
            Bindable<int> testBindable = new(11);

            sentinel = 0;
            testBindable.OnValueChanged += EventSentinel;
            Assert.DoesNotThrow(() =>
            {
                testBindable.Value = 15;
            });
            testBindable.OnValueChanged -= EventSentinel;

            Assert.AreEqual(15, sentinel);
        }
    }    
}
