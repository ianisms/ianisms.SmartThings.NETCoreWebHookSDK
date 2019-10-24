﻿#region Copyright
// <copyright file="OAuthWebhookHandler.cs" company="Ian N. Bennett">
//
// Copyright (C) 2019 Ian N. Bennett
// 
// This file is part of SmartThings.NETCoreWebHookSDK
//
// SmartThings.NETCoreWebHookSDK is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// SmartThings.NETCoreWebHookSDK is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/. 
// </copyright>
#endregion

using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace ianisms.SmartThings.NETCoreWebHookSDK.WebhookHandlers
{
    public interface IOAuthWebhookHandler
    {
        ILogger<IOAuthWebhookHandler> logger { get; }
        Task<dynamic> HandleRequestAsync(dynamic request);
    }

    public class OAuthWebhookHandler : IOAuthWebhookHandler
    {
        public ILogger<IOAuthWebhookHandler> logger { get; private set; }

        public OAuthWebhookHandler(ILogger<IOAuthWebhookHandler> logger)
        {
            this.logger = logger;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<dynamic> HandleRequestAsync(dynamic request)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            logger.LogDebug($"handling request: {request}");

            dynamic response = new JObject();
            response.oAuthCallbackData = new JObject();

            logger.LogDebug($"response: {response}");

            return response;
        }
    }
}
