
using API.Extensions;
using Microsoft.EntityFrameworkCore;

namespace API;

public class LikeRepository : ILikeRepository
{

    private readonly DataContext _context;
    public LikeRepository(DataContext context)
    {
        _context = context;
    }
    public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await _context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    public async Task<PagedList<LikeDTO>> GetUserLikes(LikeParams likeParams)
    {
        var users = _context.Users.OrderBy( u => u.UserName).AsQueryable();
        var likes = _context.Likes.AsQueryable();

        if(likeParams.predicate == "liked"){
            likes = likes.Where(like => like.SourceUserId == likeParams.UserId);
            users = likes.Select(like => like.TargetUser);
        }

        if(likeParams.predicate == "likedBy"){
            likes = likes.Where(like => like.TargetUserId == likeParams.UserId);
            users = likes.Select(like => like.SourceUser);
        }

        var likedUsers = users.Select(user => new LikeDTO {
            UserName = user.UserName,
            KnownAs = user.KnownAs,
            Age = user.DateOfBirth.CalculateAge(),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
            City = user.City,
            Id = user.Id
        });

        return await PagedList<LikeDTO>.CreateAsync(likedUsers, likeParams.PageNumber, likeParams.PageSize);
    }

    public async Task<AppUser> GetUserWithLikes(int userId)
    {
        return await _context.Users
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
    }
}
