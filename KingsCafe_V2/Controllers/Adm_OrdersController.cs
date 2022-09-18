using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using KingsCafe_V2.Models;

namespace KingsCafe_V2.Controllers
{
    public class Adm_OrdersController : Controller
    {
        //--------------Firebase Connection-------------------//

        public static FirebaseStorage firebaseStorage = new FirebaseStorage("kingscafeapp.appspot.com");
        public static FirebaseClient firebaseDatabase = new FirebaseClient("https://kingscafeapp-default-rtdb.firebaseio.com/");


        public async Task<ActionResult> Index()
        {
            var data = (await firebaseDatabase
            .Child("Order")
            .OnceAsync<Order>()).Select(item => new Order
            {
                 
                  OrderID= item.Object.OrderID,
                  OrderDate = item.Object.OrderDate,
                  OrderTime = item.Object.OrderTime,
                  Status = item.Object.Status,
                  Name = item.Object.Name,
                  Address = item.Object.Address,
                  Phone = item.Object.Phone,
                  PaymentMethod = item.Object.PaymentMethod,
                  Email = item.Object.Email,
                  DeliveryDate= item.Object.DeliveryDate,


            }).ToList();

            return View(data);
        }

        public async Task<ActionResult> Details(int? id)
        {
            var item = (await firebaseDatabase.Child("Order").OnceAsync<Order>()).Where(a => a.Object.OrderID == id).FirstOrDefault();
            return View(item.Object);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            var item = (await firebaseDatabase.Child("Order").OnceAsync<Order>()).Where(a => a.Object.OrderID == id).FirstOrDefault();
            return View(item.Object);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var item = (await firebaseDatabase.Child("Order").OnceAsync<Order>()).Where(a => a.Object.OrderID == id).FirstOrDefault();

            await firebaseDatabase.Child("Order").Child(item.Key).DeleteAsync();
            return RedirectToAction("Index");
        }
    }
}
