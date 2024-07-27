using RealState.DbContexts;
using RealState.Models;

namespace RealState.Ops
{
    public class UserOps : BasicOps<User>
    {
        public UserOps(GlobalDbContext dbContext) : base(dbContext) { }

    }
}
