using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace Sandbox.ModelDifferenceEngine.Tests
{
    public class snapshot_tests
    {
        [Fact]
        public void simple_object()
        {
            var data = new A {Name = "Name"};

            var result = ModelSnapshot.ToDictionary(data);

            Assert.Equal(2, result.Count());
            Assert.Equal(string.Empty, result.Keys.ElementAt(0));
            Assert.Equal(".Name", result.Keys.ElementAt(1));
        }

        [Fact]
        public void sub_object()
        {
            var data = new B
                {
                    A = new A {Name = "Name"}
                };

            var result = ModelSnapshot.ToDictionary(data);

            Assert.Equal(3, result.Count());
            Assert.Equal(string.Empty, result.Keys.ElementAt(0));
            Assert.Equal(".A", result.Keys.ElementAt(1));
            Assert.Equal(".A.Name", result.Keys.ElementAt(2));
        }

        [Fact]
        public void sub_collection()
        {
            var data = new C
                {
                    AList = new[] {new A {Name = "Name"}}
                };

            var result = ModelSnapshot.ToDictionary(data);

            Assert.Equal(4, result.Count());
            Assert.Equal(string.Empty, result.Keys.ElementAt(0));
            Assert.Equal(".AList", result.Keys.ElementAt(1));
            Assert.Equal(".AList[0]", result.Keys.ElementAt(2));
            Assert.Equal(".AList[0].Name", result.Keys.ElementAt(3));
        }

        [Fact]
        public void looping_hierarchy()
        {
            var data = new D {Name = "Parent"};
            data.Children = new[] {new D {Name = "Child", Parent = data}};

            var result = ModelSnapshot.ToDictionary(data);

            Assert.Equal(8, result.Count());
            Assert.Equal(string.Empty, result.Keys.ElementAt(0));
            Assert.Equal(".Name", result.Keys.ElementAt(1));
            Assert.Equal(".Parent", result.Keys.ElementAt(2));
            Assert.Equal(".Children", result.Keys.ElementAt(3));
            Assert.Equal(".Children[0]", result.Keys.ElementAt(4));
            Assert.Equal(".Children[0].Name", result.Keys.ElementAt(5));
            Assert.Equal(".Children[0].Parent", result.Keys.ElementAt(6));
            Assert.Equal(".Children[0].Children", result.Keys.ElementAt(7));
        }

        class A
        {
            public string Name { get; set; }
        }

        class B
        {
            public A A { get; set; }
        }

        class C
        {
            public IEnumerable<A> AList { get; set; }
        }

        class D
        {
            public string Name { get; set; }

            public D Parent { get; set; }
            public IEnumerable<D> Children { get; set; }
        }
    }
}