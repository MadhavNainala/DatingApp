using API.Controllers;
using API.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikeController : BaseApiController
{
    private readonly IUserRepository _userRepository;
    private readonly ILikeRepository _likeRepository;

    public LikeController(IUserRepository userRepository, ILikeRepository likeRepository)
    {
        _userRepository = userRepository;
        _likeRepository = likeRepository;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username){
        var sourceUserId = int.Parse(User.GetUserId());
        var likedUser = await _userRepository.GetUserByUsernameAsync(username);
        var sourceUser = await _likeRepository.GetUserWithLikes(sourceUserId);

        if(likedUser == null) return NotFound();

        if(sourceUser.UserName == username) return BadRequest("you cannot like yourself");

        var userLike = await _likeRepository.GetUserLike(sourceUserId, likedUser.Id);

        if(userLike != null) return BadRequest("you already liked the user");

        userLike = new UserLike {
            SourceUserId = sourceUserId,
            TargetUserId = likedUser.Id
        };

        sourceUser.LikedUsers.Add(userLike);

        if(await _userRepository.SaveAllAsync()) return Ok();

        return BadRequest("failed save user");
    }

    [HttpGet]

    public async Task<ActionResult<PagedList<LikeDTO>>> GetUserLikes([FromQuery]LikeParams likeParams){
        likeParams.UserId = int.Parse(User.GetUserId());
        var users = await _likeRepository.GetUserLikes(likeParams);
        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.TotalPages, users.PageSize, users.TotalCount));

        return Ok(users);
    }
}
