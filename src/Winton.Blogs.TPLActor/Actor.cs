// Copyright (c) Winton. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENCE in the project root for license information.

using System;
using System.Threading.Tasks;

namespace Winton.Blogs.TPLActor
{
    public sealed class Actor
    {
        private const TaskCreationOptions TaskCreationOptions = System.Threading.Tasks.TaskCreationOptions.HideScheduler;
        private readonly TaskFactory _taskFactory = new TaskFactory(new ActorTaskScheduler());

        public Task Enqueue(Action work)
        {
            return _taskFactory.StartNew(work, TaskCreationOptions);
        }

        public Task<T> Enqueue<T>(Func<T> work)
        {
            return _taskFactory.StartNew(work, TaskCreationOptions);
        }

        public async Task Enqueue(Func<Task> work)
        {
            await await _taskFactory.StartNew(work, TaskCreationOptions).ConfigureAwait(false);
        }

        public async Task<T> Enqueue<T>(Func<Task<T>> work)
        {
            return await await _taskFactory.StartNew(work, TaskCreationOptions).ConfigureAwait(false);
        }
    }
}