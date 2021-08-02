using dotNET_Paynext.Models;

namespace dotNET_Paynext.Services.Interfaces
{
    public interface IValidationService
    {
        bool ValidateCard(string cardNumber);
        bool ValidateCVV(string creditCardValue, string cvvValue);
        bool ValidateIssueDate(string issueDate);
        bool IsValidCardOwner(string cardOwner);
        CreditCardType GetAcceptedCreditCardType(string creditCardNumber);
    }
}
