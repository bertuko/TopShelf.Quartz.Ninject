using System.Threading.Tasks;

namespace Topshelf.Common.Tests
{
    public class SampleDependency : ISampleDependency
    {
        public async Task<bool> DoWork()
        {
            await Task.Delay(1);
            return true;
        }
    }
}