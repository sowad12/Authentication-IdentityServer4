using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ClientApi.Models;

namespace ClientApi.Data
{
    public class ClientApiContext : DbContext
    {
        public ClientApiContext (DbContextOptions<ClientApiContext> options)
            : base(options)
        {
        }

        public DbSet<ClientApi.Models.Movie> Movie { get; set; } = default!;
    }
}
