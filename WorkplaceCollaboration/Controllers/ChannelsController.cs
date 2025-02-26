using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration.UserSecrets;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using WorkplaceCollaboration.Controllers;
using WorkplaceCollaboration.Data;
using WorkplaceCollaboration.Models;

namespace WorkplaceCollaboration.Controllers
{
   
    public class ChannelsController : Controller

    {    
        private readonly ApplicationDbContext db;

    private readonly UserManager<ApplicationUser> _userManager;

    private readonly RoleManager<IdentityRole> _roleManager;

    public ChannelsController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager
        )
    {
        db = context;

        _userManager = userManager;

        _roleManager = roleManager;
    }

    // Se afiseaza lista tuturor canalelor impreuna cu categoria 
    // din care fac parte
    // Pentru fiecare canal se afiseaza si userul care a creat canalul respectiv
    // HttpGet implicit
    [Authorize(Roles = "User,Moderator,Admin")]
    public IActionResult Index()
    {

            Dictionary<int, List<string>> channelUserDictionary = db.ChannelUsers
    .Where(ch => ch.Status == "1")
    .AsEnumerable()
    .GroupBy(channelUser => channelUser.ChannelId)
    .ToDictionary(
        group => group.Key,
        group => group.Select(channelUser => channelUser.UserId).ToList()
    );

            ViewBag.ChannelAccess = channelUserDictionary;

            Dictionary<int, List<string>> channelPendingUserDictionary = db.ChannelUsers
    .Where(ch => ch.Status == "0")
    .AsEnumerable()
    .GroupBy(channelUser => channelUser.ChannelId)
    .ToDictionary(
        group => group.Key,
        group => group.Select(channelUser => channelUser.UserId).ToList()
    );

            ViewBag.ChannelPendingAccess = channelPendingUserDictionary;

            var searchTerm = (HttpContext.Request.Query["searchTerm"]);

        // Alegem sa afisam 3 canale pe pagina
        int _perPage = 3;

        var channels = db.Channels.Include("Category").Include("User").OrderBy(a => a.Date);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var query = db.Channels
                .Include("Category")
                .Include("User");

                query = query.Where(ch => ch.Name.Contains(searchTerm));

                channels = query
                .OrderBy(a => a.Date);
            }

        if (TempData.ContainsKey("message"))
        {
            ViewBag.Message = TempData["message"];
            ViewBag.Alert = TempData["messageType"];
        }

        // Fiind un numar variabil de canale, verificam de fiecare data utilizand
        // metoda Count()
        int totalItems = channels.Count();

        // Se preia pagina curenta din View-ul asociat
        // Numarul paginii este valoarea parametrului page din ruta
        // /Channels/Index?page=valoare
        var currentPage =
        Convert.ToInt32(HttpContext.Request.Query["page"]);
        // Pentru prima pagina offsetul o sa fie zero
        // Pentru pagina 2 o sa fie 3
        // Asadar offsetul este egal cu numarul de canale care au fost deja afisate pe paginile anterioare
        var offset = 0;
        // Se calculeaza offsetul in functie de numarul paginii la care suntem
        if (!currentPage.Equals(0))
        {
            offset = (currentPage - 1) * _perPage;
        }



        // Se preiau canalele corespunzatoare pentru fiecare pagina la care ne aflam
        // in functie de offset
        var paginatedChannels =
        channels.Skip(offset).Take(_perPage);

        // Preluam numarul ultimei pagini
        ViewBag.lastPage = Math.Ceiling((float)totalItems /
        (float)_perPage);
        // Trimitem canalele cu ajutorul unui ViewBag catre View-ul corespunzator
        ViewBag.Channels = paginatedChannels;

            SetAccessRights();

