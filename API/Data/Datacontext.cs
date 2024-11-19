using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Datacontext(DbContextOptions options) : DbContext(options)
{
    public required DbSet<AppUser> Users { get; set; }
}