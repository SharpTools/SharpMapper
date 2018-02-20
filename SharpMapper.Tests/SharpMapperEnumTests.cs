using Xunit;

namespace SharpMapper.Tests {
    public class SharpMapperEnumTests {
        [Fact]
        public void Should_map_enums() {
            var foo = new FooWithEnum {
                SuperEnum = SuperEnum.Foo
            };
            var copy = Mapper.Map<FooWithEnum>(foo);
            Assert.Equal(foo.SuperEnum, copy.SuperEnum);
        }

        [Fact]
        public void Should_map_enum_to_string() {
            var foo = new FooWithEnum {
                SuperEnum = SuperEnum.Bar
            };
            var copy = Mapper.Map<FooWithEnumAsString>(foo);
            Assert.Equal(SuperEnum.Bar.ToString(), copy.SuperEnum);
        }

        [Fact]
        public void Should_map_enum_to_int() {
            var foo = new FooWithEnum {
                SuperEnum = SuperEnum.Bar
            };
            var copy = Mapper.Map<FooWithEnumAsInt>(foo);
            Assert.Equal((int)SuperEnum.Bar, copy.SuperEnum);
        }

        [Fact]
        public void Should_map_string_to_enum() {
            var foo = new FooWithEnumAsString {
                SuperEnum = SuperEnum.Bar.ToString()
            };
            var copy = Mapper.Map<FooWithEnum>(foo);
            Assert.Equal(SuperEnum.Bar, copy.SuperEnum);
        }

        [Fact]
        public void Should_map_int_to_enum() {
            var foo = new FooWithEnumAsInt {
                SuperEnum = (int)SuperEnum.Bar
            };
            var copy = Mapper.Map<FooWithEnum>(foo);
            Assert.Equal(SuperEnum.Bar, copy.SuperEnum);
        }

        [Fact]
        public void Should_not_throw_when_mapping_unknown_value_for_enum() {
            var foo = new FooWithEnumAsString {
                SuperEnum = "asdf"
            };
            var copy = Mapper.Map<FooWithEnum>(foo);
            Assert.Equal(default(SuperEnum), copy.SuperEnum);
        }

        public enum SuperEnum {
            Foo = 1,
            Bar = 2
        }

        public class FooWithEnum {
            public SuperEnum SuperEnum { get; set; }
        }

        public class FooWithEnumAsString {
            public string SuperEnum { get; set; }
        }

        public class FooWithEnumAsInt {
            public int SuperEnum { get; set; }
        }
    }
}
