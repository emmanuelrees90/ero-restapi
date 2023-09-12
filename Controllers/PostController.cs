using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;


        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }


        [HttpGet("{postId}/user/{userId}/search/{searchParam}")]
        public IEnumerable<Post> GetPosts(int postId = 0, int userId = 0, string searchParam = "none")
        {

            string sql = @"EXEC TutorialAppSchema.spPosts_Get";
            string stringParameters = "";
            DynamicParameters sqlParameters = new DynamicParameters();

            if (userId > 0)
            {
                stringParameters += ", @UserId = @UserIdParam";
                sqlParameters.Add("@UserIdParam", userId, DbType.Int32);
            }

            if (postId > 0)
            {
                stringParameters += ", @PostId = @PostIdParam";
                sqlParameters.Add("@PostIdParam", postId, DbType.Int32);
            }


            if (searchParam != "none")
            {
                stringParameters += ", @SearchValue= @SearchParam";
                sqlParameters.Add("@SearchParam", searchParam, DbType.String);
            }

            if (stringParameters.Length > 0)
            {
                sql += stringParameters.Substring(1);
            }

            return _dapper.LoadDataWithParameters<Post>(sql, sqlParameters);
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts(int userId)
        {
            DynamicParameters sqlParameters = new DynamicParameters();
            
            sqlParameters.Add("@UserIdParam", this.User.FindFirst("userId")?.Value, DbType.Int32);

            string sql = @"EXEC TutorialAppSchema.spPosts_Get @UserId=@UserIdParam";


            return _dapper.LoadDataWithParameters<Post>(sql, sqlParameters);
        }

        [HttpPut("UpsertPost")]
        public IActionResult AddPost(Post postToUpsert)
        {

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@UserIdParam", this.User.FindFirst("userId")?.Value, DbType.Int32);
            sqlParameters.Add("@PostContentParam", postToUpsert.PostContent, DbType.String);
            sqlParameters.Add("@PostTitleParam", postToUpsert.PostTitle, DbType.String);
            string sql = @"EXEC TutorialAppSchema.spPosts_Upsert
                @UserId = @UserIdParam, @PostTitle =@PostTitleParam , @PostContent=@PostContentParam";


            if (postToUpsert.PostId > 0)
            {
                sql += ", @PostId = @PostIdParam";
                sqlParameters.Add("@PostIdParam", postToUpsert.PostId, DbType.Int32);
            }


            if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
            {
                return Ok();
            }

            throw new Exception("Failed to upsert post!");
        }

        [HttpDelete("{postId}")]
        public IActionResult DeletePost(int postId)
        {
            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@PostIdParam", postId, DbType.Int32);
            sqlParameters.Add("@UserIdParam", this.User.FindFirst("userId")?.Value, DbType.Int32);
            string sql = @"EXEC TutorialAppSchema.spPost_Delete @PostId = @PostIdParam, @UserId = @UserIdParam";

            if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
            {
                return Ok();
            }

            throw new Exception("Failed to delete post!");
        }

    }
}