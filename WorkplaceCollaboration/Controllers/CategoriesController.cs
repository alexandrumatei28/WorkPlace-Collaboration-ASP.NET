using WorkplaceCollaboration.Data;
using WorkplaceCollaboration.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WorkplaceCollaboration.Controllers
{
    public class CategoriesController : Controller
    {

            private readonly ApplicationDbContext db;

            private readonly UserManager<ApplicationUser> _userManager;

            private readonly RoleManager<IdentityRole> _roleManager;

            public CategoriesController(
                ApplicationDbContext context,
                UserManager<ApplicationUser> userManager,
                RoleManager<IdentityRole> roleManager
                )
            {
                db = context;

                _userManager = userManager;

                _roleManager = roleManager;
            }

            public ActionResult Index()
            {
                
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }


                if (TempData.ContainsKey("message"))
                {
                ViewBag.message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
                }

                var categories = from category in db.Categories
                                 orderby category.CategoryName
                                 select category;
                ViewBag.Categories = categories;
                return View();
            }

            public ActionResult Show(int id)
            {

            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            Category category = db.Categories.Find(id);


            List<Channel> channelsInCategory = db.Channels.Where(c => c.CategoryId == id).ToList();

          
            ViewBag.ChannelsInCategory = new SelectList(channelsInCategory, "Id", "Name");

            return View(category);
            }

            public ActionResult New()
            {

            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
            }

            [HttpPost]
            public ActionResult New(Category cat)
            {
            

            if (ModelState.IsValid)
                {
                    db.Categories.Add(cat);
                    db.SaveChanges();
                    TempData["message"] = "Categoria a fost adaugata!";
                    return RedirectToAction("Index");
                }

                else
                {
                    return View(cat);
                }
            }

            public ActionResult Edit(int id)
            {
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Home");
            }

            Category category = db.Categories.Find(id);
                return View(category);
            }

            [HttpPost]
            public ActionResult Edit(int id, Category requestCategory)
            {
                Category category = db.Categories.Find(id);


            if (ModelState.IsValid)
                {

                    category.CategoryName = requestCategory.CategoryName;
                    db.SaveChanges();
                    TempData["message"] = "Categoria a fost modificata!";
                    TempData["messageType"] = "alert-success";
                 return RedirectToAction("Index");
                }
                else
                {
                    return View(requestCategory);
                }
            }

            [HttpPost]
            public ActionResult Delete(int id)
            {
                Category category = db.Categories.Find(id);
                db.Categories.Remove(category);
                TempData["message"] = "Categoria a fost stearsa!";
                TempData["messageType"] = "alert-success";
                db.SaveChanges();
                return RedirectToAction("Index");
            }
        
    }
}
