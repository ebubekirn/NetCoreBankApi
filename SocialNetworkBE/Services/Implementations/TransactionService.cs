using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialNetworkBE.DAL;
using SocialNetworkBE.Models;
using SocialNetworkBE.Services.Interfaces;
using SocialNetworkBE.Utils;
using System.Text.Json.Serialization;

namespace SocialNetworkBE.Services.Implementations
{
    public class TransactionService : ITransactionService
    {
        private YouBankingDbContext _dbContext;
        ILogger<TransactionService> _logger;
        private AppSettings _settings;
        private static string _ourBankSettlementaccount;
        Response _response;
        private readonly IAccountService _accountService;

        public TransactionService(YouBankingDbContext dbContext, ILogger<TransactionService> logger, IOptions<AppSettings> settings, Response response, IAccountService accountService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _settings = settings.Value;
            _ourBankSettlementaccount = _settings.OurBankSettlementAccount;
            _response = response;
            _accountService = accountService;
        }

        public Response CreateNewTransaction(Transaction transaction)
        {
            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();
            _response.ResponseCode = "00";
            _response.ResponseMessage = "Transaction created succesfully";
            _response.Data = null;

            return _response;
        }

        public Response FindTransactionByDate(DateTime date)
        {
            var transaction = _dbContext.Transactions.Where(x => x.TransactionDate == date).ToList();
            _response.ResponseCode = "00";
            _response.ResponseMessage = "Transaction created succesfully";
            _response.Data = transaction;

            return _response;
        }

