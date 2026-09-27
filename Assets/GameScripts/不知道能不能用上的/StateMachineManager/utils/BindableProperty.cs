

using System;
using System.Collections.Generic;


public class BindableProperty<T>
{
    private T mValue = default;
    public Action<T> OnValueChanged;

    public T Value
    {
        get => mValue;
        set
        {
            if (EqualityComparer<T>.Default.Equals(value, mValue))
                return;

            mValue = value;
            OnValueChanged?.Invoke(mValue);
        }
    }
}
