﻿using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HomeScreen.Common
{
    public static class PollyUtility
    {
        private static TimeSpan FIVE_MINUTES = TimeSpan.FromMinutes(5);

        public static async Task<PolicyResult<T>> ExecuteWebRequest<T>(Func<Task<T>> action)
        {
            return await Policy
               .Handle<WebException>()
               .WaitAndRetryForeverAsync((retryCount) => GetTimeout(retryCount))
               .ExecuteAndCaptureAsync<T>(action);
        }

        private static TimeSpan GetTimeout(int retryCount)
        {
            var timeout = TimeSpan.FromSeconds(retryCount * 5);

            return timeout < FIVE_MINUTES ? timeout : FIVE_MINUTES;
        }
    }
}
