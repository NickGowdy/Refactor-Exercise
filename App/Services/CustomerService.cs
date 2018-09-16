using System;
using System.Text.RegularExpressions;
using App.DataAccess;
using App.Entities;
using App.Repositories;

namespace App.Services
{
    public class CustomerService
    {
        public virtual bool AddCustomer(string firstName, string surname, string email, DateTime dateOfBirth, int companyId)
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

            CustomerDataAccess.AddCustomer(customer);

            return true;
        }

        protected virtual Company GetCustomerCompanyHelper(int companyId)
        {
            var companyRepository = new CompanyRepository();
            var company = companyRepository.GetById(companyId);
            return company;
        }

        protected virtual void GetCreditLimitHelper(Customer customer)
        {
            using (var customerCreditService = new CustomerCreditServiceClient())
            {
                var creditLimit =
                    customerCreditService.GetCreditLimit(customer.Firstname, customer.Surname, customer.DateOfBirth);
                customer.CreditLimit = creditLimit;
            }
        }

        /// <summary>
        /// Set 
        /// </summary>
        /// <param name="company"></param>
        /// <param name="customer"></param>
        protected virtual void SetCreditValuesHelper(Company company, Customer customer)
        {
            if (company.Name == "VeryImportantClient")
            {
                // Skip credit check
                customer.HasCreditLimit = false;
            }
            else if (company.Name == "ImportantClient")
            {
                // Do credit check and double credit limit
                customer.HasCreditLimit = true;
                using (var customerCreditService = new CustomerCreditServiceClient())
                {
                    var creditLimit =
                        customerCreditService.GetCreditLimit(customer.Firstname, customer.Surname, customer.DateOfBirth);
                    creditLimit = creditLimit * 2;
                    customer.CreditLimit = creditLimit;
                }
            }
            else
            {
                // Do credit check
                customer.HasCreditLimit = true;
            }
        }

        protected virtual bool IsValidCustomerHelper(string firstName, string surname, string email, DateTime dateOfBirth)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(surname))
            {
                return false;
            }

            var isValidEmail = Regex.IsMatch(email,
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                RegexOptions.IgnoreCase);

            // use regular expression because this can break easily
            if (!isValidEmail)
            {
                return false;
            }

            var now = DateTime.Now;
            var age = DateTime.Now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month || now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day) age--;

            return age >= 21;
        }
    }
}
