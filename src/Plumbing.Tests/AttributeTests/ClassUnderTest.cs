using System;
using Plisky.Diagnostics;
using Plisky.Test;
using Xunit;

namespace Plisky.PliskyLibTests.AttributeTests {
    public class ClassUnderTest {
        protected Bilge b = new Bilge();
        public Action MethodCallback;

        [Trait("name", "value")]
        public void Trait() {
        }

        [Category("name")]
        public void Category() {
        }

        [Unit]
        public void Unit() {
        }

        [Integration]
        public void Integration() {
        }

        [Fresh]
        public void Fresh() {
        }

        [Isolated]
        public void Isolated() {
        }

        [Build(BuildType.Any)]
        public void Build() { }

        [Bug(123)]
        public void Bug() { }
    }
}
