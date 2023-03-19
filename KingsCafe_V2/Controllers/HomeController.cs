using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using KingsCafe.Models;
using KingsCafe_V2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace KingsCafe_V2.Controllers
{
    public class HomeController : Controller
    {

        public static FirebaseClient firebaseDatabase = new FirebaseClient("https://kingscafeapp-default-rtdb.firebaseio.com/");

        public async Task<ActionResult> Index()
        {
            ViewBag.Cats = (await firebaseDatabase.Child("Category").OnceAsync<Category>()).Select(item => new Category { CatID = item.Object.CatID, Name = item.Object.Name, Image = item.Object.Image }).ToList();
            return View();
        }

        public ActionResult IndexAdmin()
        {
            return View();
        }
        public ActionResult About()
        {
            return View();
        }
        public async Task<ActionResult> Menu(int? id)
        {
            ViewBag.Cats = (await firebaseDatabase.Child("Category").OnceAsync<Category>()).Select(item => new Category { CatID = item.Object.CatID, Name = item.Object.Name, Image = item.Object.Image }).ToList();
            ViewBag.Ing = (await firebaseDatabase.Child("Category").OnceAsync<Category>()).Select(item => new Category { CatID = item.Object.CatID, Name = item.Object.Name, Image = item.Object.Image }).ToList();

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

             }).AsQueryable();

            if (id != null)
            {
                data = data.Where(x => x.CatFID == id);
            }
            return View(data);
        }

        public ActionResult ShoppingCart()
        {
            return View();
        }

        public async Task<ActionResult> AddtoCart(int id)
        {

            var item = (await firebaseDatabase.Child("FoodItem").OnceAsync<FoodItem>()).FirstOrDefault(x => x.Object.ItemID == id);

            if (Session["Cart"] == null)
            {
                List<CartItem> Cart = new List<CartItem>();
                Cart.Add(new CartItem { foodItem = item.Object, quantity = 1 });

                Session["Cart"] = Cart;
            }

            else
            {
                List<CartItem> Cart = (List<CartItem>)Session["Cart"];
                var found = Cart.Any(x => x.foodItem.ItemID == id);
                if (found)
                {
                    Cart.FirstOrDefault(x => x.foodItem.ItemID == id).quantity++;
                }
                else
                {
                    Cart.Add(new CartItem { foodItem = item.Object, quantity = 1 });
                }


                Session["Cart"] = Cart;
            }

            return RedirectToAction("Menu");
        }
        public ActionResult Remove(int id)
        {
            List<CartItem> Cart = (List<CartItem>)Session["Cart"];
            int FoundItem = -1;
            for (int i = 0; i < Cart.Count; i++)
            {
                if (Cart[i].foodItem.ItemID == id)
                {
                    FoundItem = i;
                }
            }
            Cart.RemoveAt(FoundItem);

            Session["Cart"] = Cart;
            return RedirectToAction("ShoppingCart");
        }
        public ActionResult QtyPlus(int id)
        {
            List<CartItem> Cart = (List<CartItem>)Session["Cart"];
            int FoundItem = -1;
            for (int i = 0; i < Cart.Count; i++)
            {
                if (Cart[i].foodItem.ItemID == id)
                {
                    FoundItem = i;
                }
            }


            Cart[FoundItem].quantity++;

            Session["Cart"] = Cart;
            return RedirectToAction("ShoppingCart");
        }
        public ActionResult QtyMinus(int id)
        {
            List<CartItem> Cart = (List<CartItem>)Session["Cart"];
            int FoundItem = -1;
            for (int i = 0; i < Cart.Count; i++)
            {
                if (Cart[i].foodItem.ItemID == id)
                {
                    FoundItem = i;
                }
            }
            if (Cart[FoundItem].quantity > 1)
            {
                Cart[FoundItem].quantity--;
            }
            else
            {
                TempData["State"] = "error";
                TempData["Message"] = "Quantity cannot be less than 1";
                return RedirectToAction("ShoppingCart");

            }

            Session["Cart"] = Cart;
            return RedirectToAction("ShoppingCart");
        }

        public ActionResult Checkout()
        {
            return View();
        }

        public async Task<ActionResult> OrderBooked(string Name, string Address, string Phone, string Email, string PaymentMethod)
        {
            if (Session["Cart"] == null || ((List<CartItem>)Session["Cart"]).Count == 0)
            {
                TempData["State"] = "warning";
                TempData["Message"] = "Cart is Empty Please add an Item to checkout";
                return Redirect("/Home/ShoppingCart");
            }

            int LastID, NewID = 1;
            var lastrecord = (await firebaseDatabase.Child("Order").OnceAsync<Order>()).FirstOrDefault();
            if (lastrecord != null)
            {
                LastID = (await firebaseDatabase.Child("Order").OnceAsync<Order>()).Max(a => a.Object.OrderID);
                NewID = ++LastID;
            }


            //ORDER SAVING ================================================================

            Order order = new Order()
            {
                Address = Address,
                Email = Email,
                Name = Name,
                PaymentMethod = PaymentMethod,
                OrderID = NewID,
                Phone = Phone,

                OrderDate = DateTime.Now.Date,
                OrderTime = DateTime.Now.TimeOfDay,
                Status = "Pending"
            };

            await firebaseDatabase.Child("Order").PostAsync(order);

            int LastID2, NewID2 = 1;
            var lastrecord2 = (await firebaseDatabase.Child("OrderDetail").OnceAsync<OrderDetail>()).FirstOrDefault();
            if (lastrecord2 != null)
            {
                LastID2 = (await firebaseDatabase.Child("OrderDetail").OnceAsync<OrderDetail>()).Max(a => a.Object.DetailsID);
                NewID2 = ++LastID2;
            }

            double total = 0;
            List<CartItem> Cart = (List<CartItem>)Session["Cart"];
            for (int i = 0; i < Cart.Count; i++)
            {
                total = total + (Cart[i].quantity * Cart[i].foodItem.SalePrice);
                OrderDetail detail = new OrderDetail()
                {
                    OrderFID = NewID,
                    ItemFID = Cart[i].foodItem.ItemID,
                    Quantity = Cart[i].quantity
                };

                await firebaseDatabase.Child("OrderDetail").PostAsync(detail);

            }

            //================SMS Sending code==================
            String api = "https://lifetimesms.com/json?api_token=a02ce8b6084d9365748ab51acfaaed279f2bf57956&api_secret=KingsCafe&to=" + Phone + "&from=King's Cafe&message=Your Order is Booked Successfully. Thanks for Shopping here. Regards: King's Cafe";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(api);
            var httpResponse = (HttpWebResponse)req.GetResponse();

            //EMPTY CART======================================================================
            Session["Cart"] = null;


            if (PaymentMethod == "COD")
            {
                return Redirect("/Home/OrderConfirmed/" + NewID);

            }
            else if (PaymentMethod == "Paypal")
            {
                return Redirect("https://www.sandbox.paypal.com/cgi-bin/webscr?cmd=_xclick&amount=" + total / 220 + "&business=JanjuaTailors@Shop.com&item_name=ToyLand&return=https://localhost:44300/Home/OrderConfirmed/" + NewID);

            }
            else
            {

            }
            return Redirect("/Home/OrderConfirmed/" + NewID);

        }
        public async Task<ActionResult> OrderConfirmed(int id)
        {
            var OrderDetails = (await firebaseDatabase.Child("OrderDetail").OnceAsync<OrderDetail>()).Where(x => x.Object.OrderFID == id).ToList();
            List<CartItem> Cart = new List<CartItem>();

            foreach (var item in OrderDetails)
            {
                var food = (await firebaseDatabase.Child("FoodItem").OnceAsync<FoodItem>()).FirstOrDefault(x => x.Object.ItemID == item.Object.ItemFID);
                Cart.Add(new CartItem { foodItem = food.Object, quantity = item.Object.Quantity });
            }

            ViewBag.Items = Cart;
            var order = (await firebaseDatabase.Child("Order").OnceAsync<Order>()).FirstOrDefault(x => x.Object.OrderID == id);

            TempData["State"] = "success";
            TempData["Message"] = "Order has confirmed. It will be delivered soon order details are as under";

            return View(order.Object);
        }



        public ActionResult login()
        {

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> login(String Email, String Password)
        {

            var check = (await firebaseDatabase.Child("User").OnceAsync<User>()).FirstOrDefault(x => x.Object.Email == Email && x.Object.Password == Password);

            if (check.Object.Type == "Admin")
            {
                CurrentAdmin.Current_Admin = check.Object;
                return RedirectToAction("IndexAdmin");
            }
            else
                return View();
            


        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}