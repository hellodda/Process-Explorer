using System.Threading.Tasks;

namespace Process_Explorer.GUI.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
