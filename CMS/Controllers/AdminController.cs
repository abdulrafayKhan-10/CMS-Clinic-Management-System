using CMS.Data;
using CMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Signers;
using System.Globalization;

namespace CMS.Controllers
{
    public class AdminController : Controller
    {
		private readonly db_cmsContext _context;
		private readonly IHttpContextAccessor cont;


		public AdminController(db_cmsContext context , IHttpContextAccessor cont)
		{
			_context = context;
			this.cont = cont;

		}
		[HttpGet]
		public IActionResult Index()
        {
			var session_name = cont.HttpContext.Session.GetString("name");
			var session_username = cont.HttpContext.Session.GetString("username");
			var session_id = cont.HttpContext.Session.GetInt32("session_id");

			TempData["Name"] = session_name;
			TempData["username"] = session_username;
			TempData["id"] = session_id;

			var fetchuser = _context.TblClientRegisters.ToList();
			int fetchCountuser = fetchuser.Count;

			TempData["usercount"] = fetchCountuser;

			var fetchorder = _context.TblOrders.Where(x => x.Status == 0).ToList();

			int fetchcountordre = fetchorder.Count;

			TempData["ordercount"] = fetchcountordre;

            var fetchbooking = _context.TblEventBookings.ToList();

            int fetchcountbooking = fetchbooking.Count;

            TempData["ordercount"] = fetchcountbooking;

            var fetchreview = _context.TblFeedbacks.ToList();

            int fetchcounrivie = fetchreview.Count;

            TempData["orderrivew"] = fetchcounrivie;



            if (session_username != null)
			{
                // Assuming you have access to your DbContext instance (db)

                // Query tbl_orders and tbl_event_bookings to retrieve sales data
                var orders = _context.TblOrders.ToList();
                //var eventBookings = _context.TblEventBookings.ToList();

                // Combine sales data from both tables (assuming TotalPurchase represents the sales amount)
                var allSales = orders.Select(o => new { Date = o.Date, Amount = o.TotalPurchase }).ToList();

                // Group sales data by month
                var monthlySales = allSales.GroupBy(s => new { Month = s.Date.Month, Year = s.Date.Year })
                                           .Select(g => new { Month = g.Key.Month, Year = g.Key.Year, TotalSales = g.Sum(s => s.Amount) })
                                           .OrderBy(g => g.Year).ThenBy(g => g.Month)
                                           .ToList();

                // Extract labels and data for Chart.js
                var labels = monthlySales.Select(g => $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(g.Month)} {g.Year}");
                var data = monthlySales.Select(g => g.TotalSales);

                // Serialize labels and data to JSON for Chart.js
                var jsonLabels = JsonConvert.SerializeObject(labels);
                var jsonData = JsonConvert.SerializeObject(data);
				var ab = new MonthlySalesViewModel { Labels = jsonLabels, Data = jsonData };

                IEnumerable<MonthlySalesViewModel> abcd = new List<MonthlySalesViewModel>();

                return View(ab);

            }
            else
			{
				return RedirectToAction("Login", "Admin");

			}


        }
		public IActionResult Logout()
		{
			cont.HttpContext.Session.Clear();
			return RedirectToAction("Index");
		}
		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
        public IActionResult Login(TblAdmin Auth)
        {
            var front_username = Auth.Username;
            var front_password = Auth.Password;

            var fetchuser = _context.TblAdmins.Where(x => x.Username == front_username).ToList();

            if (fetchuser.Count > 0)
            {

                var backend_name = fetchuser[0].AdminName;
                var backend_username = fetchuser[0].Username;
                var backend_id = fetchuser[0].Id;
                var backend_password = fetchuser[0].Password;


                if (front_username == backend_username && front_password == backend_password)
                {
                    cont.HttpContext.Session.SetString("name", backend_name);
                    cont.HttpContext.Session.SetString("username", backend_username);
                    cont.HttpContext.Session.SetInt32("session_id", backend_id);

                    var session_name = cont.HttpContext.Session.GetString("name");
                    var session_username = cont.HttpContext.Session.GetString("username");
                    var session_id = cont.HttpContext.Session.GetInt32("session_id");

                    TempData["Name"] = session_name;
                    TempData["username"] = session_username;
                    TempData["id"] = session_id;


                    return RedirectToAction("Index", "Admin");

                }


            }
            else if (fetchuser.Count == 0)
            {
                TempData["Alertmsg"] = "No User Found Or Wrong Credential";

            }


            return View();
        }
		[HttpGet]
		public IActionResult Stock()
		{
            var session_name = cont.HttpContext.Session.GetString("name");
            var session_username = cont.HttpContext.Session.GetString("username");
            var session_id = cont.HttpContext.Session.GetInt32("session_id");

            TempData["Name"] = session_name;
            TempData["username"] = session_username;
            TempData["id"] = session_id;


            if (session_username != null)
            {
                return View(_context.TblProducts.ToList());
            }
            else
            {
                return RedirectToAction("Login", "Admin");

            }
        }

