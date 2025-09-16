using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSystem.Runtime;

namespace WSystem.Collections
{
    public sealed class InvocationList : IReadOnlyCollection<FWeakDelegate>
    {
        private InvocationEntry _start;
        private InvocationEntry _end;
        private int _count;

        public FWeakDelegate this[int index] 
        { 
            get
            {
                if((uint)index >= (uint)_count) throw new IndexOutOfRangeException();
                int i = 0;
                InvocationEntry current = _start;
                while(i < index)
                {
                    current = current.Next;
                    i++;
                }
                return current.WeakDelegate;
            }
        }

        public int Count => _count;

        public void Add(Delegate dlg, Type delegateInputTargetCastType)
        {
            if (dlg is null) return;
            if (ContainsAndCleanUp(dlg)) return;
            AddCore(FWeakDelegate.CreateFrom(dlg, delegateInputTargetCastType));
        }

        public void Add(FWeakDelegate weakDelegate)
        {
            if (!weakDelegate.IsValid()) return;
            if (ContainsAndCleanUp(weakDelegate)) return;
            AddCore(weakDelegate);
        }

        private void AddCore(FWeakDelegate dlg)
        {
            _count++;
            InvocationEntry current = new InvocationEntry() { WeakDelegate = dlg };
            if(_start is null)
            {
                _start = current;
                _end = current;
                return;
            }
            _end.Next = current;
            current.Previous = _end;
            _end = current;
        }

        private bool ContainsAndCleanUp(Delegate dlg)
        {
            if (_count <= 0 || dlg is null) return false;
            InvocationEntry current = _start;
            while (current is not null)
            {
                if (current.IsValid())
                {
                    if (current.WeakDelegate.Equals(dlg)) return true;
                    current = current.Next;
                    continue;
                }
                _count--;
                if (current == _start) _start = current.Next;
                if (current == _end) _end = current.Previous;
                if (current.Next is not null) current.Next.Previous = current.Previous;
                if (current.Previous is not null) current.Previous.Next = current.Next;
                InvocationEntry toDispose = current;
                current = current.Next;
                toDispose.Dispose();
            }
            return false;
        }

        private bool ContainsAndCleanUp(FWeakDelegate dlg)
        {
            if (_count <= 0 || !dlg.IsValid()) return false;
            InvocationEntry current = _start;
            while (current is not null)
            {
                if (current.IsValid())
                {
                    if (current.WeakDelegate.Equals(dlg)) return true;
                    current = current.Next;
                    continue;
                }
                _count--;
                if (current == _start) _start = current.Next;
                if (current == _end) _end = current.Previous;
                if (current.Next is not null) current.Next.Previous = current.Previous;
                if (current.Previous is not null) current.Previous.Next = current.Next;
                InvocationEntry toDispose = current;
                current = current.Next;
                toDispose.Dispose();
            }
            return false;
        }

        public void CleanUp()
        {
            if(_count <= 0) return;
            InvocationEntry current = _start;
            while(current is not null)
            {
                if (current.IsValid())
                {
                    current = current.Next;
                    continue;
                }
                _count--;
                if (current == _start) _start = current.Next;
                if (current == _end) _end = current.Previous;
                if (current.Next is not null) current.Next.Previous = current.Previous;
                if(current.Previous is not null) current.Previous.Next = current.Next;
                InvocationEntry toDispose = current;
                current = current.Next;
                toDispose.Dispose();
            }
        }

        public void Clear()
        {
            if(_count <= 0)
            {
                Debug.Assert(_start is null);
                Debug.Assert(_end is null);
                return;
            }
            _count = 0;
            InvocationEntry current = _start;
            _start = null;
            _end = null;
            while(current is not null)
            {
                InvocationEntry toDispose = current;
                current = toDispose.Next;
                toDispose.Dispose();
            }
        }

        public bool Contains(Delegate dlg)
        {
            return IndexOf(dlg) >= 0;
        }

        public bool Contains(FWeakDelegate fwdlg)
        {
            return IndexOf(fwdlg) >= 0;
        }

