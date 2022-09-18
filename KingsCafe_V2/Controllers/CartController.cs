//using KingsCafe_V2.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace KingsCafe.Controllers
//{
//    public class CartController : Controller
//    {
        
//        // GET: Cart

//        public ActionResult Displaycart()
//        {
//            return View();
//        } 
//        public ActionResult Ordercomplete()
//        {
//            return View();
//        } 
//        public ActionResult OrderBooked(Order order)
//        {
//            if (Session["cart"]!=null)
//            {

//                order.Status = "Booked";
//                order.OrderDate = System.DateTime.Now;
//                order.OrderTime = null;

//                db.tblOrders.Add(order);
//                db.SaveChanges();

//                foreach (var item in (List<FoodItem>)Session["cart"])
//                {
//                    OrderDetail od = new OrderDetail();

//                    od.ItemFID = item.ItemID;
//                    od.ORDER_DETAILS_PRICE = item.FOOD_PRODUCTS_PRICE;
//                    od.Quantity = item.Quantity;
//                    od.OrderFID = order.OrderID;
//                    //od.ORDER_DETAILS_isAdding = "false";
//                    db.tblOrderDetails.Add(od);
//                    db.SaveChanges();
//                }

//                TempData["cart"] = Session["cart"];
//                Session["cart"] = null;
//                return RedirectToAction("Ordercomplete");
//            }
//            else
//                return RedirectToAction("index", "Home");
//        }
//        public ActionResult Checkout()
//        {
//            return View();
//        }
//        public ActionResult Removefromcart(int id)
//        {
//            List<FoodItem> tblFoodProductslist = new List<FoodItem>();
//            if (Session["Cart"] != null)
//            {
//                tblFoodProductslist = (List<FoodItem>)Session["Cart"];
//            }
//            tblFoodProductslist.RemoveAt(id);
//            Session["Cart"] = tblFoodProductslist;
//            return RedirectToAction("Displaycart");
//        }
//        public ActionResult Addtocart(int id)

//        {
//            List<FoodItem> list = new List<FoodItem>();
//            FoodItem tblFoodProduct1 = new FoodItem();
//            if (Session["Cart"]!=null)
//            {
//                list = (List<FoodItem>)Session["Cart"];
//            }

//                tblFoodProduct1=list.Where(x=>x.ItemID==id).FirstOrDefault();
            
//            if (tblFoodProduct1==null)
//            {
//                tblFoodProduct1 = db.tblFoodProducts.Where(x => x.FOOD_PRODUCTS_ID == id).FirstOrDefault();
//                tblFoodProduct1.Quantity = 1;
//                list.Add(tblFoodProduct1);
//            }
//            else
//            {
//                tblFoodProduct1.Quantity++;
//            }
            
//            Session["cart"] = list;
//            return RedirectToAction("menu","home");
//        }
//        public ActionResult Plustocart(int id)
//        {
//            List<FoodItem> tblFoodProductslist = new List<FoodItem>();
//            if (Session["Cart"] != null)
//            {
//                tblFoodProductslist = (List<FoodItem>)Session["Cart"];
//            }
//            tblFoodProductslist[id].Quantity++;
//            Session["Cart"] = tblFoodProductslist;
//            return RedirectToAction("DisplayCart");
//        }
       
//        public ActionResult Minusfromcart(int id)
//        {
//            List<FoodItem> tblFoodProductslist = new List<FoodItem>();
//            if (Session["Cart"] != null)
//            {
//                tblFoodProductslist = (List<FoodItem>)Session["Cart"];
//            }
//            tblFoodProductslist[id].Quantity--;
//            if (tblFoodProductslist[id].Quantity < 1)
//            {
//                tblFoodProductslist.RemoveAt(id);
//            }
//            Session["Cart"] = tblFoodProductslist;
//            return RedirectToAction("DisplayCart");
//        }
//    }
//}