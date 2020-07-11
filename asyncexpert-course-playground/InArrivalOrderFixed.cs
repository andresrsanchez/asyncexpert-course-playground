using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace asyncexpert_course_playground
{
    public static class InArrivalOrderFixed
    {
        public static async Task DoWorkAsync()
        {
            var numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            foreach (var task in numbers.Select(x => CreateTask(x)).InCompletionOrder())
            {
                await task;
            }
            static async Task<int> CreateTask(int i)
            {
                await Task.Delay(1000);
                Console.WriteLine("Number: " + i);
                return i;
            }
        }

        //is not working¿?
        public static IEnumerable<Task<T>> InCompletionOrder<T>(this IEnumerable<Task<T>> tasks)
        {
            var inputs = tasks.ToList();
            var results = inputs.Select(i => new TaskCompletionSource<T>()).ToList();
            int index = -1;
            foreach (var task in inputs)
            {
                task.ContinueWith((t, state) =>
                {
                    var nextResult = results[Interlocked.Increment(ref index)];
                    nextResult.TrySetResult(t.Result);
                },
                TaskContinuationOptions.ExecuteSynchronously);
            }
            return results.Select(r => r.Task);
        }
    }
}
