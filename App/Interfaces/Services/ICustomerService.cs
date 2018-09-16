using System;

namespace App.Interfaces.Services
{
    public interface ICustomerService
    {
        bool AddCustomer(string firstName, string surname, string email, DateTime dateOfBirth, int companyId);
    }
}
