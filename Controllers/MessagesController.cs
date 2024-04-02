using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Route("API/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly ConversationContext _context;

        public MessagesController(ConversationContext context)
        {
            _context = context;
        }

        // GET: api/Messages
        [HttpGet, Authorize]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages()
        {
            return await _context.Messages.ToListAsync();
        }

        // GET: api/Messages/5
        [HttpGet("{id}"), Authorize]
        public async Task<ActionResult<Message>> GetMessage(Guid id)
        {
            var message = await _context.Messages.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            return message;
        }

        // PUT: api/Messages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}"), Authorize]
        public async Task<IActionResult> PutMessage(Guid id, Message message)
        {
            if (id != message.MessageId)
            {
                return BadRequest();
            }
            _context.Entry(message).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // POST: api/Messages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize]
        [Route("/API/Messages/Add")]
        public async Task<ActionResult<MessageDTO>> PostMessage(MessageDTO message)
        {
            _context.Messages.Add(new Models.Message()
            {
                Message1 = message.Message1,
                Reciever = message.Reciever,
                Sender = message.Sender,
                MessageId = Guid.NewGuid(),
            });
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                return NotFound();

            }
            return Ok(message);
        }

        // DELETE: api/Messages/5
        [HttpDelete("{id}"), Authorize]
        public async Task<IActionResult> DeleteMessage(Guid id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MessageExists(Guid id)
        {
            return _context.Messages.Any(e => e.MessageId == id);
        }
    }
}
