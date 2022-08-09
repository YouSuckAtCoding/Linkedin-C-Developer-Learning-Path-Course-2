using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using ModelsLibrary.Models;
using System.IO;
using System.Threading.Tasks;

namespace ASP_NET_Course_2.Pages.Admin
{
    [Authorize]
    public class AddEditRecipeModel : PageModel
    {
        private readonly IRecipesService recipesService;
        public AddEditRecipeModel(IRecipesService _recipesService)
        {
            recipesService = _recipesService;
        }

        [FromRoute]
        public long? Id { get; set; }

        [BindProperty]
        public IFormFile FormFile { get; set; }

        public bool IsNewRecipe
        {
            get { return Id == null; }
        }

        [BindProperty]
        public Recipe Recipe { get; set; }  

        public async Task OnGetAsync()
        {
            Recipe = await recipesService.FindAsync(Id.GetValueOrDefault()) ?? new Recipe();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            //For some stupid reason i can´t really understand, if i do the opposite with the Prop Recipe and the var recipe
            //It blows up. IDFK

            if (ModelState.IsValid)
            {
                var recipe = await recipesService.FindAsync(Id.GetValueOrDefault()) ?? new Recipe();
                recipe.Name = Recipe.Name;
                recipe.Description = Recipe.Description;
                recipe.Ingredients = Recipe.Ingredients;
                recipe.Directions = Recipe.Directions;

                if (FormFile != null)
                {
                    recipe.SetImage(FormFile);
                }

                await recipesService.SaveAsync(recipe);
                return RedirectToPage("/Recipe", new { id = recipe.Id });
            }
            else
            {
                return Page();
            }
            
        }

        public async Task<IActionResult> OnPostDeleteAsync()
        {
            await recipesService.DeleteAsync(Recipe.Id);
            return RedirectToPage("/Index");
        }
    }
}
