using System;
using App.Entities;
using App.Enums;
using App.Interfaces.Repositories;
using App.Interfaces.Wrappers;
using App.Services;
using Moq;
using NUnit.Framework;
using SpecsFor;

namespace App.UnitTests.Services
{   

    /// <summary>
    /// Unit tests to verify that adding customer works correctly
    /// </summary>
    public class GivenAddingCustomerAssociatedWithCompany : SpecsFor<CustomerCompanyService>
    {
        // Test values
        private const string FirstName = "Nick";
        private const string Surname = "Gowdy";
        private const string Email = "nickgowdy87@gmail.com";
        private readonly DateTime _dataOfBirth = new DateTime(1987, 9, 2, 0, 0, 0, DateTimeKind.Local);
        private const int CompanyId = 1;

        private readonly Mock<ICustomerCreditService> _customCreditServiceMock = new Mock<ICustomerCreditService>();
        private readonly Mock<ICompanyRepository> _companyRepositoryMock = new Mock<ICompanyRepository>();
        private readonly Mock<ICustomerDataAccessWrapper> _customerDataAccessWrapperMock = new Mock<ICustomerDataAccessWrapper>();


        protected override void Given()
        {
            SUT = new CustomerCompanyService(
                _customCreditServiceMock.Object, 
                _companyRepositoryMock.Object,
                _customerDataAccessWrapperMock.Object);
        }

        /// <summary>
        /// It's mandatory that customer has firstname.
        /// Return false if null or empty string.
        /// </summary>
        public class WhenFirstnameIsNullOrEmptyString : GivenAddingCustomerAssociatedWithCompany
        {
            private bool _result;

            protected override void When()
            {
                _result = SUT.AddCustomer(string.Empty, Surname, Email, _dataOfBirth, CompanyId);
            }

            [Test]
            public void ThenResultIsFalse()
            {
                Assert.IsFalse(_result);
            }

            [Test]
            public void ThenGetCompanyByIdIsNeverCalled()
            {
                _companyRepositoryMock.Verify(x => x.GetById(It.IsAny<int>()), Times.Never);
            }

            [Test]
            public void ThenGetCreditLimitIsNeverCalled()
            {
                _customCreditServiceMock.Verify(x => x.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
            }

            [Test]
            public void ThenAddCustomerIsNeverCalled()
            {
                _customerDataAccessWrapperMock.Verify(x => x.AddCustomer(It.IsAny<Customer>()), Times.Never);
            }
        }

        /// <summary>
        /// It's mandatory that customer has surname.
        /// Return false if null or empty string.
        /// </summary>
        public class WhenSurnameIsNullOrEmptyString : GivenAddingCustomerAssociatedWithCompany
        {
            private bool _result;

            protected override void When()
            {
                _result = SUT.AddCustomer(FirstName, string.Empty, Email, _dataOfBirth, CompanyId);
            }

            [Test]
            public void ThenResultIsFalse()
            {
                Assert.IsFalse(_result);
            }

            [Test]
            public void ThenGetCompanyByIdIsNeverCalled()
            {
                _companyRepositoryMock.Verify(x => x.GetById(It.IsAny<int>()), Times.Never);
            }

            [Test]
            public void ThenGetCreditLimitIsNeverCalled()
            {
                _customCreditServiceMock.Verify(x => x.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
            }

            [Test]
            public void ThenAddCustomerIsNeverCalled()
            {
                _customerDataAccessWrapperMock.Verify(x => x.AddCustomer(It.IsAny<Customer>()), Times.Never);
            }
        }

        /// <summary>
        /// It's mandatory that email is correct format
        /// Return false if isn't correct format.
        /// </summary>
        public class WhenEmailIsInvalidFormat : GivenAddingCustomerAssociatedWithCompany
        {
            private bool _result;

            private const string InvalidEmail = "nickgowdy87@@gmail.com";

            protected override void When()
            {
                _result = SUT.AddCustomer(FirstName, Surname, InvalidEmail, _dataOfBirth, CompanyId);
            }

            [Test]
            public void ThenResultIsFalse()
            {
                Assert.IsFalse(_result);
            }

            [Test]
            public void ThenGetCompanyByIdIsNeverCalled()
            {
                _companyRepositoryMock.Verify(x => x.GetById(It.IsAny<int>()), Times.Never);
            }

