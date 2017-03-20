using System.Threading.Tasks;

namespace MoqqerNamespace.Helpers
{
    public class TaskHelper
    {
        public static Task CompletedTask { get; } = FromResult(true);
        
        public static Task<T> FromResult<T>(T value)
        {
            var tcs = new TaskCompletionSource<T>();
            tcs.SetResult(value);
            return tcs.Task;
        }

    }
}
