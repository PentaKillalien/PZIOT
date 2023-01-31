using PZIOT.IServices.BASE;
using PZIOT.Model.Models;
using PZIOT.Model.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PZIOT.IServices
{
    public interface IBlogArticleServices :IBaseServices<BlogArticle>
    {
        Task<List<BlogArticle>> GetBlogs();
        Task<BlogViewModels> GetBlogDetails(int id);

    }

}
