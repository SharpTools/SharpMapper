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

            Mapper.Copy(foo, bar);
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

            Mapper.Copy(from, to);
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

            Mapper.Copy(from, to);
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

            Mapper.Copy(from, to);
            from.Foos.Add(new Foo());
            from.Foos[0].Age = 1000;

            Assert.Equal(from.Name, to.Name);
            Assert.Equal(from.Foos.Count, 3);
            Assert.Equal(to.Foos.Count, 2);

            Assert.NotEqual(from.Foos[0].Age, from.Foos[1].Age);
        }

        [Fact]
        public void Should_map_nullable_to_non_nullable() {
            var from = new WithNullable { Value = 10 };
            var to = Mapper.Map<WithNullable>(from);
            Assert.Equal(from.Value, to.Value);
        }

        [Fact]
        public void Should_map_nullable_null_to_non_nullable() {
            var from = new WithNullable { Value = null };
            var to = Mapper.Map<WithNullableTo>(from);
            Assert.Equal(0, to.Value);
        }

        [Fact]
        public void Should_map_lists() {
            var from = new List<Foo> {
                new Foo { Name="n1" },
                new Foo { Name="n2" }
            };
            var to = Mapper.Map<List<Foo>>(from);
            Assert.Equal(2, to.Count);
            Assert.Equal("n1", to[0].Name);
            Assert.Equal("n2", to[1].Name);

            to[0].Name = "asdf";
            Assert.Equal("n1", from[0].Name);
        }

        [Fact]
        public void Should_map_empty_lists() {
            var from = new List<Foo>();
            var to = Mapper.Map<List<Foo>>(from);
            Assert.Equal(0, to.Count);

            to.Add(new Foo());
            Assert.Empty(from);
        }

        [Fact]
        public void Should_map_lists_of_different_types() {
            var from = new List<Foo> {
                new Foo { Name="n1" },
                new Foo { Name="n2" }
            };

            var to = Mapper.Map<List<Bar>>(from);
            Assert.Equal(2, to.Count);
            Assert.Equal("n1", to[0].Name);
            Assert.Equal("n2", to[1].Name);

            to[0].Name = "asdf";
            Assert.Equal("n1", from[0].Name);
        }

        [Fact]
        public void Should_map_when_some_property_is_null() {
            var foo = new FooWithBar {
                Name = "foo",
                Age = 1
            };
            var copy = Mapper.Map<FooWithBar>(foo);
            Assert.Equal("foo", copy.Name);
            Assert.Equal(1, copy.Age);
            Assert.Null(foo.Bar);
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

    public class WithNullable {
        public decimal? Value { get; set; }
    }

    public class WithNullableTo {
        public decimal Value { get; set; }
    }

    public class FooWithBar : Foo {
        public Bar Bar { get; set; }
    }
}
