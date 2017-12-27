using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SharpMapper.Tests {
    public class SharpMapperTests {
        [Fact]
        public void Should_copy_same_class() {
            var foo = new Foo { Name = "foo", Age = 1 };
            var bar = new Foo();

            SharpMapper.Copy(foo, bar);
            Assert.Equal(foo.Name, bar.Name);
            Assert.Equal(foo.Age, bar.Age);
        }

        [Fact]
        public void Should_copy_property_lists() {
            var from = new Bar {
                Name = "bar"
            };
            from.Foos.Add(new Foo { Name = "foo1", Age = 1 });
            from.Foos.Add(new Foo { Name = "foo2", Age = 2 });

            var to = new Bar();

            SharpMapper.Copy(from, to);
            Assert.Equal(from.Name, to.Name);

            Assert.Equal(2, to.Foos.Count);
            for (var i = 0; i < to.Foos.Count; i++) {
                Assert.Equal(from.Foos[i].Name, to.Foos[i].Name);
                Assert.Equal(from.Foos[i].Age, to.Foos[i].Age);
            }
        }

        [Fact]
        public void Should_copy_property_dictionaries() {
            var from = new WithDictionary {
                Name = "bar"
            };
            from.Dic.Add("foo1", new Foo { Name = "foo1", Age = 1 });
            from.Dic.Add("foo2", new Foo { Name = "foo2", Age = 2 });

            var to = new WithDictionary();

            SharpMapper.Copy(from, to);
            Assert.Equal(from.Name, to.Name);

            Assert.Equal(from.Dic.Count, to.Dic.Count);
            var keys = from.Dic.Keys.ToArray();
            for (var i = 0; i < keys.Length; i++) {
                Assert.Equal(from.Dic[keys[i]].Name, to.Dic[keys[i]].Name);
                Assert.Equal(from.Dic[keys[i]].Age, to.Dic[keys[i]].Age);
            }
        }

        [Fact]
        public void Should_copy_values_not_pointers() {
            var from = new Bar {
                Name = "bar"
            };
            from.Foos.Add(new Foo { Name = "foo1", Age = 1 });
            from.Foos.Add(new Foo { Name = "foo2", Age = 1 });

            var to = new Bar();

            SharpMapper.Copy(from, to);
            from.Foos.Add(new Foo());
            from.Foos[0].Age = 1000;

            Assert.Equal(from.Name, to.Name);
            Assert.Equal(from.Foos.Count, 3);
            Assert.Equal(to.Foos.Count, 2);

            Assert.NotEqual(from.Foos[0].Age, from.Foos[1].Age);
        }
    }

    public class Foo {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class Bar {
        public string Name { get; set; }
        public List<Foo> Foos { get; set; } = new List<Foo>();
    }

    public class WithDictionary {
        public string Name { get; set; }
        public Dictionary<string, Foo> Dic { get; set; } = new Dictionary<string, Foo>();
    }
}
