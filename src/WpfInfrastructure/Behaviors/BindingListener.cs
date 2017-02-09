/* Copyright 2017 Cimpress

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License. */


// --------------------------------------------------------------------------------------------------------------------
// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;

namespace VP.FF.PT.Common.WpfInfrastructure.Behaviors
{
    /// <summary>
    ///     Helper class for adding Bindings to non-FrameworkElements
    /// </summary>
    public class BindingListener
    {
        #region Static Fields

        /// <summary>
        /// The free listeners.
        /// </summary>
        private static readonly List<DependencyPropertyListener> FreeListeners = new List<DependencyPropertyListener>();

        #endregion

        #region Fields

        /// <summary>
        /// The changed handler.
        /// </summary>
        private readonly ChangedHandler changedHandler;

        /// <summary>
        /// The binding.
        /// </summary>
        private Binding binding;

        /// <summary>
        /// The listener.
        /// </summary>
        private DependencyPropertyListener listener;

        /// <summary>
        /// The target.
        /// </summary>
        private FrameworkElement target;

        /// <summary>
        /// The value.
        /// </summary>
        private object value;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingListener"/> class. 
        /// Constructor.
        /// </summary>
        /// <param name="changedHandler">
        /// Callback whenever the value of this binding has changed.
        /// </param>
        public BindingListener(ChangedHandler changedHandler)
        {
            this.changedHandler = changedHandler;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingListener"/> class. 
        ///     Constructor
        /// </summary>
        public BindingListener()
        {
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     Delegate for when the binding listener has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ChangedHandler(object sender, BindingChangedEventArgs e);

        #endregion

        #region Public Properties

        /// <summary>
        ///     The Binding which is to be evaluated
        /// </summary>
        public Binding Binding
        {
            get
            {
                return this.binding;
            }

            set
            {
                this.binding = value;
                this.Attach();
            }
        }

        /// <summary>
        ///     The element to be used as the context on which to evaluate the binding.
        /// </summary>
        public FrameworkElement Element
        {
            get
            {
                return this.target;
            }

            set
            {
                this.target = value;
                this.Attach();
            }
        }

        /// <summary>
        ///     The current value of this binding.
        /// </summary>
        public object Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if (this.listener != null)
                {
                    this.listener.SetValue(value);
                }
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The attach.
        /// </summary>
        private void Attach()
        {
            this.Detach();

            if (this.target != null && this.binding != null)
            {
                this.listener = this.GetListener();
                this.listener.Attach(this.target, this.binding);
            }
        }

        /// <summary>
        /// The detach.
        /// </summary>
        private void Detach()
        {
            if (this.listener != null)
            {
                this.ReturnListener();
            }
        }

        /// <summary>
        /// The get listener.
        /// </summary>
        /// <returns>
        /// The <see cref="DependencyPropertyListener"/>.
        /// </returns>
        private DependencyPropertyListener GetListener()
        {
            DependencyPropertyListener dependencyPropertyListener;

            if (FreeListeners.Count != 0)
            {
                dependencyPropertyListener = FreeListeners[FreeListeners.Count - 1];
                FreeListeners.RemoveAt(FreeListeners.Count - 1);

                return dependencyPropertyListener;
            }
            dependencyPropertyListener = new DependencyPropertyListener();

            dependencyPropertyListener.Changed += this.HandleValueChanged;

            return dependencyPropertyListener;
        }

        /// <summary>
        /// The handle value changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void HandleValueChanged(object sender, BindingChangedEventArgs e)
        {
            this.value = e.EventArgs.NewValue;

            if (this.changedHandler != null)
            {
                this.changedHandler(this, e);
            }
        }

        /// <summary>
        /// The return listener.
        /// </summary>
        private void ReturnListener()
        {
            this.listener.Changed -= this.HandleValueChanged;

            FreeListeners.Add(this.listener);

            this.listener = null;
        }

        #endregion

        /// <summary>
        /// The dependency property listener.
        /// </summary>
        private class DependencyPropertyListener
        {
            #region Static Fields

            /// <summary>
            /// The index.
            /// </summary>
            private static int index;

            #endregion

            #region Fields

            /// <summary>
            /// The property.
            /// </summary>
            private readonly DependencyProperty property;

            /// <summary>
            /// The target.
            /// </summary>
            private FrameworkElement target;

            #endregion

            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="DependencyPropertyListener"/> class.
            /// </summary>
            public DependencyPropertyListener()
            {
                this.property = DependencyProperty.RegisterAttached(
                    "DependencyPropertyListener" + index++,
                    typeof(object),
                    typeof(DependencyPropertyListener),
                    new PropertyMetadata(null, this.HandleValueChanged));
            }

            #endregion

            #region Public Events

            /// <summary>
            /// The changed.
            /// </summary>
            public event EventHandler<BindingChangedEventArgs> Changed;

            #endregion

            #region Public Methods and Operators

            /// <summary>
            /// The attach.
            /// </summary>
            /// <param name="element">
            /// The element.
            /// </param>
            /// <param name="binding">
            /// The binding.
            /// </param>
            /// <exception cref="Exception">
            /// </exception>
            public void Attach(FrameworkElement element, Binding binding)
            {
                if (this.target != null)
                {
                    throw new Exception("Cannot attach an already attached listener");
                }

                this.target = element;

                this.target.SetBinding(this.property, binding);
            }

            /// <summary>
            /// The detach.
            /// </summary>
            public void Detach()
            {
                this.target.ClearValue(this.property);
                this.target = null;
            }

            /// <summary>
            /// The set value.
            /// </summary>
            /// <param name="value">
            /// The value.
            /// </param>
            public void SetValue(object value)
            {
                this.target.SetValue(this.property, value);
            }

            #endregion

            #region Methods

            /// <summary>
            /// The handle value changed.
            /// </summary>
            /// <param name="sender">
            /// The sender.
            /// </param>
            /// <param name="e">
            /// The e.
            /// </param>
            private void HandleValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            {
                if (this.Changed != null)
                {
                    this.Changed(this, new BindingChangedEventArgs(e));
                }
            }

            #endregion
        }
    }

    /// <summary>
    ///     Event args for when binding values change.
    /// </summary>
    public class BindingChangedEventArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BindingChangedEventArgs"/> class. 
        /// Constructor
        /// </summary>
        /// <param name="e">
        /// </param>
        public BindingChangedEventArgs(DependencyPropertyChangedEventArgs e)
        {
            this.EventArgs = e;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Original event args.
        /// </summary>
        public DependencyPropertyChangedEventArgs EventArgs { get; private set; }

        #endregion
    }
}
