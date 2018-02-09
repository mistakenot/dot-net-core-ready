using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

namespace DotNetCoreReady.Services
{
    public class SqlEmailAlertsRepository : IEmailAlertsRepository
    {
        private readonly string _connectionString;

        public SqlEmailAlertsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<CreateResult> CreateIfNotExists(string email, string packageId, bool optedInToMarketing)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                var rows = await connection.ExecuteAsync(@"
                    insert into dncr.packages(package_id) 
                    values (@packageId) 
                    on conflict do nothing;

                    insert into dncr.emails(address, package_id, opted_marketing) 
                        select @email, id, @optedInToMarketing
                        from dncr.packages
                        where package_id = @packageId
                    on conflict do nothing;",
                    new {packageId, email, optedInToMarketing});

                return new CreateResult();
            }
        }

        public void EnsureSchema()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                connection.Execute(@"
                    create schema if not exists dncr;

                    create table if not exists dncr.packages(
                        id serial primary key,
                        package_id char(50) not null unique 
                    );

                    create table if not exists dncr.emails(
                        id serial primary key,
                        address char(50) not null,
                        opted_marketing boolean not null,
                        package_id int references dncr.packages(id) not null,
                        
                        constraint address_package_unique unique(address, package_id)
                    );");
            }
        }
    }
}