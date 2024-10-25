using System.Linq;
using System.Threading.Tasks;
using in_houseLPIWeb.Data;
using Microsoft.EntityFrameworkCore;

namespace in_houseLPIWeb
{
    public class UserService
    {
        private readonly WebDbContext _dbContext;
        public UserService(WebDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<string> GetUserLevelAsync(string username)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Name == username);

            return user?.UserLevel;
        }
    }
}
