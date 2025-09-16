using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WSystem.Collections;
using WSystem.Runtime;

namespace WSystem
{
    /// <summary>
    /// Base class for custom event system which is based on <see cref="WeakReference{T}"/>.
    /// </summary>
    public abstract class Event
    {
        private InvocationList invocationList = new InvocationList();
        /// <summary>
        /// The invocation list for this event.
        /// </summary>
        protected InvocationList InvocationList => invocationList;

        /// <summary>
        /// Bind a delegate to this event.
        /// </summary>
        /// <param name="toBind">The delegate to bind.</param>
        protected void Bind(Delegate toBind)
        {
            ArgumentNullException.ThrowIfNull(toBind);
            Type castType = GetDelegateCastType();
            ArgumentNullException.ThrowIfNull(castType);
            if (!castType.IsSubclassOf(typeof(Delegate))) throw new ArgumentException("Cast type must be a subclass of Delegate.");
            invocationList.Add(toBind, castType);
        }
        /// <summary>
        /// Unbind a delegate from this event.
        /// </summary>
        /// <param name="toUnbind"></param>
        protected void Unbind(Delegate toUnbind)
        {
            ArgumentNullException.ThrowIfNull(toUnbind);
            invocationList.Remove(toUnbind);
        }
        /// <summary>
        /// Dynamically invoke this event.
        /// </summary>
        /// <remarks>
        /// <b>It is prefered to use <see langword="Call"/> method on the actual <see langword="class"/> of the <see cref="Event"/> rather than <see cref="Invoke(object[])"/>.</b>
        /// </remarks>
        /// <param name="args">The arguments to pass down into the call of the event.</param>
        /// <returns><see langword="null"/> if delegate is <see langword="void"/>, otherwise <see cref="object"/> returned by the delegate.</returns>
        public abstract object Invoke(params object[] args);
        /// <summary>
        /// Clears this event of all bindings.
        /// </summary>
        public void ClearBindings()
        {
            invocationList.Clear();
        }
        /// <summary>
        /// When using a target to call a weak delegate, a delegate has to be casted to a type that accepts the target pointer as a parameter.
        /// </summary>
        /// <returns>The delegate type to cast to.</returns>
        protected abstract Type GetDelegateCastType();
    }

    /// <summary>
    /// A typed base class for custom event system which is based on <see cref="WeakReference{T}"/>.
    /// </summary>
    public abstract class TypedEvent<TDelegate> : Event where TDelegate : Delegate
    {
        /// <summary>
        /// Bind a delegate to this event.
        /// </summary>
        /// <param name="toBind">The delegate to bind.</param>
        public void Bind(TDelegate toBind)
        {
            if (toBind is null) return;
            base.Bind(toBind);
        }

        /// <summary>
        /// Unbind a delegate from this event.
        /// </summary>
        /// <param name="toUnbind"></param>
        public void Unbind(TDelegate toUnbind)
        {
            if (toUnbind is null) return;
            base.Unbind(toUnbind);
        }

        protected TParam GetInvokeArgAs<TParam>(object arg)
        {
            if (arg is not TParam param)
            {
                if (!(arg is null && CLR.IsAssignableNull(typeof(TParam)))) throw new ArgumentException("Argument type missmatch.");
                param = default(TParam);
            }
            return param;
        }
    }

