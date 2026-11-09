using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanDth_FPT.Models;

namespace WebBanDth_FPT.Controllers
{
    public class CustomerController : Controller
    {
        WebBanDthEntities database = new WebBanDthEntities();

        // GET: Hiển thị form Đăng ký
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Xử lý lưu thông tin Đăng ký vào database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Customer customer)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra email đã tồn tại chưa
                var checkEmail = database.Customers.FirstOrDefault(c => c.Email == customer.Email);
                if (checkEmail == null)
                {
                    customer.CreatedDate = DateTime.Now;
                    database.Customers.Add(customer);
                    database.SaveChanges();

                    return RedirectToAction("Login");
                }
                else
                {
                    ViewBag.ErrorRegister = "Email này đã được sử dụng!";
                    return View(customer);
                }
            }
            return View(customer);
        }

        // GET: Hiển thị form Đăng nhập
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        // POST: Xử lý kiểm tra Đăng nhập
        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            // Lấy dữ liệu từ form HTML
            var email = collection["Email"];
            var password = collection["PasswordHash"];

            if (String.IsNullOrEmpty(email) || String.IsNullOrEmpty(password))
            {
                ViewBag.ErrorLogin = "Vui lòng nhập đầy đủ Email và Mật khẩu.";
                return View();
            }

            // Kiểm tra thông tin trong CSDL
            var customer = database.Customers.SingleOrDefault(c => c.Email == email && c.PasswordHash == password);

            if (customer != null)
            {
                // Lưu vào Session nếu đúng
                Session["Account"] = customer;
                return RedirectToAction("Index", "PhoneStore"); // Chuyển về trang chủ sau khi login thành công
            }
            else
            {
                ViewBag.ErrorLogin = "Email hoặc mật khẩu không chính xác!";
                return View();
            }
        }

        // Xử lý Đăng xuất
        public ActionResult Logout()
        {
            Session["Account"] = null;
            return RedirectToAction("Index", "PhoneStore");
        }
    }
}
