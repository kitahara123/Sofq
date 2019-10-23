using System.Threading.Tasks;

namespace Sofq
{
    public interface IServer
    {
        Task<string> DoSpin(int bet);
        Task<string> GetCurrentStats();
        void Restart();
    }
}