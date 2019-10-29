using System.Threading.Tasks;

namespace Topshelf.Common.Tests
{
    public interface ISampleDependency
    {
        Task<bool> DoWork();
    }
}