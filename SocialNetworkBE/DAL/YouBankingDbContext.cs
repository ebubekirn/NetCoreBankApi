using Microsoft.EntityFrameworkCore;
using SocialNetworkBE.Models;

namespace SocialNetworkBE.DAL
{
    public class YouBankingDbContext : DbContext
    {
        public YouBankingDbContext(DbContextOptions<YouBankingDbContext> options) : base(options)
        {

        }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
