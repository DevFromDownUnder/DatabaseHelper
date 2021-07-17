using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace DatabaseHelper.Extensions
{
    public static class BackgroundWorkerExtensions
    {
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public static void ReportProgress(this BackgroundWorker worker, object? userState)
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        {
            worker.ReportProgress(0, userState);
        }

    }
}