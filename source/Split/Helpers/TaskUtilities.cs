using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Split.Helpers
{
    public static class TaskUtilities
    {
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        public static async void FireAndForgetSafeAsync(this Task task, Action<Exception> handleErrorAction = null)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        {
            try
            {
                await task.ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                handleErrorAction?.Invoke(ex);
            }
        }
    }
}

