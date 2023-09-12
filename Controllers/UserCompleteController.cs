using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{

    //private readonly ILogger<UserCompleteController> _logger;
    DataContextDapper _dapper;
    ReusableSql _reusableSql;

    public UserCompleteController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        
        _reusableSql = new ReusableSql(config);
    }

    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }



    [HttpGet("GetUsers")]
    public IEnumerable<UserComplete> GetUsers()
    {
        string sql = @"EXEC TutorialAppSchema.spUsers_Get";

        IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql);
        return users;
    }

    [HttpGet("GetUser/{userId}/{isActive}")]
    public IEnumerable<UserComplete> GetUser(int userId, bool isActive)
    {
        string stringParameters = "";
        string sql = @"EXEC TutorialAppSchema.spUsers_Get";

        DynamicParameters sqlParameters = new DynamicParameters();

        if (userId > 0)
        {
            stringParameters += ", @UserId=@UserIdParam";
            sqlParameters.Add("@UserIdParam", userId, DbType.Int32);
        }

        if (isActive)
        {
            stringParameters += ", @Active=@ActiveParam";
            sqlParameters.Add("@ActiveParam", isActive, DbType.Boolean);
        }

        if (stringParameters.Length > 0)
        {
            sql += stringParameters.Substring(1);
        }

        return _dapper.LoadDataWithParameters<UserComplete>(sql, sqlParameters); ;
    }

    [HttpPut("UpSertUser")]
    public IActionResult UpSertUser(UserComplete userComplete)
    {

        if (_reusableSql.UpSertUser(userComplete))
        {
            return Ok();
        }
        throw new Exception("Failed to upsert User");
    }


    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"EXEC TutorialAppSchema.spUser_Delete @UserId = @UserIdParam";

        DynamicParameters sqlParameters = new DynamicParameters();

        sqlParameters.Add("@UserIdParam", userId, DbType.Int32);

        if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
        {
            return Ok();
        }
        throw new Exception("Failed to Delete User");
    }
}
