﻿using System;
using System.Collections.Generic;
using Grouchy.HttpApi.Server.Abstractions.EventCallbacks;
using Grouchy.HttpApi.Server.Abstractions.Events;

namespace Grouchy.HttpApi.Server.Extensions
{
    public static class HttpServerEventCallbackExtensions
    {
        public static void Invoke<TEvent>(this IEnumerable<IHttpServerEventCallback> callbacks, Func<TEvent> eventFactory) where TEvent : IHttpServerEvent
        {
            try
            {
                var @event = eventFactory();
                
                foreach (var callback in callbacks)
                {
                    try
                    {
                        callback.Invoke(@event);
                    }
                    catch (Exception)
                    {
                        // Just in case callback handler doesn't catch exceptions
                    }
                }
            }
            catch (Exception)
            {
                // Just in case factory doesn't catch exceptions
            }
        }
    }
}