        return View();
    }

        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Index2()
        {

            Dictionary<int, List<string>> channelUserDictionary = db.ChannelUsers
    .Where(ch => ch.Status == "1")
    .AsEnumerable()
    .GroupBy(channelUser => channelUser.ChannelId)
    .ToDictionary(
        group => group.Key,
        group => group.Select(channelUser => channelUser.UserId).ToList()
    );

            ViewBag.ChannelAccess = channelUserDictionary;

            Dictionary<int, List<string>> channelPendingUserDictionary = db.ChannelUsers
    .Where(ch => ch.Status == "0")
    .AsEnumerable()
    .GroupBy(channelUser => channelUser.ChannelId)
    .ToDictionary(
        group => group.Key,
        group => group.Select(channelUser => channelUser.UserId).ToList()
    );

            ViewBag.ChannelPendingAccess = channelPendingUserDictionary;

            var searchTerm = (HttpContext.Request.Query["searchTerm"]);

            // Alegem sa afisam 3 canale pe pagina
            int _perPage = 3;

            List<int> channelInclude = db.ChannelUsers.Where(row => row.Status == "1" 
            && row.UserId == _userManager.GetUserId(User)).Select(row => row.ChannelId).ToList();


            if (channelInclude.Count == 0)
            {
                TempData["message"] = "Nu sunteti inscris in niciun canal!";
                TempData["messageType"] = "alert-danger";
            }


            ViewBag.ChannelInclude = channelInclude;

            var channels = db.Channels.Include("Category").Include("User").Where(row => channelInclude.Contains(row.Id)).OrderBy(a => a.Date);



            if (!string.IsNullOrEmpty(searchTerm))
            {
                var query = db.Channels
                .Include("Category")
                .Include("User");

                query = query.Where(ch => ch.Name.Contains(searchTerm));




                channels = query
                .OrderBy(a => a.Date);
            }



            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            // Fiind un numar variabil de canale, verificam de fiecare data utilizand
            // metoda Count()
            int totalItems = channels.Count();

            // Se preia pagina curenta din View-ul asociat
            // Numarul paginii este valoarea parametrului page din ruta
            // /Channels/Index?page=valoare
            var currentPage =
            Convert.ToInt32(HttpContext.Request.Query["page"]);
            // Pentru prima pagina offsetul o sa fie zero
            // Pentru pagina 2 o sa fie 3
            // Asadar offsetul este egal cu numarul de canale care au fost deja afisate pe paginile anterioare
            var offset = 0;
            // Se calculeaza offsetul in functie de numarul paginii la care suntem
            if (!currentPage.Equals(0))
            {
                offset = (currentPage - 1) * _perPage;
            }



            // Se preiau canalele corespunzatoare pentru fiecare pagina la care ne aflam
            // in functie de offset
            var paginatedChannels =
            channels.Skip(offset).Take(_perPage);

            // Preluam numarul ultimei pagini
            ViewBag.lastPage = Math.Ceiling((float)totalItems /
            (float)_perPage);
            // Trimitem canalele cu ajutorul unui ViewBag catre View-ul corespunzator
            ViewBag.Channels = paginatedChannels;

            SetAccessRights();

            return View();
        }

        // Se afiseaza un singur canal in functie de id-ul sau 
        // impreuna cu categoria din care face parte
        // In plus sunt preluate si toate mesajele asociate unui canal
        // Se afiseaza si userul care a postat canalul respectiv
        // HttpGet implicit

        [Authorize(Roles = "User,Moderator,Admin")]
    public IActionResult Show(int id)
    {
        Channel channel = db.Channels.Include("Category")
                                     .Include("User")
                                     .Include("Messages")
                                     .Include("Messages.User")
                                     .Where(ch => ch.Id == id)
                                     .First();

            List<ChannelUser> channelUserList = db.ChannelUsers
                                                      .Where(ch => ch.ChannelId == id).ToList();

            Dictionary<string, string> userStatusDictionary = db.ChannelUsers
    .Where(ch => ch.ChannelId == id)
    .ToDictionary(
        channelUser => db.Users.FirstOrDefault(u => u.Id == channelUser.UserId)?.UserName,
        channelUser => channelUser.Status
    );
            ViewBag.UserStatus = userStatusDictionary;


        SetAccessRights();

        if (User.IsInRole("Admin") || User.IsInRole("Moderator"))
            {
                return View(channel);
            } 
        else

        foreach (ChannelUser user in channelUserList)
        {
                if (user.UserId == _userManager.GetUserId(User))
                    return View(channel);
            }

            TempData["message"] = "Nu aveti drepturi pentru a vizualiza acest canal!";
            TempData["messageType"] = "alert-danger";


            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
                ViewBag.Alert = TempData["messageType"];
            }

            return RedirectToAction("Index");

            

       }
    

    // Adaugarea unui mesaj asociat unui canal in baza de date
    // Toate rolurile pot adauga mesaje in baza de date
    [HttpPost]
    [Authorize(Roles = "User,Moderator,Admin")]
    public IActionResult Show([FromForm] Message message)
    {
        message.Date = DateTime.Now;
        message.UserId = _userManager.GetUserId(User);

        if (ModelState.IsValid)
        {
            db.Messages.Add(message);
            db.SaveChanges();
            return Redirect("/Channels/Show/" + message.ChannelId);
        }

        else
        {
            Channel ch = db.Channels.Include("Category")
                                     .Include("User")
                                     .Include("Messages")
                                     .Include("Messages.User")
                                     .Where(ch => ch.Id == message.ChannelId)
                                     .First();

           

           

            SetAccessRights();

            return View(ch);
        }
    }

    

    // Se afiseaza formularul in care se vor completa datele unui canal
    // impreuna cu selectarea categoriei din care face parte
 
    // HttpGet implicit

    [Authorize(Roles = "Moderator,Admin,User")]
    public IActionResult New()
    {
        Channel channel = new Channel();

        // Se preia lista de categorii cu ajutorul metodei GetAllCategories()
        channel.Categ = GetAllCategories();


        return View(channel);
    }

    // Se adauga canalul in baza de date
   
    [Authorize(Roles = "Moderator,Admin,User")]
    [HttpPost]
    public IActionResult New(Channel channel)
    {
        channel.Date = DateTime.Now;

            // preluam id-ul utilizatorului care creeaza canalul
            channel.UserId = _userManager.GetUserId(User);

        if (ModelState.IsValid)
        {
            db.Channels.Add(channel);
               
            db.SaveChanges();

                ChannelUser channelUser = new ChannelUser();
                channelUser.ChannelId = channel.Id;
                channelUser.UserId = channel.UserId;
                channelUser.Status = "1";
                db.ChannelUsers.Add(channelUser);

                db.SaveChanges();

                TempData["message"] = "Canalul a fost creat";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Index");
        }
        else
        {
            channel.Categ = GetAllCategories();
            return View(channel);
        }
    }

        // Se editeaza un canal existent in baza de date impreuna cu categoria din care face parte
        // Categoria se selecteaza dintr-un dropdown
        // Se afiseaza formularul impreuna cu datele aferente canalului din baza de date
        // Doar utilizatorii cu rolul de Moderator sau Admin pot edita canale
        // Adminii pot edita orice canal din baza de date
        // Moderatorii pot edita doar canelele proprii (cele pe care ei le-au creat)
        // HttpGet implicit

        [Authorize(Roles = "Moderator,Admin,User")]
    public IActionResult Edit(int id)
    {

        Channel channel = db.Channels.Include("Category")
                                    .Where(ch => ch.Id == id)
                                    .First();

        channel.Categ = GetAllCategories();

        if (channel.UserId == _userManager.GetUserId(User) || User.IsInRole("Moderator") || User.IsInRole("Admin"))
        {
            return View(channel);
        }

        else
        {
            TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui canal care nu va apartine";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("Index");
        }

    }

    // Se adauga canalul modificat in baza de date
    // Verificam rolul utilizatorilor care au dreptul sa editeze (Moderator,Admin sau User)
    [HttpPost]
    [Authorize(Roles = "Moderator,Admin,User")]
    public IActionResult Edit(int id, Channel requestChannel)
    {
        Channel channel = db.Channels.Find(id);


        if (ModelState.IsValid)
        {
            if (channel.UserId == _userManager.GetUserId(User) || User.IsInRole("Moderator") || User.IsInRole("Admin"))
            {
                    channel.Name = requestChannel.Name;
                    channel.Description = requestChannel.Description;
                    channel.CategoryId = requestChannel.CategoryId;
                TempData["message"] = "Canalul a fost modificat";
                TempData["messageType"] = "alert-success";
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra unui canal care nu va apartine";
                TempData["messageType"] = "alert-danger";
                return RedirectToAction("Index");
            }
        }
        else
        {
                requestChannel.Categ = GetAllCategories();
            return View(requestChannel);
        }
    }



    // Se sterge un canal din baza de date
    // Utilizatorii cu rolul de Moderator sau Admin pot sterge canalele
    // Adminii pot sterge orice canal din baza de date

    [HttpPost]
    [Authorize(Roles = "Moderator,Admin,User")]
    public ActionResult Delete(int id)
    {
        Channel channel = db.Channels.Include("Messages")
                                     .Where(ch => ch.Id == id)
                                     .First();

        List<ChannelUser> channelUserList = db.ChannelUsers.Where(ch => ch.ChannelId == id).ToList();

        if (channel.UserId == _userManager.GetUserId(User) || User.IsInRole("Moderator") || User.IsInRole("Admin"))
        {
            for (int i = 0; i < channelUserList.Count(); i++)
                    db.ChannelUsers.Remove(channelUserList[i]);

            db.Channels.Remove(channel);
            db.SaveChanges();

            
            TempData["message"] = "Canalul a fost sters!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Index");
        }

        else
        {
            TempData["message"] = "Nu aveti dreptul sa stergeti un canal care nu va apartine";
            TempData["messageType"] = "alert-danger";
            return RedirectToAction("Index");
        }
    }


        [HttpPost]
        [Authorize(Roles = "Moderator,Admin,User")]
        public ActionResult CancelJoin(int id)
        {
            ChannelUser channelUser = db.ChannelUsers
            .FirstOrDefault(ch => ch.ChannelId == id && ch.UserId == _userManager.GetUserId(User));

            db.ChannelUsers.Remove(channelUser);
            db.SaveChanges();


            TempData["message"] = "Cererea a fost stearsa!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Moderator,Admin,User")]
        public ActionResult Reject(string id)
        {
            var strings = id.Split("-");
            var userName = strings[0];
            var channelId = int.Parse(strings[1]);
            ApplicationUser user = db.Users.FirstOrDefault(u => u.UserName == userName);


            ChannelUser channelUser = db.ChannelUsers
            .FirstOrDefault(ch => ch.ChannelId == channelId && ch.UserId == user.Id);

            db.ChannelUsers.Remove(channelUser);
            db.SaveChanges();


            TempData["message"] = "Cererea a fost respinsa!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Show", new { id = channelId });
        }

        [HttpPost]
        [Authorize(Roles = "Moderator,Admin,User")]
        public ActionResult Accept(string id)
        {
            var strings = id.Split("-");
            var userName = strings[0];
            var channelId = int.Parse(strings[1]);
            ApplicationUser user = db.Users.FirstOrDefault(u => u.UserName == userName);


            ChannelUser channelUser = db.ChannelUsers
            .FirstOrDefault(ch => ch.ChannelId == channelId && ch.UserId == user.Id);
            db.ChannelUsers.Remove(channelUser);

            ChannelUser updatedChannelUser = new ChannelUser();
            updatedChannelUser.ChannelId = channelId;
            updatedChannelUser.UserId = user.Id;
            updatedChannelUser.Status = "1";
            db.ChannelUsers.Add(updatedChannelUser);

            db.SaveChanges();


            TempData["message"] = "Cererea a fost acceptata!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Show", new { id = channelId });
        }

        [HttpPost]
        [Authorize(Roles = "Moderator,Admin,User")]
        public ActionResult Join(int id)
        {
            ChannelUser channelUser = new ChannelUser();

            channelUser.ChannelId = id;
            channelUser.UserId = _userManager.GetUserId(User);
            channelUser.Status = "0";

            db.ChannelUsers.Add(channelUser);
            db.SaveChanges();


            TempData["message"] = "S-a solicitat inscrierea!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Index");
        }


        //Eliminarea userilor din canale
        [HttpPost]
        [Authorize(Roles = "Moderator,Admin,User")]
        public ActionResult Leave(string id)
        {
            var strings = id.Split("-");
            var userName = strings[0];
            var channelId = int.Parse(strings[1]);
            ApplicationUser user = db.Users.FirstOrDefault(u => u.UserName == userName);


            ChannelUser channelUser = db.ChannelUsers
            .FirstOrDefault(ch => ch.ChannelId == channelId && ch.UserId == user.Id);

            db.ChannelUsers.Remove(channelUser);
            db.SaveChanges();


            TempData["message"] = "Utilizatorul a fost exclus!";
            TempData["messageType"] = "alert-success";
            return RedirectToAction("Show", new { id = channelId });
        }

        //Userii pot iesi singuri din canale
        public ActionResult Exit(string id)
        {
            var strings = id.Split("-");
            var userName = strings[0];
            var channelId = int.Parse(strings[1]);
            ApplicationUser user = db.Users.FirstOrDefault(u => u.UserName == userName);


            ChannelUser channelUser = db.ChannelUsers
            .FirstOrDefault(ch => ch.ChannelId == channelId && ch.UserId == user.Id);

            db.ChannelUsers.Remove(channelUser);
            db.SaveChanges();

            return RedirectToAction("Index2", new { id = channelId });
        }


        // Conditiile de afisare a butoanelor de editare si stergere
        private void SetAccessRights()
    {
        ViewBag.EsteAdmin = User.IsInRole("Admin");
            ViewBag.EsteModerator = User.IsInRole("Moderator");

            ViewBag.UserCurent = _userManager.GetUserId(User);
            ViewBag.UserNameCurent = _userManager.GetUserName(User);
    }

    [NonAction]
    public IEnumerable<SelectListItem> GetAllCategories()
    {
        // generam o lista de tipul SelectListItem fara elemente
        var selectList = new List<SelectListItem>();

        // extragem toate categoriile din baza de date
        var categories = from cat in db.Categories
                         select cat;

        // iteram prin categorii
        foreach (var category in categories)
        {
            // adaugam in lista elementele necesare pentru dropdown
            // id-ul categoriei si denumirea acesteia
            selectList.Add(new SelectListItem
            {
                Value = category.Id.ToString(),
                Text = category.CategoryName.ToString()
            });
        }
        


        // returnam lista de categorii
        return selectList;
    }
    public IActionResult IndexNou()
    {
        return View();
    }
    }



}
