using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextBrowser.UI
{
    internal class DelegateDisposable : IDisposable
    {
        private readonly Action onDispose;

        public DelegateDisposable(Action onDispose)
        {
            this.onDispose = onDispose;
        }

        public void Dispose()
        {
            if (onDispose != null)
                onDispose();
        }
    }
}
