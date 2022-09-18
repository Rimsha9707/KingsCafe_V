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
    public class Adm_FoodItemsController : Controller
    {
        //--------------Firebase Connection-------------------//

        public static FirebaseStorage firebaseStorage = new FirebaseStorage("kingscafeapp.appspot.com");
        public static FirebaseClient firebaseDatabase = new FirebaseClient("https://kingscafeapp-default-rtdb.firebaseio.com/");


        public async Task<ActionResult> Index()
        {
            var data = (await firebaseDatabase
            .Child("FoodItem")
            .OnceAsync<FoodItem>()).Select(item => new FoodItem
            {
                ItemID = item.Object.ItemID,
                CatFID = item.Object.CatFID,
                Rating = item.Object.Rating,
                SalePrice = item.Object.SalePrice,
                Name = item.Object.Name,
                Image = item.Object.Image,
                Status = item.Object.Status

            }).ToList();

            return View(data);
        }

        public async Task<ActionResult> Details(int? id)
        {
            var item = (await firebaseDatabase.Child("FoodItem").OnceAsync<FoodItem>()).Where(a => a.Object.ItemID == id).FirstOrDefault();
            return View(item.Object);
        }

        public async Task<ActionResult> Create()
        {
            ViewBag.Cats = (await firebaseDatabase.Child("Category").OnceAsync<Category>()).Select(item => new Category { CatID = item.Object.CatID, Name = item.Object.Name, }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(FoodItem item, HttpPostedFileBase imgInp)
        {
            int LastID, NewID = 1;
            var lastrecord = (await firebaseDatabase.Child("FoodItem").OnceAsync<FoodItem>()).FirstOrDefault();
            if (lastrecord != null)
            {
                LastID = (await firebaseDatabase.Child("FoodItem").OnceAsync<FoodItem>()).Max(a => a.Object.ItemID);
                NewID = ++LastID;
            }

            var stroageImage = await new FirebaseStorage("kingscafeapp.appspot.com")
                  .Child("ItemImages")
                  .Child(NewID + "_" + item.Name + ".jpg")
                  .PutAsync(imgInp.InputStream);
            string imgurl = stroageImage;

            item.ItemID = NewID;
            item.Image = imgurl;


            await firebaseDatabase.Child("FoodItem").PostAsync(item);
            return RedirectToAction("Index");
        }
        public async Task<ActionResult> Edit(int? id)
        {
            var item = (await firebaseDatabase.Child("FoodItem").OnceAsync<FoodItem>()).Where(a => a.Object.ItemID == id).FirstOrDefault();
            return View(item.Object);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(FoodItem item, HttpPostedFileBase imgInp)
        {
            if (imgInp != null)
            {
                var stroageImage = await new FirebaseStorage("kingscafeapp.appspot.com")
                      .Child("ItemImages").Child(item.ItemID + "_" + item.Name + ".jpg")
                      .PutAsync(imgInp.InputStream);
                string imgurl = stroageImage;

                item.Image = imgurl;
            }

            var toUpdatePerson = (await firebaseDatabase.Child("FoodItem").OnceAsync<FoodItem>()).Where(a => a.Object.ItemID == item.ItemID).FirstOrDefault();

            await firebaseDatabase.Child("FoodItem").Child(toUpdatePerson.Key).PutAsync(item);
            return RedirectToAction("Index");
        }
        public async Task<ActionResult> Delete(int? id)
        {
            var item = (await firebaseDatabase.Child("FoodItem").OnceAsync<FoodItem>()).Where(a => a.Object.ItemID == id).FirstOrDefault();
            return View(item.Object);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var item = (await firebaseDatabase.Child("FoodItem").OnceAsync<FoodItem>()).Where(a => a.Object.ItemID == id).FirstOrDefault();

            await firebaseDatabase.Child("FoodItem").Child(item.Key).DeleteAsync();
            return RedirectToAction("Index");
        }
    }
}
