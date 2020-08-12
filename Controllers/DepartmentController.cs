using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using netcoreClass.Models;

namespace netcoreClass.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase {
        private readonly ContosoUniversityContext _context;

        public DepartmentController (ContosoUniversityContext context) {
            _context = context;
        }

        // GET: api/Department
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> GetDepartment () {
            return await _context.Department.ToListAsync ();
        }

        // GET: api/Department/5
        [HttpGet ("{id}")]
        public async Task<ActionResult<Department>> GetDepartment (int id) {
            var department = await _context.Department.FindAsync (id);

            if (department == null) {
                return NotFound ();
            }

            return department;
        }

        // PUT: api/Department/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut ("{id}")]
        public async Task<IActionResult> PutDepartment (int id, Department department) {
            if (id != department.DepartmentId) {
                return BadRequest ();
            }

            _context.Entry (department).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync ();
            } catch (DbUpdateConcurrencyException) {
                if (!DepartmentExists (id)) {
                    return NotFound ();
                } else {
                    throw;
                }
            }

            return NoContent ();
        }

        // POST: api/Department
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public ActionResult<Department> PostDepartment (Department department) {

            var result = _context.Department.FromSqlInterpolated ($"EXECUTE dbo.Department_Insert  {department.Name},{department.Budget},{department.StartDate},{department.InstructorId}")
                .Select (d => new Department { DepartmentId = d.DepartmentId, RowVersion = d.RowVersion })
                .AsEnumerable ()
                .FirstOrDefault ();

            return CreatedAtAction ("GetDepartment", new { id = result.DepartmentId }, new { result.DepartmentId, result.RowVersion });
        }

        // DELETE: api/Department/5
        [HttpDelete ("{id}")]
        public async Task<ActionResult<Department>> DeleteDepartment (int id) {
            var department = await _context.Department.FindAsync (id);
            if (department == null) {
                return NotFound ();
            }

            _context.Department.Remove (department);
            await _context.SaveChangesAsync ();

            return department;
        }

        private bool DepartmentExists (int id) {
            return _context.Department.Any (e => e.DepartmentId == id);
        }
    }
}