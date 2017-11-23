using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Rooms.Models;
using System.IO;
using System.Dynamic;
using Newtonsoft.Json;

namespace Rooms.Controllers
{
    public class KitchenController : Controller
    {
        MisRoomsEntities db = new MisRoomsEntities();
        #region main
        // GET: Kitchen
        public ActionResult Index()
        {
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        public ActionResult Kitchen()
        {
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        public ActionResult Nutrition()
        {
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        public ActionResult Test()
        {
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Nutrition(Recipe recipe, FormCollection coll, HttpPostedFileBase recipeimg)
        {
            if (recipeimg != null)
            {
                string path = Server.MapPath("~/RecipeImages/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                recipe.image = recipe.recipe_name + Path.GetExtension(recipeimg.FileName);
                recipeimg.SaveAs(path + recipe.recipe_name + Path.GetExtension(recipeimg.FileName));
            }
            recipe.cal = Convert.ToInt32(coll["cal"].ToString());
            db.Recipes.Add(recipe);
            db.SaveChanges();
            int id = recipe.ID;
            int noofingredients = Convert.ToInt32(coll["rowcount"].ToString());
            RecipeIngredient RI = new RecipeIngredient();
            for (int i = 0; i <= noofingredients; i++)
            {
                RI.recipe_name = id;
                RI.qty = coll["processedweight" + i].ToString();
                RI.raw_weight = coll["rawweight" + i].ToString();
                string ingredientname = coll["ingname" + i].ToString();
                int ingid = db.Ingredients.Where(ik => ik.INGREDIENT_NAME == ingredientname).Select(ik => ik.ing_id).FirstOrDefault();
                if (ingid != 0)
                {
                    RI.ingre = ingid;
                }
                else
                {
                    db.Recipes.Remove(db.Recipes.Find(id));
                    db.SaveChanges();
                    break;
                }
                RI.cal = coll["calories" + i].ToString();
                db.RecipeIngredients.Add(RI);
                db.SaveChanges();
            }
            return Redirect("Nutrition");
        }
        public ActionResult RecipeReport()
        {
            if (string.IsNullOrEmpty(Session["LogedUserID"] as string))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                dynamic flexible = new ExpandoObject();
                flexible.Int = 3;
                flexible.String = "hi";

                var dictionary = (IDictionary<string, object>)flexible;
                dictionary.Add("Bool", false);

                var serialized = JsonConvert.SerializeObject(dictionary);
                return View();
            }
        }
        #endregion
        #region AJAX
        [ActionName("GRD")]
        public JsonResult GETRECIPEDATA(int recipeid)
        {
            var result = (from R in db.Recipes
                          join RI in db.RecipeIngredients on R.ID equals RI.recipe_name
                          join IN in db.Ingredients on RI.ingre equals IN.ing_id
                          where R.ID == recipeid
                          orderby IN.INGREDIENT_NAME
                          select new
                          {
                              R,
                              RI,
                              IN
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        [ActionName("GR")]
        public JsonResult GETRECIPES()
        {
            var result = db.Recipes.Select(i => new { i.recipe_name, i.ID }).OrderBy(i => i.recipe_name).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [ActionName("GI")]
        public JsonResult GetIngredients()
        {
            var result = db.Ingredients.OrderBy(i => i.INGREDIENT_NAME).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [ActionName("GC")]
        public ContentResult GetCalories(string Ingredient)
        {
            var result = db.Ingredients.Where(i => i.INGREDIENT_NAME == Ingredient).Select(i => i.CALORIES_KCAL).FirstOrDefault();
            if (result == null)
            {
                result = "0";
            }
            return Content(result.ToString());
        }
        [ActionName("GCAT")]
        public JsonResult GetCategories()
        {
            var categories = db.RecipeCategories.OrderBy(i => i.category).ToList();
            return Json(categories, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}