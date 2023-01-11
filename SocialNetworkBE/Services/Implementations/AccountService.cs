using SocialNetworkBE.DAL;
using SocialNetworkBE.Models;
using System.Text;

namespace SocialNetworkBE.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private YouBankingDbContext _dbContext;

        public AccountService(YouBankingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Account Authenticate(string AccountNumber, string Pin)
        {
            var account = _dbContext.Accounts.Where(x => x.AccountNumberGenerated == AccountNumber).SingleOrDefault();
            if (account == null)
                return null;
            if (!VerifyPinHash(Pin, account.PinHash, account.PinSalt))
                return null;
            return account;
        }

        private static bool VerifyPinHash(string Pin, byte[] pinHash, byte[] pinSalt)
        {
            if (string.IsNullOrWhiteSpace(Pin)) throw new ArgumentNullException("Pin");
            using (var hmac = new System.Security.Cryptography.HMACSHA512(pinSalt))
            {
                var computedPinHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Pin));
                for (int i = 0; i < computedPinHash.Length; i++)
                {
                    if (computedPinHash[i] != pinHash[i]) return false;
                }
            }
            return true;
        }

        public Account Create(Account account, string Pin, string ConfirmPin)
        {
            // This is to create a new account
            if (_dbContext.Accounts.Any(x => x.Email == account.Email)) throw new ApplicationException("An account already exists with this email");
            // Validate pin
            if (!Pin.Equals(ConfirmPin)) throw new ArgumentException("Pins do not match", "Pin");
            // We are hashing/encrypting pin
            byte[] pinHash, pinSalt;
            CreatePinHash(Pin, out pinHash, out pinSalt);

            account.PinHash = pinHash;
            account.PinSalt = pinSalt;

            _dbContext.Accounts.Add(account);
            _dbContext.SaveChanges();
            return account;
        }

        private static void CreatePinHash(string pin, out byte[] pinHash, out byte[] pinSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                pinSalt = hmac.Key;
                pinHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(pin));
            }
        }

        public void Delete(int Id)
        {
            var account = _dbContext.Accounts.Find(Id);
            if (account != null)
            {
                _dbContext.Accounts.Remove(account);
                _dbContext.SaveChanges();
            }
        }

        public IEnumerable<Account> GetAllAccount()
        {
            return _dbContext.Accounts.ToList();
        }

        public Account GetByAccountNumber(string AccountNumber)
        {
            var account = _dbContext.Accounts.Where(x => x.AccountNumberGenerated == AccountNumber).FirstOrDefault();
            if (account == null) return null;

            return account;
        }

        public Account GetById(int Id)
        {
            var account = _dbContext.Accounts.Where(x => x.Id == Id).FirstOrDefault();
            if (account == null) return null;

            return account;
        }

        public void Update(Account account, string Pin = null)
        {
            var accountToBeUpdated = _dbContext.Accounts.Where(x => x.Email == account.Email).SingleOrDefault();
            if (accountToBeUpdated == null) throw new ApplicationException("Account does not exist");
            // If it exists, let's listen for user wanting to change any of his properties
            if (!string.IsNullOrWhiteSpace(account.Email))
            {
                // This means the user wishes to change his email
                // Check if the one he's changing to is not already taken
                if (_dbContext.Accounts.Any(x => x.Email == account.Email)) throw new ApplicationException("This email " + account.Email + " already exists");
                // else change email for him
                accountToBeUpdated.Email = account.Email;
            }

            // Actually we want to allow the user to be able to change only email and phonenumber and pin
            if (!string.IsNullOrWhiteSpace(account.PhoneNumber))
            {
                // This means the user wishes to change his phone
                // Check if the one he's changing to is not already taken
                if (_dbContext.Accounts.Any(x => x.PhoneNumber == account.PhoneNumber)) throw new ApplicationException("This Phone Number " + account.PhoneNumber + " already exists");
                // else change email for him
                accountToBeUpdated.PhoneNumber = account.PhoneNumber;
            }

            if (!string.IsNullOrWhiteSpace(Pin))
            {
                // This means the user wishes to change his pin

                byte[] pinHash, pinSalt;
                CreatePinHash(Pin, out pinHash, out pinSalt);

                accountToBeUpdated.PinHash = pinHash;
                accountToBeUpdated.PinSalt = pinSalt;
            }
            accountToBeUpdated.DateLastUpdated = DateTime.Now;

            // Now persist this update to db
            _dbContext.Accounts.Update(accountToBeUpdated);
            _dbContext.SaveChanges();
        }
    }
}
