using System;
using System.Threading.Tasks;

namespace EntityDynamicAttributes;

internal static class DelegateExtensions
{
    public static async Task<object?> DynamicInvokeAdvanced(this Delegate fn, params object?[] args)
    {
        var val = fn.DynamicInvoke(args);

        if (val is not Task task)
            return val;

        await task.ConfigureAwait(false);

        var taskType = task.GetType();

        if (taskType.IsGenericType && taskType.GenericTypeArguments[0].Name == "VoidTaskResult")
            return null;

        return taskType.GetProperty(nameof(Task<int>.Result))?.GetValue(task);
    }
}
