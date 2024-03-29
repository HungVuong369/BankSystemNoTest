namespace BankSystem.Dtos.Response
{
    public class BalanceTotal
    {
        public decimal TotalAmount
        {
            get
            {
                return UsableAmount + HoldAmount;
            }
        }
        public decimal UsableAmount { get; set; }
        public decimal HoldAmount { get; set; }
    }
}
