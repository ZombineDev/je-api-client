using System;
using System.Linq;
using Reactive.Bindings;
using System.Reactive.Linq;

namespace UIClient.ViewModels
{
    public class Reactive<T> : Reactive.Bindings.ReactiveProperty<T>
    {
        public T InitialValue { get; }
        public ReadOnlyReactiveProperty<bool> Differs { get; private set; }

        public Reactive()
        {
            this.Differs = Observable.Return(false).ToReadOnlyReactiveProperty();
        }

        public static Reactive<U> FromInitialValue<U>(U initialValue) where U : T, IEquatable<T>
        {
            var res = new Reactive<U>(initialValue);
            res.Differs = res
                .Select(currentValue =>
                    !Object.ReferenceEquals(currentValue, initialValue) &&
                    !currentValue.Equals(res.InitialValue))
                .ToReadOnlyReactiveProperty();
            return res;
        }

        private Reactive(T initialValue) : base(initialValue) =>
            this.InitialValue = initialValue;
    }
}
