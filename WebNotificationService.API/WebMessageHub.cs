﻿using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities.Types;
using Utilities.Extensions;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Components;

namespace WebNotificationService.API
{
    [Service(ServiceLifetime.Transient)]
    class WebMessageHub : IWebMessageHub, IDisposable
    {
        readonly IDisposable _subscriptions;

        public event Func<TestAddedWebMessage, Task> TestAddedAsync = m => Task.CompletedTask;
        public event Func<TestCompletedWebMessage, Task> TestCompletedAsync = m => Task.CompletedTask;
        public event Func<TestDeletedWebMessage, Task> TestDeletedAsync = m => Task.CompletedTask;
        public event Func<TestBegunWebMessage, Task> TestBegunAsync = m => Task.CompletedTask;
        public event Func<TestRecordedWebMessage, Task> TestRecordedAsync = m => Task.CompletedTask;
        public event Func<TestCancelledWebMessage, Task> TestCancelledAsync = m => Task.CompletedTask;
        public event Func<EntryChangedWebMessageBase, Task> EntryChangedAsync = m => Task.CompletedTask;

        [Inject] public IWebMessageHubConnectionProvider ConnectionProvider { get; set; }

        public WebMessageHub(IDependencyResolver di)
        {
            di.ResolveProperties(this);

            _subscriptions = new DisposingActions()
            {
                ConnectionProvider.Connection.On<TestAddedWebMessage>("TestAdded", async (message) =>
                {
                    await TestAddedAsync.InvokeAndWaitAsync(message);
                }),

                ConnectionProvider.Connection.On<TestCompletedWebMessage>("TestCompleted", async (message) =>
                {
                   await TestCompletedAsync.InvokeAndWaitAsync(message);
                }),

                ConnectionProvider.Connection.On<TestDeletedWebMessage>("TestDeleted", async (message) =>
                {
                   await TestDeletedAsync.InvokeAndWaitAsync(message);
                }),

                ConnectionProvider.Connection.On<TestBegunWebMessage>("TestBegun", async (message) =>
                {
                   await TestBegunAsync.InvokeAndWaitAsync(message);
                }),

                ConnectionProvider.Connection.On<TestRecordedWebMessage>("TestRecorded", async (message) =>
                {
                    await TestRecordedAsync.InvokeAndWaitAsync(message);
                }),

                ConnectionProvider.Connection.On<TestCancelledWebMessage>("TestCancelled", async (message) =>
                {
                    await TestCancelledAsync.InvokeAndWaitAsync(message);
                }),

                ConnectionProvider.Connection.On<EntryChangedWebMessageBase>("EntryChanged", async (message) =>
                {
                    await EntryChangedAsync.InvokeAndWaitAsync(message);
                })
            };
        }

        public void Dispose()
        {
            _subscriptions.Dispose();
        }
    }
}
