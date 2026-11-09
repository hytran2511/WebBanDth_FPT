using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanDth_FPT.Models;

namespace WebBanDth_FPT.Controllers
{
    public class CartController : Controller
    {
        WebBanDthEntities database = new WebBanDthEntities();

        public List<CartItem> GetCart()
        {
            List<CartItem> cart = Session["Cart"] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                Session["Cart"] = cart;
            }
            return cart;
        }

        public ActionResult AddToCart(int id, string strURL)
        {
            List<CartItem> cart = GetCart();
            CartItem product = cart.Find(n => n.ProductId == id);

            if (product == null)
            {
                product = new CartItem(id);
                cart.Add(product);
            }
            else
            {
                product.Quantity++;
            }
            return Redirect(strURL);
        }

        // Hàm tính tổng số lượng
        private int GetTotalQuantity()
        {
            int totalQty = 0;
            List<CartItem> cart = Session["Cart"] as List<CartItem>;
            if (cart != null)
            {
                totalQty = cart.Sum(n => n.Quantity);
            }
            return totalQty;
        }

        // Hàm tính tổng tiền
        private double GetTotalAmount()
        {
            double totalAmount = 0;
            List<CartItem> cart = Session["Cart"] as List<CartItem>;
            if (cart != null)
            {
                totalAmount = cart.Sum(n => n.GetTotalPrice());
            }
            return totalAmount;
        }

        // Hàm Partial hiển thị icon Giỏ hàng trên Layout
        public ActionResult CartPartial()
        {
            ViewBag.TotalQuantity = GetTotalQuantity();
            return PartialView();
        }

        public ActionResult ShowCart()
        {
            List<CartItem> cart = GetCart();
            if (cart.Count == 0)
            {
                return RedirectToAction("Index", "PhoneStore");
            }
            // Gọi 2 hàm tính toán vừa viết truyền vào ViewBag
            ViewBag.TotalQuantity = GetTotalQuantity();
            ViewBag.TotalAmount = GetTotalAmount();
            return View(cart);
        }

        // Cập nhật số lượng
        [HttpPost]
        public ActionResult UpdateCart(int id, FormCollection collection)
        {
            List<CartItem> cart = GetCart();
            CartItem product = cart.SingleOrDefault(n => n.ProductId == id);
            if (product != null)
            {
                product.Quantity = int.Parse(collection["txtQuantity"].ToString());
            }
            return RedirectToAction("ShowCart");
        }

        // Xóa sản phẩm khỏi giỏ
        public ActionResult RemoveItem(int id)
        {
            List<CartItem> cart = GetCart();
            CartItem product = cart.SingleOrDefault(n => n.ProductId == id);
            if (product != null)
            {
                cart.Remove(product);
            }
            if (cart.Count == 0)
            {
                return RedirectToAction("Index", "PhoneStore");
            }
            return RedirectToAction("ShowCart");
        }

        [HttpGet]
        public ActionResult Order()
        {
            // Kiểm tra khách hàng đã đăng nhập chưa
            if (Session["Account"] == null || Session["Account"].ToString() == "")
            {
                return RedirectToAction("Login", "Customer");
            }

            // Kiểm tra giỏ hàng có trống không
            if (Session["Cart"] == null)
            {
                return RedirectToAction("Index", "PhoneStore");
            }

            // Lấy giỏ hàng truyền ra View
            List<CartItem> cart = GetCart();
            ViewBag.TotalQuantity = GetTotalQuantity();
            ViewBag.TotalAmount = GetTotalAmount();

            return View(cart);
        }

        // POST: Xử lý lưu đơn hàng vào Database
        [HttpPost]
        public ActionResult Order(FormCollection collection)
        {
            // 1. Lấy thông tin giỏ hàng và tài khoản
            List<CartItem> cart = GetCart();
            Customer customer = (Customer)Session["Account"];

            // 2. Khởi tạo và lưu thông tin vào bảng OrderPro (Đơn hàng)
            OrderPro order = new OrderPro();
            order.CustomerId = customer.CustomerId;
            order.OrderDate = System.DateTime.Now;
            order.TotalAmount = (decimal)GetTotalAmount();
            order.Status = "0"; // 0: Đang chờ duyệt, 1: Đang giao, v.v.
            order.ShippingAddress = customer.Address; // Lấy tạm địa chỉ của khách, có thể cho nhập riêng nếu muốn

            database.OrderProes.Add(order);
            database.SaveChanges(); // Gọi SaveChanges để EF sinh ra mã OrderId

            // 3. Khởi tạo và lưu thông tin vào bảng OrderDetail (Chi tiết đơn hàng)
            foreach (var item in cart)
            {
                OrderDetail detail = new OrderDetail();
                detail.OrderId = order.OrderId;
                detail.ProductId = item.ProductId;
                detail.Quantity = item.Quantity;
                detail.UnitPrice = (decimal)item.UnitPrice;

                database.OrderDetails.Add(detail);
            }
            database.SaveChanges();

            // 4. Reset giỏ hàng sau khi đặt thành công
            Session["Cart"] = null;

            return RedirectToAction("OrderSuccess", "Cart");
        }

        // Trang thông báo đặt hàng thành công
        public ActionResult OrderSuccess()
        {
            return View();
        }
    }
}