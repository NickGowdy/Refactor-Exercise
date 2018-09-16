using App.Entities;

namespace App.Interfaces.Repositories
{
    public interface ICompanyRepository
    {
        Company GetById(int id);
    }
}
