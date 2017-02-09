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
using System.Collections.Generic;
using System.Threading;

namespace VP.FF.PT.Common.Infrastructure
{
    /// <summary>
    /// A semaphore is a concurrency utility that contains a number of "tokens". Threads try to acquire
    /// (take) and release (put) these tokens into the semaphore. When a semaphore contains no tokens,
    /// threads that try to acquire a token will block until a token is released into the semaphore.
    /// This implementation guarantees the order in which client threads are able to acquire tokens
    /// will be in a first in first out manner.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This semaphore implementation guarantees that that threads will be served tokens on a first
    /// in first out basis. However, one should note that a thread is considered "in" not by when
    /// it calls any of the acquire methods, but by when it acquires the internal lock inside 
    /// any of the acquire methods. This is not normally an issue one need worry about.
    /// </para>
    /// <para>
    /// This class supports the safe use of interrupts. An interrupt that occurs within a method of
    /// this class results in the action performed by the method not occurring.
    /// </para>
    /// <para>
    /// This class does not support the safe use of <see cref="Thread.Abort()"/>. Its use may leave
    /// this class in an undefined state.
    /// </para>
    /// </remarks>
    public class FifoSemaphore : ISemaphore
    {
        private readonly Queue<Waiter> _waitQueue;

        /// <summary>
        /// The lock object for this class
        /// </summary>
        private readonly object _lock;

        /// <summary>
        /// The tokens count for this class
        /// </summary>
        private int _tokens;

        /// <summary>
        /// Constructor, creates a FifoSemaphore
        /// </summary>
        /// <param name="tokens">The number of tokens the semaphore will start with</param>
        public FifoSemaphore(int tokens)
        {
            _tokens = tokens;
            _lock = new object();
            _waitQueue = new Queue<Waiter>();
        }

        /// <summary>
        /// Try to acquire a token but time out if a token cannot be acquired after certain amount of time.
        /// </summary>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds to wait to acquire a token. This can be set to 
        /// <see cref="Timeout.Infinite"/> if you want to wait forever.
        /// </param>
        /// <returns>
        /// true if a token was acquired successfully; false if a token has not been acquired after the amount
        /// of time specified by <paramref name="millisecondsTimeout"/> has elapsed.
        /// </returns>
        /// <exception cref="ThreadInterruptedException">
        /// If the calling thread was interrupted while waiting to acquire a token
        /// </exception>
        public bool TryAcquire(int millisecondsTimeout)
        {
            Waiter waiter;

            lock (_lock)
            {
                if (_tokens > 0)
                {
                    _tokens--;
                    return true;
                }

                waiter = new Waiter();
                _waitQueue.Enqueue(waiter);
            }

            return waiter.TryWait(millisecondsTimeout); //I will be woken up when I'm at the head of the queue 
            //or if I'm interrupted (an exception will be thrown then)
        }

        /// <summary>
        /// Try to acquire a token but time out if a token cannot be acquired after certain amount of time.
        /// <see cref="ThreadInterruptedException"/> are guaranteed not be thrown by this method.
        /// </summary>
        /// <param name="millisecondsTimeout">
        /// The number of milliseconds to wait to acquire a token. This can be set to 
        /// <see cref="Timeout.Infinite"/> if you want to wait forever.
        /// </param>
        /// <returns>
        /// true if a token was acquired successfully; false if a token has not been acquired after the amount
        /// of time specified by <paramref name="millisecondsTimeout"/> has elapsed.
        /// </returns>
        public bool ForceTryAcquire(int millisecondsTimeout)
        {
            bool retval;

            try
            {
            }
            finally
            {
                retval = TryAcquire(millisecondsTimeout);
            }

            return retval;
        }

        /// <summary>
        /// Acquires a token waiting for as long as necessary to do so.
        /// </summary>
        /// <exception cref="ThreadInterruptedException">
        /// If the calling thread was interrupted while waiting to acquire a token
        /// </exception>
        public void Acquire()
        {
            TryAcquire(Timeout.Infinite);
        }

