using MailKit.Net.Smtp;
using MailKit.Security;
using CMS.Data;
using CMS.Models;
using MailKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using MimeKit;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Identity;
using NuGet.Protocol;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.AspNetCore.Http;

namespace CMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor cont;
        private readonly db_cmsContext db;


        public HomeController(ILogger<HomeController> logger , db_cmsContext db , IHttpContextAccessor cont)
        {
            _logger = logger;
            this.db = db;
            this.cont = cont;

        }
		[HttpGet]
        public IActionResult Index()
        {

			TempData["Name"] = cont.HttpContext.Session.GetString("name");
			TempData["Email"] = cont.HttpContext.Session.GetString("email");
			TempData["id"] = cont.HttpContext.Session.GetInt32("session_id");

			var fetchevent = db.TblEventBookings.Where(x => x.UserId == cont.HttpContext.Session.GetInt32("session_id")).ToList();

			if (fetchevent.Count > 0)
			{
				var E_uid = fetchevent[0].UserId;

				cont.HttpContext.Session.SetInt32("uid", (int)E_uid);

				
				TempData["e_id"] = cont.HttpContext.Session.GetInt32("uid");

			}


			var viewModel = new AllTables
			{
			product_list = db.TblProducts.ToList(),
		    upcoming_events = db.TblUpcomingEvents.ToList()

			};

		
			return View(viewModel);
        }

        public IActionResult ShowEvents()
		{
			TempData["Name"] = cont.HttpContext.Session.GetString("name");
			TempData["Email"] = cont.HttpContext.Session.GetString("email");
			TempData["id"] = cont.HttpContext.Session.GetInt32("session_id");
			TempData["e_id"] = cont.HttpContext.Session.GetInt32("uid");

			return View(db.TblUpcomingEvents.ToList());
		}
		

        [HttpGet]
		public IActionResult EventBookingForm( int id)
		{
			TempData["Name"] = cont.HttpContext.Session.GetString("name");
			TempData["Email"] = cont.HttpContext.Session.GetString("email");
			TempData["id"] = cont.HttpContext.Session.GetInt32("session_id");
			TempData["e_id"] = cont.HttpContext.Session.GetInt32("uid");

			var model = db.TblUpcomingEvents.FirstOrDefault(x => x.Id == id);


			var sadsa = "sdfd";
			return View(model);

        }
		[HttpPost]
		public IActionResult EventBookingForm(TblEventBooking book, IFormFile img, int userid, int eventid)
		{
			if (img != null && img.Length > 0)
			{
				var fileExt = System.IO.Path.GetExtension(img.FileName).Substring(1);

				var random = Path.GetFileName(img.FileName);

				var FileName = Guid.NewGuid() + random;

				string imgFolder = Path.Combine(HttpContext.Request.PathBase.Value, "wwwroot/TransactionImages");

				if (!Directory.Exists(imgFolder))
				{
					Directory.CreateDirectory(imgFolder);
				}
				string filepath = Path.Combine(imgFolder, FileName);
				using (var stream = new FileStream(filepath, FileMode.Create))
				{
					img.CopyTo(stream);
				}
				


				var dbAddress = Path.Combine("TransactionImages", FileName);


				TblEventBooking tbl = new TblEventBooking();
				tbl.Img = dbAddress;
				tbl.EventId = eventid;
				tbl.UserId = userid;
				tbl.Status = 0;

				


				var abs = tbl;

				var dsd = "fsdf";

				// Save event data to tbl_event_booking
				db.TblEventBookings.Add(tbl);
				db.SaveChanges();
				return RedirectToAction("Confirmation");

			}

			return View();

		}

		[HttpGet]
        public IActionResult AddtoCart(int id, int? qty)
        {
			
            var product = db.TblProducts.Find(id);

			int quantityToAdd = qty.HasValue && qty > 0 ? qty.Value : 1;
			// Retrieve existing cart items from session or create a new list
			List<CartItem> cartItems = HttpContext.Session.Get<List<CartItem>>("cart") ?? new List<CartItem>();

            // Check if the item is already in the cart
            var existingItem = cartItems.FirstOrDefault(item => item.Id == product.Id);
			var gfv = "lji";

            if (existingItem != null)
            {
                // Update quantity if item already exists in the cart
                existingItem.Quantity += quantityToAdd;
            }
            else
            {
                // Add a new item to the cart
                cartItems.Add(new CartItem
                {
                    Name = product.ProductName,
                    Description = product.Description,
                    Price = (int)product.Saleprice,
                    Quantity = quantityToAdd,
					Image = product.Image,
                    Id = product.Id
                });

            }
            // Update the session with the new cart items
            HttpContext.Session.Set("cart", cartItems);

            return RedirectToAction("Cart");
        }

        [HttpGet]
        public IActionResult Cart()
        {
			TempData["Name"] = cont.HttpContext.Session.GetString("name");
			TempData["Email"] = cont.HttpContext.Session.GetString("email");
			TempData["id"] = cont.HttpContext.Session.GetInt32("session_id");

			List<CartItem> cartItems = HttpContext.Session.Get<List<CartItem>>("cart");

            if (cartItems != null && cartItems.Any())
            {
                ViewBag.Items = cartItems;
			}
			
			
			
            return View();

        }
		[HttpPost]
		public IActionResult Checkout(TblOrder c_order)
		{
			TempData["Name"] = cont.HttpContext.Session.GetString("name");
			TempData["Email"] = cont.HttpContext.Session.GetString("email");
			TempData["id"] = cont.HttpContext.Session.GetInt32("session_id");
			TempData["e_id"] = cont.HttpContext.Session.GetInt32("uid");

			var order = new TblOrder
                {
                    // Set order properties
                    Address = c_order.Address, 
                    UId = c_order.UId, 
                    Date = c_order.Date, 
                    TotalPurchase = c_order.TotalPurchase 
                };
                db.TblOrders.Add(order);

			db.SaveChanges();

            int orderId = order.Id;

            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>("cart");

			if (cart == null || cart.Count == 0 )
			{
				return RedirectToAction("Index");
			}
			else
			{

			// Save cart items to tbl_checkout and tbl_order
			foreach (var item in cart)
			{
					var checkoutItem = new TblCheckout
					{
						PName = item.Name,
						PDes = item.Description,
						PPrice = item.Price,
						PQty = item.Quantity,
						OrderId = orderId
                        // Set other properties accordingly
                    };
				
					var ghbhb = "jhujbj";
				db.TblCheckouts.Add(checkoutItem);

                }
			

				TempData["Order"] = "Your Order has been placed";


                // Clear the cart after checkout
                HttpContext.Session.Remove("cart");
                db.SaveChanges();

                return View("Confirmation");
			}

		}

		[HttpGet]
		public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
		public IActionResult Login(TblClientRegister Auth)
		{
			var front_email = Auth.Email;
			var front_password = Auth.Password;

			var fetchuser = db.TblClientRegisters.Where(x => x.Email == front_email).ToList();

            if (fetchuser.Count > 0)
            {
                
            var backend_name = fetchuser[0].Name;
			var backend_email = fetchuser[0].Email;
            var backend_id = fetchuser[0].Id;
			var backend_password = fetchuser[0].Password;


            if (front_email == backend_email && front_password == backend_password)
			{
				cont.HttpContext.Session.SetString("name", backend_name);
				cont.HttpContext.Session.SetString("email", backend_email);
				cont.HttpContext.Session.SetInt32("session_id", backend_id);

				var session_name = cont.HttpContext.Session.GetString("name");
				var session_email = cont.HttpContext.Session.GetString("email");
				var session_id = cont.HttpContext.Session.GetInt32("session_id");

                TempData["Name"] = session_name;
                TempData["Email"] = session_email;
                TempData["id"] = session_id;


                    return RedirectToAction("Index", "Home");

			}
			

          }else if(fetchuser.Count == 0)
			{
				TempData["Alertmsg"] = "No User Found Or Wrong Credential";
				
			}


			return View();
		}
		public IActionResult Logout()
		{
			cont.HttpContext.Session.Clear();
			return RedirectToAction("Index");
		}
		public IActionResult Aboutus()
        {
			TempData["Name"] = cont.HttpContext.Session.GetString("name");
			TempData["Email"] = cont.HttpContext.Session.GetString("email");
			TempData["id"] = cont.HttpContext.Session.GetInt32("session_id");
			TempData["e_id"] = cont.HttpContext.Session.GetInt32("uid");

		 TblAbout fetchabout = db.TblAbouts.FirstOrDefault(x => x.Id == 1);


			return View(fetchabout);
        } 
        public IActionResult Contactus()
        {
			TempData["Name"] = cont.HttpContext.Session.GetString("name");
			TempData["Email"] = cont.HttpContext.Session.GetString("email");
			TempData["id"] = cont.HttpContext.Session.GetInt32("session_id");
			TempData["e_id"] = cont.HttpContext.Session.GetInt32("uid");

			return View();
        }
		[HttpGet]
		public IActionResult EventPass()
		{
			TempData["Name"] = cont.HttpContext.Session.GetString("name");
			TempData["Email"] = cont.HttpContext.Session.GetString("email");
			TempData["id"] = cont.HttpContext.Session.GetInt32("session_id");
			TempData["e_id"] = cont.HttpContext.Session.GetInt32("uid");

            var fetchevent = db.TblEventBookings.Where(x => x.UserId == cont.HttpContext.Session.GetInt32("session_id")).Include(x=>x.Event).ToList();

          

            return View(fetchevent);	
		}

		[HttpGet]
		public IActionResult Register()
		{

			return View();
		}
        [HttpPost]
		public IActionResult Register(TblClientRegister auth, string confrmpassword)
		{
			var front_email = auth.Email;
            var front_name = auth.Name;
            var msgbody = " Hello "+ front_name + "! Your Account has been registered!  " +
				" Best Regards, Providence Clinic  " +
				"Welcome to Providence Clinic";



			var pass_one = auth.Password;
            var pass_two = confrmpassword;


			var existingClient = db.TblClientRegisters.FirstOrDefault(c => c.Email == front_email);
			if (existingClient != null)
			{
				TempData["Alertmesg"] = "Email Already Exist";

			}
			else
			{


				if (pass_one == pass_two)
				{
					string value = "vwrj iivm qgpy vqdu";

					var msg = new MimeMessage();
					msg.From.Add(new MailboxAddress("Providence Clinic", "huzaifairfan2144@gmail.com"));
					msg.To.Add(new MailboxAddress(front_name, front_email));
					msg.Subject = "Providence Clinic Account Registration";
					msg.Body = new TextPart { Text = msgbody };

					using var client = new SmtpClient();
					client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTlsWhenAvailable);
					client.Authenticate("huzaifairfan2144@gmail.com", value);
					client.Send(msg);

					db.TblClientRegisters.Add(auth);
					db.SaveChanges();
					return RedirectToAction("Confirmation");
				}
				else
				{
					return RedirectToAction("Index");

				}
			}
            return View();
		}
		[HttpGet]
		public IActionResult ProductsPage(string category)
		{
			TempData["Name"] = cont.HttpContext.Session.GetString("name");
			TempData["Email"] = cont.HttpContext.Session.GetString("email");
			TempData["id"] = cont.HttpContext.Session.GetInt32("session_id");
			TempData["e_id"] = cont.HttpContext.Session.GetInt32("uid");

			ViewBag.category = category;
			ViewBag.AllProducts = db.TblProducts.ToList();
			ViewBag.MedicineProductsCount = db.TblProducts.Where(x=>x.Category == "medicine").Count();
			ViewBag.ScientificProductsCount = db.TblProducts.Where(x=>x.Category == "scientific").Count();
			ViewBag.AllProductsCount = db.TblProducts.Count();
			var fetchproducts = db.TblProducts.Where(x=>x.Category == category).Take(9).ToList();
			return View(fetchproducts);
		}
		public IActionResult Confirmation()
		{
			TempData["Name"] = cont.HttpContext.Session.GetString("name");
			TempData["Email"] = cont.HttpContext.Session.GetString("email");
			TempData["id"] = cont.HttpContext.Session.GetInt32("session_id");
			TempData["e_id"] = cont.HttpContext.Session.GetInt32("uid");

			return View();
		}
		[HttpGet]
		public IActionResult SubmitReview()
		{
			return View();
		}
		[HttpPost]
		public IActionResult SubmitReview(int id, int rating_input, string Title, string Text
			)
		{
			var session_id = cont.HttpContext.Session.GetInt32("session_id");

			TblFeedback model = new TblFeedback
			{
				UserId = session_id,
				Title = Title,
				PId = id,
				Rating = rating_input,
				Text = Text
			};
			db.TblFeedbacks.Add(model);
			db.SaveChanges();
			TempData["review_submitted"] = "Review Submitted";
			return RedirectToAction("SubmitReview");
		}
		[HttpGet]
		public IActionResult ProductDetails(int Id)
		{
			ViewBag.pid = Id;
		
			TempData["Name"] = cont.HttpContext.Session.GetString("name");
			TempData["Email"] = cont.HttpContext.Session.GetString("email");
			TempData["id"] = cont.HttpContext.Session.GetInt32("session_id");

			AllTables main_model = new AllTables()
			{
				product_list = db.TblProducts.Take(4).ToList(),
			client_register = db.TblClientRegisters.ToList(),
			upcoming_events = db.TblUpcomingEvents.ToList(),
			Images_list = db.TblImages.Where(x => x.PId == Id).ToList(),
			feedback_list = db.TblFeedbacks.Where(x => x.PId == Id).ToList(),
			productss = db.TblProducts.FirstOrDefault(x => x.Id == Id),
			imagess = db.TblImages.FirstOrDefault(x => x.PId == Id),
			clientRegister = new TblClientRegister(),
			upcomingEvent = new TblUpcomingEvent(),
			bookingss = new TblEventBooking(),
			reviewUsers = new List<string>()
			};

			foreach (var feedback in main_model.feedback_list)
			{
				var user = db.TblClientRegisters.FirstOrDefault(u => u.Id == feedback.UserId);
					string name = user.Name;
				TempData["review_u"] = name;
				main_model.reviewUsers.Add(name);
			}

			var a = "sdsa";

			return View(main_model);
		}

		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
