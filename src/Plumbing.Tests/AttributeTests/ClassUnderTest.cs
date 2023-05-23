using System;
using Plisky.Diagnostics;
using Plisky.Test;
using Xunit;

namespace Plisky.PliskyLibTests.AttributeTests {

    public class ClassUnderTest {
        public Action MethodCallback;
        protected Bilge b = new Bilge();

        [Bug(123)]
        public void Bug() { }

        [Build(BuildType.Any)]
        public void Build() { }

        [Category("name")]
        public void Category() {
        }

        [Fresh]
        public void Fresh() {
        }

        [Integration]
        public void Integration() {
        }

        [Isolated]
        public void Isolated() {
        }

        [Trait("name", "value")]
        public void Trait() {
        }

        [Unit]
        public void Unit() {
        }
    }
}