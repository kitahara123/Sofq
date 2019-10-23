using System.Threading.Tasks;

namespace Sofq
{
    public interface IServer
    {
        Task<string> DoSpin(int bet);
    }
}