using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreReady.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Completes when a certain number of a given collection of tasks has completed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasks"></param>
        /// <param name="count"></param>
        /// <param name="milliTimeout"></param>
        /// <param name="throwOnTimeout"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Task>> WhenSome(
            this IEnumerable<Task> tasks, 
            int count, 
            int milliTimeout = 5000, 
            bool throwOnTimeout = false)
        {
            var completed = new List<Task>();
            var uncompleted = tasks.ToList();
            var timeoutTask = Task.Delay(milliTimeout);

            for (var x = 0; x < count; x++)
            {
                var completedTask = await Task.WhenAny(uncompleted.Concat(new [] {timeoutTask}));

                if (completedTask == timeoutTask)
                {
                    if (throwOnTimeout)
                    {
                        throw new TimeoutException();
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    uncompleted.Remove(completedTask);
                    completed.Add(completedTask);
                }
            }

            return completed;
        }

        /// <summary>
        /// Completes when a certain number of a given collection of tasks has completed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasks"></param>
        /// <param name="count"></param>
        /// <param name="milliTimeout"></param>
        /// <param name="throwOnTimeout"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<Task<T>>> WhenSome<T>(
            this IEnumerable<Task<T>> tasks,
            int count,
            int milliTimeout = 5000,
            bool throwOnTimeout = false)
        {
            var completed = await WhenSome(tasks.Cast<Task>(), count, milliTimeout, throwOnTimeout);
            return completed.Cast<Task<T>>();
        }
    }
}