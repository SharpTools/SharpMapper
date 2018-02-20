using Xunit;

namespace SharpMapper.Tests {
    public class SharpMapperNullableTests {
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
        public void Should_map_nullable_null_to_nullable_null() {
            var from = new WithNullable { Value = null };
            var to = Mapper.Map<WithNullable>(from);
            Assert.Null(to.Value);
        }

        public class WithNullable {
            public decimal? Value { get; set; }
        }

        public class WithNullableTo {
            public decimal Value { get; set; }
        }
    }
}
