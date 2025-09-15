using AcademiaDoZe.Application.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace AcademiaDoZe.Application.DependencyInjection
{
    public class RepositoryConfig
    {
        public required string ConnectionString { get; set; }
        public required EAppDatabaseType DatabaseType { get; set; }
    }
}