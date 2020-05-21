using System;
using System.Threading.Tasks;
using Xunit;

namespace TestCommon.Unit
{
    public class HealthCheck
    {
        [Fact]
        public void ThisShouldPass()
        {
            Assert.True(true);
        }

        [Fact]
        public async Task ThisShouldFail()
        {
            await Task.Run(() => { throw new Exception("boom"); }).ConfigureAwait(false);
        }
    }
}
