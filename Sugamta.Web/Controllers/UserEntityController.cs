
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sugamta.Web.Models;
using Sugamta.Web.Models.UserDTOs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sugamta.Web.Controllers
{
    public class UserEntityController : Controller
    {
        // string apiUrl = "https://localhost:7109/api/User";
         string DeleteUrl = "https://localhost:7109/api/User";

        public UserEntityController()
        {
        }


        
        public async Task<ActionResult> Index()
        {
            List<UserEntity> userEntities = await GetUserEntitiesAsync();
            return View(userEntities);
        }

        //[Authorize(Roles = "Admin")]
        //[Authorize(Policy = "AdminPolicy")]
        [HttpGet]
        public async Task<ActionResult> UserEntityDetails()
        {
            return PartialView("_UserEntityDetails");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            List<UserEntity> userEntities = await GetUserEntitiesAsync();
            return Json(new { data = userEntities.ToList() });
        }

        public async Task<List<UserEntity>> GetUserEntitiesAsync()
        {
            List<UserEntity> userEntitiesWithRole = new List<UserEntity>();

            using (var client = new HttpClient())
            {
                string bearerToken = HttpContext.Session.GetString("BearerToken");

                // Set the Authorization header with the bearer token
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

                HttpResponseMessage response = await client.GetAsync(ApiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    List<UserEntity> userWithRoleDtos = JsonConvert.DeserializeObject<List<UserEntity>>(content);

                    userEntitiesWithRole = userWithRoleDtos.Select(dto =>
                        new UserEntity
                        {
                            Email = dto.Email,
                            Name = dto.Name,
                            Password = dto.Password,
                            CreationDate = dto.CreationDate,
                            IsDeleted = dto.IsDeleted,
                            RoleType = dto.RoleType
                        }).ToList();
                }
                else
                {
                    // Handle the error condition, you might want to log it or take other actions.
                    ModelState.AddModelError(string.Empty, "Failed to retrieve user entities. Please try again.");
                }

                return userEntitiesWithRole;
            }
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
           //return View();
           return PartialView("_AddUserEntity");
        }

        string ApiUrl = "https://localhost:7109/api/User";
       
        string baseURL = "https://localhost:7109/api/User";
        [HttpPost]
        public async Task<IActionResult> Create(UserCreateDto user)
        {
            //UserEntity obj = new UserEntity()
            //{
            //    // UserID = user.UserID,
            //    Email = user.Email,
            //    Name = user.Name,
            //    Password = user.Password,
            //    RoleId = user.RoleId,
            //    CreationDate = DateTime.Now,
            //   CreatedBy = user.Email,
            //    IsDeleted = user.IsDeleted,
            //    //UpdationDate = user.UpdationDate,
            //};
            if (user.Email != null)
            {

                using (var client = new HttpClient())
                {
                    HttpResponseMessage getData = await client.PostAsJsonAsync<UserCreateDto>("https://localhost:7109/api/User/create-user", user);

                    if (getData.IsSuccessStatusCode)
                    {
                        return RedirectToAction("UserEntityDetails");

                    }
                    else
                    {
                        Console.WriteLine("Error Calling Web API");
                    }

                }
            }
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Edit(string email)
        {
            UserDetails userDetails = new()
            {
                Email = email
            };

            using (var client = new HttpClient())
            {

                string apiUrl = $"https://localhost:7109/api/get-user-details/{Uri.EscapeDataString(userDetails.Email)}";

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    UserDetails fetchedUserDetails = JsonConvert.DeserializeObject<UserDetails>(content);
                    return PartialView("_EditUserEntity", fetchedUserDetails);
                }
            }

            return PartialView("_EditUserEntity");
        }

         public async Task<UserEntity> GetUserEntityByEmailAsync(string email)   
         {
              using (var client = new HttpClient())
              {
                string apiUrl1 = $"https://localhost:7109/api/User/{Uri.EscapeDataString(email)}";
                HttpResponseMessage response = await client.GetAsync(apiUrl1);

                  if (response.IsSuccessStatusCode)
                  {
                      string content = await response.Content.ReadAsStringAsync();
                      UserEntity userEntity = JsonConvert.DeserializeObject<UserEntity>(content);
                      return userEntity;
                  }
                  else
                  {
                      return null; 
                  }
              }
         }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit(string email, UserEntity userEntity)
        //{
        //    try
        //    {
        //        if (email != userEntity.Email)
        //        {
        //            return NotFound();
        //        }

        //        using (var client = new HttpClient())
        //        {
        //            string editApiUrl = $"https://localhost:7109/api/User/update-user/{Uri.EscapeDataString(email)}";
        //            string serializedUserEntity = JsonConvert.SerializeObject(userEntity);
        //            var content = new StringContent(serializedUserEntity, Encoding.UTF8, "application/json");

        //            HttpResponseMessage response = await client.PutAsync(editApiUrl, content);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                //  return RedirectToAction("Index");
        //                //return RedirectToAction("_UserEntityDetails");
        //                return View("_UserEntityDetails", "userEntities");
        //               // return PartialView("_UserEntityDetails", "userEntities");
        //            }
        //            else
        //            {
        //                var errorMessage = await response.Content.ReadAsStringAsync();
        //                Console.WriteLine($"Error: {response.StatusCode} - {errorMessage}");


        //                ModelState.AddModelError(string.Empty, "Failed to update user. Please try again.");

        //                return View(userEntity);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Exception: {ex.Message}");


        //        ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");

        //        return View(userEntity);
        //    }
        //}

        //[HttpPost]
        //public async Task<ActionResult> Edit(UserUpdateDto userEntity)
        //{
        //    try
        //    {
        //        var formData = new MultipartFormDataContent();

        //        formData.Add(new StringContent(userEntity.Email), "Email");
        //        formData.Add(new StringContent(userEntity.Name), "Name");
        //        formData.Add(new StringContent(userEntity.Password), "Password");
        //        //formData.Add(new StringContent(userEntity.RoleId.ToString()), "RoleId");

        //        using (var client = new HttpClient())
        //        {
        //            HttpResponseMessage response = await client.PutAsync("https://localhost:7109/api/User/update-user-without-otp", formData);

        //            if (response.IsSuccessStatusCode)
        //            {
        //                //  return RedirectToAction("Index");
        //                //return RedirectToAction("_UserEntityDetails");
        //                return View("_UserEntityDetails", "userEntities");
        //                // return PartialView("_UserEntityDetails", "userEntities");
        //            }
        //            else
        //            {
        //                var errorMessage = await response.Content.ReadAsStringAsync();
        //                Console.WriteLine($"Error: {response.StatusCode} - {errorMessage}");


        //                ModelState.AddModelError(string.Empty, "Failed to update user. Please try again.");

        //                return View(userEntity);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Exception: {ex.Message}");


        //        ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");

        //        return View(userEntity);
        //    }
        //}

        public async Task<ActionResult> Edit(UserUpdateDto userEntity)
        {
            try
            {
                var formData = new MultipartFormDataContent();

                formData.Add(new StringContent(userEntity.Email), "Email");
                formData.Add(new StringContent(userEntity.Name), "Name");
                formData.Add(new StringContent(userEntity.Password), "Password");
                formData.Add(new StringContent(userEntity.RoleId.ToString()), "RoleId");
                formData.Add(new StringContent(userEntity.IsDeleted.ToString()), "IsDeleted");

                using (var client = new HttpClient())
                {
                    HttpResponseMessage response = await client.PutAsync("https://localhost:7109/api/User/update-user-without-otp", formData);

                    if (response.IsSuccessStatusCode)
                    {
                        // Successful update, you can redirect or return a view
                        return RedirectToAction("Index"); // Change "Index" to the desired action/method
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error: {response.StatusCode} - {errorMessage}");

                        // Update failed, add an error message to ModelState and return the view
                        ModelState.AddModelError(string.Empty, "Failed to update user. Please try again.");
                        return View(userEntity);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");

                // An unexpected error occurred, add an error message to ModelState and return the view
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                return View(userEntity);
            }
        }


        [HttpGet]
        public async Task<ActionResult> Delete(string email)
        {
            UserEntity userEntity = await GetUserEntityByEmailAsync(email);

            if (userEntity == null)
            {
                return NotFound();
            }

            return View(userEntity);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirm(string email)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.DeleteAsync($"{DeleteUrl}/{Uri.EscapeDataString(email)}");

                if (response.IsSuccessStatusCode)
                {

                     return View("UserEntityDetails");
                    //  return Content("User deleted successfully");
                    // return PartialView("_UserEntityDetails","userEntity");
                    //return RedirectToAction(UserEntityDetails);
                }
               
                else
                {
                    
                    return PartialView("_UserEntityDetail");
                }
            }
        }
     
        




    }
}





