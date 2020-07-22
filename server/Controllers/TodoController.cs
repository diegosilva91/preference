namespace preference.Controllers
{
    using preference.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.SignalR;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using server.Hubs;    
    
    [Produces("application/json")]
    [Route("api/[Controller]")]
    public class TodosController: Controller
    {
        private readonly IHubContext<IssueHub> hubContext;
        private readonly ITodoRepository _repo;        
        public TodosController(ITodoRepository repo,IHubContext<IssueHub> hubContext)
        {
            _repo = repo;
            this.hubContext = hubContext;
        }        
        // GET api/todos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Todo>>> Get()
        {
            await this.hubContext.Clients.All.SendAsync("IssuesChanges",await _repo.GetAllTodos());
            return new ObjectResult(await _repo.GetAllTodos());
        }        
        // GET api/todos/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Todo>> Get(long id)
        {
            var todo = await _repo.GetTodo(id);
            if (todo == null)
                return new NotFoundResult();
            
            return new ObjectResult(todo);
        }        // POST api/todos
        [HttpPost]
        public async Task<ActionResult<Todo>> Post([FromBody] Todo todo)
        {
            todo.Id = await _repo.GetNextId();
            await _repo.Create(todo);
            return new OkObjectResult(todo);
        }        // PUT api/todos/1
        [HttpPut("{id}")]
        public async Task<ActionResult<Todo>> Put(long id, [FromBody] Todo todo)
        {
            var todoFromDb = await _repo.GetTodo(id);            
            if (todoFromDb == null)
                return new NotFoundResult();            
            todo.Id = todoFromDb.Id;
            todo.InternalId = todoFromDb.InternalId;            
            await _repo.Update(todo);            
            return new OkObjectResult(todo);
        }        // DELETE api/todos/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            var post = await _repo.GetTodo(id);            
            if (post == null)
                return new NotFoundResult();            
            await _repo.Delete(id);            
            return new OkResult();
        }
        /*[HttpPatch("{id}/upvote")]
        public async Task<ActionResult> UpvoteQuestionAsync(Guid id){

        }*/
    }
}