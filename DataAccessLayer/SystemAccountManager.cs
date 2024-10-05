using BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class SystemAccountManager
    {
        //using singleton pattern
        private static SystemAccountManager instance = null;
        public static readonly object instanceLock = new object();
        private SystemAccountManager() { }
        public static SystemAccountManager Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new SystemAccountManager();
                    }
                    return instance;
                }
            }
        }
        //------------------------------------------

        public IEnumerable<SystemAccount> GetAllSystemAccount()
        {
            IEnumerable<SystemAccount> list = new List<SystemAccount>();
            using (var context = new FunewsManagementFall2024Context())
            {
                list = context.SystemAccounts.ToList();
            }
            return list;
        }

        public SystemAccount GetSystemAccount(short id)
        {
            SystemAccount sa = new SystemAccount();
            using (var context = new FunewsManagementFall2024Context())
            {
                sa = context.SystemAccounts.FirstOrDefault(x => x.AccountId == id);

            }
            return sa;
        }

        public void UpdaterAccount(SystemAccount sa)
        {
                using (var context = new FunewsManagementFall2024Context())
                {
                    SystemAccount? oldSa = context.SystemAccounts.FirstOrDefault(x => x.AccountId == sa.AccountId);
                    if (oldSa != null)
                    {
                        if (oldSa.AccountEmail == sa.AccountEmail)
                        {
                            if (oldSa != null)
                            {
                                oldSa.AccountName = sa.AccountName;
                                oldSa.AccountEmail = sa.AccountEmail;
                                oldSa.AccountPassword = sa.AccountPassword;
                                context.SaveChanges();
                                return;
                            }
                        }
                        else
                        {
                            var existEmail = context.SystemAccounts.Where(x => x.AccountEmail == sa.AccountEmail).Any();
                            if (existEmail)
                            {
                                throw new IOException("Email exist!!");
                            }

                            oldSa.AccountName = sa.AccountName;
                            oldSa.AccountEmail = sa.AccountEmail;
                            oldSa.AccountPassword = sa.AccountPassword;
                            context.SaveChanges();
                            return;

                        }
                    }

                }
          

        }

        public void UpdaterAccount2(SystemAccount sa)
        {
   
                using (var context = new FunewsManagementFall2024Context())
                {
                    SystemAccount? oldSa = context.SystemAccounts.FirstOrDefault(x => x.AccountId == sa.AccountId);
                    if (oldSa != null)
                    {
                        if (oldSa.AccountEmail == sa.AccountEmail)
                        {
                            if (oldSa != null)
                            {
                                oldSa.AccountName = sa.AccountName;
                                oldSa.AccountEmail = sa.AccountEmail;
                                oldSa.AccountRole = sa.AccountRole;
                                oldSa.AccountPassword = sa.AccountPassword;
                                context.SaveChanges();
                                return;
                            }
                        }
                        else
                        {
                            var existEmail = context.SystemAccounts.Where(x => x.AccountEmail == sa.AccountEmail).Any();
                            if (existEmail)
                            {
                                throw new IOException("Email exist!!");
                            }

                            oldSa.AccountName = sa.AccountName;
                            oldSa.AccountEmail = sa.AccountEmail;
                            oldSa.AccountPassword = sa.AccountPassword;
                            oldSa.AccountRole = sa.AccountRole;
                            context.SaveChanges();
                            return;

                        }
                    }
                }

        }

        public void AddAccount(SystemAccount sa)
        {

                using (var context = new FunewsManagementFall2024Context())
                {
                    var existEmail = context.SystemAccounts.Where(x => x.AccountEmail == sa.AccountEmail).Any();
                    if (existEmail)
                    {
                        throw new IOException("Email exist!!");
                }
                    var lastItem = context.SystemAccounts.OrderByDescending(t => t.AccountId).FirstOrDefault();
                    if (lastItem != null)
                    {
                        sa.AccountId = (short)(lastItem.AccountId + 1);
                        context.SystemAccounts.Add(sa);
                        context.SaveChanges();
                    }

                }

        }

        public bool SystemAccountExists(short id)
        {
            try
            {
                using (var context = new FunewsManagementFall2024Context())
                {
                    return (context.SystemAccounts?.Any(e => e.AccountId == id)).GetValueOrDefault();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int RemoveAccount(short id)
        {
            try
            {
                using (var context = new FunewsManagementFall2024Context())
                {
                    var listNews = context.NewsArticles.Where(x => x.CreatedById == id).ToList();
                    if (listNews.Count > 0)
                    {
                        return 0;
                    }

                    var accountRemove = context.SystemAccounts.FirstOrDefault(x => x.AccountId == id);
                    context.SystemAccounts.Remove(accountRemove);
                    context.SaveChanges();
                    return 1;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