        public Response MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin)
        {
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            // First check that user - account is valid
            var authUser = _accountService.Authenticate(AccountNumber, TransactionPin);
            if (authUser == null) throw new ApplicationException("Invalid Credentials");

            // So validation passes
            try
            {
                // For deposit our bankSettlementAccount is the source giving money to the users account
                sourceAccount = _accountService.GetByAccountNumber(_ourBankSettlementaccount);
                destinationAccount = _accountService.GetByAccountNumber(AccountNumber);

                // Update account balances
                sourceAccount.CurrentAccountBalance -= Amount;
                destinationAccount.CurrentAccountBalance += Amount;

                // Check if there is update
                if ((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) && (_dbContext.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    // Transaction is succesfull
                    transaction.TransactionStatus = TranStatus.Success;
                    _response.ResponseCode = "00";
                    _response.ResponseMessage = "Transaction Succesful";
                    _response.Data = null;
                }
                else
                {
                    // Transaction is unsuccesfull
                    transaction.TransactionStatus = TranStatus.Failed;
                    _response.ResponseCode = "02";
                    _response.ResponseMessage = "Transaction Unsuccesful";
                    _response.Data = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AN ERROR OCCURED... => {ex.Message}");
            }

            // Set other props of transaction here
            transaction.TransactionType = TranType.Deposit;
            transaction.TransactionSourceAccount = _ourBankSettlementaccount;
            transaction.TransactionDestinationAccount = AccountNumber;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionsParticulars = $"NEW TRANSACTION FROM SOURCE => {JsonConvert.SerializeObject(transaction.TransactionSourceAccount)} TO DESTINATION ACCOUNT => {JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} ON DATE => {transaction.TransactionDate} FOR AMOUNT => {JsonConvert.SerializeObject(transaction.TransactionAmount)} TRANSACTION TYPE => {JsonConvert.SerializeObject(transaction.TransactionType)} TRANSACTION STATUS => {transaction.TransactionStatus}";

            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            return _response;
        }

        public Response MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            // First check that user - account is valid
            var authUser = _accountService.Authenticate(FromAccount, TransactionPin);
            if (authUser == null) throw new ApplicationException("Invalid Credentials");

            // So validation passes
            try
            {
                // For deposit our bankSettlementAccount is the destination getting money from the users account
                sourceAccount = _accountService.GetByAccountNumber(FromAccount);
                destinationAccount = _accountService.GetByAccountNumber(ToAccount);

                // Update account balances
                sourceAccount.CurrentAccountBalance -= Amount;
                destinationAccount.CurrentAccountBalance += Amount;

                // Check if there is update
                if ((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) && (_dbContext.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    // Transaction is succesfull
                    transaction.TransactionStatus = TranStatus.Success;
                    _response.ResponseCode = "00";
                    _response.ResponseMessage = "Transaction Succesful";
                    _response.Data = null;
                }
                else
                {
                    // Transaction is unsuccesfull
                    transaction.TransactionStatus = TranStatus.Failed;
                    _response.ResponseCode = "02";
                    _response.ResponseMessage = "Transaction Unsuccesful";
                    _response.Data = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AN ERROR OCCURED... => {ex.Message}");
            }

            // Set other props of transaction here
            transaction.TransactionType = TranType.Transfer;
            transaction.TransactionSourceAccount = FromAccount;
            transaction.TransactionDestinationAccount = ToAccount;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionsParticulars = $"NEW TRANSACTION FROM SOURCE => {JsonConvert.SerializeObject(transaction.TransactionSourceAccount)} TO DESTINATION ACCOUNT => {JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} ON DATE => {transaction.TransactionDate} FOR AMOUNT => {JsonConvert.SerializeObject(transaction.TransactionAmount)} TRANSACTION TYPE => {JsonConvert.SerializeObject(transaction.TransactionType)} TRANSACTION STATUS => {transaction.TransactionStatus}";

            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            return _response;
        }

        public Response MakeWithdrawal(string AccountNumber, decimal Amount, string TransactionPin)
        {
            Account sourceAccount;
            Account destinationAccount;
            Transaction transaction = new Transaction();

            // First check that user - account is valid
            var authUser = _accountService.Authenticate(AccountNumber, TransactionPin);
            if (authUser == null) throw new ApplicationException("Invalid Credentials");

            // So validation passes
            try
            {
                // For deposit our bankSettlementAccount is the destination getting money from the users account
                sourceAccount = _accountService.GetByAccountNumber(AccountNumber);
                destinationAccount = _accountService.GetByAccountNumber(_ourBankSettlementaccount);

                // Update account balances
                sourceAccount.CurrentAccountBalance -= Amount;
                destinationAccount.CurrentAccountBalance += Amount;

                // Check if there is update
                if ((_dbContext.Entry(sourceAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified) && (_dbContext.Entry(destinationAccount).State == Microsoft.EntityFrameworkCore.EntityState.Modified))
                {
                    // Transaction is succesfull
                    transaction.TransactionStatus = TranStatus.Success;
                    _response.ResponseCode = "00";
                    _response.ResponseMessage = "Transaction Succesful";
                    _response.Data = null;
                }
                else
                {
                    // Transaction is unsuccesfull
                    transaction.TransactionStatus = TranStatus.Failed;
                    _response.ResponseCode = "02";
                    _response.ResponseMessage = "Transaction Unsuccesful";
                    _response.Data = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AN ERROR OCCURED... => {ex.Message}");
            }

            // Set other props of transaction here
            transaction.TransactionType = TranType.Withdrawal;
            transaction.TransactionSourceAccount = AccountNumber;
            transaction.TransactionDestinationAccount = _ourBankSettlementaccount;
            transaction.TransactionAmount = Amount;
            transaction.TransactionDate = DateTime.Now;
            transaction.TransactionsParticulars = $"NEW TRANSACTION FROM SOURCE => {JsonConvert.SerializeObject(transaction.TransactionSourceAccount)} TO DESTINATION ACCOUNT => {JsonConvert.SerializeObject(transaction.TransactionDestinationAccount)} ON DATE => {transaction.TransactionDate} FOR AMOUNT => {JsonConvert.SerializeObject(transaction.TransactionAmount)} TRANSACTION TYPE => {JsonConvert.SerializeObject(transaction.TransactionType)} TRANSACTION STATUS => {transaction.TransactionStatus}";

            _dbContext.Transactions.Add(transaction);
            _dbContext.SaveChanges();

            return _response;
        }
    }
}
