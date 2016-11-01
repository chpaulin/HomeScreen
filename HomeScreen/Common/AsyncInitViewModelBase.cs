using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeScreen.Common
{
    public abstract class AsyncInitViewModelBase : ViewModelBase, IAsyncInit
    {
        public abstract Task Init();
    }
}
