namespace API;

public interface ILikeRepository
{
    Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);

    Task<AppUser> GetUserWithLikes(int userId);

    Task<PagedList<LikeDTO>> GetUserLikes(LikeParams likeParams);
}
