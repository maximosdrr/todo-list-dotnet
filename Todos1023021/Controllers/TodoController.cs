using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Todos1023021.Contexts;
using Todos1023021.Models;
using Microsoft.EntityFrameworkCore;

namespace Todos1023021.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoController(TodoContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }



        [Route("Insert")]
        [HttpPost]
        public async Task<Todo> Insert(Todo todo)
        {
            var todoToInsert = new Todo
            {
                Id = Guid.NewGuid().ToString(),
                Title = todo.Title,
                Content = todo.Content
            };

            await _context.AddAsync(todoToInsert);

            await _context.SaveChangesAsync();
            return todoToInsert;
        }

        [Route("FindMany")]
        [HttpGet]
        public async Task<List<Todo>> FindMany([FromQuery(Name = "page")] int page = 0,
            [FromQuery(Name = "limit")] int limit = 10
            )
        {
            var savedSearchs = _context.Todos.Skip(page * limit).Take(limit); //O SKIP TEM QUE VIR ANTES DO TAKE
            return await savedSearchs.ToListAsync();
        }

        [Route("FindOne")]
        [HttpGet]
        public async Task<Todo> FindOne([FromQuery(Name = "id")] string id)
        {
            var todo = await _context.Todos.FindAsync(id);
            if (todo != null)
            {
                return todo;
            }
            else
            {
                HttpContext.Response.StatusCode = 404;
                return null;
            }
        }

        [Route("Update")]
        [HttpPut]

        public async Task<Todo> Update(Todo todo)
        {
            var todoToUpdate = await _context.Todos.FindAsync(todo.Id);
            if (todoToUpdate != null)
            {
                todoToUpdate.Title = todo.Title;
                todoToUpdate.Content = todo.Content;
                _context.Update(todoToUpdate);
                await _context.SaveChangesAsync();
                return todoToUpdate;
            }
            else
            {
                HttpContext.Response.StatusCode = 404;
                return null;
            }
        }

        [Route("Delete")]
        [HttpDelete]
        public async Task<bool> Delete([FromQuery(Name = "id")] string id)
        {
            var todoToDelete = _context.Todos.Find(id);
            if (todoToDelete != null)
            {
                _context.Remove(todoToDelete);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                HttpContext.Response.StatusCode = 404;
                return false;
            }

        }

    }
}
