using System.ComponentModel;

namespace AutoMapperDirect.Execution
{
    public abstract class ProxyBase
    {
        protected void NotifyPropertyChanged(PropertyChangedEventHandler handler, string method)
        {
            handler?.Invoke(this, new PropertyChangedEventArgs(method));
        }
    }
}