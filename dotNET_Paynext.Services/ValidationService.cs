using dotNET_Paynext.Models;
using dotNET_Paynext.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace dotNET_Paynext.Services
{
    public class ValidationService : IValidationService
    {
        public List<CreditCard> creditCardPatterns = new List<CreditCard>
        {
            new CreditCard
            {
                CreditCardType = CreditCardType.MasterCard,
                RegexPattern = @"^5[1-5][0-9]{14}$|^2(?:2(?:2[1-9]|[3-9][0-9])|[3-6][0-9][0-9]|7(?:[01][0-9]|20))[0-9]{12}$"
            },
            new CreditCard
            {
                CreditCardType = CreditCardType.Visa,
                RegexPattern =  @"^4[0-9]{12}(?:[0-9]{3})?$"
            },
            new CreditCard
            {
                CreditCardType = CreditCardType.AmericanExpress,
                RegexPattern = @"^3[47][0-9]{13}$",
            }
        };
        /// <summary>
        /// Takes a creditcard number and validates it on a list of valid creditcard regex patterns.
        /// </summary>
        /// <param name="creditCardValue">The creditcard number</param>
        /// <returns>The credit card type. Returns unknown if not valid.</returns>
        public CreditCardType GetAcceptedCreditCardType(string creditCardValue)
        {
            foreach (var creditCardPattern in creditCardPatterns)
            {
                var regex = new Regex(creditCardPattern.RegexPattern);
                var val = RemoveAllNonDigitCharacters(creditCardValue);
                if (regex.IsMatch(val))
                {
                    return creditCardPattern.CreditCardType;
                }
            }

            return CreditCardType.Unknown;
        }
        /// <summary>
        /// Implementation of the Luhn Algorithm to check card validation.
        /// </summary>
        /// <param name="cardNumber">The credit card number.</param>
        /// <returns>True if valid, false if invalid.</returns>
        public bool ValidateCard(string cardNumber)
        {
            var onlyNumericValues = new Regex("^[0-9]+$");
            if (!onlyNumericValues.IsMatch(cardNumber.Replace(" ", "")))
            {
                return false;
            }
            // remove all non digit characters
            var value = RemoveAllNonDigitCharacters(cardNumber);
            var sum = 0;
            var shouldDouble = false;

            // loop through values starting at the rightmost side
            for (var i = value.Length - 1; i >= 0; i--)
            {
                var digit = int.Parse(value[i].ToString());

                if (shouldDouble)
                {
                    if ((digit *= 2) > 9) digit -= 9;
                }

                sum += digit;
                shouldDouble = !shouldDouble;
            }

            return (sum % 10) == 0;
        }
        /// <summary>
        /// Remove all non digits characters from given string.
        /// </summary>
        /// <param name="value">An unformatted string.</param>
        /// <returns>A formatted string without digits.</returns>
        private string RemoveAllNonDigitCharacters(string value)
        {
            return Regex.Replace(value, "[^0-9]", "");
        }
        /// <summary>
        /// Validates the CVV of a credit card.
        /// American express has an unusual CVV that is 4 digits. All other credit cards have 3 digits.
        /// </summary>
        /// <param name="creditCardValue">The creditcard number.</param>
        /// <param name="cvvValue">The cvv number.</param>
        /// <returns>True if CVV is valid, false if invalid.</returns>
        public bool ValidateCVV(string creditCardValue, string cvvValue)
        {
            var creditCard = RemoveAllNonDigitCharacters(creditCardValue);
            var cvv = RemoveAllNonDigitCharacters(cvvValue);

            var americanExpressRegexPattern = string.Empty;

            var americanExpress = creditCardPatterns.Where(_ => _.CreditCardType == CreditCardType.AmericanExpress).FirstOrDefault();

            if (americanExpress != null)
            {
                americanExpressRegexPattern = americanExpress.RegexPattern;
            }

            if (!string.IsNullOrEmpty(americanExpressRegexPattern))
            {
                if (new Regex(americanExpressRegexPattern).IsMatch(creditCard))
                {
                    // American express has a CVV of 4 digits.
                    if (new Regex(@"^\d{4}$").IsMatch(cvv))
                    {
                        return true;
                    }
                    else
                    {
                        // american express CVV is invalid.
                        return false;
                    }
                }
            }

            // Other card and cvv is 3 digits
            if (new Regex(@"^\d{3}$").IsMatch(cvv))
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// Validates the expiry date of a credit card. According to what I found on google, credit cards have an average expiry date of 3 years.
        /// </summary>
        /// <param name="issueDate">Issue date in MM/yyyy format.</param>
        /// <returns>True if valid, false if invalid.</returns>
        public bool ValidateIssueDate(string issueDate)
        {
            var monthCheck = new Regex(@"^(0[1-9]|1[0-2])$");
            var yearCheck = new Regex(@"^20[0-9]{2}$");

            var dateParts = issueDate.Split('/'); //expiry date MM/yyyy            
            if (!monthCheck.IsMatch(dateParts[0]) || !yearCheck.IsMatch(dateParts[1]))
            {
                return false;
            }

            var year = int.Parse(dateParts[1]);
            var month = int.Parse(dateParts[0]);
            var lastDateOfExpiryMonth = DateTime.DaysInMonth(year, month); //get actual expiry date
            var cardExpiry = new DateTime(year, month, lastDateOfExpiryMonth, 23, 59, 59);

            // on average credit cards expire three years after the card was issued.
            return (cardExpiry > DateTime.Now && cardExpiry < DateTime.Now.AddYears(3));
        }
        /// <summary>
        /// Validates the card owner. Just a validation on name with no digits or other special characters.
        /// </summary>
        /// <param name="cardOwner">Name of the owner.</param>
        /// <returns>True  if valid, false if invalid.</returns>
        public bool IsValidCardOwner(string cardOwner)
        {
            var nameParts = cardOwner.Split(' ');
            var valid = false;
            for (var i = 0; i < nameParts.Length; i++)
            {
                valid = Regex.IsMatch(nameParts[i], @"^[a-zA-Z]+$");
            }
            return valid;
        }
    }
}
