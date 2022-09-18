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
    public class Adm_CategoriesController : Controller
    {
        //--------------Firebase Connection-------------------//

        public static FirebaseStorage firebaseStorage = new FirebaseStorage("kingscafeapp.appspot.com");
        public static FirebaseClient firebaseDatabase = new FirebaseClient("https://kingscafeapp-default-rtdb.firebaseio.com/");


        public async Task<ActionResult> Index()
        {
            var data = (await firebaseDatabase
            .Child("Category")
            .OnceAsync<Category>()).Select(item => new Category
            {
                 CatID = item.Object.CatID,
                  Image = item.Object.Image,
                  Name = item.Object.Name,
                  Status = item.Object.Status
                 
            }).ToList();

            return View(data);
        }

        public async Task<ActionResult> Details(int? id)
        {
            var item = (await firebaseDatabase.Child("Category").OnceAsync<Category>()).Where(a => a.Object.CatID == id).FirstOrDefault();
            return View(item.Object);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Category item, HttpPostedFileBase imgInp)
        {
            int LastID, NewID = 1;
            var lastrecord = (await firebaseDatabase.Child("Category").OnceAsync<Category>()).FirstOrDefault();
            if (lastrecord != null)
            {
                LastID = (await firebaseDatabase.Child("Category").OnceAsync<Category>()).Max(a => a.Object.CatID);
                NewID = ++LastID;
            }

            var stroageImage = await new FirebaseStorage("kingscafeapp.appspot.com")
                  .Child("ItemImages")
                  .Child(NewID + "_" + item.Name + ".jpg")
                  .PutAsync(imgInp.InputStream);
            string imgurl = stroageImage;

            item.CatID = NewID;
            item.Image = imgurl;


            await firebaseDatabase.Child("Category").PostAsync(item);
            return RedirectToAction("Index");
        }
        public async Task<ActionResult> Edit(int? id)
        {
            var item = (await firebaseDatabase.Child("Category").OnceAsync<Category>()).Where(a => a.Object.CatID == id).FirstOrDefault();
            return View(item.Object);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Category item, HttpPostedFileBase imgInp)
        {
            if (imgInp != null)
            {
                var stroageImage = await new FirebaseStorage("kingscafeapp.appspot.com")
                      .Child("ItemImages").Child(item.CatID + "_" + item.Name + ".jpg")
                      .PutAsync(imgInp.InputStream);
                string imgurl = stroageImage;

                item.Image = imgurl;
            }

            var toUpdatePerson = (await firebaseDatabase.Child("Category").OnceAsync<Category>()).Where(a => a.Object.CatID == item.CatID).FirstOrDefault();

            await firebaseDatabase.Child("Category").Child(toUpdatePerson.Key).PutAsync(item);
            return RedirectToAction("Index");
        }
        public async Task<ActionResult> Delete(int? id)
        {
            var item = (await firebaseDatabase.Child("Category").OnceAsync<Category>()).Where(a => a.Object.CatID == id).FirstOrDefault();
            return View(item.Object);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var item = (await firebaseDatabase.Child("Category").OnceAsync<Category>()).Where(a => a.Object.CatID == id).FirstOrDefault();

            await firebaseDatabase.Child("Category").Child(item.Key).DeleteAsync();
            return RedirectToAction("Index");
        }
    }
}
