using EatTogether.Models.DTOs;
using EatTogether.Models.Services;
using EatTogether.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EatTogether.Controllers
{
    public class SetMealsController : Controller
    {
        private readonly SetMealService _setMealService;
        private readonly DishService _dishService;

        public SetMealsController(SetMealService setMealService, DishService dishService)
        {
            _setMealService = setMealService;
            _dishService    = dishService;
        }

        // GET: /SetMeals
        public async Task<IActionResult> Index()
        {
            var dtos = await _setMealService.GetAllAsync();
            var vms = dtos.Select(d => d.ToViewModel()).ToList();
            return View(vms);
        }

        // GET: /SetMeals/Create
        public IActionResult Create()
        {
            return View(new SetMealViewModel());
        }

        // POST: /SetMeals/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SetMealViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value!.Errors.Count > 0)
                    .ToDictionary(
                        e => e.Key,
                        e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray()
                    );
                return BadRequest(errors);
            }

            await _setMealService.CreateAsync(vm.ToDto());
            return Ok();
        }

        // GET: /SetMeals/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var dto = await _setMealService.GetByIdAsync(id);
            if (dto == null) return NotFound();

            var vm = dto.ToViewModel();
            return View(vm);
        }

        // POST: /SetMeals/Edit/5
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [FromBody] SetMealViewModel vm)
        {
            if (id != vm.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(e => e.Value!.Errors.Count > 0)
                    .ToDictionary(
                        e => e.Key,
                        e => e.Value!.Errors.Select(x => x.ErrorMessage).ToArray()
                    );
                return BadRequest(errors);
            }

            await _setMealService.UpdateAsync(vm.ToDto());
            return Ok();
        }

        // POST: /SetMeals/Disable/5
        [HttpPost]
        public async Task<IActionResult> Disable(int id)
        {
            await _setMealService.DisableAsync(id);
            return Ok();
        }

        // POST: /SetMeals/AddItem
        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] SetMealItemViewModel vm)
        {
            try
            {
                await _setMealService.AddItemAsync(vm.ToItemDto());
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: /SetMeals/RemoveItem/5
        [HttpPost]
        public async Task<IActionResult> RemoveItem(int id)
        {
            await _setMealService.RemoveItemAsync(id);
            return Ok();
        }

        // =============================================
        // 私有輔助方法：產生餐點下拉選單
        // =============================================
        private async Task<List<SelectListItem>> GetDishOptionsAsync()
        {
            var dishes = await _dishService.GetAllAsync();
            return dishes
                .Select(d => new SelectListItem
                {
                    Value = d.Id.ToString(),
                    Text  = $"{d.DishName}（${d.Price}）"
                })
                .ToList();
        }
    }
}