        [HttpGet]
        public IActionResult ProductAdd()
        {
			var session_name = cont.HttpContext.Session.GetString("name");
			var session_username = cont.HttpContext.Session.GetString("username");
			var session_id = cont.HttpContext.Session.GetInt32("session_id");

			TempData["Name"] = session_name;
			TempData["username"] = session_username;
			TempData["id"] = session_id;

			if (session_username != null)
			{
				return View();
			}
			else
			{
				return RedirectToAction("Login", "Admin");

			}
        }
		[HttpPost]
		public async Task<IActionResult> ProductAdd(AllTables model, List<IFormFile> imgs, IFormFile banner)
		{
		
			if (banner != null && banner.Length > 0)
			{
				var fileExt = System.IO.Path.GetExtension(banner.FileName).Substring(1);

				var random = Path.GetFileName(banner.FileName);

				var FileName = Guid.NewGuid() + random;

				string imgFolder = Path.Combine(HttpContext.Request.PathBase.Value, "wwwroot/myImages");

				if (!Directory.Exists(imgFolder))
				{
					Directory.CreateDirectory(imgFolder);
				}
				string filepath = Path.Combine(imgFolder, FileName);
				using (var stream = new FileStream(filepath, FileMode.Create))
				{
					banner.CopyTo(stream);
				}
				var dbAddress = Path.Combine("myImages", FileName);
				model.productss.Image = dbAddress;




				// Save product data to tbl_product
				_context.TblProducts.Add(model.productss);
				_context.SaveChanges();

				// Save images to tbl_image
				foreach (var img in imgs)
				{
					var filesExt = System.IO.Path.GetExtension(img.FileName).Substring(1);

					var randoms = Path.GetFileName(img.FileName);

					var FilesName = Guid.NewGuid() + random;

					string imgsFolder = Path.Combine(HttpContext.Request.PathBase.Value, "wwwroot/myImages");

					if (!Directory.Exists(imgsFolder))
					{
						Directory.CreateDirectory(imgsFolder);
					}

					string filespath = Path.Combine(imgsFolder, FilesName);

					if (img != null && img.Length > 0)
					{
						using (var stream = new FileStream(filespath, FileMode.Create))
						{
							img.CopyTo(stream);

							var dbAddresses = Path.Combine("myImages", FilesName);
							var image = new TblImage
							{
								PId = model.productss.Id, // foreign key relationship
								ImageName = dbAddresses,
								AltTxt = "Image Alt Text", // Set your Alt text accordingly
							};

							_context.TblImages.Add(image);
							await _context.SaveChangesAsync();
						}
					}
				}
				var sadsa = "sdsad";
				return RedirectToAction("ProductView"); // Redirect to a view after successful submission
			}

			return View();
		}
		public IActionResult ProductView()
        {
			var session_name = cont.HttpContext.Session.GetString("name");
			var session_username = cont.HttpContext.Session.GetString("username");
			var session_id = cont.HttpContext.Session.GetInt32("session_id");

			TempData["Name"] = session_name;
			TempData["username"] = session_username;
			TempData["id"] = session_id;

			if (session_username != null)
			{
				var model = _context.TblProducts.ToList();
			return View(model);
			}
			else
			{
				return RedirectToAction("Login", "Admin");

			}
        }
		[HttpGet]
        public IActionResult AddEvent()
        {
			var session_name = cont.HttpContext.Session.GetString("name");
			var session_username = cont.HttpContext.Session.GetString("username");
			var session_id = cont.HttpContext.Session.GetInt32("session_id");

			TempData["Name"] = session_name;
			TempData["username"] = session_username;
			TempData["id"] = session_id;

			if (session_username != null)
			{
				return View();
			}
			else
			{
				return RedirectToAction("Login", "Admin");

			}
        }
        [HttpPost]
        public IActionResult AddEvent(TblUpcomingEvent newevent , IFormFile banner)
        {

			if (banner != null && banner.Length > 0)
			{
				// GETTING IMAGE FILE EXTENSION 
				var fileExt = System.IO.Path.GetExtension(banner.FileName).Substring(1);

				// GETTING IMAGE NAME
				var random = Path.GetFileName(banner.FileName);

				// GUID ID COMBINE WITH IMAGE NAME - TO ESCAPE IMAGE NAME REDENDNCY 
				var FileName = Guid.NewGuid() + random;

				// GET PATH OF CUSTOM IMAGE FOLDER
				string imgFolder = Path.Combine(HttpContext.Request.PathBase.Value, "wwwroot/BannerImgs");

				// CHECKING FOLDER EXIST OR NOT - IF NOT THEN CREATE F0LDER 
				if (!Directory.Exists(imgFolder))
				{
					Directory.CreateDirectory(imgFolder);
				}

				// MAKING CUSTOM AND COMBINE FOLDER PATH WITH IMAHE 
				string filepath = Path.Combine(imgFolder, FileName);

				// COPY IMAGE TO REAL PATH TO DEVELOPER PATH
				using (var stream = new FileStream(filepath, FileMode.Create))
				{
					banner.CopyTo(stream);
				}

				// READY SEND PATH TO  IMAGE TO DB  
				var dbAddress = Path.Combine("BannerImgs", FileName);

				// EQUALIZE TABLE (MODEL) PROPERTY WITH CUSTOM PATH 
				newevent.Banner = dbAddress;
				//MYIMAGES/imagetodbContext.JGP

				// SEND TO TABLE 
				_context.TblUpcomingEvents.Add(newevent);

				//SSAVE TO DB 

				_context.SaveChanges();

				return RedirectToAction("ViewEvent");
			}
			return View();
        }
        public IActionResult ViewEvent()
        {
			var session_name = cont.HttpContext.Session.GetString("name");
			var session_username = cont.HttpContext.Session.GetString("username");
			var session_id = cont.HttpContext.Session.GetInt32("session_id");

			TempData["Name"] = session_name;
			TempData["username"] = session_username;
			TempData["id"] = session_id;

			if (session_username != null)
			{
			return View(_context.TblUpcomingEvents.ToList());
			}
			else
			{
				return RedirectToAction("Login", "Admin");

			}
        }
        public IActionResult Bookings()
        {

            var tbl = _context.TblEventBookings.Include(y => y.User).Include(x => x.Event).Where(x => x.Status == 0).ToList();
			var session_name = cont.HttpContext.Session.GetString("name");
			var session_username = cont.HttpContext.Session.GetString("username");
			var session_id = cont.HttpContext.Session.GetInt32("session_id");

			TempData["Name"] = session_name;
			TempData["username"] = session_username;
			TempData["id"] = session_id;

			if (session_username != null)
			{
			return View(tbl);
			}
			else
			{
				return RedirectToAction("Login", "Admin");

			}
        }
        public IActionResult Approve( int id )
        {
            var booking = _context.TblEventBookings.FirstOrDefault(x => x.Id == id);

            if (booking != null)
            {
                booking.Status = 1;
                _context.SaveChanges();
              
           
			var seatD = _context.TblEventBookings.FirstOrDefault(x => x.Id == id);

			var bc = seatD.EventId;
			var seatdec = _context.TblUpcomingEvents.FirstOrDefault(y => y.Id == bc);

			var seats = seatdec.TotalSeats;

			var updatedseat = seats - 1;

			seatdec.TotalSeats = updatedseat;
			_context.SaveChanges();
			return RedirectToAction("Bookings");
			}


			return View();
        }
        public IActionResult Reject(int id)
        {
            var booking = _context.TblEventBookings.FirstOrDefault(x => x.Id == id);

            if (booking != null)
            {
                booking.Status = 2;
                _context.SaveChanges();
                return RedirectToAction("Bookings");
            }
            return View();
        }
        public IActionResult RegisteredUser()
		{
			var session_name = cont.HttpContext.Session.GetString("name");
			var session_username = cont.HttpContext.Session.GetString("username");
			var session_id = cont.HttpContext.Session.GetInt32("session_id");

			TempData["Name"] = session_name;
			TempData["username"] = session_username;
			TempData["id"] = session_id;

			if (session_username != null)
			{
			return View(_context.TblClientRegisters.ToList());
			}
			else
			{
				return RedirectToAction("Login", "Admin");

			}
		}

