﻿#region Copyright
// <copyright file="MyService.cs" company="Ian N. Bennett">
// MIT License
//
// Copyright (C) 2020 Ian N. Bennett
// 
// This file is part of MyWebhookLib
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
// </copyright>
#endregion
using ianisms.SmartThings.NETCoreWebHookSDK.Models.SmartThings;
using ianisms.SmartThings.NETCoreWebHookSDK.Utils.InstalledApp;
using ianisms.SmartThings.NETCoreWebHookSDK.Utils.SmartThings;
using ianisms.SmartThings.NETCoreWebHookSDK.Utils.State;
using ianisms.SmartThings.NETCoreWebHookSDK.WebhookHandlers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MyWebhookLib.Models;
using System;
using System.Threading.Tasks;

namespace MyWebhookLib.Services
{
    public interface IMyService
    {
        Task<dynamic> HandleRequestAsync(HttpRequest request);
        Task<MyState> GetStateAsync(string installedAppId);
        Task RemoveStateAsync(string installedAppId);
        Task DeviceCommandAsync(string installedAppId,
            string deviceId,
            dynamic command);
        Task LightSwitchCommandAsync(string installedAppId,
            string deviceId,
            bool toggle);
    }

    public class MyService : IMyService
    {
        private readonly ILogger<IMyService> logger;
        private readonly IRootWebhookHandler rootHandler;
        private readonly IStateManager<MyState> stateManager;
        private readonly IInstalledAppManager installedAppManager;
        private readonly ISmartThingsAPIHelper smartThingsAPIHelper;

        public MyService(ILogger<IMyService> logger,
            IRootWebhookHandler rootHandler,
            IStateManager<MyState> stateManager,
            IInstalledAppManager installedAppManager,
            ISmartThingsAPIHelper smartThingsAPIHelper)
        {
            _ = logger ?? throw new ArgumentNullException(nameof(logger));
            _ = rootHandler ?? throw new ArgumentNullException(nameof(rootHandler));
            _ = stateManager ?? throw new ArgumentNullException(nameof(stateManager));
            _ = installedAppManager ?? throw new ArgumentNullException(nameof(installedAppManager));
            _ = smartThingsAPIHelper ?? throw new ArgumentNullException(nameof(smartThingsAPIHelper));

            this.logger = logger;
            this.rootHandler = rootHandler;
            this.stateManager = stateManager;
            this.installedAppManager = installedAppManager;
            this.smartThingsAPIHelper = smartThingsAPIHelper;
        }

        public async Task<dynamic> HandleRequestAsync(HttpRequest request)
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));

            try
            {
                return await rootHandler.HandleRequestAsync(request).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception calling rootHandler.HandleRequestAsync");
                throw;
            }
        }

        public async Task<MyState> GetStateAsync(string installedAppId)
        {
            _ = installedAppId ?? throw new ArgumentNullException(nameof(installedAppId));

            try
            {
                return await stateManager.GetStateAsync(installedAppId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception calling stateManager.GetStateAsync");
                throw;
            }
        }

        public async Task RemoveStateAsync(string installedAppId)
        {
            _ = installedAppId ?? throw new ArgumentNullException(nameof(installedAppId));

            try
            {
                await stateManager.RemoveStateAsync(installedAppId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception calling stateManager.GetStateAsync");
                throw;
            }
        }

        public async Task DeviceCommandAsync(string installedAppId,
            string deviceId,
            dynamic command)
        {
            _ = installedAppId ?? throw new ArgumentNullException(nameof(installedAppId));
            _ = deviceId ?? throw new ArgumentNullException(nameof(deviceId));
            _ = command ?? throw new ArgumentNullException(nameof(command));

            try
            {
                var installedApp = await installedAppManager.GetInstalledAppAsync(installedAppId).ConfigureAwait(false);
                await smartThingsAPIHelper.DeviceCommandAsync(installedApp, deviceId, command);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception calling smartThingsAPIHelper.DeviceCommandAsync");
                throw;
            }
        }

        public async Task LightSwitchCommandAsync(string installedAppId,
            string deviceId,
            bool toggle)
        {
            _ = installedAppId ?? throw new ArgumentNullException(nameof(installedAppId));
            _ = deviceId ?? throw new ArgumentNullException(nameof(deviceId));

            try
            {
                var installedApp = await installedAppManager.GetInstalledAppAsync(installedAppId).ConfigureAwait(false);
                var command = LightSwitch.GetDeviceCommand(toggle);
                await smartThingsAPIHelper.DeviceCommandAsync(installedApp, deviceId, command);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Exception calling smartThingsAPIHelper.DeviceCommandAsync");
                throw;
            }
        }
    }
}
