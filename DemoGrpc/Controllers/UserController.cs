using DemoGrpc.Models;
using Google.Protobuf.WellKnownTypes;
using GrpcService;
using Microsoft.AspNetCore.Mvc;
using Empty = GrpcService.Empty;

namespace DemoGrpc.Controllers
{
    public class UserController : Controller
    {
        private readonly UserContract.UserContractClient _client;

        public UserController(UserContract.UserContractClient client)
        {
            _client = client;
        }

        // GET: UserController
        public async Task<ActionResult> Index()
        {
            var response = await _client.GetAllAsync(new Empty());
            var users = new List<User>();
            users.AddRange(response.Items);
            return View(users.Select(ToModel).ToList());
        }

        private UserModel ToModel(User user)
        {
            return new UserModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Fullname = user.Fullname,
                CreatedDate = user.CreatedDate == null ? DateTime.Now : DateTime.SpecifyKind(user.CreatedDate.ToDateTime(), DateTimeKind.Local),
                LastActive = user.LastActive == null ? DateTime.Now : DateTime.SpecifyKind(user.LastActive.ToDateTime(), DateTimeKind.Local),
                Disable = user.Disable
            };
        }

        private User ToGrpcModel(UserModel userModel)
        {
            return new User
            {
                Id = userModel.Id,
                Username = userModel.Username,
                Email = userModel.Email,
                Fullname = userModel.Fullname,
                CreatedDate = Timestamp.FromDateTime(DateTime.SpecifyKind(userModel.CreatedDate, DateTimeKind.Utc)),
                LastActive = Timestamp.FromDateTime(DateTime.SpecifyKind(userModel.LastActive, DateTimeKind.Utc)),
                Disable = userModel.Disable
            };
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            ViewData["Action"] = nameof(Create);
            return View("Form");
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserModel userModel)
        {
            try
            {
                await _client.CreateAsync(ToGrpcModel(userModel));
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewData["Action"] = nameof(Create);
                return View("Form", userModel);
            }
        }

        // GET: UserController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var user = await _client.GetAsync(new Key { Id = id });
                ViewData["Action"] = nameof(Edit);
                return View("Form", ToModel(user));
            }
            catch (Exception ex)
            {
                return NotFound();
            }
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, UserModel userModel)
        {
            try
            {
                await _client.UpdateAsync(ToGrpcModel(userModel));
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewData["Action"] = nameof(Edit);
                return View("Form", userModel);
            }
        }

        // Get: UserController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _client.DeleteAsync(new Key { Id = id });
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                return NotFound();
            }

        }
    }
}