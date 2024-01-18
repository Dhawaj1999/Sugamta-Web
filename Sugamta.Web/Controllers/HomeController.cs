using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sugamta.Web.Models;
using System.Data;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System.Net.Http.Formatting;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sugamta.Web.Services;
using Sugamta.Web.Models.UserDTOs;
using System.Security.Cryptography;
using System.Text;
using Sugamta.Web.Models.PrimaryClientDTOs;

namespace Sugamta.Web.Controllers
{
    public class HomeController : Controller
    {
        // private static List<UserEntity> users = new List<UserEntity>();
        private readonly ILogger<HomeController> _logger;
        private readonly IMailService _emailService;
        private readonly OtpService _otpService;

        public static readonly string encryptionKeyToRegister = "ThisIsASampleKey123!ThisIsASampleKey123!";

        string baseURL = "https://localhost:7109/api/User/create-user";

        public HomeController(ILogger<HomeController> logger, IMailService emailService, OtpService otpService)
        {
            _logger = logger;
            _emailService = emailService;
            _otpService = otpService;
        }

        public IActionResult Index()
        {

            return View("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Register(string uniqueCode)
        {
            if (!string.IsNullOrEmpty(uniqueCode))
            {
                using (var client = new HttpClient())
                {
                    string decodedUniqueCode = Uri.UnescapeDataString(uniqueCode.Replace("%20", "+"));

                    string generatedLink = $"https://localhost:7246/Home/Register?uniqueCode={decodedUniqueCode}";

                    string apiUrl = $"https://localhost:7109/api/LoginHistory/check-generated-link/{Uri.EscapeDataString(generatedLink)}";

                    HttpResponseMessage response = client.GetAsync(apiUrl).GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        string userEmail = Decrypt(Uri.UnescapeDataString(decodedUniqueCode.Replace(" ", "+")), encryptionKeyToRegister);
                        var user = await GetUser(userEmail);
                        PrimaryClientRegisterDto userEntity = new()
                        {
                            AgencyName = user.Name,
                            AgencyEmail = userEmail
                        };
                        return View("RegisterWithLink", userEntity);
                    } 
                }
            }

            return View();
        }


        //[HttpGet]
        //public IActionResult GenerateRegistrationLink()
        //{
        //    string userEmail = HttpContext.Session.GetString("UserEmail");
        //    string uniqueCode = Guid.NewGuid().ToString();

        //    string registrationLink = $"https://localhost:7246/Home/Register?uniqueCode={uniqueCode}";

        //    LinkGeneration linkGeneration = new()
        //    {
        //        RegistrationLink = registrationLink
        //    };

        //    var formData = new MultipartFormDataContent();

        //    formData.Add(new StringContent(linkGeneration.RegistrationLink), "RegistrationLink");

        //    using (var client = new HttpClient())
        //    {
        //        HttpResponseMessage response = client.PostAsync("https://localhost:7109/api/LoginHistory/generate-link", formData).GetAwaiter().GetResult();

        //        if (response.IsSuccessStatusCode)
        //        {
        //            return Json(new { RegistrationLink = registrationLink });
        //        }
        //    }

        //    return null;
        //}

        [HttpGet]
        public IActionResult GenerateRegistrationLink()
        {
            string userEmail = HttpContext.Session.GetString("UserEmail");
            string uniqueCode = Encrypt(userEmail, encryptionKeyToRegister);

            string registrationLink = $"https://localhost:7246/Home/Register?uniqueCode={uniqueCode}";

            LinkGeneration linkGeneration = new()
            {
                RegistrationLink = registrationLink
            };

            var formData = new MultipartFormDataContent();

            formData.Add(new StringContent(linkGeneration.RegistrationLink), "RegistrationLink");

            using (var client = new HttpClient())
            {
                HttpResponseMessage response = client.PostAsync("https://localhost:7109/api/LoginHistory/generate-link", formData).GetAwaiter().GetResult();

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { RegistrationLink = registrationLink });
                }
            }

            return null;
        }

        private string Encrypt(string plainText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = GetValidKey(key, aesAlg.KeySize);
                aesAlg.IV = new byte[16]; // Use a secure random IV for each encryption

                // Generate a random nonce (unique code)
                byte[] nonce = new byte[8]; // You can adjust the size as needed
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(nonce);
                }

