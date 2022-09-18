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
    public class Adm_IngredientsController : Controller
    {
        //--------------Firebase Connection-------------------//

        public static FirebaseStorage firebaseStorage = new FirebaseStorage("kingscafeapp.appspot.com");
        public static FirebaseClient firebaseDatabase = new FirebaseClient("https://kingscafeapp-default-rtdb.firebaseio.com/");


     

        // GET: Adm_Ingredients
        public async Task<ActionResult> Index()
        {
            var data = (await firebaseDatabase
             .Child("Ingredient")
             .OnceAsync<Ingredient>()).Select(item => new Ingredient
             {
                 IngredientID = item.Object.IngredientID,
                 Image = item.Object.Image,
                 Name = item.Object.Name,
                 Price = item.Object.Price,
                 Type = item.Object.Type

             }).ToList();

            return View(data);
        }

        // GET: Adm_Ingredients/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            var item = (await firebaseDatabase.Child("Ingredient").OnceAsync<Ingredient>()).Where(a => a.Object.IngredientID == id).FirstOrDefault();
            return View(item.Object);
        }

        // GET: Adm_Ingredients/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Adm_Ingredients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Ingredient item, HttpPostedFileBase imgInp)
        {
            int LastID, NewID = 1;
            var lastrecord = (await firebaseDatabase.Child("Ingredient").OnceAsync<Ingredient>()).FirstOrDefault();
            if (lastrecord != null)
            {
                LastID = (await firebaseDatabase.Child("Ingredient").OnceAsync<Ingredient>()).Max(a => a.Object.IngredientID);
                NewID = ++LastID;
            }

            var stroageImage = await new FirebaseStorage("kingscafeapp.appspot.com")
                  .Child("ItemImages")
                  .Child(NewID + "_" + item.Name + ".jpg")
                  .PutAsync(imgInp.InputStream);
            string imgurl = stroageImage;

            item.IngredientID = NewID;
            item.Image = imgurl;


            await firebaseDatabase.Child("Ingredient").PostAsync(item);
            return RedirectToAction("Index");
        }

        // GET: Adm_Ingredients/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            var item = (await firebaseDatabase.Child("Ingredient").OnceAsync<Ingredient>()).Where(a => a.Object.IngredientID == id).FirstOrDefault();
            return View(item.Object);
        }

        // POST: Adm_Ingredients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Ingredient item, HttpPostedFileBase imgInp)
        {
            if (imgInp != null)
            {
                var stroageImage = await new FirebaseStorage("kingscafeapp.appspot.com")
                      .Child("ItemImages").Child(item.IngredientID + "_" + item.Name + ".jpg")
                      .PutAsync(imgInp.InputStream);
                string imgurl = stroageImage;

                item.Image = imgurl;
            }

            var toUpdatePerson = (await firebaseDatabase.Child("Ingredient").OnceAsync<Ingredient>()).Where(a => a.Object.IngredientID == item.IngredientID).FirstOrDefault();

            await firebaseDatabase.Child("Ingredient").Child(toUpdatePerson.Key).PutAsync(item);
            return RedirectToAction("Index");
        }

        // GET: Adm_Ingredients/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            var item = (await firebaseDatabase.Child("Ingredient").OnceAsync<Ingredient>()).Where(a => a.Object.IngredientID == id).FirstOrDefault();
            return View(item.Object);
        }

        // POST: Adm_Ingredients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var item = (await firebaseDatabase.Child("Ingredient").OnceAsync<Ingredient>()).Where(a => a.Object.IngredientID == id).FirstOrDefault();

            await firebaseDatabase.Child("Ingredient").Child(item.Key).DeleteAsync();
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
