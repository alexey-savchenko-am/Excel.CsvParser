using System;
using System.Collections.Generic;
using System.Text;

namespace UnitTests
{
    public abstract class CleanableTest
        : IDisposable
    {
        protected abstract void CleanUp();


        void IDisposable.Dispose()
        {
            CleanUp();
        }
    }
}
