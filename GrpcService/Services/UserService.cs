using Grpc.Core;
using GrpcService.Database;
using Microsoft.EntityFrameworkCore;

namespace GrpcService.Services
{
    public class UserService : UserContract.UserContractBase
    {
        private readonly ApplicationDbContext _dbContext;

        public UserService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public override async Task<Key> Create(User request, ServerCallContext context)
        {
            _dbContext.Users.Add(request);
            await _dbContext.SaveChangesAsync();
            return new Key { Id = request.Id };
        }

        public override async Task<Empty> Delete(Key request, ServerCallContext context)
        {
            var user = await _dbContext.Users.FindAsync(request.Id);
            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return new Empty();
        }

        public override async Task<User> Get(Key request, ServerCallContext context)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == request.Id);
            return user;
        }

        public override async Task<Users> GetAll(Empty request, ServerCallContext context)
        {
            var users = await _dbContext.Users.ToListAsync();
            var response = new Users();
            response.Items.AddRange(users);
            return response;
        }

        public override async Task<Empty> Update(User request, ServerCallContext context)
        {
            _dbContext.Users.Attach(request);
            _dbContext.Entry(request).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
            return new Empty();
        }
    }
}