		[HttpGet]
		public IActionResult Feedbacks()
		{
			var session_name = cont.HttpContext.Session.GetString("name");
			var session_username = cont.HttpContext.Session.GetString("username");
			var session_id = cont.HttpContext.Session.GetInt32("session_id");

			TempData["Name"] = session_name;
			TempData["username"] = session_username;
			TempData["id"] = session_id;
		 	AllTables data = new AllTables()
			 {

			 feedback_list =  _context.TblFeedbacks.Include(x=>x.PIdNavigation).ToList(),
             reviewUsers = new List<string>()
             };
            foreach (var feedback in data.feedback_list)
            {
                var user = _context.TblClientRegisters.FirstOrDefault(u => u.Id == feedback.UserId);
                string name = user.Name;
                data.reviewUsers.Add(name);
            }
            if (session_username != null)
			{
				return View(data);
			}
			else
			{
				return RedirectToAction("Login", "Admin");

			}
		}
		public IActionResult Orders()
		{
			  var session_name = cont.HttpContext.Session.GetString("name");
            var session_username = cont.HttpContext.Session.GetString("username");
            var session_id = cont.HttpContext.Session.GetInt32("session_id");

            TempData["Name"] = session_name;
            TempData["username"] = session_username;
            TempData["id"] = session_id;
			

            if (session_username != null)
            {
                return View(_context.TblOrders.Where(x => x.Status == 0).ToList());
            }
            else
            {
                return RedirectToAction("Login", "Admin");

            }
		}
		[HttpGet]
		public IActionResult OrderUpdate(int id)
		{
            var delivered = _context.TblOrders.FirstOrDefault(x => x.Id == id);

            if (delivered != null)
            {
                delivered.Status = 1;
                _context.SaveChanges();
          

			var stock = _context.TblCheckouts.FirstOrDefault(x => x.OrderId == id);

			var stockname = stock.PName;
			var stockpqty = stock.PQty;

			var stockminuss = _context.TblProducts.FirstOrDefault(x => x.ProductName == stockname);

			var stockqty = stockminuss.Stock;

			var updatedstock = stockqty - stockpqty;

			stockminuss.Stock = updatedstock;
            _context.SaveChanges();
                return RedirectToAction("Orders");
            }

            return View();
        }

