using dotNET_Paynext.Models;
using dotNET_Paynext.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace dotNET_Paynext.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValidationController : Controller
    {
        private readonly IValidationService _validationService;
        public ValidationController(IValidationService validationService)
        {
            _validationService = validationService;
        }
        [HttpPost]
        public IActionResult Validate(string cardOwner, string creditCardNumber, string issueDate, string CVC)
        {
            if (!string.IsNullOrWhiteSpace(cardOwner))
            {
                if (!_validationService.IsValidCardOwner(cardOwner))
                {
                    return BadRequest("Card owner contains incorrect information. It can not contain special characters or numbers.");
                }
            }
            else
            {
                return BadRequest("Card owner must be filled in.");
            }
            if (string.IsNullOrWhiteSpace(CVC))
            {
                return BadRequest("CVC must be filled in.");
            }

            var validCreditCardType = CreditCardType.Unknown;

            if (!string.IsNullOrWhiteSpace(creditCardNumber))
            {
                CreditCardType creditCardType = _validationService.GetAcceptedCreditCardType(creditCardNumber);
                if (creditCardType == CreditCardType.Unknown)
                {
                    return BadRequest("This type of credit card is not valid. Only Master Card, Visa and American Express are supported.");
                }
                else
                {
                    validCreditCardType = creditCardType;
                }

                if (!_validationService.ValidateCard(creditCardNumber))
                {
                    return BadRequest("Credit card information is not valid.");
                }

                if (!_validationService.ValidateCVV(creditCardNumber, CVC))
                {
                    return BadRequest("CVC is not valid.");
                }
            }
            else
            {
                return BadRequest("Creditcard number must be filled in.");
            }

            if (!string.IsNullOrWhiteSpace(issueDate))
            {
                if (!_validationService.ValidateIssueDate(issueDate))
                {
                    return BadRequest("Credit card issuedate is not valid. Please use format MM/yyyy or check its validity.");
                }
            }
            else
            {
                return BadRequest("Creditcard expiry date must be filled in.");
            }

            return Ok(validCreditCardType.ToString());
        }
    }
}
