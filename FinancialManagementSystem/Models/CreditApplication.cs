namespace FinancialManagementSystem.Models;

public class CreditApplication
{
    public int  CreditApplicationId { get; set; }
    public string ClientRfc { get; set; }
    public Models.CreditType SelectedCredit { get; set; }
    public Reference[] References { get; set; }
    public byte[] IdentificationPdf { get; set; }
    public byte[] ProofOfIncomePdf { get; set; }
    public byte[] ProofOfAddressPdf { get; set; }
    
    public class Reference
    {
        public string Name { get; set; }
        public string FirstLastname { get; set; }
        public string SecondLastname { get; set; }
        public string PhoneNumber { get; set; }
    }
}