		[HttpGet]
		public IActionResult Purchases(int id)
		{
            var model =  _context.TblCheckouts.Where(x => x.OrderId == id).ToList();

            return View(model);


        }


        [HttpGet]
		public IActionResult AboutUs()
		{
			var session_name = cont.HttpContext.Session.GetString("name");
			var session_username = cont.HttpContext.Session.GetString("username");
			var session_id = cont.HttpContext.Session.GetInt32("session_id");

			TempData["Name"] = session_name;
			TempData["username"] = session_username;
			TempData["id"] = session_id;

			if (session_username != null)
			{
				return View();
			}
			else
			{
				return RedirectToAction("Login", "Admin");

			}
		}
		[HttpPost]
		public IActionResult AboutUs(TblAbout contentUpdate, IFormFile imgone , IFormFile imgtwo , IFormFile imgthree , IFormFile imgfour)
        {
			if (imgone != null && imgone.Length > 0 || imgtwo != null && imgtwo.Length > 0 || imgthree != null && imgthree.Length > 0 || imgfour != null && imgfour.Length > 0)
			{
				// GETTING IMAGE FILE EXTENSION 
				var fileExt = System.IO.Path.GetExtension(imgone.FileName).Substring(1);
				var fileExts = System.IO.Path.GetExtension(imgtwo.FileName).Substring(1);
				var fileExtss = System.IO.Path.GetExtension(imgthree.FileName).Substring(1);
				var fileExtsss = System.IO.Path.GetExtension(imgfour.FileName).Substring(1);



				// GETTING IMAGE NAME
				var random = Path.GetFileName(imgone.FileName);
				var randoms = Path.GetFileName(imgtwo.FileName);
				var randomss = Path.GetFileName(imgthree.FileName);
				var randomsss = Path.GetFileName(imgfour.FileName);



				// GUID ID COMBINE WITH IMAGE NAME - TO ESCAPE IMAGE NAME REDENDNCY 
				var FileName = Guid.NewGuid() + random;
				var FileNames = Guid.NewGuid() + randoms;
				var FileNamess = Guid.NewGuid() + randomss;
				var FileNamesss = Guid.NewGuid() + randomsss;



				// GET PATH OF CUSTOM IMAGE FOLDER
				string imgFolder = Path.Combine(HttpContext.Request.PathBase.Value, "wwwroot/AboutImg");

				// CHECKING FOLDER EXIST OR NOT - IF NOT THEN CREATE F0LDER 
				if (!Directory.Exists(imgFolder))
				{
					Directory.CreateDirectory(imgFolder);
				}

				// MAKING CUSTOM AND COMBINE FOLDER PATH WITH IMAHE 
				string filepath = Path.Combine(imgFolder, FileName);
				string filepaths = Path.Combine(imgFolder, FileNames);
				string filepathss = Path.Combine(imgFolder, FileNamess);
				string filepathsss = Path.Combine(imgFolder, FileNamesss);



				// COPY IMAGE TO REAL PATH TO DEVELOPER PATH
				using (var stream = new FileStream(filepath, FileMode.Create))
				{
					imgone.CopyTo(stream);
				}



				// READY SEND PATH TO  IMAGE TO DB  
				var dbAddress = Path.Combine("AboutImg", FileName);
				var dbAddresss = Path.Combine("AboutImg", FileNames);
				var dbAddressss = Path.Combine("AboutImg", FileNamess);
				var dbAddresssss = Path.Combine("AboutImg", FileNamesss);

				// EQUALIZE TABLE (MODEL) PROPERTY WITH CUSTOM PATH 
				contentUpdate.Imageone = dbAddress;
				contentUpdate.Imagetwo = dbAddresss;
				contentUpdate.Imagethree = dbAddressss;
				contentUpdate.Imagefour = dbAddresssss;

			


				TblAbout tblAbout = _context.TblAbouts.FirstOrDefault(cols => cols.Id == 1);

			tblAbout.Id = 1;
			tblAbout.MainTitle = contentUpdate.MainTitle;

			tblAbout.Headingone = contentUpdate.Headingone;
			tblAbout.Imageone = contentUpdate.Imageone;
			tblAbout.Textone = contentUpdate.Textone;

			tblAbout.Headingtwo = contentUpdate.Headingtwo;
			tblAbout.Imagetwo = contentUpdate.Imagetwo;
			tblAbout.Texttwo = contentUpdate.Texttwo;

			tblAbout.Headingthree = contentUpdate.Headingthree;
			tblAbout.Imagethree = contentUpdate.Imagethree;
			tblAbout.Textthree = contentUpdate.Textthree;

			tblAbout.Headingfour = contentUpdate.Headingfour;
			tblAbout.Imagefour = contentUpdate.Imagefour;
			tblAbout.Textfour = contentUpdate.Textfour;

			var hj = "sajnsa";

			_context.SaveChanges();
			}
			return View();
        }
    }
}
