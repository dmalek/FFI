using System.Threading.Tasks;

namespace FFI.Base
{
    public interface IFFICommand
    {
        Task Execute();
    }

    public interface IFFICommand<TParameter>
    {
        Task Execute(TParameter param);
    }

    public interface IFFICommand<TParameter, TResult>
    {
        Task<TResult> Execute(TParameter param);
    }
}