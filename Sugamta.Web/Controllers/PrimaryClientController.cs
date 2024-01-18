using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sugamta.Web.Models;
using Sugamta.Web.Models.PrimaryClientDTOs;
using Sugamta.Web.Models.UserDTOs;
using static Org.BouncyCastle.Bcpg.Attr.ImageAttrib;

namespace Sugamta.Web.Controllers
{
    public class PrimaryClientController : Controller
    {
        public async Task<ActionResult> Index()
        {
            List<PrimaryClient> userEntities = await GetPrimaryClientAsync();
            return View(userEntities);
        }


        [HttpGet]
        public async Task<ActionResult> GetPrimaryClient()
        {
            return PartialView("GetPrimaryClient");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPrimaryClient()
        {
            List<PrimaryClient> userEntities = await GetPrimaryClientAsync();
            return Json(new { data = userEntities.ToList() });
        }

        string getPrimaryClientURL = "https://localhost:7109/api/get-primary-client-all";
        public async Task<List<PrimaryClient>> GetPrimaryClientAsync()
        {
            List<PrimaryClient> userEntitiesWithRole = new List<PrimaryClient>();

            using (var client = new HttpClient())
            {
                string bearerToken = HttpContext.Session.GetString("BearerToken");

                // Set the Authorization header with the bearer token
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

                HttpResponseMessage response = await client.GetAsync(getPrimaryClientURL);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    List<PrimaryClient> userWithRoleDtos = JsonConvert.DeserializeObject<List<PrimaryClient>>(content);

                    userEntitiesWithRole = userWithRoleDtos.Select(dto =>
                        new PrimaryClient
                        {
                            PrimaryClientEmail = dto.PrimaryClientEmail,
                            PrimaryClientName = dto.PrimaryClientName,
                            Password = dto.Password,
                            CreationDate = dto.CreationDate,
                            IsDeleted = dto.IsDeleted,
                            AgencyName = dto.AgencyName
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
            return PartialView("AddPrimaryClient");
        }

        [HttpPost]
        public async Task<IActionResult> Create(PrimaryClientRegisterDto user)
        {
            if (user.PrimaryClientEmail != null)
            {

                using (var client = new HttpClient())
                {
                    HttpResponseMessage getData = await client.PostAsJsonAsync<PrimaryClientRegisterDto>("https://localhost:7109/api/add-primary-client", user);

                    if (getData.IsSuccessStatusCode)
                    {
                        TempData["PrimaryClientMessage"] = "Primary Client Registered Successfully";
                        // return RedirectToAction("Index", "Home");
                        //return View("HomePage");
                        return RedirectToAction("HomePage", "Home");

                    }
                    else
                    {
                        Console.WriteLine("Error Calling Web API");
                    }

                }
            }
            return PartialView("GetPrimaryClient");
        }

        public async Task<IActionResult> GetPrimaryClientImage(string email)
        {
            using (var client = new HttpClient())
            {
                string imageURL = $"https://localhost:7109/api/get-primary-client-image/{Uri.EscapeDataString(email)}";

                HttpResponseMessage response = await client.GetAsync(imageURL);

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
            ViewData["PrimaryClientEmail"] = email;
            PrimaryClientDetails userDetails = new()
            {
                PrimaryClientEmail = email
            };

            using (var client = new HttpClient())
            {

                string getClientURL = $"https://localhost:7109/api/get-primary-client-details/{Uri.EscapeDataString(userDetails.PrimaryClientEmail)}";

                HttpResponseMessage response = await client.GetAsync(getClientURL);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    PrimaryClientDetailsEditDto fetchedUserDetails = JsonConvert.DeserializeObject<PrimaryClientDetailsEditDto>(content);
                    return PartialView("EditPrimaryClient", fetchedUserDetails);
                }
            }

            return PartialView("EditPrimaryClient");
        }

        public async Task<PrimaryClientDetails> GetPrimaryClientDetailsForCreationOrUpdate(string email)
        {
            using (var client = new HttpClient())
            {
                string apiUrl = $"https://localhost:7109/api/get-primary-client-details-for-create-or-update/{Uri.EscapeDataString(email)}";

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Assuming the API returns JSON
                    // UserDetails userDetails = await response.Content.ReadAsAsync<UserDetails>();
                    // return userDetails;
                    string content = await response.Content.ReadAsStringAsync();

                    // Deserialize the string into UserDetailsDto using JSON serializer
                    PrimaryClientDetails userDetails = JsonConvert.DeserializeObject<PrimaryClientDetails>(content);

                    return userDetails;
                }
                else
                {
                    return null;
                }
            }
        }

        string UpdatePrimaryClientDetailsURL = "https://localhost:7109/api/update-primary-client-details";
        string AddPrimaryClientDetailsURL = "https://localhost:7109/api/add-primary-client-details";
        [HttpPost]
        public async Task<IActionResult> AddOrUpdatePrimaryClientDetails(PrimaryClientDetailsCreateOrUpdateDto userDetails)
        {
            try
            {
               // userDetails.PrimaryClientEmail = HttpContext.Session.GetString("UserEmail");

                var existingUser = await GetPrimaryClientDetailsForCreationOrUpdate(userDetails.PrimaryClientEmail);

                var formData = new MultipartFormDataContent();

                formData.Add(new StringContent(userDetails.PrimaryClientEmail), "PrimaryClientEmail");
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
                        client.BaseAddress = new Uri(UpdatePrimaryClientDetailsURL);

                        // Send the PUT request with multipart/form-data
                        HttpResponseMessage response = await client.PutAsync("https://localhost:7109/api/update-primary-client-details", formData);

                        if (response.IsSuccessStatusCode)
                        {
                            TempData["UpdatePrimaryClientDetails"] = "Primary Client Details Updated Successfully";
                            //return RedirectToAction("Index", "Home");
                            return RedirectToAction("HomePage","Home");
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
                        client.BaseAddress = new Uri(AddPrimaryClientDetailsURL);

                        HttpResponseMessage response = await client.PostAsync("https://localhost:7109/api/add-primary-client-details", formData);

                        if (response.IsSuccessStatusCode)
                        {
                            TempData["SavePrimaryClientDetails"] = "Primary Client Details Added Successfully";
                            return RedirectToAction("HomePage","Home");
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
                var deleteUrl = new Uri($"https://localhost:7109/api/delete-primary-client/{Uri.EscapeDataString(email)}");

                HttpResponseMessage response = await client.DeleteAsync(deleteUrl);

                if (response.IsSuccessStatusCode)
                {
                    TempData["DeletePrimaryClientDetails"] = "Primary Client Deleted Successfully";
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
