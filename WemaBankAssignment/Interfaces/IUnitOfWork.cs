using System;
using System.Threading.Tasks;

namespace WemaBankAssignment.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IStateRepository StateRepository { get; }
        ILgaRepository LgaRepository { get; }
        Task Save();
    }
}
