using dotNET_Paynext.Models;
using dotNET_Paynext.Services;
using NUnit.Framework;

namespace dotNET_Paynext.Tests
{
    public class ValidationServiceTests
    {

        [Test]
        public void ValidateCard_MasterCard_Should_Return_True()
        {
            var testNumber = "5555 5555 5555 4444";
            var validationService = new ValidationService();
            Assert.IsTrue(validationService.ValidateCard(testNumber));
        }
        [Test]
        public void ValidateCard_MasterCard_DifferentFormat_Should_Return_True()
        {
            var testNumber = "5555555555554444";
            var validationService = new ValidationService();
            Assert.IsTrue(validationService.ValidateCard(testNumber));
        }
       
        [Test]
        public void ValidateCard_AmericanExpress_Should_Return_True()
        {
            var testNumber = "3782 822463 10005";
            var validationService = new ValidationService();
            Assert.IsTrue(validationService.ValidateCard(testNumber));
        }

        [Test]
        public void ValidateCard_Visa_Should_Return_True()
        {
            var testNumber = "4242 4242 4242 4242";
            var validationService = new ValidationService();
            Assert.IsTrue(validationService.ValidateCard(testNumber));
        }

        [Test]
        public void GetAcceptedCardType_Should_Return_VisaType()
        {
            var testNumber = "4242 4242 4242 4242";
            var validationService = new ValidationService();
            Assert.AreEqual(CreditCardType.Visa, validationService.GetAcceptedCreditCardType(testNumber));
        }
        [Test]
        public void GetAcceptedCardType_Should_Return_AmericanExpressType()
        {
            var testNumber = "3782 822463 10005";
            var validationService = new ValidationService();
            Assert.AreEqual(CreditCardType.AmericanExpress, validationService.GetAcceptedCreditCardType(testNumber));
        }
        [Test]
        public void GetAcceptedCardType_Should_Return_MasterCardType()
        {
            var testNumber = "5555 5555 5555 4444";
            var validationService = new ValidationService();
            Assert.AreEqual(CreditCardType.MasterCard, validationService.GetAcceptedCreditCardType(testNumber));
        }
        [Test]
        public void ValidateCVV_MasterCard_3Digits_Should_Return_True()
        {
            var testNumber = "5555 5555 5555 4444";
            var testCVV = "123";
            var validationService = new ValidationService();
            Assert.IsTrue(validationService.ValidateCVV(testNumber, testCVV));
        }
        [Test]
        public void ValidateCVV_Visa_3Digits_Should_Return_True()
        {
            var testNumber = "4242 4242 4242 4242";
            var testCVV = "123";
            var validationService = new ValidationService();
            Assert.IsTrue(validationService.ValidateCVV(testNumber, testCVV));
        }
        [Test]
        public void ValidateCVV_AmericanExpress_4Digits_Should_Return_True()
        {
            var testNumber = "3782 822463 10005";
            var testCVV = "1234";
            var validationService = new ValidationService();
            Assert.IsTrue(validationService.ValidateCVV(testNumber, testCVV));
        }
        [Test]
        public void ValidateCVV_AmericanExpress_3Digits_Should_Return_False()
        {
            var testNumber = "3782 822463 10005";
            var testCVV = "123";
            var validationService = new ValidationService();
            Assert.IsFalse(validationService.ValidateCVV(testNumber, testCVV));
        }
        [Test]
        public void Validate_CreditCardOwner_ValidName_Should_Return_True()
        {
            var testName = "Robin Demeulenaere";
            var validationService = new ValidationService();

            Assert.IsTrue(validationService.IsValidCardOwner(testName));
        }
        [Test]
        public void Validate_CreditCardOwner_WithDigitsAndOtherCharacters_Should_Return_False()
        {
            var testName = "R@b1n Dem3ulenaere 12351gtest";
            var validationService = new ValidationService();

            Assert.IsFalse(validationService.IsValidCardOwner(testName));
        }
        [Test]
        public void Validate_IssueDate_WithinNext3Years_Should_Return_True()
        {
            var issueDate = "11/2022";
            var validationService = new ValidationService();
            Assert.IsTrue(validationService.ValidateIssueDate(issueDate));
        }
        [Test]
        public void Validate_IssueDate_Expired_Should_Return_False()
        {
            var issueDate = "11/2019";
            var validationService = new ValidationService();
            Assert.IsFalse(validationService.ValidateIssueDate(issueDate));
        }
        [Test]
        public void Validate_IssueDate_InvalidMonth_Should_Return_False()
        {
            var issueDate = "13/2019";
            var validationService = new ValidationService();
            Assert.IsFalse(validationService.ValidateIssueDate(issueDate));
        }

        [Test]
        public void Validate_IssueDate_InvalidYear_Should_Return_False()
        {
            var issueDate = "13/209";
            var validationService = new ValidationService();
            Assert.IsFalse(validationService.ValidateIssueDate(issueDate));
        }
    }
}