                // Concatenate the nonce and email
                string dataToEncrypt = Convert.ToBase64String(nonce) + plainText;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(dataToEncrypt);
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        private string Decrypt(string cipherText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = GetValidKey(key, aesAlg.KeySize);
                aesAlg.IV = new byte[16]; // Use a secure random IV for each encryption

                // Convert the base64 string to bytes
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                // Create a decryptor
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        // Read the decrypted bytes from the decrypting stream
                        string decryptedData = srDecrypt.ReadToEnd();

                        // Extract the nonce and email
                        byte[] nonceBytes = Convert.FromBase64String(decryptedData.Substring(0, 12)); // Adjust the length based on your nonce size
                        string email = decryptedData.Substring(12);

                        return email;
                    }
                }
            }
        }

        private byte[] GetValidKey(string key, int keySize)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] keyBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
                Array.Resize(ref keyBytes, keySize / 8);
                return keyBytes;
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddUser(UserEntity user)
        {
            try
            {
                var userExist = await GetUserDetailsForCreationOrUpdate(user.Email);
                if (userExist != null)
                {
                    // ModelState.AddModelError("Email", "User with this email already exists.");
                    TempData["UserExist"] = "User Already Registerd";
                    return RedirectToAction("Register", "Home");
                }
                UserEntity obj = new UserEntity()
                {
                    // UserID = user.UserID,
                    Email = user.Email,
                    Name = user.Name,
                    Password = user.Password,
                    RoleId = user.RoleId,
                    CreationDate = DateTime.Now,
                    CreatedBy = user.Email,
                    IsDeleted = user.IsDeleted,
                    //UpdationDate = user.UpdationDate,
                };
                if (user.Email != null)
                {

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(baseURL);

                        HttpResponseMessage getData = await client.PostAsJsonAsync<UserEntity>(client.BaseAddress, obj);

                        if (getData.IsSuccessStatusCode)
                        {
                            TempData["RegisterMessage"] = "Registration Successfully";
                            return RedirectToAction("Index", "Home");

                        }
                        else
                        {
                            // Console.WriteLine("Error Calling Web API");
                            TempData["RegisterErrorMessage"] = "Please Insure the all Required Filled Correctly Inserted";
                            return RedirectToAction("Register", "Home");
                        }

                    }
                }
                else
                {
                    TempData["Message"] = "This Email Can Not be Registered";
                    return RedirectToAction("Register", "Home");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return new JsonResult(new { Error = "An unexpected error occurred" }) { StatusCode = 500 };
            }
        }



        string apiUrl = "https://localhost:7109/api/LoginHistory/login";

        [HttpPost]
        public async Task<IActionResult> Login(UserEntity user)
        {
            try
            {
                var userExist = await GetUser(user.Email);
                {
                    if (userExist != null)
                    {
                        using (var client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(apiUrl);

                            // Send the user credentials to the login API
                            HttpResponseMessage response = await client.PostAsJsonAsync(apiUrl, user);

                            string token = "";

                            if (response.IsSuccessStatusCode)
                            {
                                // Parse the response to check authentication status
                                //bool isAuthenticated = await response.Content.ReadAsAsync<bool>();


                                // Successful login, you might want to handle authentication hereSSsSSS
                                // For example, set a cookie or JWT token for the user
                                //return new JsonResult(new { Message = "Login successful" });
                                var responseData = await response.Content.ReadAsStringAsync();

                                using (JsonDocument document = JsonDocument.Parse(responseData))
                                {
                                    JsonElement root = document.RootElement;

                                    if (root.TryGetProperty("token", out JsonElement tokenElement))
                                    {
                                        token = tokenElement.GetString();

                                    }
                                }

                                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                                JwtSecurityToken jwtToken = handler.ReadJwtToken(token) as JwtSecurityToken;

                                var claims = jwtToken.Claims;
                                string email = claims.First(c => c.Type == "email").Value;
                                string role = claims.First(c => c.Type == "role").Value;
                                //string name = claims.First(c => c.Type == "name").Value;

                                HttpContext.Session.SetString("UserEmail", email);
                                HttpContext.Session.SetString("BearerToken", token.ToString());
                                HttpContext.Session.SetString("Role", role);

                                //HttpContext.Session.SetString("UserName", name);
                                // TempData["LoginSuccess"] = "Login Successfully";

                                ViewData["Email"] = email;

                                //await Otp(email);

                                //return View("VerifyOtp");
                                return View("HomePage");


                            }
                            else
                            {
                                //return new JsonResult(new { Error = "Error calling API" }) { StatusCode = 500 };
                                TempData["Errormessage"] = "Email or Password is not correct";
                                return RedirectToAction("Index", "Home");
                            }
                        }
                    }

                    else
                    {
                        // Handle API error, for example, return an error response
                        // return new JsonResult(new { Error = "Error calling API" }) { StatusCode = 500 };
                        TempData["UserNotExist"] = "This Account does not Exist Please! Register First";
                        return RedirectToAction("Index", "Home");

                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle unexpected exceptions
                Console.WriteLine($"Exception: {ex.Message}");
                // return new JsonResult(new { Error = "An unexpected error occurred" }) { StatusCode = 500 };
                TempData["Exception"] = "Please fill the Email & Password Filled";
                return RedirectToAction("Index", "Home");
            }
        }

        public async Task<IActionResult> Otp(string email)
        {
            //if (email == null)
            //{
            //    email = HttpContext.Session.GetString("UserEmail");
            //}

            if (email != null)
            {
                var otp = _otpService.GenerateOtp();

                using (var client = new HttpClient())
                {
                    UserOtpDto userOtpDto = new()
                    {
                        Email = email,
                        OTP = otp
                    };

                    var formData = new MultipartFormDataContent();

                    formData.Add(new StringContent(userOtpDto.Email), "Email");
                    formData.Add(new StringContent(userOtpDto.OTP), "OTP");

                    HttpResponseMessage getData = await client.PutAsync("https://localhost:7109/api/User/update-user", formData);

                    if (getData.IsSuccessStatusCode)
                    {
                        Mailrequest mailrequest = new Mailrequest();
                        mailrequest.ToEmail = "dhawaj777@gmail.com";
                        mailrequest.Subject = "OTP Verfication";
                        mailrequest.Body = $"Your OTP for login at Sugamta is {otp}";
                        await _emailService.SendEmailAsync(mailrequest);
                        //return RedirectToAction("Index", "Home");

                    }
                }

                //return RedirectToAction("VerifyOtp", new { email });
            }

            return RedirectToAction("Index");
        }

        //public async Task<IActionResult> VerifyOtp()
        //{
        //    return View();
        //}

        [HttpGet]
        public async Task<IActionResult> ResendOtp(string email)
        {
            var user = await GetUser(email);

            if (user != null)
            {
                //ViewData["QrCode"] = Convert.ToBase64String(_otpService.GenerateQrCode(user.OTP));
                return View("VerifyOtp");
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> VerifyOtp(string email)
        {
            var user = await GetUser(email);

            if (user != null)
            {
                //ViewData["QrCode"] = Convert.ToBase64String(_otpService.GenerateQrCode(user.OTP));
                return View();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(string email, string enteredOtp)
        {
            if (email == null)
            {
                email = HttpContext.Session.GetString("UserEmail");
            }

            var user = await GetUser(email);

            UserOtpDto userOtpDto = new()
            {
                Email = user.Email,
                OTP = user.OTP
            };

            if(userOtpDto.Email == email && userOtpDto.OTP == enteredOtp)
            {
                if (user != null)
                {
                    userOtpDto.OTP = null;

                    using (var client = new HttpClient())
                    {
                        var formData = new MultipartFormDataContent();

                        formData.Add(new StringContent(userOtpDto.Email), "Email");

                        HttpResponseMessage getData = await client.PutAsync("https://localhost:7109/api/User/update-user", formData);

                        if (getData.IsSuccessStatusCode)
                        {
                            return View("HomePage");

                        }
                    }
                }
            } else if(userOtpDto.OTP != enteredOtp)
            {
                TempData["Errormessage"] = "Incorrect OTP";
            }

            return View("VerifyOtp");
        }

        public IActionResult Privacy()
        {
            return View();
        }


        string AddUserDetailsURL = "https://localhost:7109/api/add-user-details";
        string UpdateUserDetailsURL = "https://localhost:7109/api/update-user-details";

        public async Task<UserDetails> GetUserDetails(string email)
        {
            using (var client = new HttpClient())
            {

                string apiUrl = $"https://localhost:7109/api/get-user-details/{Uri.EscapeDataString(email)}";

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Assuming the API returns JSON
                    // UserDetails userDetails = await response.Content.ReadAsAsync<UserDetails>();
                    // return userDetails;
                    string content = await response.Content.ReadAsStringAsync();

                    // Deserialize the string into UserDetailsDto using JSON serializer
                    UserDetails userDetails = JsonConvert.DeserializeObject<UserDetails>(content);
                    return userDetails;

                    //return userDetails;
                }
                else
                {
                    string content = await response.Content.ReadAsStringAsync();

                    // Deserialize the string into UserDetailsDto using JSON serializer
                    UserDetails userDetails = new()
                    {
                        Email = content
                    };

                    return userDetails;
                }
            }
        }

        public async Task<IActionResult> GetProfileImage(string email)
        {
            using (var client = new HttpClient())
            {
                string apiUrl = $"https://localhost:7109/api/get-profile-image/{Uri.EscapeDataString(email)}";

                HttpResponseMessage response = await client.GetAsync(apiUrl);

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

        public async Task<UserDetails> GetUserDetailsForCreationOrUpdate(string email)
        {
            using (var client = new HttpClient())
            {
                string apiUrl = $"https://localhost:7109/api/get-user-details-for-create-or-update/{Uri.EscapeDataString(email)}";

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Assuming the API returns JSON
                    // UserDetails userDetails = await response.Content.ReadAsAsync<UserDetails>();
                    // return userDetails;
                    string content = await response.Content.ReadAsStringAsync();

                    // Deserialize the string into UserDetailsDto using JSON serializer
                    UserDetails userDetails = JsonConvert.DeserializeObject<UserDetails>(content);

                    return userDetails;
                }
                else
                {
                    return null;
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrUpdateUserDetails(UserDetailsCreateOrUpdateDto userDetails)
        {
            try
            {
                userDetails.Email = HttpContext.Session.GetString("UserEmail");

                var existingUser = await GetUserDetailsForCreationOrUpdate(userDetails.Email);

                var formData = new MultipartFormDataContent();

                formData.Add(new StringContent(userDetails.Email), "Email");
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
                        client.BaseAddress = new Uri(UpdateUserDetailsURL);

                        // Send the PUT request with multipart/form-data
                        HttpResponseMessage response = await client.PutAsync("https://localhost:7109/api/update-user-details", formData);

                        if (response.IsSuccessStatusCode)
                        {
                            TempData["UpdateMessage"] = "User Details Updated Successfully";
                            //return RedirectToAction("Index", "Home");
                            return View("HomePage");
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
                        client.BaseAddress = new Uri(AddUserDetailsURL);

                        HttpResponseMessage response = await client.PostAsync("https://localhost:7109/api/add-user-details", formData);

                        if (response.IsSuccessStatusCode)
                        {
                            TempData["SaveMessage"] = "User Details Added Successfully";
                            return View("HomePage");
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

        public async Task<UserDetails> GetUser(string email)
        {

            using (var client = new HttpClient())
            {
                string UserApiUrl = $"https://localhost:7109/api/User/{Uri.EscapeDataString(email)}";

                HttpResponseMessage response = await client.GetAsync(UserApiUrl);

                if (response.IsSuccessStatusCode)
                {
                    // Assuming the API returns JSON
                    // UserDetails userDetails = await response.Content.ReadAsAsync<UserDetails>();
                    // return userDetails;
                    string content = await response.Content.ReadAsStringAsync();

                    // Deserialize the string into UserDetailsDto using JSON serializer
                    UserDetails userDetails = JsonConvert.DeserializeObject<UserDetails>(content);

                    return userDetails;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<List<State>> GetState()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7109/api/");
                    var response = await client.GetAsync("get-state-list");

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsAsync<List<State>>();
                    }
                    else
                    {
                        // Log the error or handle it appropriately
                        ModelState.AddModelError(string.Empty, "Server error. Unable to fetch state data.");
                        return new List<State>();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle exceptions
                Console.WriteLine($"Exception: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                return new List<State>();
            }
        }

        public async Task<List<Country>> GetCountry()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7109/api/");
                    var response = await client.GetAsync("get-country-list");

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsAsync<List<Country>>();
                    }
                    else
                    {
                        // Log the error or handle it appropriately
                        ModelState.AddModelError(string.Empty, "Server error. Unable to fetch country data.");
                        return new List<Country>();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle exceptions
                Console.WriteLine($"Exception: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                return new List<Country>();
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult HomePage()
        {
            return View();
        }
    }
}
