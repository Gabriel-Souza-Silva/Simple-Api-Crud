using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleCrud.Data;
using SimpleCrud.Models;
using SimpleCrud.Models.Request;
using SimpleCrud.Services.JwtService;
using SimpleCrud.Validator;

namespace SimpleCrud.Controllers
{
    [Route("api/[controller]")]
    [ApiController, Authorize]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        public IJwtService _jwtService { get; }

        private readonly UserRequestDtoValidator _validator;

        public UserController(DataContext context, IJwtService jwtService, UserRequestDtoValidator validator)
        {
            _context = context;
            _jwtService = jwtService;
            _validator = validator;
        }

        [HttpGet()]
        public ActionResult<List<User>> Get()
        {
            return Ok(_context.Users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetOne(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (user is null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost()]
        [AllowAnonymous]
        public IActionResult Register(UserRequestDto request)
        {
            try
            {
                var validationResult = _validator.Validate(request);

                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var userValition = _context.Users.FirstOrDefault(u => u.Email == request.Email);

                if (userValition is not null)
                    return BadRequest("Email já cadastrado");

                userValition = _context.Users.FirstOrDefault(u => u.Phone == request.Phone);

                if (userValition is not null)
                    return BadRequest("Nickname já utilizado");

                var (passwordSalt, passwordHash) = _jwtService.CreatePasswordHash(request.Password);

                var user = new User
                {
                    Email = request.Email,
                    Phone = request.Phone,
                    Adress = request.Adress,
                    Age = request.Age,
                    DocumentNumber = request.DocumentNumber,
                    Name = request.Name,
                    Gender = request.Gender,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    TokenCreated = DateTime.Now,
                    TokenExpires = DateTime.Now.AddHours(2),
                    VerificationToken = _jwtService.CreateRandomToken(),
                };

                _context.Users.Add(user);

                _context.SaveChanges();

                return NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody]UserRequestDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (user is null)
                return NotFound();

            user.Email = request.Email;
            user.Phone = request.Phone;
            user.Adress = request.Adress;
            user.Age = request.Age;
            user.DocumentNumber = request.DocumentNumber;
            user.Name = request.Name;
            user.Gender = request.Gender;

            _context.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (user is null)
                return NotFound();

            _context.Users.Remove(user);

            _context.SaveChanges();

            return NoContent();
        }
    }
}
