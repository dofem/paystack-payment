using Microsoft.AspNetCore.Mvc;
using PaymentIntegration.Data;
using PaymentIntegration.Models;
using PayStack.Net;

namespace PaymentIntegration.Controllers
{
    public class PaySchoolFeeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string token;
        private PayStackApi payStack;
        private readonly ApplicationDbContext _context;

        public PaySchoolFeeController(IConfiguration configuration,ApplicationDbContext context)
        {
            _configuration = configuration;
            token = _configuration["Payment:PaystackSK"];
            payStack = new PayStackApi(token);
            _context = context;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(PaySchoolFeeViewModel paySchoolFee)
        {
            TransactionInitializeRequest request = new()
            {
                AmountInKobo = paySchoolFee.Amount * 100,
                Email = paySchoolFee.Email,
                Reference = GenerateReference(),
                Currency = "NGN",
                CallbackUrl = "http://localhost:5904/PaySchoolFee/Verify"

            };

            TransactionInitializeResponse response = payStack.Transactions.Initialize(request);
            if (response.Status)
            {
                var Transaction = new TransactionModel()
                {
                    Amount = paySchoolFee.Amount,
                    Email = paySchoolFee.Email,
                    TransRef = request.Reference,
                    StudentName = paySchoolFee.StudentName,
                };
                await _context.transactionModels.AddAsync(Transaction);
                await _context.SaveChangesAsync();
                return Redirect(response.Data.AuthorizationUrl);
            }
            ViewData["error"] = response.Message; 
            return View();
        }


        [HttpGet]
        public async Task <ActionResult<List<TransactionModel>>> ViewPaymentList()
        {
            var transactions = _context.transactionModels.Where(x => x.Status == true).ToList();
            ViewData["transactions"] = transactions;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Verify(string reference)
        { 
            TransactionVerifyResponse response = payStack.Transactions.Verify(reference);
            if(response.Data.Status == "success")
            {
                var transaction = _context.transactionModels.Where(x=>x.TransRef== reference).FirstOrDefault(); 
                if (transaction != null) 
                {
                    transaction.Status = true;
                    _context.transactionModels.Update(transaction);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("ViewPaymentList");
                }
               
            }
            ViewData["error"] = response.Data.GatewayResponse;
            return RedirectToAction("Index");
        }




        private string GenerateReference()
        {
           var reference = $"{Guid.NewGuid().ToString().Replace("-", "").Substring(1, 10)}";
            return reference;
        }

    }
}
