using System.Threading.Tasks;

namespace FFI.Base
{
    public interface IFFIProcess    
    {
        IFFILogger Logger { get; set; }
        Task Execute();
    }

    public interface IFFIProcess<TParam>
        where TParam : class
    {
        IFFILogger Logger { get; set; }
        Task Execute(TParam parameters);
    }
}