        /// <summary>
        /// Acquires a token waiting for as long as necessary to do so. 
        /// <see cref="ThreadInterruptedException"/> are guaranteed not be thrown by this method.
        /// </summary>
        public void ForceAcquire()
        {
            try
            {
            }
            finally
            {
                Acquire();
            }
        }

        /// <summary>
        /// Releases a token.
        /// </summary>
        /// <exception cref="ThreadInterruptedException">
        /// If the calling thread was interrupted while waiting to release a token
        /// </exception>
        public void Release()
        {
            ReleaseMany(1);
        }

        /// <summary>
        /// Releases many tokens.
        /// </summary>
        /// <param name="tokens">The number of tokens to release</param>
        /// <exception cref="ThreadInterruptedException">
        /// If the calling thread was interrupted while waiting to release tokens
        /// </exception>
        public virtual void ReleaseMany(int tokens)
        {
            lock (_lock)
            {
                for (int i = 0; i < tokens; i++)
                {
                    if (_waitQueue.Count > 0)
                    {
                        Waiter waiter = _waitQueue.Dequeue();
                        bool releasedSuccessfully = waiter.Release();

                        if (!releasedSuccessfully) //That thread was interrupted or timed out!
                            i--; //Try again with the next
                    }
                    else
                    {
                        //We've got no one waiting, so add a token
                        _tokens++;
                    }
                }
            }
        }

        /// <summary>
        /// Releases a token. <see cref="ThreadInterruptedException"/> are guaranteed not be thrown by this
        /// method.
        /// </summary>
        public void ForceRelease()
        {
            try
            {
            }
            finally
            {
                Release();
            }
        }

        /// <summary>
        /// Releases many tokens. <see cref="ThreadInterruptedException"/> are guaranteed not be thrown by this
        /// method.
        /// </summary>
        /// <param name="tokens">The number of tokens to release</param>
        public void ForceReleaseMany(int tokens)
        {
            try
            {
            }
            catch (Exception)
            {
                ReleaseMany(_tokens);
            }
        }

        /// <summary>
        /// Waiter helper class that allows threads to queue for tokens
        /// </summary>
        private class Waiter
        {
            private readonly object _lock;
            private bool _released;

            public Waiter()
            {
                _lock = new object();
                _released = false;
            }

            /// <summary>
            /// Causes a thread to acquire an internal lock and then wait on it until it is pulsed or a timeout
            /// occurs
            /// </summary>
            /// <param name="millisecondsTimeout">
            /// The number of milliseconds to wait on the lock. This can be set to <see cref="Timeout.Infinite"/>
            /// if you want to wait forever.
            /// </param>
            /// <returns>True if the thread was released successfully, false if a timeout occurred</returns>
            public bool TryWait(int millisecondsTimeout)
            {
                lock (_lock)
                {
                    if (_released) //We've been released before we even started waiting!
                        return true;

                    try
                    {
                        bool success = Monitor.Wait(_lock, millisecondsTimeout);

                        if (!success)
                            _released = true; //Note that we've been released early

                        return success;
                    }
                    catch (ThreadInterruptedException)
                    {
                        if (_released == false)
                        {
                            _released = true; //Note that we've been released early
                            throw;
                        }

                        //We've already been released, so we might as well succeed at
                        //the operation and get interrupted later
                        Thread.CurrentThread.Interrupt();
                        return true;
                    }
                }
            }

            /// <summary>
            /// Causes the thread currently waiting on the lock to be woken up if it is still waiting
            /// </summary>
            /// <returns>
            /// True if the thread was woken successfully, false if it was woken early by an interrupt or a timeout
            /// </returns>
            public bool Release()
            {
                lock (_lock)
                {
                    if (_released) //If released already (this means we've been interrupted or we timed out!)
                        return false;

                    _released = true;
                    Monitor.Pulse(_lock);
                    return true;
                }
            }
        }
    }
}
