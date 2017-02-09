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


using System.Threading;

namespace VP.FF.PT.Common.Infrastructure
{
    /// <summary>
    /// A semaphore is a concurrency utility that contains a number of "tokens". Threads try to acquire
    /// (take) and release (put) these tokens into the semaphore. When a semaphore contains no tokens,
    /// threads that try to acquire a token will block until a token is released into the semaphore.
    /// </summary>
    public interface ISemaphore
    {
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
        bool TryAcquire(int millisecondsTimeout);

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
        bool ForceTryAcquire(int millisecondsTimeout);

        /// <summary>
        /// Acquires a token waiting for as long as necessary to do so.
        /// </summary>
        /// <exception cref="ThreadInterruptedException">
        /// If the calling thread was interrupted while waiting to acquire a token
        /// </exception>
        void Acquire();

        /// <summary>
        /// Acquires a token waiting for as long as necessary to do so. 
        /// <see cref="ThreadInterruptedException"/> are guaranteed not be thrown by this method.
        /// </summary>
        void ForceAcquire();

        /// <summary>
        /// Releases a token.
        /// </summary>
        /// <exception cref="ThreadInterruptedException">
        /// If the calling thread was interrupted while waiting to release a token
        /// </exception>
        void Release();

        /// <summary>
        /// Releases many tokens.
        /// </summary>
        /// <param name="tokens">The number of tokens to release</param>
        /// <exception cref="ThreadInterruptedException">
        /// If the calling thread was interrupted while waiting to release tokens
        /// </exception>
        void ReleaseMany(int tokens);

        /// <summary>
        /// Releases a token. <see cref="ThreadInterruptedException"/> are guaranteed not be thrown by this
        /// method.
        /// </summary>
        void ForceRelease();

        /// <summary>
        /// Releases many tokens. <see cref="ThreadInterruptedException"/> are guaranteed not be thrown by this
        /// method.
        /// </summary>
        /// <param name="tokens">The number of tokens to release</param>
        void ForceReleaseMany(int tokens);
    }
}
