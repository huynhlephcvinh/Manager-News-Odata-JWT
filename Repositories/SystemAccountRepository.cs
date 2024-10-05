using BusinessObject;
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories
{
    public interface ISystemAccountRepository 
    {
        public IEnumerable<SystemAccount> GetAllSystemAccount();
        public SystemAccount GetSystemAccount(short id);
        public void UpdateAccount(SystemAccount sa);
        public void AddAccount(SystemAccount sa);
        public int RemoveAccount(short id);
        public void UpdaterAccount2(SystemAccount sa);
        public bool SystemAccountExists(short id);
    }

    public class SystemAccountRepository : ISystemAccountRepository
    {
        public IEnumerable<SystemAccount> GetAllSystemAccount() => SystemAccountManager.Instance.GetAllSystemAccount();

        public SystemAccount GetSystemAccount(short id) => SystemAccountManager.Instance.GetSystemAccount(id);

        public void UpdateAccount(SystemAccount sa)
        {
            try
            {
                SystemAccountManager.Instance.UpdaterAccount(sa);
            }
            catch (IOException)
            {
                throw; // Re-throw the IOException
            }
            catch (Exception ex)
            {
                throw new IOException("Repository error", ex);
            }
        }

        public void AddAccount(SystemAccount sa)
        {
            try
            {
                SystemAccountManager.Instance.AddAccount(sa);
            }
            catch (IOException)
            {
                throw; // Re-throw the IOException
            }
            catch (Exception ex)
            {
                throw new IOException("Repository error", ex);
            }
        }

        public int RemoveAccount(short id) => SystemAccountManager.Instance.RemoveAccount(id);

        public void UpdaterAccount2(SystemAccount sa)
        {
            try
            {
                SystemAccountManager.Instance.UpdaterAccount2(sa);
            }
            catch (IOException)
            {
                throw; // Re-throw the IOException
            }
            catch (Exception ex)
            {
                throw new IOException("Repository error", ex);
            }
        }
          

        public bool SystemAccountExists(short id) => SystemAccountManager.Instance.SystemAccountExists(id);

    }
}
