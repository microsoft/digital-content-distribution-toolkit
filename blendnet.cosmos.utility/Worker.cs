// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using AutoMapper;
using blendnet.cms.repository.Interfaces;
using blendnet.common.dto.Cms;
using blendnet.cosmos.utility.BroadCastMigration;
using blendnet.cosmos.utility.DeviceFilterMigration;
using blendnet.cosmos.utility.Repository;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace blendnet.cosmos.utility
{
    /// <summary>
    /// Current objective is to run this manually and see if in future we can extend this further.
    /// Hence, started with WorkerProcess, ideal was Console application.
    /// </summary>
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private readonly BroadcastMigrationWorker _broadcastMigrationWorker;

        private readonly DeviceFilterMigrationWorker _deviceFilterMigrationWorker;

        public Worker(  ILogger<Worker> logger, 
                        BroadcastMigrationWorker broadcastMigrationWorker,
                        DeviceFilterMigrationWorker deviceFilterMigrationWorker)
        {
            _logger = logger;

            _broadcastMigrationWorker = broadcastMigrationWorker;

            _deviceFilterMigrationWorker = deviceFilterMigrationWorker;
        }

        /// <summary>
        /// Invoke the respective worker to do data migartion
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Process Started at {DateTime.Now}");

            //await _broadcastMigrationWorker.DoWork();

            //await _deviceFilterMigrationWorker.DoWork();

            _logger.LogInformation($"Process Completed at {DateTime.Now}");

            return Task.CompletedTask;
        }
       
    }
}
