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
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using VP.FF.PT.Common.Infrastructure.Logging;

namespace VP.FF.PT.Common.Infrastructure.Database
{
    /// <summary>
    /// Polling based DB listener which compares the last executed query result with the new one. It detects inserts, removes and updates.
    /// Note that this approach can be expensive, depending on the result size. Try to keep the result size low, e.g. by using timesamps if possible.
    /// </summary>
    /// <example>
    /// SELECT Id, Processed, Job_Id, ItemHostId FROM dbo.JobItems WHERE Processed IS NOT NULL AND DATEDIFF(minute, Processed, GETDATE()) < 5
    /// </example>
    /// <seealso cref="VP.FF.PT.Common.Infrastructure.Database.IDatabaseListener" />
    /// <seealso cref="VP.FF.PT.Common.Infrastructure.IShutdown" />
    [Export(typeof(IShutdown))]
    [Export(typeof(IDatabaseListener))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PollingDatabaseListener : IDatabaseListener, IShutdown
    {
        private readonly ILogger _logger;
        private readonly SqlConnection _sqlConnection;
        private string _sqlSelect;
        private readonly string _connectionString;
        private Thread _thread;
        private bool _shutdownThread;
        private const int DefaultPollingRateMilliseconds = 200;
        private ICollection<DbChanges> _lastDbDtos = new List<DbChanges>();

        public event EventHandler<DbChanges> DbChanged;

        [ImportingConstructor]
        public PollingDatabaseListener(ILogger logger, IConnectionStringProvider connectionStringProvider)
        {
            _logger = logger;
            _logger.Init(GetType());
            _logger.Debug("Creating the PollingDatabaseListener");

            _connectionString = connectionStringProvider.ConnectionString;

            _sqlConnection = new SqlConnection(_connectionString);

            PollingRateMilliseconds = DefaultPollingRateMilliseconds;
        }

        public void Initialize(string sqlSelect)
        {
            _sqlSelect = sqlSelect;
        }

        public int PollingRateMilliseconds { get; set; }

        public void StartListening()
        {
            if (string.IsNullOrEmpty(_sqlSelect))
            {
                throw new ApplicationException("DatabaseListener not initialized");
            }

            try
            {
                _sqlConnection.Open();
            }
            catch (SqlException e)
            {
                _logger.Error("Could not start DatabaseListener", e);
            }

            if (_thread == null)
            {
                _thread = new Thread(PollingThread)
                {
                    Name = "PollingDatabaseListener",
                    IsBackground = true
                };
            }

            _shutdownThread = false;
            _thread.Start();
        }

        public void StopListening()
        {
            if (_thread != null)
            {
                _shutdownThread = true;
                _thread.Join();
                _thread = null;
            }

            if (_sqlConnection.State != ConnectionState.Closed)
            {
                try
                {
                    SqlDependency.Stop(_connectionString);
                    _sqlConnection.Close();
                }
                catch (Exception e)
                {
                    _logger.Error("Could not stop DatabaseListener", e);
                }
            }
        }

        private void PollingThread()
        {
            while (!_shutdownThread)
            {
                Thread.Sleep(PollingRateMilliseconds);

                using (SqlCommand command = new SqlCommand(_sqlSelect, _sqlConnection))
                {
                    try
                    {
                        var lastDbDtos = new List<DbChanges>();

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var dbDto = new DbChanges();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    dbDto.Data.Add(reader.GetName(i), reader[i]);
                                }

                                if (!ExistsInLastDtos(dbDto))
                                {
                                    if (DbChanged != null)
                                        DbChanged(this, dbDto);
                                }

                                lastDbDtos.Add(dbDto);
                            }
                        }

                        _lastDbDtos = lastDbDtos;
                    }
                    catch (Exception e)
                    {
                        _logger.Error("Cannot read DB", e);
                    }
                }
            }
        }

        public void Shutdown()
        {
            StopListening();
        }

        private bool ExistsInLastDtos(DbChanges dbDto)
        {
            foreach (var lastDbDto in _lastDbDtos)
            {
                if (dbDto.Equals(lastDbDto))
                    return true;
            }

            return false;
        }
    }
}
