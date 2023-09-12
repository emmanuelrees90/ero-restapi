using AutoMapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{

    private DataContextEF _entityFramework;
    private IMapper _mapper;

    private IUserRepository _userRepository;

    public UserEFController(IConfiguration config, IUserRepository userRepository)
    {
        _entityFramework = new DataContextEF(config);
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserToAddDto, User>();
        }));
        _userRepository = userRepository;
    }



    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = _userRepository.GetUsers();
        return users;
    }

    [HttpGet("GetUser/{userId}")]
    public User GetUser(int userId)
    {
        return _userRepository.GetUser(userId);
    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        User userDb = _mapper.Map<User>(user);

        _userRepository.AddEntity<User>(userDb);
        if (_userRepository.SaveChanges())
        {
            return Ok();
        }

        throw new Exception("Failed to Add User");

    }

    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userDb = _userRepository.GetUser(user.UserId);
        if (userDb != null)
        {

            userDb.Active = user.Active;
            userDb.FirstName = user.FirstName;
            userDb.LastName = user.LastName;
            userDb.Email = user.Email;
            userDb.Gender = user.Gender;
            userDb.Active = user.Active;
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
        }
        throw new Exception("Failed to Update User");

    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {

        User? userDb = _userRepository.GetUser(userId);
        if (userDb != null)
        {
            _userRepository.RemoveEntity<User>(userDb);
            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
        }
        throw new Exception("Failed to Delete User");

    }
}