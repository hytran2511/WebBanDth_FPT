using PagedList;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebBanDth_FPT.Models;
namespace WebBanDth_FPT.Models.Controllers
{
    public class PhoneStoreController : Controller
    {
        // Use DbContext to manage database
        WebBanDthEntities database = new WebBanDthEntities();
        private List<Product> GetNewPhone(int soluong)
        {
            // Sắp xếp điện thoại theo ngày cập nhật giảm dần, lấy đúng số lượng điện thoại cần
            // Chuyển qua dạng danh sách kết quả đạt được
            return database.Products.OrderByDescending(phone =>
            phone.CreatedDate).Take(soluong).ToList();
        }
        // GET: PhoneStore
        public ActionResult Index(int? page)
        {
            int pageSize = 2; // Số sản phẩm hiển thị trên 1 trang
            int pageNum = (page ?? 1); // Nếu page null thì mặc định lấy trang 1

            var dsPhone = database.Products.OrderByDescending(p => p.CreatedDate).ToList();

            // Trả về ToPagedList
            return View(dsPhone.ToPagedList(pageNum, pageSize));
        }
        // Lấy danh sách hãng điện thoại
        public ActionResult GetCategories()
        {
            var dsPhoneBrand = database.Categories.ToList();
            return PartialView(dsPhoneBrand);
        }
        public ActionResult GetProductsByCategory(int id)
        {
            //Lấy danh sách điện thoại theo hãng
            var dsPhoneByCategory = database.Products.Where(phone => phone.CategoryId == id).ToList();
            // Trả về view Index với danh sách điện thoại theo hãng
            // View("Index", dsPhoneByCategory) sẽ trả về view Index và truyền danh sách điện thoại theo hãng vào view
            return View("Index", dsPhoneByCategory);
        }
        public ActionResult Details(int id)
        {
            // Lấy thông tin chi tiết điện thoại theo id
            var phone = database.Products.FirstOrDefault(p => p.ProductId == id);
            return View(phone);
        }
    }
}