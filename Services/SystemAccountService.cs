using BusinessObject;
using DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repositories;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface ISystemAccountService 
    {
        AuthDTO Authenticate(string email, string password);
        SystemAccount GetSystemAccount(short id);
        IEnumerable<SystemAccount> GetAllSystemAccount();
        void UpdaterAccount(SystemAccount sa);
        void UpdaterAccount2(SystemAccount sa);
        void AddAccount(SystemAccount sa);
        bool SystemAccountExists(short id);
        int RemoveAccount(short id);
    }

    public class SystemAccountService : ISystemAccountService
    {
        public ISystemAccountRepository _systemAccountRepository;
        public IConfiguration _configuration;
        public JwtGenerator _jwtGenerator;
        public SystemAccountService(ISystemAccountRepository systemAccountRepository, IConfiguration configuration) {
            _systemAccountRepository = systemAccountRepository;
            _configuration = configuration;
            _jwtGenerator = new JwtGenerator(configuration);
        }

        public AuthDTO Authenticate(string email, string password)
        {
            IConfiguration config = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", true, true)
                                .Build();
            var adminEmail = config["AdminAccount:Email"];
            var adminPassword = config["AdminAccount:Password"];
            var user =  _systemAccountRepository.GetAllSystemAccount().Where(x=>x.AccountEmail == email).FirstOrDefault();
            if (user == null || user.AccountPassword != password)
            {
                if(email == adminEmail && password == adminPassword)
                {
                    SystemAccount sa = new SystemAccount {
                        AccountEmail = email,
                        AccountPassword = password,
                        AccountRole = 3
                    };
                    var tokenAdmin = _jwtGenerator.GenerateTokenAdmin(sa);
                    return new AuthDTO
                    {
                        IsAuthenticated = true,
                        Token = tokenAdmin,
                        Expiration = DateTime.UtcNow.AddHours(1),
                        Message = "Authentication successful",
                        Role = sa.AccountRole,
                        Name = "Admin"
                    };

                }
                return new AuthDTO
                {
                    IsAuthenticated = false,
                    Message = "Email or password is not correct"
                };
            }

            var token = _jwtGenerator.GenerateToken(user);
            return new AuthDTO
            {
                IsAuthenticated = true,
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(1),
                Message = "Authentication successful",
                Role = user.AccountRole,
                Name = user.AccountName
            };
        }


        public SystemAccount GetSystemAccount(short id)
        {
            SystemAccount sa = _systemAccountRepository.GetSystemAccount(id);
            return sa;
        }

        public IEnumerable<SystemAccount> GetAllSystemAccount()
        {
            return _systemAccountRepository.GetAllSystemAccount();
        }

        public void UpdaterAccount(SystemAccount sa)
        {
            try
            {

                _systemAccountRepository.UpdateAccount(sa);
            }
            catch (IOException)
            {
                throw; // Re-throw the IOException
            }
            catch (Exception ex)
            {
                throw new IOException("Service error", ex);
            }
        }

        public void UpdaterAccount2(SystemAccount sa)
        {
            try
            {
                _systemAccountRepository.UpdaterAccount2(sa);
            }
            catch (IOException)
            {
                throw; // Re-throw the IOException
            }
            catch (Exception ex)
            {
                throw new IOException("Service error", ex);
            }

        }

        public void AddAccount(SystemAccount sa)
        {
            try
            {
                _systemAccountRepository.AddAccount(sa);
             }
            catch (IOException)
            {
                throw; // Re-throw the IOException
            }
            catch (Exception ex)
            {
                throw new IOException("Service error", ex);
            }

        }

        public bool SystemAccountExists(short id)
        {
            return _systemAccountRepository.SystemAccountExists(id);
        }

        public int RemoveAccount(short id)
        {
           return _systemAccountRepository.RemoveAccount(id);
        }


    }
}
