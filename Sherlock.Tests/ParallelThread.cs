using System;
using System.Threading;

namespace Sherlock.Tests
{
    /// <summary>
    /// Since Parallel.Invoke does not guarantee the actions will
    /// run on separate threads, use this helper class. 
    /// </summary>
    internal static class ParallelThread
    {
        public static void Invoke(params Action[] actions)
        {
            foreach (var action in actions)
            {
                var thread = new Thread(new ThreadStart(action));
                thread.Start();
            }
        }
    }
}