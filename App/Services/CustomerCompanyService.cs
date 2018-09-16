using System;
using App.Entities;
using App.Interfaces.Repositories;
using App.Interfaces.Services;
using App.Interfaces.Wrappers;

namespace App.Services
{
    /// <summary>
    /// Refactored CustomerService to CustomerCompanyService
    /// This version has de-coupled dependencies to make it easier to unit test
    /// and swap out implementations via interfaces
    /// </summary>
    public class CustomerCompanyService : CustomerService, ICustomerService
    {

        private readonly ICustomerCreditService _customerCreditService;
        private readonly ICompanyRepository _companyRepository;
        private readonly ICustomerDataAccessWrapper _customerDataAccessWrapper;

        public CustomerCompanyService(ICustomerCreditService customerCreditService, ICompanyRepository companyRepository, ICustomerDataAccessWrapper customerDataAccessWrapper)
        {
            _customerCreditService = customerCreditService;
            _companyRepository = companyRepository;
            _customerDataAccessWrapper = customerDataAccessWrapper;
        }

        public override bool AddCustomer(string firstName, string surname, string email, DateTime dateOfBirth, int companyId)
        {
            // Validate customer fields before going further
            if (!IsValidCustomerHelper(firstName, surname, email, dateOfBirth)) return false;

            var company = GetCustomerCompanyHelper(companyId);

            var customer = new Customer
            {
                Company = company,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                Firstname = firstName,
                Surname = surname
            };

            SetCreditValuesHelper(company, customer);
            GetCreditLimitHelper(customer);

            if (customer.HasCreditLimit && customer.CreditLimit < 500)
            {
                return false;
            }

            _customerDataAccessWrapper.AddCustomer(customer);

            return true;
        }

        protected override Company GetCustomerCompanyHelper(int companyId)
        {
            var company = _companyRepository.GetById(companyId);
            return company;
        }

        protected override void GetCreditLimitHelper(Customer customer)
        {
            //TODO: NG - removed using (have to make sure IDisposable is still being used correctly
                var creditLimit =
                    _customerCreditService.GetCreditLimit(customer.Firstname, customer.Surname, customer.DateOfBirth);
                customer.CreditLimit = creditLimit;
        }

        /// <summary>
        /// Refactored SetCreditValuesHelper
        /// Reduced line count
        /// </summary>
        /// <param name="company"></param>
        /// <param name="customer"></param>
        protected override void SetCreditValuesHelper(Company company, Customer customer)
        {
            // Has limit if not VeryImportantClient.
            // Could re-factor further and remove magic string
            customer.HasCreditLimit = company.Name != "VeryImportantClient";

            // Do credit check and double credit limit
            if (!customer.HasCreditLimit) return;
            var creditLimit =
                _customerCreditService.GetCreditLimit(customer.Firstname, customer.Surname, customer.DateOfBirth);
            creditLimit = creditLimit * 2;
            customer.CreditLimit = creditLimit;
        }
    }
}
