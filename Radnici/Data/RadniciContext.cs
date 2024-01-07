using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Radnici.Models;

namespace Radnici.Data
{
    public class RadniciContext : DbContext
    {
        public RadniciContext (DbContextOptions<RadniciContext> options)
            : base(options)
        {
        }

        public DbSet<Radnici.Models.Radnik> Radnik { get; set; } = default!;
    }
}
