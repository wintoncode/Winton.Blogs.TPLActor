// Copyright (c) Winton. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENCE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Winton.Blogs.TPLActor
{
    internal sealed class ActorTaskScheduler : TaskScheduler
    {
        private readonly Queue<Task> _taskQueue = new Queue<Task>();
        private readonly object _syncObject = new object();
        private readonly ActorSynchronizationContext _synchronizationContext;

        private bool _isActive = false;

        public ActorTaskScheduler()
        {
            _synchronizationContext = new ActorSynchronizationContext(this);
        }

        public override int MaximumConcurrencyLevel => 1;

        protected override IEnumerable<Task> GetScheduledTasks()
        {
            throw new NotSupportedException();
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) => false;

        protected override void QueueTask(Task task)
        {
            Console.WriteLine("Queuing task.");

            lock (_syncObject)
            {
                _taskQueue.Enqueue(task);

                if (!_isActive)
                {
                    _isActive = true;
                    ThreadPool.QueueUserWorkItem(
                        _ =>
                        {
                            SynchronizationContext.SetSynchronizationContext(_synchronizationContext);
                            Task nextTask = null;

                            while ((nextTask = TryGetNextTask()) != null)
                            {
                                TryExecuteTask(nextTask);
                            }

                            SynchronizationContext.SetSynchronizationContext(null);
                        });
                }
            }
        }

        private Task TryGetNextTask()
        {
            lock (_syncObject)
            {
                if (_taskQueue.Count > 0)
                {
                    return _taskQueue.Dequeue();
                }
                else
                {
                    _isActive = false;
                    return null;
                }
            }
        }
    }
}