using System.Threading;
using Cysharp.Threading.Tasks;

namespace Module.InteractiveEditor.Runtime
{
    public interface IActionTask
    {
        UniTask Execute(CancellationToken token);
        UniTask Undo(CancellationToken token);
    }
}