using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.APM.Extension.Component
{
    internal class CompositeDisposable01 : IDisposable
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private readonly object _lock = new object();
        private bool _isDisposed;

        /// <summary>
        /// Adds an instance of <see cref="T:System.IDisposable" /> to the collection
        /// </summary>
        /// <param name="disposable">A disposable</param>
        /// <returns>This instance of <see cref="T:Elastic.Apm.CompositeDisposable" /></returns>
        public CompositeDisposable01 Add(IDisposable disposable)
        {
            if (this._isDisposed)
                throw new ObjectDisposedException(nameof(CompositeDisposable01));
            lock (this._lock)
            {
                if (this._isDisposed)
                    throw new ObjectDisposedException(nameof(CompositeDisposable01));
                this._disposables.Add(disposable);
                return this;
            }
        }

        public void Dispose()
        {
            if (this._isDisposed)
                return;
            lock (this._lock)
            {
                if (this._isDisposed)
                    return;
                this._isDisposed = true;
                foreach (IDisposable disposable in this._disposables)
                    disposable?.Dispose();
            }
        }
    }
}
