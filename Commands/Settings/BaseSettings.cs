using System.CommandLine.Binding;

namespace TinyCity.Commands.Settings
{
    public abstract class BaseSettings<T> : BinderBase<T> where T : BaseSettings<T>
    {
        public bool Extra { get; set; }
    }
}