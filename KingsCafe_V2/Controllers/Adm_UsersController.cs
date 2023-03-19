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
    public class Adm_UsersController : Controller
    {
        //--------------Firebase Connection-------------------//

        public static FirebaseStorage firebaseStorage = new FirebaseStorage("kingscafeapp.appspot.com");
        public static FirebaseClient firebaseDatabase = new FirebaseClient("https://kingscafeapp-default-rtdb.firebaseio.com/");

      

        // GET: Adm_Users
        public async Task<ActionResult> Index()
        {
            var data = (await firebaseDatabase
           .Child("User")
           .OnceAsync<User>()).Select(item => new User
           {
               UserID = item.Object.UserID,
               Email = item.Object.Email,
               Name = item.Object.Name,
               Password = item.Object.Password,
               Status = item.Object.Status,
               Image = item.Object.Image,
               Type = item.Object.Type,

           }).ToList();

            return View(data);
        }

        // GET: Adm_Users/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            var item = (await firebaseDatabase.Child("User").OnceAsync<User>()).Where(a => a.Object.UserID == id).FirstOrDefault();
            return View(item.Object);
        }

        // GET: Adm_Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Adm_Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(User item, HttpPostedFileBase imgInp)
        {
            int LastID, NewID = 1;
            var lastrecord = (await firebaseDatabase.Child("User").OnceAsync<User>()).FirstOrDefault();
            if (lastrecord != null)
            {
                LastID = (await firebaseDatabase.Child("User").OnceAsync<User>()).Max(a => a.Object.UserID);
                NewID = ++LastID;
            }
            var stroageImage = await new FirebaseStorage("kingscafeapp.appspot.com")
                 .Child("ItemImages")
                 .Child(NewID + "_" + item.Name + ".jpg")
                 .PutAsync(imgInp.InputStream);
            string imgurl = stroageImage;
            item.UserID = NewID;
            item.Image = imgurl;
            await firebaseDatabase.Child("User").PostAsync(item);
            return RedirectToAction("Index");
        }

        // GET: Adm_Users/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var item = (await firebaseDatabase.Child("User").OnceAsync<User>()).Where(a => a.Object.UserID == id).FirstOrDefault();
            return View(item.Object);
        }

        // POST: Adm_Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(User item, HttpPostedFileBase imgInp)
        {
            if (imgInp != null)
            {
                var stroageImage = await new FirebaseStorage("kingscafeapp.appspot.com")
                      .Child("ItemImages").Child(item.UserID + "_" + item.Name + ".jpg")
                      .PutAsync(imgInp.InputStream);
                string imgurl = stroageImage;

                item.Image = imgurl;
            }

            var toUpdatePerson = (await firebaseDatabase.Child("User").OnceAsync<User>()).Where(a => a.Object.UserID == item.UserID).FirstOrDefault();

            await firebaseDatabase.Child("User").Child(toUpdatePerson.Key).PutAsync(item);
            return RedirectToAction("Index");
        }

        // GET: Adm_Users/Delete/5
        public async Task<ActionResult>  Delete(int? id)
        {
            var item = (await firebaseDatabase.Child("User").OnceAsync<User>()).Where(a => a.Object.UserID == id).FirstOrDefault();
            return View(item.Object);
        }

        // POST: Adm_Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var item = (await firebaseDatabase.Child("User").OnceAsync<User>()).Where(a => a.Object.UserID == id).FirstOrDefault();

            await firebaseDatabase.Child("User").Child(item.Key).DeleteAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
