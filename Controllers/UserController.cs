using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{

    //private readonly ILogger<UserController> _logger;
    DataContextDapper _dapper;

    public UserController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        Console.WriteLine(config.GetConnectionString("DefaultConnection"));
    }

    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }



    [HttpGet("GetUsers/")]
    public IEnumerable<User> GetUsers()
    {
        string sql = @"
        SELECT [UserId],
            [FirstName],
            [LastName],
            [Email],
            [Gender],
            [Active] 
        FROM TutorialAppSchema.Users";
        IEnumerable<User> users = _dapper.LoadData<User>(sql);
        return users;
    }

    [HttpGet("GetUser/{userId}")]
    public User GetUser(string userId)
    {
        string sql = @"
            SELECT [UserId],
            [FirstName],
            [LastName],
            [Email],
            [Gender],
            [Active] 
        FROM TutorialAppSchema.Users WHERE UserId =" + userId.ToString();
        User user = _dapper.LoadDataSingle<User>(sql);
        return user;
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        string sql = @"INSERT INTO TutorialAppSchema.Users(   
            [FirstName],
            [LastName],
            [Email],
            [Gender],
            [Active]
            ) 
        VALUES (" +
                "'" + user.FirstName +
                "','" + user.LastName +
                "','" + user.Email +
                "','" + user.Gender +
                "','" + user.Active.ToString() +
                "')";

        bool isSaved = _dapper.ExecuteSql(sql);
        if (isSaved)
        {
            return Ok();
        }
        else
        {
            throw new Exception("Failed to Add User");
        }
    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string sql = @"
        UPDATE TutorialAppSchema.Users 
                SET [FirstName] = '" + user.FirstName +
                "', [LastName] = '" + user.LastName +
                "', [Email]  = '" + user.Email +
                "', [Gender] = '" + user.Gender +
                "', [Active] = '" + user.Active.ToString() +
            "' WHERE UserId = " + user.UserId;
        Console.WriteLine(sql);
        bool isSaved = _dapper.ExecuteSql(sql);
        if (isSaved)
        {
            return Ok();
        }
        else
        {
            throw new Exception("Failed to Update User");
        }

    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"
        DELETE FROM TutorialAppSchema.Users WHERE UserId = " + userId.ToString();


        Console.WriteLine(sql);
        bool isSaved = _dapper.ExecuteSql(sql);
        if (isSaved)
        {
            return Ok();
        }
        throw new Exception("Failed to Delete User");
    }
}
