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


using System;
using System.ComponentModel.Composition;
using VP.FF.PT.Common.Infrastructure.Logging;

namespace VP.FF.PT.Common.Infrastructure
{
    /// <summary>
    /// Helper class which raises an event within a try-catch block and automatically unsubscribe the observer in case of error.
    /// </summary>
    /// <remarks>
    /// E.g. useful for WCF services using callbacks (=events): They must not crash when a client is not responsible anymore, they
    /// will be just unsubscribed from the event invokation list. Makes a log entry, optional.
    /// </remarks>
    [Export(typeof(ISafeEventRaiser))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SafeEventRaiser : ISafeEventRaiser
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeEventRaiser"/> class, without logger.
        /// </summary>
        public SafeEventRaiser()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeEventRaiser"/> class.
        /// </summary>
        /// <param name="logger">The logger logs info entries in case of unsubscribe.</param>
        [ImportingConstructor]
        public SafeEventRaiser(ILogger logger)
        {
            _logger = logger;
            _logger.Init(GetType());
        }

        /// <summary>
        /// Raises the specified event without arguments.
        /// </summary>
        public void Raise(ref Action @event)
        {
            try
            {
                var invokationArray = @event.GetInvocationList();
                if (invokationArray.Length > 0)
                {
                    for (int index = invokationArray.Length; index > 0; --index)
                    {
                        var eventItem = invokationArray[index - 1] as Action;
                        try
                        {
                            eventItem();
                        }
                        catch (Exception)
                        {
                            // no rethrow exception required because thats a cleanup
                            @event -= eventItem;

                            if (_logger != null)
                                if (@event.Method.DeclaringType != null)
                                    _logger.Info("Detected faulted client. It has been removed from subscription. Declaring Type: " + @event.Method.DeclaringType.Name);
                                else
                                    _logger.Info("Detected faulted client. It has been removed from subscription");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (_logger != null)
                    _logger.Error("Failed to raise event", e);
            }
        }

        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <typeparam name="T">The type of the dto</typeparam>
        /// <param name="event">The event.</param>
        /// <param name="dto">The dto.</param>
        public void Raise<T>(ref Action<T> @event, T dto)
        {
            if (@event == null)
            {
                return;
            }
            try
            {
                var invokationArray = @event.GetInvocationList();
                if (invokationArray.Length > 0)
                {
                    for (int index = invokationArray.Length; index > 0; --index)
                    {
                        var eventItem = invokationArray[index - 1] as Action<T>;
                        try
                        {
                            eventItem(dto);
                        }
                        catch (Exception ex)
                        {
                            // no rethrow exception required because thats a cleanup
                            @event -= eventItem;

                            if (_logger != null)
                            {
                                _logger.Warn("Listener threw an exception:", ex);
                                if (@event.Method.DeclaringType != null)
                                    _logger.Info("Detected faulted client. It has been removed from subscription. Declaring Type: " + @event.Method.DeclaringType.Name);
                                else
                                    _logger.Info("Detected faulted client. It has been removed from subscription");
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                if (_logger != null) 
                    _logger.Error("Failed to raise event", e);
            }
        }

        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <typeparam name="T1">The type of the 1. dto</typeparam>
        /// <typeparam name="T2">The type of the 2. dto</typeparam>
        /// <param name="event">The event.</param>
        /// <param name="dto1">The dto1.</param>
        /// <param name="dto2">The dto2.</param>
        public void Raise<T1, T2>(ref Action<T1, T2> @event, T1 dto1, T2 dto2)
        {
            // check if someone subscribed
            if (@event == null)
                return;

            try
            {
                var invokationArray = @event.GetInvocationList();
                if (invokationArray.Length > 0)
                {
                    for (int index = invokationArray.Length; index > 0; --index)
                    {
                        var eventItem = invokationArray[index - 1] as Action<T1, T2>;
                        try
                        {
                            eventItem(dto1, dto2);
                        }
                        catch (Exception)
                        {
                            // no rethrow exception required because thats a cleanup
                            @event -= eventItem;

                            if (_logger != null)
                                if (@event.Method.DeclaringType != null)
                                    _logger.Info("Detected faulted client. It has been removed from subscription. Declaring Type: " + @event.Method.DeclaringType.Name);
                                else
                                    _logger.Info("Detected faulted client. It has been removed from subscription");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (_logger != null)
                    _logger.Error("Failed to raise event", e);
            }
        }

        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <typeparam name="T1">The type of the 1. dto</typeparam>
        /// <typeparam name="T2">The type of the 2. dto</typeparam>
        /// <typeparam name="T3">The type of the 3. dto</typeparam>
        /// <param name="event">The event.</param>
        /// <param name="dto1">The dto1.</param>
        /// <param name="dto2">The dto2.</param>
        /// <param name="dto3">The dto3.</param>
        public void Raise<T1, T2, T3>(ref Action<T1, T2, T3> @event, T1 dto1, T2 dto2, T3 dto3)
        {
            // check if someone subscribed
            if (@event == null)
                return;

            try
            {
                var invokationArray = @event.GetInvocationList();
                if (invokationArray.Length > 0)
                {
                    for (int index = invokationArray.Length; index > 0; --index)
                    {
                        var eventItem = invokationArray[index - 1] as Action<T1, T2, T3>;
                        try
                        {
                            eventItem(dto1, dto2, dto3);
                        }
                        catch (Exception)
                        {
                            // no rethrow exception required because thats a cleanup
                            @event -= eventItem;

                            if (_logger != null)
                                if (@event.Method.DeclaringType != null)
                                    _logger.Info("Detected faulted client. It has been removed from subscription. Declaring Type: " + @event.Method.DeclaringType.Name);
                                else
                                    _logger.Info("Detected faulted client. It has been removed from subscription");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (_logger != null)
                    _logger.Error("Failed to raise event", e);
            }
        }

        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <typeparam name="T1">The type of the 1. dto</typeparam>
        /// <typeparam name="T2">The type of the 2. dto</typeparam>
        /// <typeparam name="T3">The type of the 3. dto</typeparam>
        /// <typeparam name="T4">The type of the 4. dto</typeparam>
        /// <param name="event">The event.</param>
        /// <param name="dto1">The dto1.</param>
        /// <param name="dto2">The dto2.</param>
        /// <param name="dto3">The dto3.</param>
        /// <param name="dto4">The dto4.</param>
        public void Raise<T1, T2, T3, T4>(ref Action<T1, T2, T3, T4> @event, T1 dto1, T2 dto2, T3 dto3, T4 dto4)
        {
            // check if someone subscribed
            if (@event == null)
                return;

            try
            {
                var invokationArray = @event.GetInvocationList();
                if (invokationArray.Length > 0)
                {
                    for (int index = invokationArray.Length; index > 0; --index)
                    {
                        var eventItem = invokationArray[index - 1] as Action<T1, T2, T3, T4>;
                        try
                        {
                            eventItem(dto1, dto2, dto3, dto4);
                        }
                        catch (Exception)
                        {
                            // no rethrow exception required because thats a cleanup
                            @event -= eventItem;

                            if (_logger != null)
                                if (@event.Method.DeclaringType != null)
                                    _logger.Info("Detected faulted client. It has been removed from subscription. Declaring Type: " + @event.Method.DeclaringType.Name);
                                else
                                    _logger.Info("Detected faulted client. It has been removed from subscription");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (_logger != null)
                    _logger.Error("Failed to raise event", e);
            }
        }

        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <typeparam name="T1">The type of the 1. dto</typeparam>
        /// <typeparam name="T2">The type of the 2. dto</typeparam>
        /// <typeparam name="T3">The type of the 3. dto</typeparam>
        /// <typeparam name="T4">The type of the 4. dto</typeparam>
        /// <typeparam name="T5">The type of the 5. dto</typeparam>
        /// <param name="event">The event.</param>
        /// <param name="dto1">The dto1.</param>
        /// <param name="dto2">The dto2.</param>
        /// <param name="dto3">The dto3.</param>
        /// <param name="dto4">The dto4.</param>
        /// <param name="dto5">The dto5.</param>
        public void Raise<T1, T2, T3, T4, T5>(ref Action<T1, T2, T3, T4, T5> @event, T1 dto1, T2 dto2, T3 dto3, T4 dto4, T5 dto5)
        {
            // check if someone subscribed
            if (@event == null)
                return;

            try
            {
                var invokationArray = @event.GetInvocationList();
                if (invokationArray.Length > 0)
                {
                    for (int index = invokationArray.Length; index > 0; --index)
                    {
                        var eventItem = invokationArray[index - 1] as Action<T1, T2, T3, T4, T5>;
                        try
                        {
                            eventItem(dto1, dto2, dto3, dto4, dto5);
                        }
                        catch (Exception)
                        {
                            // no rethrow exception required because thats a cleanup
                            @event -= eventItem;

                            if (_logger != null)
                                if (@event.Method.DeclaringType != null)
                                    _logger.Info("Detected faulted client. It has been removed from subscription. Declaring Type: " + @event.Method.DeclaringType.Name);
                                else
                                    _logger.Info("Detected faulted client. It has been removed from subscription");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (_logger != null)
                    _logger.Error("Failed to raise event", e);
            }
        }
    }
}
