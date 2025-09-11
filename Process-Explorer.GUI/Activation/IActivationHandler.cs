using System.Threading.Tasks;

namespace Process_Explorer.GUI.Activation;

public interface IActivationHandler
{
    bool CanHandle(object args);

    Task HandleAsync(object args);
}
