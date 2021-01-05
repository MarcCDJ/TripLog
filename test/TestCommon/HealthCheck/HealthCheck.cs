using System;
using FluentAssertions;
using Xunit;

namespace TestCommon.HealthCheck
{
    public class HealthCheck
    {
        [Fact]
        public void ThisShouldPass()
        {
            Assert.True(true);
        }

        [Fact]
        public void ThisShouldThrowAnException()
        {
            // Act
            Action act = () => throw new Exception("boom");

            // Assert
            act.Should().Throw<Exception>();
        }
    }
}
