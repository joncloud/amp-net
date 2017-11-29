using System.Threading.Tasks;

namespace Amp
{
    public interface IWarmUp
    {
        Task InvokeAsync();
    }
}
