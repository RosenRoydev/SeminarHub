using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SeminarHub.Data;
using SeminarHub.Data.Models;
using SeminarHub.Models;
using SeminarHub.Models.Category;
using SeminarHub.Models.Seminar;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Claims;
using static SeminarHub.Data.Common.DataValidation;

namespace SeminarHub.Controllers
{
    [Authorize]
    public class SeminarController : Controller
    {
        private readonly SeminarHubDbContext context;
        public SeminarController(SeminarHubDbContext _context)
        {
            context = _context;
        }
        public async Task<IActionResult> All()
        {
            var allSeminars = await context.Seminars
               .AsNoTracking()
               .Select(s => new SeminarAllViewModel()
               {
                   Id = s.Id,
                   Topic = s.Topic,
                   Lecturer = s.Lecturer,
                   Category = s.Category.Name,
                   Organizer = s.Organizer.UserName,
                   DateAndTime = s.DateAndTime.ToString($"{DateTimeExactFormat}"),

               }).ToListAsync();

            //Returning all of the events to the view
            return View(allSeminars);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new SeminarFormViewModel();
            model.Categories = await GetCategorysTypes();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(SeminarFormViewModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            DateTime time;
            if (!DateTime.TryParseExact(model.DateAndTime, DateTimeExactFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out time))
            {
                ModelState.AddModelError(nameof(model.DateAndTime), $"Invalid date! Format must be {DateTimeExactFormat}");
            }

            if (!ModelState.IsValid)
            {
                List<string> errorMessages = new List<string>();
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        errorMessages.Add(error.ErrorMessage);
                    }
                }
            }



            Seminar seminar = new Seminar()
            {
                Topic = model.Topic,
                Lecturer = model.Lecturer,
                Details = model.Details,
                DateAndTime = time,
                CategoryId = model.CategoryId,
                OrganizerId = GetUserId(),


            };

            await context.AddAsync(seminar);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            var searchedSeminar = await context.Seminars
                .AsNoTracking()
                .Where(s => s.Id == id)
                .FirstOrDefaultAsync();

            //Event is not found
            if (searchedSeminar == null)
            {
                return BadRequest();
            }

            //If the User tries to edit an Event, which is not his
            if (searchedSeminar.OrganizerId != GetUserId())
            {
                return Unauthorized();
            }

            var seminarForEdit = await context.Seminars
                .Where(s => s.Id == id)
                .Select(s => new SeminarEditViewModel()
                {
                    Id = s.Id,
                    Topic = s.Topic,
                    Lecturer = s.Lecturer,
                    Details = s.Details,
                    DateAndTime = s.DateAndTime.ToString($"{DateTimeExactFormat}"),
                    Duration = s.Duration,
                    CategoryId = s.CategoryId



                })
                .FirstOrDefaultAsync();


            if (seminarForEdit == null)
            {
                return BadRequest();
            }

            seminarForEdit.Categories = await GetCategorysTypes();
            return View(seminarForEdit);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(SeminarEditViewModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            DateTime time;
            if (!DateTime.TryParseExact(model.DateAndTime, DateTimeExactFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out time))
            {
                ModelState.AddModelError(nameof(model.DateAndTime), $"Invalid date! Format must be {DateTimeExactFormat}!");
            }

            if (!ModelState.IsValid)
            {
               
                List<string> errorMessages = new List<string>();
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        errorMessages.Add(error.ErrorMessage);
                    }
                }

                model.Categories = await GetCategorysTypes();
                return View(model);
            }

            var seminarForEdit = await context.Seminars
                .Where(s => s.Id == model.Id)
                .FirstOrDefaultAsync();

            
            if (seminarForEdit == null)
            {
                return BadRequest();
            }

            seminarForEdit.Topic = model.Topic;
            seminarForEdit.Lecturer = model.Lecturer;
            seminarForEdit.Details = model.Details;
            seminarForEdit.DateAndTime = time;
            seminarForEdit.Duration = model.Duration;
            seminarForEdit.CategoryId = model.CategoryId;