            [Test]
            public void ThenGetCreditLimitIsNeverCalled()
            {
                _customCreditServiceMock.Verify(x => x.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
            }

            [Test]
            public void ThenAddCustomerIsNeverCalled()
            {
                _customerDataAccessWrapperMock.Verify(x => x.AddCustomer(It.IsAny<Customer>()), Times.Never);
            }
        }

        /// <summary>
        /// It's mandatory that the customer is 21 or over.
        /// Return false if his age isn't valid.
        /// </summary>
        public class WhenAgeIsUnder21 : GivenAddingCustomerAssociatedWithCompany
        {
            private bool _result;

            readonly DateTime _ageOfTwentyYearOld = DateTime.Now.AddYears(-20);

            protected override void When()
            {
                _result = SUT.AddCustomer(FirstName, Surname, Email, _ageOfTwentyYearOld, CompanyId);
            }

            [Test]
            public void ThenResultIsFalse()
            {
                Assert.IsFalse(_result);
            }

            [Test]
            public void ThenGetCompanyByIdIsNeverCalled()
            {
                _companyRepositoryMock.Verify(x => x.GetById(It.IsAny<int>()), Times.Never);
            }

            [Test]
            public void ThenGetCreditLimitIsNeverCalled()
            {
                _customCreditServiceMock.Verify(x => x.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
            }

            [Test]
            public void ThenAddCustomerIsNeverCalled()
            {
                _customerDataAccessWrapperMock.Verify(x => x.AddCustomer(It.IsAny<Customer>()), Times.Never);
            }
        }

        /// <summary>
        /// When customer data is valid and customer is VeryImportantClient
        /// Sets credit and credit limit and adds customer
        /// Returns true 
        /// </summary>
        public class WhenCompanyNameIsVeryImportantClient : GivenAddingCustomerAssociatedWithCompany
        {
            private bool _result;

            protected override void When()
            {
                _companyRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(new Company
                {
                    Id = 1,
                    Name = "VeryImportantClient",
                    Classification = Classification.Gold
                });

                _customCreditServiceMock
                    .Setup(x => x.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                    .Returns(5000);

                _customerDataAccessWrapperMock.Setup(x => x.AddCustomer(It.IsAny<Customer>()));

                _result = SUT.AddCustomer(FirstName, Surname, Email, _dataOfBirth, CompanyId);
            }

            [Test]
            public void ThenResultIsTrue()
            {
                Assert.IsTrue(_result);
            }

            [Test]
            public void ThenGetCompanyByIdIsCalledOnce()
            {
                _companyRepositoryMock.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            }

            [Test]
            public void ThenGetCreditLimitIsCalledOnce()
            {
                _customCreditServiceMock.Verify(x => x.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            }

            [Test]
            public void ThenAddCustomerIsCalledOnce()
            {
                _customerDataAccessWrapperMock.Verify(x => x.AddCustomer(It.IsAny<Customer>()), Times.Once);
            }
        }

        /// <summary>
        /// When customer data is valid and customer is ImportantClient
        /// Sets credit and credit limit and adds customer
        /// Returns true 
        /// </summary>
        public class WhenCompanyNameIsImportantClient : GivenAddingCustomerAssociatedWithCompany
        {
            private bool _result;

            protected override void When()
            {
                _companyRepositoryMock.Setup(x => x.GetById(It.IsAny<int>())).Returns(new Company
                {
                    Id = 2,
                    Name = "ImportantClient",
                    Classification = Classification.Silver
                });

                _customCreditServiceMock
                    .Setup(x => x.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                    .Returns(5000);

                _customerDataAccessWrapperMock.Setup(x => x.AddCustomer(It.IsAny<Customer>()));

                _result = SUT.AddCustomer(FirstName, Surname, Email, _dataOfBirth, CompanyId);
            }

            [Test]
            public void ThenResultIsTrue()
            {
                Assert.IsTrue(_result);
            }

            [Test]
            public void ThenGetCompanyByIdIsCalledOnce()
            {
                _companyRepositoryMock.Verify(x => x.GetById(It.IsAny<int>()), Times.Once);
            }

            [Test]
            public void ThenGetCreditLimitIsCalledOnce()
            {
                //TODO: NG - Being called twice seems like it's too much
                _customCreditServiceMock.Verify(x => x.GetCreditLimit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Exactly(2));
            }

            [Test]
            public void ThenAddCustomerIsCalledOnce()
            {
                _customerDataAccessWrapperMock.Verify(x => x.AddCustomer(It.IsAny<Customer>()), Times.Once);
            }
        }
    }
}
