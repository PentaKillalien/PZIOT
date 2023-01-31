using PZIOT.IServices.BASE;
using PZIOT.Model.Models;

namespace PZIOT.IServices
{
    public interface IAdvertisementServices :IBaseServices<Advertisement>
    {
        //int Sum(int i, int j);
        //int Add(Advertisement model);
        //bool Delete(Advertisement model);
        //bool Update(Advertisement model);
        //List<Advertisement> Query(Expression<Func<Advertisement, bool>> whereExpression);

        void ReturnExp();
    }
}
