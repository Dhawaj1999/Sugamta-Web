
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sugamta.Web.Models;
using Sugamta.Web.Models.SecondaryClientDTOs;
using Sugamta.Web.Models.UserDTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Sugamta.Web.Controllers
{
    public class SecondaryClientController : Controller
    {
        private readonly string ApiUrl = "https://localhost:7109/api/get-all-secondary-client";
        private readonly string AddSecondaryClientApiUrl = "https://localhost:7109/api/add-secondary-client-records";

        public SecondaryClientController()
        {
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            List<SecondaryClient> userEntities = await GetUserEntitiesAsync();
            return View(userEntities);
        }

        [HttpGet]
        public async Task<ActionResult> GetSecondaryClient()
        {
            return PartialView("_SecondaryClientDetail");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll1()
        {
            List<SecondaryClient> userEntities = await GetUserEntitiesAsync();
            return Json(new { data = userEntities.ToList() });
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            return PartialView("_AddSecondaryClient");
        }

        [HttpPost]
        public async Task<IActionResult> Create(SecondaryClientCreateDto user)
        {
            if (user.SecondaryClientEmail != null)
            {
                using (var client = new HttpClient())
                {
                    try
                    {
                        HttpResponseMessage getData = await client.PostAsJsonAsync<SecondaryClientCreateDto>("https://localhost:7109/api/add-secondary-client-records", user);

                        if (getData.IsSuccessStatusCode)
                        {
                            TempData["SecondaryClientRegistration"] = "Secondary Client Registered Successfully";
                            return RedirectToAction("HomePage", "Home");
                            // return RedirectToAction("GetSecondaryClient");
                        }
                        else
                        {
                            Console.WriteLine("Error Calling Web API");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Exception: {ex.Message}");
                    }
                }
            }

            return View();
        }

        private async Task<List<SecondaryClient>> GetUserEntitiesAsync()
        {
            List<SecondaryClient> userEntitiesWithRole = new List<SecondaryClient>();

            using (var client = new HttpClient())
            {
                string bearerToken = HttpContext.Session.GetString("BearerToken");

                // Set the Authorization header with the bearer token
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

                try
                {
                    HttpResponseMessage response = await client.GetAsync(ApiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string content = await response.Content.ReadAsStringAsync();
                        List<SecondaryClient> userWithRoleDtos = JsonConvert.DeserializeObject<List<SecondaryClient>>(content);

                        userEntitiesWithRole = userWithRoleDtos.Select(dto =>
                            new SecondaryClient
                            {
                                SecondaryClientEmail = dto.SecondaryClientEmail,
                                Name = dto.Name,
                                Password = dto.Password,
                                CreationDate = dto.CreationDate,
                                RoleType = dto.RoleType
                            }).ToList();
                    }
                    else
                    {
                        // Handle the error condition, you might want to log it or take other actions.
                        ModelState.AddModelError(string.Empty, "Failed to retrieve user entities. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    // Handle exception appropriately
                    Console.WriteLine($"Exception: {ex.Message}");
                }

                return userEntitiesWithRole;
            }
        }


        public async Task<IActionResult> GetProfileImageSecondaryClient(string email)
        {

            using (var client = new HttpClient())
            {
                string secClientImageURL = $"https://localhost:7109/api/secondary-client/secondory-client-image/{Uri.EscapeDataString(email)}";

                HttpResponseMessage response = await client.GetAsync(secClientImageURL);

                if (response.IsSuccessStatusCode)
                {
                    // Assuming the API returns a base64 string directly
                    string base64Image = await response.Content.ReadAsStringAsync();

                    // Return the base64 string as content with appropriate content type
                    return Content(base64Image, "image/png"); // Adjust content type based on your image format
                }
                else
                {
                    // Return a not found result or handle the error based on your application logic
                    return NotFound();
                }
            }
        }


        [HttpGet]
        public async Task<ActionResult> Edit(string email)
        {
            ViewData["SecondaryClientEmail"] = email;
            SecondaryClientDetails userDetails = new()
            {
                SecondaryClientEmail = email
            };

            using (var client = new HttpClient())
            {

                string apiUrl = $"https://localhost:7109/api/secondary-client/get-secondary-client-details/{Uri.EscapeDataString(email)}";

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    SecondaryClientDetailsEditDtos fetchedUserDetails = JsonConvert.DeserializeObject<SecondaryClientDetailsEditDtos>(content);
                    return PartialView("_EditSecondaryClient", fetchedUserDetails);
                }
            }

            return PartialView("_EditSecondaryClient");
        }

        public async Task<SecondaryClientDetails> GetUserEntityByEmailAsync(string email)
        {
            using (var client = new HttpClient())
            {
                string apiUrl1 = $"https://localhost:7109/api/get-all-secondary-client/{Uri.EscapeDataString(email)}";
                HttpResponseMessage response = await client.GetAsync(apiUrl1);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    SecondaryClientDetails userEntity = JsonConvert.DeserializeObject<SecondaryClientDetails>(content);
                    return userEntity;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<SecondaryClientDetails> GetSecondaryClientDetailsForCreationOrUpdate(string email)
        {
            using (var client = new HttpClient())
            {
                string getSecondaryClientURL = $"https://localhost:7109/api/secondary-client/get-secondary-client-details/{Uri.EscapeDataString(email)}";

                HttpResponseMessage response = await client.GetAsync(getSecondaryClientURL);

                if (response.IsSuccessStatusCode)
                {
                    // Assuming the API returns JSON
                    // UserDetails userDetails = await response.Content.ReadAsAsync<UserDetails>();
                    // return userDetails;
                    string content = await response.Content.ReadAsStringAsync();

                    // Deserialize the string into UserDetailsDto using JSON serializer
                    SecondaryClientDetails userDetails = JsonConvert.DeserializeObject<SecondaryClientDetails>(content);

                    return userDetails;
                }
                else
                {
                    return null;
                }
            }
        }

        string UpdateSecondaryClientDetailsURL = "https://localhost:7109/api/secondary-client/update-secondary-client-details";
        string AddSecondaryClientDetailsURL = "https://localhost:7109/api/secondary-client/add-secondary-client-details";
        [HttpPost]
        public async Task<IActionResult> AddOrUpdateSecondaryClientDetails(SecondaryClientDetailsCreateOrUpdateDtos userDetails)
        {
            try
            {
                // userDetails.SecondaryClientEmail = HttpContext.Session.GetString("SecondaryClientEmail");

                var existingUser = await GetSecondaryClientDetailsForCreationOrUpdate(userDetails.SecondaryClientEmail);

                var formData = new MultipartFormDataContent();

                formData.Add(new StringContent(userDetails.SecondaryClientEmail), "SecondaryClientEmail");
                // formData.Add(new StringContent(userDetails.Name), "Name");
                formData.Add(new StringContent(userDetails.Address), "Address");
                formData.Add(new StringContent(userDetails.City), "City");
                /* formData.Add(new StringContent(userDetails.StateId), "State");
                 formData.Add(new StringContent(userDetails.CountryId), "Country");*/

                formData.Add(new StringContent(userDetails.StateId.ToString()), "StateId");
                formData.Add(new StringContent(userDetails.CountryId.ToString()), "CountryId");
                formData.Add(new StringContent(userDetails.PhoneNumber), "PhoneNumber");
                formData.Add(new StringContent(userDetails.AlternatePhoneNumber), "AlternatePhoneNumber");

                // Check if formFile is not null and has content
                if (userDetails.formFile != null && userDetails.formFile.Length > 0)
                {
                    // Add the file content
                    formData.Add(new StreamContent(userDetails.formFile.OpenReadStream())
                    {
                        Headers =
                                    {
                                        ContentLength = userDetails.formFile.Length,
                                        ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(userDetails.formFile.ContentType)
                                    }
                    },
                    "formFile", userDetails.formFile.FileName);
                }

                if (existingUser != null)
                {
                    // User already exists, call the update API
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(UpdateSecondaryClientDetailsURL);

                        // Send the PUT request with multipart/form-data
                        HttpResponseMessage response = await client.PutAsync("https://localhost:7109/api/secondary-client/update-secondary-client-details", formData);

                        if (response.IsSuccessStatusCode)
                        {
                            TempData["UpdateSecondaryClientDetails"] = "Secondary Client Details Updated Successfully";
                            //return RedirectToAction("Index", "Home");
                            return RedirectToAction("HomePage", "Home");
                        }
                        else
                        {
                            string errorMessage = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"Error Calling Update Web API: {errorMessage}");
                            return StatusCode((int)response.StatusCode, errorMessage);
                        }
                    }
                }
                else
                {
                    // User does not exist, call the add API
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(AddSecondaryClientDetailsURL);

                        HttpResponseMessage response = await client.PostAsync("https://localhost:7109/api/secondary-client/add-secondary-client-details", formData);

                        if (response.IsSuccessStatusCode)
                        {
                            TempData["SaveSecondaryClientDetails"] = "Secondary Client Details Added Successfully";
                            return RedirectToAction("HomePage", "Home");
                        }
                        else
                        {
                            string errorMessage = await response.Content.ReadAsStringAsync();
                            Console.WriteLine($"Error Calling Add Web API: {errorMessage}");
                            return StatusCode((int)response.StatusCode, errorMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Failed to add/update UserDetails: {ex.Message}");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string email)
        {
            using (var client = new HttpClient())
            {
                var deleteSecondaryUrl = new Uri($"https://localhost:7109/api/delete-secondary-records/{Uri.EscapeDataString(email)}");

                HttpResponseMessage response = await client.DeleteAsync(deleteSecondaryUrl);

                if (response.IsSuccessStatusCode)
                {
                    TempData["DeleteSecondaryClientDetails"] = "Secondary Client Deleted Successfully";
                    return RedirectToAction("HomePage", "Home");
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error Calling Add Web API: {errorMessage}");
                    return StatusCode((int)response.StatusCode, errorMessage);
                }
            }
        }
    }
}
