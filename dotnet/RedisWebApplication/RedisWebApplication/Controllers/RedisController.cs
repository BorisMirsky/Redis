using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;




namespace RedisWebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RedisController : ControllerBase
    {

        private readonly UserService _service;
        ApplicationContext db;

        public RedisController(UserService service)
        {
            _service = service;
        }

        [HttpGet("/")]
        public IActionResult Index()
        {
            return Ok("Start page, bitch");
        }


        [HttpGet("{id}")]
        public IActionResult User(int id)
        {

            var _user = _service.GetUser(id);
            if (_user == null)
            {
                return BadRequest("Пользователь не найден");
            }
            return Ok(_user);     

        }
    }
}
