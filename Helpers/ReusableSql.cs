using System.Data;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Models;

namespace DotnetAPI.Helpers {
    public class ReusableSql
    {
        private IConfiguration _config;

        private readonly DataContextDapper _dapper;
        public ReusableSql(IConfiguration config)
        {
            _config = config;

            _dapper = new DataContextDapper(config);
        }

        public bool UpSertUser(UserComplete userComplete){
            string sql = @"EXEC TutorialAppSchema.spUser_Upsert
                 @FirstName = @FirstNameParam
                 , @LastName = @LastNameParam 
                 , @Email  = @EmailParam
                 , @Gender = @GenderParam
                 , @Active = @ActiveParam
                 , @JobTitle = @JobTitleParam
                 , @Department = @DepartmentParam
                , @Salary = @SalaryParam 
                , @UserId = @UserIdParam";

            DynamicParameters sqlParameters = new DynamicParameters();

            sqlParameters.Add("@FirstNameParam", userComplete.FirstName, DbType.String);
            sqlParameters.Add("@LastNameParam", userComplete.LastName, DbType.String);
            sqlParameters.Add("@EmailParam", userComplete.Email, DbType.String);
            sqlParameters.Add("@GenderParam", userComplete.Gender, DbType.String);
            sqlParameters.Add("@JobTitleParam", userComplete.JobTitle, DbType.String);
            sqlParameters.Add("@ActiveParam", userComplete.Active, DbType.Boolean);
            sqlParameters.Add("@SalaryParam", userComplete.Salary, DbType.Decimal);
            sqlParameters.Add("@DepartmentParam", userComplete.Department, DbType.String);
            sqlParameters.Add("@UserIdParam", userComplete.UserId, DbType.Int32);


        
            return _dapper.ExecuteSqlWithParameters(sql, sqlParameters);
        }
    }
}