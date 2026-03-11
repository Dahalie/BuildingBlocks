using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Persistence.EfCore.DbContexts;

public class EfCoreDbContext(DbContextOptions options) : DbContext(options);