    /// <summary>
    /// An <see cref="Action"/> <see langword="event"/>.
    /// </summary>
    public sealed class ActionEvent : TypedEvent<Action>
    {
        /// <summary>
        /// Calls the event.
        /// </summary>
        public void Call()
        {
            InvocationList list = InvocationList;
            foreach (FWeakDelegate dlg in list)
            {
                if (dlg.InvocationTarget is null)
                {
                    Action action = dlg.Delegate as Action;
                    action();
                }
                else
                {
                    Action<object> action = Unsafe.As<Action<object>>(dlg.Delegate);
                    action(dlg.GetTarget());
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override object Invoke(params object[] args)
        {
            Call();
            return null;
        }

        protected override Type GetDelegateCastType()
        {
            return typeof(Action<object>);
        }

        public static ActionEvent operator +(ActionEvent ae, Action action)
        {
            if (ae is null) ae = new ActionEvent();
            ae.Bind(action);
            return ae;
        }
        public static ActionEvent operator -(ActionEvent ae, Action action)
        {
            if (ae is null) return ae;
            ae.Unbind(action);
            return ae;
        }
    }

    /// <summary>
    /// An <see cref="Action{T}"/> <see langword="event"/>.
    /// </summary>
    public sealed class ActionEvent<T> : TypedEvent<Action<T>>
    {
        /// <summary>
        /// Calls the event.
        /// </summary>
        public void Call(T param)
        {
            InvocationList list = InvocationList;
            foreach (FWeakDelegate dlg in list)
            {
                if (dlg.InvocationTarget is null)
                {
                    Action<T> action = dlg.Delegate as Action<T>;
                    action(param);
                }
                else
                {
                    Action<object, T> action = Unsafe.As<Action<object, T>>(dlg.Delegate);
                    action(dlg.GetTarget(), param);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override object Invoke(params object[] args)
        {
            ArgumentNullException.ThrowIfNull(args);
            if (args.Length != 1) throw new ArgumentException("There must be exactly 1 parameter.");
            Call(GetInvokeArgAs<T>(args[0]));
            return null;
        }

        protected override Type GetDelegateCastType()
        {
            return typeof(Action<object, T>);
        }

        public static ActionEvent<T> operator +(ActionEvent<T> ae, Action<T> action)
        {
            if (ae is null) ae = new ActionEvent<T>();
            ae.Bind(action);
            return ae;
        }
        public static ActionEvent<T> operator -(ActionEvent<T> ae, Action<T> action)
        {
            if (ae is null) return ae;
            ae.Unbind(action);
            return ae;
        }
    }

    /// <summary>
    /// An <see cref="Action{T1, T2}"/> <see langword="event"/>.
    /// </summary>
    public sealed class ActionEvent<T1, T2> : TypedEvent<Action<T1, T2>>
    {
        /// <summary>
        /// Calls the event.
        /// </summary>
        public void Call(T1 param, T2 param2)
        {
            InvocationList list = InvocationList;
            foreach (FWeakDelegate dlg in list)
            {
                if (dlg.InvocationTarget is null)
                {
                    Action<T1, T2> action = dlg.Delegate as Action<T1, T2>;
                    action(param, param2);
                }
                else
                {
                    Action<object, T1, T2> action = Unsafe.As<Action<object, T1, T2>>(dlg.Delegate);
                    action(dlg.GetTarget(), param, param2);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override object Invoke(params object[] args)
        {
            ArgumentNullException.ThrowIfNull(args);
            if (args.Length != 2) throw new ArgumentException("There must be exactly 2 parameters.");
            T1 param = GetInvokeArgAs<T1>(args[0]);
            T2 param2 = GetInvokeArgAs<T2>(args[1]);
            Call(param, param2);
            return null;
        }

        protected override Type GetDelegateCastType()
        {
            return typeof(Action<object, T1, T2>);
        }

        public static ActionEvent<T1, T2> operator +(ActionEvent<T1, T2> ae, Action<T1, T2> action)
        {
            if (ae is null) ae = new ActionEvent<T1, T2>();
            ae.Bind(action);
            return ae;
        }
        public static ActionEvent<T1, T2> operator -(ActionEvent<T1, T2> ae, Action<T1, T2> action)
        {
            if (ae is null) return ae;
            ae.Unbind(action);
            return ae;
        }
    }

    /// <summary>
    /// An <see cref="Action{T1, T2, T3}"/> <see langword="event"/>.
    /// </summary>
    public sealed class ActionEvent<T1, T2, T3> : TypedEvent<Action<T1, T2, T3>>
    {
        /// <summary>
        /// Calls the event.
        /// </summary>
        public void Call(T1 param, T2 param2, T3 param3)
        {
            InvocationList list = InvocationList;
            foreach (FWeakDelegate dlg in list)
            {
                if (dlg.InvocationTarget is null)
                {
                    Action<T1, T2, T3> action = dlg.Delegate as Action<T1, T2, T3>;
                    action(param, param2, param3);
                }
                else
                {
                    Action<object, T1, T2, T3> action = Unsafe.As<Action<object, T1, T2, T3>>(dlg.Delegate);
                    action(dlg.GetTarget(), param, param2, param3);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override object Invoke(params object[] args)
        {
            ArgumentNullException.ThrowIfNull(args);
            if (args.Length != 3) throw new ArgumentException("There must be exactly 3 parameters.");
            T1 param = GetInvokeArgAs<T1>(args[0]);
            T2 param2 = GetInvokeArgAs<T2>(args[1]);
            T3 param3 = GetInvokeArgAs<T3>(args[2]);
            Call(param, param2, param3);
            return null;
        }

        protected override Type GetDelegateCastType()
        {
            return typeof(Action<object, T1, T2, T3>);
        }

        public static ActionEvent<T1, T2, T3> operator +(ActionEvent<T1, T2, T3> ae, Action<T1, T2, T3> action)
        {
            if (ae is null) ae = new ActionEvent<T1, T2, T3>();
            ae.Bind(action);
            return ae;
        }
        public static ActionEvent<T1, T2, T3> operator -(ActionEvent<T1, T2, T3> ae, Action<T1, T2, T3> action)
        {
            if (ae is null) return ae;
            ae.Unbind(action);
            return ae;
        }
    }

    /// <summary>
    /// An <see cref="Action{T1, T2, T3, T4}"/> <see langword="event"/>.
    /// </summary>
    public sealed class ActionEvent<T1, T2, T3, T4> : TypedEvent<Action<T1, T2, T3, T4>>
    {
        /// <summary>
        /// Calls the event.
        /// </summary>
        public void Call(T1 param, T2 param2, T3 param3, T4 param4)
        {
            InvocationList list = InvocationList;
            foreach (FWeakDelegate dlg in list)
            {
                if (dlg.InvocationTarget is null)
                {
                    Action<T1, T2, T3, T4> action = dlg.Delegate as Action<T1, T2, T3, T4>;
                    action(param, param2, param3, param4);
                }
                else
                {
                    Action<object, T1, T2, T3, T4> action = Unsafe.As<Action<object, T1, T2, T3, T4>>(dlg.Delegate);
                    action(dlg.GetTarget(), param, param2, param3, param4);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override object Invoke(params object[] args)
        {
            ArgumentNullException.ThrowIfNull(args);
            if (args.Length != 4) throw new ArgumentException("There must be exactly 4 parameters.");
            T1 param = GetInvokeArgAs<T1>(args[0]);
            T2 param2 = GetInvokeArgAs<T2>(args[1]);
            T3 param3 = GetInvokeArgAs<T3>(args[2]);
            T4 param4 = GetInvokeArgAs<T4>(args[3]);
            Call(param, param2, param3, param4);
            return null;
        }

        protected override Type GetDelegateCastType()
        {
            return typeof(Action<object, T1, T2, T3, T4>);
        }

        public static ActionEvent<T1, T2, T3, T4> operator +(ActionEvent<T1, T2, T3, T4> ae, Action<T1, T2, T3, T4> action)
        {
            if (ae is null) ae = new ActionEvent<T1, T2, T3, T4>();
            ae.Bind(action);
            return ae;
        }
        public static ActionEvent<T1, T2, T3, T4> operator -(ActionEvent<T1, T2, T3, T4> ae, Action<T1, T2, T3, T4> action)
        {
            if (ae is null) return ae;
            ae.Unbind(action);
            return ae;
        }
    }

    /// <summary>
    /// An <see cref="Action{T1, T2, T3, T4, T5}"/> <see langword="event"/>.
    /// </summary>
    public sealed class ActionEvent<T1, T2, T3, T4, T5> : TypedEvent<Action<T1, T2, T3, T4, T5>>
    {
        /// <summary>
        /// Calls the event.
        /// </summary>
        public void Call(T1 param, T2 param2, T3 param3, T4 param4, T5 param5)
        {
            InvocationList list = InvocationList;
            foreach (FWeakDelegate dlg in list)
            {
                if (dlg.InvocationTarget is null)
                {
                    Action<T1, T2, T3, T4, T5> action = dlg.Delegate as Action<T1, T2, T3, T4, T5>;
                    action(param, param2, param3, param4, param5);
                }
                else
                {
                    Action<object, T1, T2, T3, T4, T5> action = Unsafe.As<Action<object, T1, T2, T3, T4, T5>>(dlg.Delegate);
                    action(dlg.GetTarget(), param, param2, param3, param4, param5);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override object Invoke(params object[] args)
        {
            ArgumentNullException.ThrowIfNull(args);
            if (args.Length != 5) throw new ArgumentException("There must be exactly 5 parameters.");
            T1 param = GetInvokeArgAs<T1>(args[0]);
            T2 param2 = GetInvokeArgAs<T2>(args[1]);
            T3 param3 = GetInvokeArgAs<T3>(args[2]);
            T4 param4 = GetInvokeArgAs<T4>(args[3]);
            T5 param5 = GetInvokeArgAs<T5>(args[4]);
            Call(param, param2, param3, param4, param5);
            return null;
        }

        protected override Type GetDelegateCastType()
        {
            return typeof(Action<object, T1, T2, T3, T4, T5>);
        }

        public static ActionEvent<T1, T2, T3, T4, T5> operator +(ActionEvent<T1, T2, T3, T4, T5> ae, Action<T1, T2, T3, T4, T5> action)
        {
            if (ae is null) ae = new ActionEvent<T1, T2, T3, T4, T5>();
            ae.Bind(action);
            return ae;
        }
        public static ActionEvent<T1, T2, T3, T4, T5> operator -(ActionEvent<T1, T2, T3, T4, T5> ae, Action<T1, T2, T3, T4, T5> action)
        {
            if (ae is null) return ae;
            ae.Unbind(action);
            return ae;
        }
    }

    /// <summary>
    /// An <see cref="Action{T1, T2, T3, T4, T5, T6}"/> <see langword="event"/>.
    /// </summary>
    public sealed class ActionEvent<T1, T2, T3, T4, T5, T6> : TypedEvent<Action<T1, T2, T3, T4, T5, T6>>
    {
        /// <summary>
        /// Calls the event.
        /// </summary>
        public void Call(T1 param, T2 param2, T3 param3, T4 param4, T5 param5, T6 param6)
        {
            InvocationList list = InvocationList;
            foreach (FWeakDelegate dlg in list)
            {
                if (dlg.InvocationTarget is null)
                {
                    Action<T1, T2, T3, T4, T5, T6> action = dlg.Delegate as Action<T1, T2, T3, T4, T5, T6>;
                    action(param, param2, param3, param4, param5, param6);
                }
                else
                {
                    Action<object, T1, T2, T3, T4, T5, T6> action = Unsafe.As<Action<object, T1, T2, T3, T4, T5, T6>>(dlg.Delegate);
                    action(dlg.GetTarget(), param, param2, param3, param4, param5, param6);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override object Invoke(params object[] args)
        {
            ArgumentNullException.ThrowIfNull(args);
            if (args.Length != 6) throw new ArgumentException("There must be exactly 6 parameters.");
            T1 param = GetInvokeArgAs<T1>(args[0]);
            T2 param2 = GetInvokeArgAs<T2>(args[1]);
            T3 param3 = GetInvokeArgAs<T3>(args[2]);
            T4 param4 = GetInvokeArgAs<T4>(args[3]);
            T5 param5 = GetInvokeArgAs<T5>(args[4]);
            T6 param6 = GetInvokeArgAs<T6>(args[5]);
            Call(param, param2, param3, param4, param5, param6);
            return null;
        }

        protected override Type GetDelegateCastType()
        {
            return typeof(Action<object, T1, T2, T3, T4, T5, T6>);
        }

        public static ActionEvent<T1, T2, T3, T4, T5, T6> operator +(ActionEvent<T1, T2, T3, T4, T5, T6> ae, Action<T1, T2, T3, T4, T5, T6> action)
        {
            if (ae is null) ae = new ActionEvent<T1, T2, T3, T4, T5, T6>();
            ae.Bind(action);
            return ae;
        }
        public static ActionEvent<T1, T2, T3, T4, T5, T6> operator -(ActionEvent<T1, T2, T3, T4, T5, T6> ae, Action<T1, T2, T3, T4, T5, T6> action)
        {
            if (ae is null) return ae;
            ae.Unbind(action);
            return ae;
        }
    }

    /// <summary>
    /// An <see cref="EventHandler"/> <see langword="event"/>.
    /// </summary>
    public sealed class HandlerEvent : TypedEvent<EventHandler>
    {
        /// <summary>
        /// Calls the event.
        /// </summary>
        public void Call(object sender, EventArgs e)
        {
            InvocationList list = InvocationList;
            foreach (FWeakDelegate dlg in list)
            {
                if (dlg.InvocationTarget is null)
                {
                    EventHandler action = dlg.Delegate as EventHandler;
                    action(sender, e);
                }
                else
                {
                    Action<object, object, EventArgs> action = Unsafe.As<Action<object, object, EventArgs>>(dlg.Delegate);
                    action(dlg.GetTarget(), sender, e);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override object Invoke(params object[] args)
        {
            ArgumentNullException.ThrowIfNull(args);
            if (args.Length != 2) throw new ArgumentException("There must be exactly 2 parameters.");
            object param = args[0];
            EventArgs param2 = GetInvokeArgAs<EventArgs>(args[1]);
            Call(param, param2);
            return null;
        }

        protected override Type GetDelegateCastType()
        {
            return typeof(Action<object, object, EventArgs>);
        }

        public static HandlerEvent operator +(HandlerEvent ae, EventHandler action)
        {
            if (ae is null) ae = new HandlerEvent();
            ae.Bind(action);
            return ae;
        }
        public static HandlerEvent operator -(HandlerEvent ae, EventHandler action)
        {
            if (ae is null) return ae;
            ae.Unbind(action);
            return ae;
        }
    }

    /// <summary>
    /// An <see cref="EventHandler{TEventArgs}"/> <see langword="event"/>.
    /// </summary>
    public sealed class HandlerEvent<TEventArgs> : TypedEvent<EventHandler<TEventArgs>>
    {
        /// <summary>
        /// Calls the event.
        /// </summary>
        public void Call(object sender, TEventArgs e)
        {
            InvocationList list = InvocationList;
            foreach (FWeakDelegate dlg in list)
            {
                if (dlg.InvocationTarget is null)
                {
                    EventHandler<TEventArgs> action = dlg.Delegate as EventHandler<TEventArgs>;
                    action(sender, e);
                }
                else
                {
                    Action<object, object, TEventArgs> action = Unsafe.As<Action<object, object, TEventArgs>>(dlg.Delegate);
                    action(dlg.GetTarget(), sender, e);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public override object Invoke(params object[] args)
        {
            ArgumentNullException.ThrowIfNull(args);
            if (args.Length != 2) throw new ArgumentException("There must be exactly 2 parameters.");
            object param = args[0];
            TEventArgs param2 = GetInvokeArgAs<TEventArgs>(args[1]);
            Call(param, param2);
            return null;
        }

        protected override Type GetDelegateCastType()
        {
            return typeof(Action<object, object, TEventArgs>);
        }

        public static HandlerEvent<TEventArgs> operator +(HandlerEvent<TEventArgs> ae, EventHandler<TEventArgs> action)
        {
            if (ae is null) ae = new HandlerEvent<TEventArgs>();
            ae.Bind(action);
            return ae;
        }
        public static HandlerEvent<TEventArgs> operator -(HandlerEvent<TEventArgs> ae, EventHandler<TEventArgs> action)
        {
            if (ae is null) return ae;
            ae.Unbind(action);
            return ae;
        }
    }
}
