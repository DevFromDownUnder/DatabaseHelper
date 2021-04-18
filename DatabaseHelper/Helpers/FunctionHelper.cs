using System;
using System.Threading;

namespace DatabaseHelper.Helpers
{
    public class FunctionHelper
    {
        public static void DoNothing()
        {
        }

        public static void Wait5Seconds()
        {
            for (int i = 0; i < 5000; i++)
            {
                Thread.Sleep(1);
            }
        }

        public static void ConsumeException(Action action)
        {
            try
            {
                action();
            }
            catch { }
        }
    }
}