            await  context.SaveChangesAsync();
            return RedirectToAction(nameof(All));

        }

        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {
           
            var currentUser = GetUserId();

            
            if (!await context.Seminars
                .AsNoTracking()
                .AnyAsync(s => s.Id == id))
            {
                return BadRequest();
            }

            
            if (await context.SeminarsParticipants
                .AsNoTracking()
                .AnyAsync(sp => sp.SeminarId == id && sp.ParticipantId == currentUser))
            {
                return RedirectToAction(nameof(All));
            }

            
            var seminarParticipant = new SeminarParticipant()
            {
                SeminarId = id,
                ParticipantId = currentUser
            };

           
            await context.SeminarsParticipants.AddAsync(seminarParticipant);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Joined));
        }

        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            
            var currentUserId = GetUserId();

            
            var currentUserEvents = await context.SeminarsParticipants
                .Where(sp => sp.ParticipantId == currentUserId)
                .Select(sp => new SeminarAllViewModel()
                {
                    Id = sp.SeminarId,
                    Topic = sp.Seminar.Topic,
                    DateAndTime = sp.Seminar.DateAndTime.ToString(DateTimeExactFormat),
                  
                    Lecturer = sp.Seminar.Lecturer,
                })
                .ToListAsync();

            
            return View(currentUserEvents);
        }
        [HttpPost]
        public async Task<IActionResult> Leave(int id)
        {
          
            
            if (!await context.Seminars
                .AsNoTracking()
                .AnyAsync(s => s.Id == id))
            {
                return BadRequest();
            }

            var currentUser = GetUserId();
            var seminarParticipant = await context.SeminarsParticipants
                .Where(sp => sp.SeminarId == id && sp.ParticipantId == currentUser )
                .FirstOrDefaultAsync();

            
            if (seminarParticipant == null)
            {
                return BadRequest();
            }

            
            context.SeminarsParticipants.Remove(seminarParticipant);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {

            var searchedSeminar = await context.Seminars
                .Where(s => s.Id == id)
                .Select(s => new SeminarDetailsViewModel
                {
                    Id = s.Id,
                    Topic = s.Topic,
                    DateAndTime = s.DateAndTime.ToString($"{DateTimeExactFormat}"),
                    Duration = s.Duration.ToString(),
                    Lecturer = s.Lecturer,
                    Details = s.Details,
                    Category = s.Category.Name,
                    Organizer = s.Organizer.UserName
                    
                    
                })
                .FirstOrDefaultAsync();

            
            if (searchedSeminar == null)
            {
                return BadRequest();
            }
            
            return View(searchedSeminar);
        }

        [HttpGet]
        public async Task<IActionResult>Delete(int id)
        {
            var seminar = await context.Seminars
                .Where(s => s.Id == id)
                .Select (s => new SeminarDeleteViewModel
                {
                    Id= s.Id,
                    Topic = s.Topic,
                    DateAndTime= s.DateAndTime.ToString($"{DateTimeExactFormat}"),
                    OrganizerId = s.OrganizerId,
                })
                .FirstOrDefaultAsync();

            if (seminar == null)
            {
                return BadRequest();
            }
            return View(seminar);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(SeminarDeleteViewModel model)
        {
            
            var seminarForDelete = await context.Seminars.FindAsync(model.Id);

            if(seminarForDelete?.OrganizerId != GetUserId())
            {
                return Unauthorized();
            }

            
            if (seminarForDelete != null) 
            { 
            
              var seminarParticipants = await context.SeminarsParticipants
                    .Where(sp => sp.SeminarId == model.Id)
                    .ToListAsync();
                context.SeminarsParticipants.RemoveRange(seminarParticipants);
                context.Seminars.Remove(seminarForDelete);
                await context.SaveChangesAsync();
               
            
            }

            return RedirectToAction(nameof(All));
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
        private async Task<ICollection<CategoriesTypesViewModel>> GetCategorysTypes()
        {
            return await context.Category
                .Select(c => new CategoriesTypesViewModel()
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }
    }
}
