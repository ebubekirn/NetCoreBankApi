using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SocialNetworkBE.Models;
using SocialNetworkBE.Services.Interfaces;
using System.Text.RegularExpressions;

namespace SocialNetworkBE.Controllers
{
    [ApiController]
    [Route("api/v3/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private ITransactionService _transactionService;
        IMapper _mapper;
        public TransactionsController(ITransactionService transactionService, IMapper mapper)
        {
            _transactionService = transactionService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("create_new_transactions")]
        public IActionResult CreateNewTransactions([FromBody] TransactionRequestDto transactionRequest)
        {
            if (!ModelState.IsValid) return BadRequest(transactionRequest);

            var transaction = _mapper.Map<Transaction>(transactionRequest);
            return Ok(_transactionService.CreateNewTransaction(transaction));
        }

        [HttpPost]
        [Route("make_deposit")]
        public IActionResult MakeDeposit(string AccountNumber, decimal Amount, string TransactionPin)
        {
            if (!Regex.IsMatch(AccountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$")) return BadRequest("Account number must be 10 digit");
            return Ok(_transactionService.MakeDeposit(AccountNumber, Amount, TransactionPin));
        }

        [HttpPost]
        [Route("make_withdrawal")]
        public IActionResult MakeWithdrawal(string AccountNumber, decimal Amount, string TransactionPin)
        {
            if (!Regex.IsMatch(AccountNumber, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$")) return BadRequest("Account number must be 10 digit");
            return Ok(_transactionService.MakeWithdrawal(AccountNumber, Amount, TransactionPin));
        }

        [HttpPost]
        [Route("make_funds_transfer")]
        public IActionResult MakeFundsTransfer(string FromAccount, string ToAccount, decimal Amount, string TransactionPin)
        {
            if (!Regex.IsMatch(FromAccount, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$") || !Regex.IsMatch(ToAccount, @"^[0][1-9]\d{9}$|^[1-9]\d{9}$")) return BadRequest("Account number must be 10 digit");

            return Ok(_transactionService.MakeFundsTransfer(FromAccount, ToAccount, Amount, TransactionPin));
        }
    }
}
