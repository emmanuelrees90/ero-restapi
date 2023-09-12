using System.Data;
using System.Security.Cryptography;
using AutoMapper;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotnetAPI;
[Authorize]
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly DataContextDapper _dapper;

    private readonly AuthHelper _authHelper;

    private readonly ReusableSql _reusableSql;

    private readonly IMapper _mapper;

    public AuthController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);

        _reusableSql = new ReusableSql(config);

        _authHelper = new AuthHelper(config);

        _mapper = new Mapper(new MapperConfiguration(cfg => {
            cfg.CreateMap<UserForRegistrationDto, UserComplete>();
        }));
    }

    [AllowAnonymous]
    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDto userForRegistrationDto)
    {
        
        if (userForRegistrationDto.Password == userForRegistrationDto.PasswordConfirm)
        {
            string sqlCheckUserExists = "SELECT * FROM TutorialAppSchema.Auth WHERE Email = '" + userForRegistrationDto.Email + "'";
            IEnumerable<string> existingUser = _dapper.LoadData<string>(sqlCheckUserExists);
            if (existingUser.Count() == 0)
            {

                UserForLoginDto userForSetPassword = new UserForLoginDto()
                {
                    Email = userForRegistrationDto.Email,
                    Password = userForRegistrationDto.Password
                };

                if (_authHelper.setPassword(userForSetPassword))
                {
                    UserComplete userComplete = _mapper.Map<UserComplete>(userForRegistrationDto);
                    userComplete.Active = true;

                    if (_reusableSql.UpSertUser(userComplete))
                    {
                        return Ok();
                    }
                    throw new Exception("Failed to add user");
                }
                throw new Exception("Failed to register user.");


            }
            throw new Exception("User with this email already exists!");
        }
        throw new Exception("Passwords do not match");

    }

    [AllowAnonymous]
    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userForLoginDto)
    {

        string sqlForHashAndSalt = @"EXEC TutorialAppSchema.spLoginConfirmation_Get @Email = @EmailParam";

        DynamicParameters sqlParameters = new DynamicParameters();

        sqlParameters.Add("@EmailParam", userForLoginDto.Email, DbType.String);

        UserForLoginConfirmationDto userForConfirmation = _dapper.LoadDataSingleWithParameters<UserForLoginConfirmationDto>(sqlForHashAndSalt, sqlParameters);

        byte[] passwordHash = _authHelper.GetPasswordHash(userForLoginDto.Password, userForConfirmation.PasswordSalt);

        for (int index = 0; index < passwordHash.Length; index++)
        {
            if (passwordHash[index] != userForConfirmation.PasswordHash[index])
            {
                return StatusCode(401, "Incorrect password!");
            }
        }

        string userIdSql = @"SELECT UserId FROM TutorialAppSchema.Users WHERE Email = '" + userForLoginDto.Email + "'";

        int userId = _dapper.LoadDataSingle<int>(userIdSql);

        return Ok(new Dictionary<string, string> {
            {"token", _authHelper.CreateToken(userId)}
        });
    }

    [HttpGet("RefreshToken")]
    public string RefreshToken()
    {
        string userIdSql = @"SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = '" + User.FindFirst("userId")?.Value + "'";

        int userId = _dapper.LoadDataSingle<int>(userIdSql);
        return _authHelper.CreateToken(userId);
    }


    [HttpPut("ResetPassword")]
    public IActionResult ResetPassword(UserForLoginDto userForSetPassword)
    {
        if (_authHelper.setPassword(userForSetPassword))
        {
            return Ok();
        }
        throw new Exception("Failed to update password!");
    }


}


