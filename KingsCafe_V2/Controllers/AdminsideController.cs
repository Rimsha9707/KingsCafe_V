using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using KingsCafe_V2.Models;

namespace KingsCafe.Controllers
{
    public class AdminsideController : Controller
    {
        public static FirebaseStorage firebaseStorage = new FirebaseStorage("kingscafeapp.appspot.com");
        public static FirebaseClient firebaseDatabase = new FirebaseClient("https://kingscafeapp-default-rtdb.firebaseio.com/");


        // GET: Adminside
        public async Task<ActionResult> NewOrders()
        {

            ViewBag.Category = (await firebaseDatabase.Child("Category").OnceAsync<Category>()).Select(item => new Category { CatID = item.Object.CatID, Name = item.Object.Name, }).ToList();
            ViewBag.Products = (await firebaseDatabase.Child("FoodItem").OnceAsync<FoodItem>()).Select(item => new FoodItem { ItemID = item.Object.ItemID, Name = item.Object.Name, }).ToList();


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

            }).ToList();
            return View(data);
        }
        public async Task<ActionResult> ProceedOrders()
        {
            ViewBag.Category = (await firebaseDatabase.Child("Category").OnceAsync<Category>()).Select(item => new Category { CatID = item.Object.CatID, Name = item.Object.Name, }).ToList();
            ViewBag.Products = (await firebaseDatabase.Child("FoodItem").OnceAsync<FoodItem>()).Select(item => new FoodItem { ItemID = item.Object.ItemID, Name = item.Object.Name, }).ToList();


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

            }).Where(x => x.Status == "Proceed");
            //var orderlist=  db.tblOrders.Where(x => x.ORDER_STATUS == "Proceed").ToList();
            return View(data);
        }
        //public ActionResult DeliveredOrders()
        //{

        //   var orderlist=  db.tblOrders.Where(x => x.ORDER_STATUS == "Delivered").ToList();
        //    return View(orderlist);
        //}
        //public ActionResult Invoice(int id)
        //{
        //   var invoicedata=  db.tblOrders.Where(x => x.ORDER_ID == id).FirstOrDefault();
        //    return View(invoicedata);
        //}
        public async Task<ActionResult> Sendtoproceed(int id)
        {
            var item = (await firebaseDatabase.Child("Order").OnceAsync<Order>()).Where(a => a.Object.OrderID == id).FirstOrDefault();
            item.Object.Status = "Proceed";
            await firebaseDatabase.Child("Order").Child(item.Key).PutAsync(item);
            TempData["msg"] = "  Your order is " + id + " now in proceed list ";
            return RedirectToAction("NewOrders");
        }
        public async Task<ActionResult> SendToDelivered(int id, Order item)
        {
            //var Orderdata=  db.tblOrders.Find(id);
            // Orderdata.ORDER_STATUS = "Delivered";
            // db.Entry(Orderdata).State = EntityState.Modified;
            // db.SaveChanges();
            var toUpdatePerson = (await firebaseDatabase.Child("Order").OnceAsync<Order>()).Where(a => a.Object.OrderID == id).FirstOrDefault();
            item.Status = "Delivered";
            await firebaseDatabase.Child("Order").Child(toUpdatePerson.Key).PutAsync(item);
            TempData["msg"] = " Your order is " + id + " now in delivered list ";
            return RedirectToAction("ProceedOrders");
        }

    }
}