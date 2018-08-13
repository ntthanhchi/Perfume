using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ShopNuocHoa;

namespace ShopNuocHoa.Controllers
{
    public class perfumes_apiController : ApiController
    {
        private Entities db = new Entities();

        // GET: api/perfumes_api
        public IQueryable<perfume> Getperfume()
        {
            return db.perfume;
        }

        // GET: api/perfumes_api/5
        [ResponseType(typeof(perfume))]
        public async Task<IHttpActionResult> Getperfume(int id)
        {
            perfume perfume = await db.perfume.FindAsync(id);
            if (perfume == null)
            {
                return NotFound();
            }

            return Ok(perfume);
        }

        // PUT: api/perfumes_api/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> Putperfume(int id, perfume perfume)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != perfume.id)
            {
                return BadRequest();
            }

            db.Entry(perfume).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!perfumeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/perfumes_api
        [ResponseType(typeof(perfume))]
        public async Task<IHttpActionResult> Postperfume(perfume perfume)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.perfume.Add(perfume);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = perfume.id }, perfume);
        }

        // DELETE: api/perfumes_api/5
        [ResponseType(typeof(perfume))]
        public async Task<IHttpActionResult> Deleteperfume(int id)
        {
            perfume perfume = await db.perfume.FindAsync(id);
            if (perfume == null)
            {
                return NotFound();
            }

            db.perfume.Remove(perfume);
            await db.SaveChangesAsync();

            return Ok(perfume);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool perfumeExists(int id)
        {
            return db.perfume.Count(e => e.id == id) > 0;
        }
    }
}