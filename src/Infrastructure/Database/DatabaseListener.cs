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
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using VP.FF.PT.Common.Infrastructure.Logging;

namespace VP.FF.PT.Common.Infrastructure.Database
{
    /* 
     * TODO: investigate why this approach works only during the first minutes. afterwards the delay time gets bigger and bigger. the DbChanged event gets fired after hours or never.
     * TODO: however, this approach would be better than the polling solution.
     * 
     * See http://stackoverflow.com/questions/7588572/what-are-the-limitations-of-sqldependency for some limitations of this approach.
    */
    [Obsolete("does not work in the current state, needs development", true)]
    [Export(typeof(IShutdown))]
    [Export("eventbased", typeof(IDatabaseListener))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DatabaseListener : IDatabaseListener, IShutdown
    {
        private readonly ILogger _logger;
        private readonly SqlConnection _sqlConnection;
        private string _sqlCommandText;
        private readonly string _connectionString;

        public event EventHandler<DbChanges> DbChanged;

        [ImportingConstructor]
        public DatabaseListener(ILogger logger, IConnectionStringProvider connectionStringProvider)
        {
            _logger = logger;
            _logger.Init(GetType());
            _logger.Debug("Creating the DatabaseListener");

            _connectionString = connectionStringProvider.ConnectionString;

            _sqlConnection = new SqlConnection(_connectionString);
        }

        public void Initialize(string sqlCommand)
        {
            _sqlCommandText = sqlCommand;
        }

        public void StartListening()
        {
            if (string.IsNullOrEmpty(_sqlCommandText))
            {
                throw new ApplicationException("DatabaseListener not initialized");
            }

            try
            {
                _sqlConnection.Open();
                SqlDependency.Start(_connectionString);
                ListenToChanges();
            }
            catch (Exception e)
            {
                _logger.Error("Could not start DatabaseListener", e);
            }
        }

        public void StopListening()
        {
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

        private void ListenToChanges(SqlNotificationEventArgs e = null)
        {
            using (SqlCommand command = new SqlCommand(_sqlCommandText, _sqlConnection))
            {
                // Create a dependency and associate it with the SqlCommand.
                var sqlDependency = new SqlDependency(command);

                sqlDependency.OnChange += OnChange;

                // Execute the command here to prepopulate the current state and then invalidate the
                // state upon asynchronous notification
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var dbDto = new DbChanges();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            dbDto.Data.Add(reader.GetName(i), reader[i]);
                        }

                        if (DbChanged != null)
                            DbChanged(this, dbDto);
                    }
                }
            }
        }

        private void OnChange(object sender, SqlNotificationEventArgs e)
        {
            ListenToChanges(e);
        }

        public void Shutdown()
        {
            StopListening();
        }
    }
}
