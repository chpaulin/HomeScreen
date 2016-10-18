using Polly;
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
        public static async Task<PolicyResult<T>> ExecuteWebRequest<T>(Func<Task<T>> action)
        {
            return await Policy
               .Handle<WebException>()
               .WaitAndRetryAsync(new[]
               {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(30),
               }).ExecuteAndCaptureAsync<T>(action);
        }
    }
}
