using Microsoft.EntityFrameworkCore;
using PaymentIntegration.Models;

namespace PaymentIntegration.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        

        public DbSet<TransactionModel> transactionModels { get; set; }

        
    }
}
