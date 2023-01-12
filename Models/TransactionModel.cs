namespace PaymentIntegration.Models
{
    public class TransactionModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string StudentName { get; set; }
        public string Email { get; set;}
        public int Amount { get; set;}
        public string TransRef { get; set;}
        public bool Status { get; set;}
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
