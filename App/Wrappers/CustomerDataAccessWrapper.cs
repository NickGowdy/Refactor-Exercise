using App.DataAccess;
using App.Entities;
using App.Interfaces.Wrappers;

namespace App.Wrappers
{
    public class CustomerDataAccessWrapper : ICustomerDataAccessWrapper
    {
        /// <summary>
        /// This is a wrapper so we can mock adding customer without
        /// changing legacy CustomerDataAccess class.
        /// </summary>
        /// <param name="customer"></param>
        public void AddCustomer(Customer customer)
        {
            CustomerDataAccess.AddCustomer(customer);
        }
    }
}
