using System.Runtime.CompilerServices;

namespace GHelper;

public static class AsyncHelpers
{
    public static void Forget(this Task task)
    {
        if (!task.IsCompleted || task.IsFaulted)
        {
            _ = ForgetAwaited(task);
        }
            
        static async Task ForgetAwaited(Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger.WriteLine(e.ToString());
            }
        }
    }
}