using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdServer.Data
{
    public class ApplicationIdDbContext:IdentityDbContext
    {
        public ApplicationIdDbContext(DbContextOptions<ApplicationIdDbContext>options):base(options) { }
       
    }
}
