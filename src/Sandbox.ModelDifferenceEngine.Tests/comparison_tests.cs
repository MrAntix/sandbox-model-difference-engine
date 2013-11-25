using System.Collections.Generic;
using System.Linq;

using Xunit;

namespace Sandbox.ModelDifferenceEngine.Tests
{
    public class comparison_tests
    {
        [Fact]
        public void no_comparison()
        {
            var data = new List<A> {new A {Name = "one"}, new A {Name = "two"}};

            var audit = new ModelDifference();
            var snapshot = audit.Snapshot(data);

            data.RemoveAt(0);
            data.Add(new A {Name = "one"});

            var result = snapshot.GetChanges(data);
            Output.WriteLine(result);

            Assert.Equal(2, result.Count());
            Assert.Equal("[0]", result.ElementAt(0).Path);
            Assert.Equal("[2]", result.ElementAt(1).Path);
        }

        [Fact]
        public void name_comparison()
        {
            var data = new List<A> {new A {Name = "one"}, new A {Name = "two"}};

            var audit = new ModelDifference()
                .RegisterComparison<A, string>(a => a.Name);

            var snapshot = audit.Snapshot(data);

            data.RemoveAt(0);
            data.Add(new A {Name = "one"});

            var result = snapshot.GetChanges(data);
            Output.WriteLine(result);

            Assert.Equal(0, result.Count());
        }

        [Fact]
        public void name_comparison_with_change()
        {
            var data = new List<A> {new A {Name = "one"}, new A {Name = "two"}};

            var audit = new ModelDifference()
                .RegisterComparison<A, string>(a => a.Name);

            var snapshot = audit.Snapshot(data);

            data.RemoveAt(0);
            data.Add(new A {Name = "one", Is = true});

            var result = snapshot.GetChanges(data);
            Output.WriteLine(result);

            Assert.Equal(1, result.Count());
            Assert.Equal("[0].Is", result.ElementAt(0).Path);
        }

        class A
        {
            public string Name { get; set; }
            public bool Is { get; set; }
        }
    }
}