        public int IndexOf(Delegate dlg)
        {
            if (_count <= 0 || dlg is null) return -1;
            int i = 0;
            InvocationEntry current = _start;
            while(current is not null)
            {
                if (current.WeakDelegate.Equals(dlg)) return i;
                current = current.Next;
                i++;
            }
            return -1;
        }

        public int IndexOf(FWeakDelegate fwdlg)
        {
            if (_count <= 0 || fwdlg.Delegate is null) return -1;
            int i = 0;
            InvocationEntry current = _start;
            while (current is not null)
            {
                if (current.WeakDelegate.Equals(fwdlg)) return i;
                current = current.Next;
                i++;
            }
            return -1;
        }

        public bool Remove(Delegate dlg, bool cleanList = false)
        {
            if (_count <= 0 || dlg is null) return false;
            bool keepGoing = true;
            bool found = false;
            InvocationEntry current = _start;
            while (current is not null && keepGoing)
            {
                if (current.IsValid())
                {
                    if(!found)
                    {
                        if (current.WeakDelegate.Equals(dlg))
                        {
                            keepGoing = cleanList;
                            found = true;
                            goto Remove;
                        }
                    }
                    current = current.Next;
                    continue;
                }
                Remove:
                _count--;
                if (current == _start) _start = current.Next;
                if (current == _end) _end = current.Previous;
                if (current.Next is not null) current.Next.Previous = current.Previous;
                if (current.Previous is not null) current.Previous.Next = current.Next;
                InvocationEntry toDispose = current;
                current = current.Next;
                toDispose.Dispose();
            }
            return found;
        }

        public bool Remove(FWeakDelegate fwdlg, bool cleanList = false)
        {
            if (_count <= 0 || !fwdlg.IsValid()) return false;
            bool keepGoing = true;
            bool found = false;
            InvocationEntry current = _start;
            while (current is not null && keepGoing)
            {
                if (current.IsValid())
                {
                    if (!found)
                    {
                        if (current.WeakDelegate.Equals(fwdlg))
                        {
                            keepGoing = cleanList;
                            found = true;
                            goto Remove;
                        }
                    }
                    current = current.Next;
                    continue;
                }
            Remove:
                _count--;
                if (current == _start) _start = current.Next;
                if (current == _end) _end = current.Previous;
                if (current.Next is not null) current.Next.Previous = current.Previous;
                if (current.Previous is not null) current.Previous.Next = current.Next;
                InvocationEntry toDispose = current;
                current = current.Next;
                toDispose.Dispose();
            }
            return found;
        }

        public Enumerator GetEnumerator() => GetEnumerator(true);
        public Enumerator GetEnumerator(bool CleanUpDuringIteration)
        {
            return new Enumerator(this, CleanUpDuringIteration);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator(false);
        }

        IEnumerator<FWeakDelegate> IEnumerable<FWeakDelegate>.GetEnumerator()
        {
            return GetEnumerator(false);
        }

        public struct Enumerator : IEnumerator<FWeakDelegate>
        {
            InvocationList _list;
            InvocationEntry _current;
            bool reachedEnd;
            public bool CleanUpDuringIteration { get; set; }
            public FWeakDelegate Current
            {
                get
                {
                    if (_current is null) return default;
                    return _current.WeakDelegate;
                }
            }

            object IEnumerator.Current => Current;

            public Enumerator(InvocationList list) : this(list, true) { }
            public Enumerator(InvocationList list, bool cleanUpDuringIteration)
            {
                ArgumentNullException.ThrowIfNull(list);
                _list = list;
                _current = null;
                CleanUpDuringIteration = cleanUpDuringIteration;
                reachedEnd = false;
            }

            public void Dispose()
            {
                _list = null;
                _current = null;
            }

