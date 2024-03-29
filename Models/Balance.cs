namespace BankSystem.Models
{
    public class Balance
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string AccountNo { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public decimal UsableAmount { get; set; }
        public decimal HoldAmount { get; set; }
    }
}
