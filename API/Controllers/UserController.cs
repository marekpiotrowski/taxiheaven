using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;
using API.App_Start;
using API.Models;
using AutoMapper.QueryableExtensions;
using Backend.Database.Business;
using Backend.Database.Model;
using AutoMapper;

namespace API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UserController : ApiController
    {
        private readonly UserRepository _userRepository;
        private readonly UserRoleRepository _userRoleRepository;

        public UserController()
        {
            TaxiHeavenContext ctx = new TaxiHeavenContext();
            _userRepository = new UserRepository(ctx);
            _userRoleRepository = new UserRoleRepository(ctx);
        }

        [HttpPost]
        [Route("api/User/SignIn")]
        public IHttpActionResult SignIn(string login, string password)
        {
            var user = _userRepository.Get().FirstOrDefault(x => x.Email == login && x.Password == password);
            var userModel = Mapper.Map<UserModel>(user);
            if(userModel == null)
                return BadRequest("Login lub hasło jest niepoprawne.");
            return Ok(userModel);
        }

        public IHttpActionResult Post(RegisterUserModel user)
        {
            if (user.Password != user.ConfirmPassword)
                return BadRequest("Hasłą nie są takie same.");
            if (string.IsNullOrEmpty(user.Email))
                return BadRequest("Adres e-mail jest niepoprawny.");
            if (string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.ConfirmPassword) ||
                string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.FirstName) ||
                string.IsNullOrEmpty(user.LastName) || string.IsNullOrEmpty(user.PhoneNumber) ||
                string.IsNullOrEmpty(user.TicketNumber))
                return BadRequest("Wszystkie pola muszą być wypełnione.");

            var userExists = _userRepository.Get().Where(x => x.Email == user.Email).Count() > 0;
            if (userExists)
                return BadRequest("Istnieje już użytkownik z takim adresem e-mail.");

            var ticketExists = _userRepository.Get().FirstOrDefault(x => x.TicketNumber == user.TicketNumber);

            if(ticketExists == null)
                return BadRequest("Niestety, zaproszenie jest niepoprawne.");

            ticketExists.Email = user.Email;
            ticketExists.Password = user.Password;
            ticketExists.FirstName = user.FirstName;
            ticketExists.LastName = user.LastName;
            ticketExists.PhoneNumber = user.PhoneNumber;
            ticketExists.Active = true;
            _userRepository.Put(ticketExists);

            return Ok("Rejestracja przebiegła pomyślnie. Można się już zalogować.");
        }

        [HttpPost]
        [Route("api/User/AddEmployee")]
        [BasicHttpAuthorize(1)]
        public IHttpActionResult AddEmployee(bool admin, bool employee, string guid)
        {
            if ((bool) this.RequestContext.RouteData.Values["Authorized"] == false)
                return Unauthorized();
            if (!admin && !employee)
                return BadRequest("Przynajmniej jedna rola musi być wybrana.");
            var user = new User();
            user.TicketNumber = guid;
            var addedUser = _userRepository.Post(user);
            if (admin)
            {
                var userRole = new UserRole();
                userRole.RoleId = 1;
                userRole.UserId = addedUser.Id;
                _userRoleRepository.Post(userRole);
            }
            if (employee)
            {
                var userRole = new UserRole();
                userRole.RoleId = 3;
                userRole.UserId = addedUser.Id;
                _userRoleRepository.Post(userRole);
            }
            return Ok("Dodano pracownika. Musi on/ona teraz odpowiedzieć na zaproszenie.");
        }
    }
}
