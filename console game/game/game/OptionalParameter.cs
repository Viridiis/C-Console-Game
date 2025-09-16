using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public readonly ref struct OptionalParameter<T>
    {
        public bool HasValue { get; }
        public T Value { get; }

        public OptionalParameter(T value) { Value = value; HasValue = true; }

        public static implicit operator T(OptionalParameter<T> parameter) { return parameter.Value; }
        public static implicit operator OptionalParameter<T>(T parameter) { return new OptionalParameter<T>(parameter); }
    }
}
