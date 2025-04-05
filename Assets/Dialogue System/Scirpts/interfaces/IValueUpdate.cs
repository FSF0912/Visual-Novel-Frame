using UnityEngine;

namespace FSF.VNG
{
    public interface IValueUpdate
    {
        public void UpdateValue(object value);
    }

    public interface IValueUpdate<T> : IValueUpdate
    {
        public void UpdateValue(T value);
    }
    public interface IValueUpdate<T1, T2> : IValueUpdate
    {
        public void UpdateValue(T1 value1, T2 value2);
    }
}