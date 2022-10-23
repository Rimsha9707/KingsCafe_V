using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using KingsCafe_V2.Models;
using KingsCafe_V2.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KingsCafe.Controllers
{
    public class ReportsController : Controller
    {
        public static FirebaseStorage firebaseStorage = new FirebaseStorage("kingscafeapp.appspot.com");
        public static FirebaseClient firebaseDatabase = new FirebaseClient("https://kingscafeapp-default-rtdb.firebaseio.com/");

        // GET: Reports
        public async Task<ActionResult> SaleReport(DateTime? ToDate, DateTime? FromDate, string Status = "", string Customer = "")
        {

            var data = (await firebaseDatabase.Child("Order").OnceAsync<Order>()).Select(item => new Order
            {
                Address = item.Object.Address,
                DeliveryDate = item.Object.DeliveryDate,
                Email = item.Object.Email,
                OrderDate = item.Object.OrderDate,
                OrderID = item.Object.OrderID,
                OrderTime = item.Object.OrderTime,
                PaymentMethod = item.Object.PaymentMethod,
                Phone = item.Object.Phone,
                Name = item.Object.Name,
                Status = item.Object.Status

            }).AsQueryable();

            if (ToDate != null)
            {
                data = data.Where(x => x.OrderDate <= ToDate);
            }
            if (FromDate != null)
            {
                data = data.Where(x => x.OrderDate >= FromDate);
            }

            if (!string.IsNullOrEmpty(Status) && Status != "Select Status")
            {
                data = data.Where(x => x.Status == Status);
            }

            if (!string.IsNullOrEmpty(Customer))
            {
                data = data.Where(x => x.Name.Contains(Customer) || x.Name.StartsWith(Customer));
            }


            return View(data);
        }

        public async Task<ActionResult> Process(int? id, string act)
        {
            if (act == "Pending")
            {
                var item = (await firebaseDatabase.Child("Order").OnceAsync<Order>()).Where(a => a.Object.OrderID == id).FirstOrDefault();
                item.Object.Status = "Pending";
                await firebaseDatabase.Child("Order").Child(item.Key).PutAsync(item.Object);
            }

            if (act == "Ready")
            {
                var item = (await firebaseDatabase.Child("Order").OnceAsync<Order>()).Where(a => a.Object.OrderID == id).FirstOrDefault();
                item.Object.Status = "Ready";
                await firebaseDatabase.Child("Order").Child(item.Key).PutAsync(item.Object);
            }

            if (act == "Delivered")
            {
                var item = (await firebaseDatabase.Child("Order").OnceAsync<Order>()).Where(a => a.Object.OrderID == id).FirstOrDefault();
                item.Object.Status = "Delivered";
                await firebaseDatabase.Child("Order").Child(item.Key).PutAsync(item.Object);
            }

            if (act == "Cancelled")
            {
                var item = (await firebaseDatabase.Child("Order").OnceAsync<Order>()).Where(a => a.Object.OrderID == id).FirstOrDefault();
                item.Object.Status = "Cancelled";
                await firebaseDatabase.Child("Order").Child(item.Key).PutAsync(item.Object);
            }

            if (act == "Delete")
            {
                var item = (await firebaseDatabase.Child("Order").OnceAsync<Order>()).Where(a => a.Object.OrderID == id).FirstOrDefault();
                await firebaseDatabase.Child("Order").Child(item.Key).DeleteAsync();
            }
            return Redirect("/Reports/SaleReport");
        }

        public async Task<ActionResult> ViewInvoice(int? id)
        {
            var item = (await firebaseDatabase.Child("Order").OnceAsync<Order>()).Where(a => a.Object.OrderID == id).FirstOrDefault();
            return View(item.Object);
        }

        public Order OrderFID { get; set; }
        public async Task<ActionResult> PLReport()
        {

            var OrderList = new List<OrderDetail_VM>();
            var orderDetails = (await firebaseDatabase.Child("OrderDetail").OnceAsync<OrderDetail>()).ToList();
            foreach (var item in orderDetails)
            {
                var order = (await firebaseDatabase.Child("Order").OnceAsync<Order>()).Where(a => a.Object.OrderID == item.Object.OrderFID).FirstOrDefault();
                var foodItem = (await firebaseDatabase.Child("FoodItem").OnceAsync<FoodItem>()).Where(a => a.Object.ItemID == item.Object.ItemFID).FirstOrDefault();
                var categ = (await firebaseDatabase.Child("Category").OnceAsync<Category>()).Where(a => a.Object.CatID == foodItem.Object.CatFID).FirstOrDefault();

                OrderList.Add(new OrderDetail_VM { OrderDetails = item.Object, OrderFID = order.Object,ItemFID = foodItem.Object,CatFID =  categ.Object});
            }


            return View(OrderList);
        }
    }
}