            public bool MoveNext()
            {
                if(reachedEnd) return false;
                if(_list.Count <= 0)
                {
                    reachedEnd = true;
                    return false;
                }
                if (_current is null)
                {
                    _current = _list._start;
                }
                else _current = _current.Next;
                while (_current is not null)
                {
                    if (_current.IsValid()) return true;
                    if(!CleanUpDuringIteration)
                    {
                        _current = _current.Next;
                        continue;
                    }
                    _list._count--;
                    if (_current == _list._start) _list._start = _current.Next;
                    if (_current == _list._end) _list._end = _current.Previous;
                    if (_current.Next is not null) _current.Next.Previous = _current.Previous;
                    if (_current.Previous is not null) _current.Previous.Next = _current.Next;
                    InvocationEntry toDispose = _current;
                    _current = _current.Next;
                    toDispose.Dispose();
                }
                reachedEnd = true;
                return false;
            }

            public void Reset()
            {
                _current = null;
                reachedEnd = false;
            }
        }
    }

    public sealed class InvocationEntry
    {
        public InvocationEntry Previous { get; internal set; }
        public InvocationEntry Next { get; internal set; }
        public FWeakDelegate WeakDelegate { get; internal set; }
        internal void Dispose()
        {
            Previous = null;
            Next = null;
            WeakDelegate = default;
        }
        public bool IsValid()
        {
            return WeakDelegate.IsValid();
        }
    }

    public readonly struct FWeakDelegate : IEquatable<FWeakDelegate>, IEquatable<Delegate>
    {
        public WeakReference<object> InvocationTarget { get; }
        public Delegate Delegate { get; }

        private FWeakDelegate(object target, Delegate nullTargetDelegate)
        {
            Debug.Assert(nullTargetDelegate is not null);
            if (target is not null)
            {
                InvocationTarget = new WeakReference<object>(target);
            }
            else InvocationTarget = null;
            Delegate = nullTargetDelegate;
        }

        public bool Equals(FWeakDelegate other)
        {
            if(InvocationTarget is null)
            {
                if (other.InvocationTarget is not null) return false;
            }
            else
            {
                if (other.InvocationTarget is null) return false;
                InvocationTarget.TryGetTarget(out object thisTarget);
                other.InvocationTarget.TryGetTarget(out object otherTarget);
                if (!object.ReferenceEquals(thisTarget, otherTarget)) return false;
            }
            return (Delegate)Delegate == other.Delegate;
        }

        public bool Equals(Delegate other)
        {
            if (other is null) return false;
            if(other.Target is null)
            {
                if (InvocationTarget is not null) return false;
            }
            else
            {
                if (InvocationTarget is null) return false;
                InvocationTarget.TryGetTarget(out object thisTarget);
                if(other.Target != thisTarget) return false;
            }
            if (ReferenceEquals(Delegate, other)) return true;
            if (InvocationTarget is null) return (Delegate)Delegate == other;
            return DelegateData.EventDelegateEquals(Delegate, other);
        }

        public static FWeakDelegate CreateFrom(Delegate dlg, Type delegateInputTargetCastType)
        {
            ArgumentNullException.ThrowIfNull(dlg);
            if (dlg.Target is null) return new FWeakDelegate(null, dlg);
            if (CLR.GetHeapSizeOfType(dlg.GetType()) != CLR.GetHeapSizeOfType(delegateInputTargetCastType)) throw new ArgumentException("Size of target cast type does not match with given delegate type.", nameof(delegateInputTargetCastType));
            object target = dlg.Target;
            Type ditctDefinition = delegateInputTargetCastType.GetGenericTypeDefinition();
            ArgumentNullException.ThrowIfNull(ditctDefinition);
            Type[] gArgs = new Type[ditctDefinition.GetGenericArguments().Length];
            delegateInputTargetCastType.GetGenericArguments().CopyTo(gArgs, 0);
            gArgs[0] = target.GetType(); //Can probably be replaced with gArgs[0] = typeof(object) or just removed entirely.
            dlg = (Delegate)dlg.Method.CreateDelegate(ditctDefinition.MakeGenericType(gArgs));
            return new FWeakDelegate(target, dlg);
        }

        public bool IsValid()
        {
            if(InvocationTarget is null)
            {
                return Delegate is not null;
            }
            return InvocationTarget.TryGetTarget(out object target) && Delegate is not null;
        }

        public object GetTarget()
        {
            if(InvocationTarget is null) return null;
            InvocationTarget.TryGetTarget(out object result);
            return result;
        }
    }
}