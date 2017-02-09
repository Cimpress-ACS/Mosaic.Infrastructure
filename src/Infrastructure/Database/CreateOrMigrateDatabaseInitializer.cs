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


using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;

using System.Linq;

namespace VP.FF.PT.Common.Infrastructure.Database
{
    //http://stackoverflow.com/questions/15796115/how-to-create-initializer-to-create-and-migrate-mysql-database
    //http://stackoverflow.com/questions/5559043/entity-framework-code-first-two-foreign-keys-from-same-table
    public class CreateOrMigrateDatabaseInitializer<TContext, TConfiguration> : CreateDatabaseIfNotExists<TContext>, IDatabaseInitializer<TContext>
        where TContext : DbContext
        where TConfiguration : DbMigrationsConfiguration<TContext>, new()
    {
        private readonly DbMigrationsConfiguration _configuration;

        public CreateOrMigrateDatabaseInitializer()
        {
            _configuration = new TConfiguration();
        }

        public CreateOrMigrateDatabaseInitializer(string connection)
        {
            _configuration = new TConfiguration { TargetDatabase = new DbConnectionInfo(connection) };
        }

        void IDatabaseInitializer<TContext>.InitializeDatabase(TContext context)
        {
            var doseed = !context.Database.Exists();

            var migrator = new DbMigrator(_configuration);
            if (migrator.GetPendingMigrations().Any())
                migrator.Update();

            // move on with the 'CreateDatabaseIfNotExists' for the 'Seed'
            base.InitializeDatabase(context);
            if (doseed)
            {
                Seed(context);
                context.SaveChanges();
            }
        }

        protected override void Seed(TContext context)
        {
        }
    }
}
