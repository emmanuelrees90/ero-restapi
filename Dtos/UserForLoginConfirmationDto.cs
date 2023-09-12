namespace DotnetAPI.Dtos
{
    partial class UserForLoginConfirmationDto
    {
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        public UserForLoginConfirmationDto()
        {
            
            if (PasswordSalt == null)
            {
                PasswordSalt = new byte[0];
            }
            if (PasswordHash == null)
            {
                PasswordHash = new byte[0];
            }
        }
    }
}