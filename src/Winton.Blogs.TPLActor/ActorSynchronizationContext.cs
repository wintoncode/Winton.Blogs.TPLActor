// Copyright (c) Winton. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENCE in the project root for license information.

using System.Threading;
using System.Threading.Tasks;

namespace Winton.Blogs.TPLActor
{
    internal sealed class ActorSynchronizationContext : SynchronizationContext
    {
        private readonly TaskFactory _scheduler;

        public ActorSynchronizationContext(ActorTaskScheduler scheduler)
        {
            _scheduler = new TaskFactory(scheduler);
        }

        public override void Post(SendOrPostCallback callback, object state)
        {
            _scheduler.StartNew(() => callback(state), TaskCreationOptions.HideScheduler);
        }
    }
}