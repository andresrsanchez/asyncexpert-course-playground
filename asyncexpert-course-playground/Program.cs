using System;
using System.Threading;
using System.Threading.Tasks;

namespace asyncexpert_course_playground
{
    class Program
    {
        static void Main(string[] args)
        {
            DeadLock.Deadlock().Wait(); // never ends!!!
        }
    }

    public class DeadLock
    {
        private static ManualResetEventSlim _mutex = new ManualResetEventSlim();
        public static async Task Deadlock()
        {
            await ProcessAsync();
            Console.WriteLine("FIN: " + Environment.CurrentManagedThreadId);

            //Thread.Sleep(10000); // Don't let execute Console.WriteLine because is blocking the same thread continuation
            //await Task.Delay(10000); // Let the continuation flowing because we are not blocking 
            //_mutex.Wait(); //We are fucked up
        }
        private static Task ProcessAsync()
        {
            var tcs = new TaskCompletionSource<bool>();
            Task.Run(() =>
            {
                Thread.Sleep(1000); // Simulate some work
                tcs.SetResult(true); // Code awaiting on this task will resume on this thread and `Deadlock` continuation
                                     // will be inlined, so we are blocked on _mutex.Wait, while Set is the next line :(

                Console.WriteLine("END: " + Environment.CurrentManagedThreadId);
                //_mutex.Set();
            });
            return tcs.Task;
        }
    }
}
