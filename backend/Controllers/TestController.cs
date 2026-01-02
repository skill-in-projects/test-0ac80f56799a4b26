using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using Npgsql;

namespace Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly string _connectionString;

    public TestController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? Environment.GetEnvironmentVariable("DATABASE_URL") 
            ?? throw new InvalidOperationException("Database connection string not found");
    }

    // GET: api/test
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TestProjects>>> GetAll()
    {
        var projects = new List<TestProjects>();
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        var quote = Convert.ToChar(92).ToString() + Convert.ToChar(34).ToString();
        var sql = "SELECT " + quote + "Id" + quote + ", " + quote + "Name" + quote + " FROM " + quote + "TestProjects" + quote + " ORDER BY " + quote + "Id" + quote + " ";
        using var cmd = new NpgsqlCommand(sql, conn);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            projects.Add(new TestProjects
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            });
        }
        return Ok(projects);
    }

    // GET: api/test/5
    [HttpGet("{id}")]
    public async Task<ActionResult<TestProjects>> Get(int id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        var quote = Convert.ToChar(92).ToString() + Convert.ToChar(34).ToString();
        var sql = "SELECT " + quote + "Id" + quote + ", " + quote + "Name" + quote + " FROM " + quote + "TestProjects" + quote + " WHERE " + quote + "Id" + quote + " = @id ";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", id);
        using var reader = await cmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return Ok(new TestProjects
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            });
        }
        return NotFound();
    }

    // POST: api/test
    [HttpPost]
    public async Task<ActionResult<TestProjects>> Create([FromBody] TestProjects project)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        var quote = Convert.ToChar(92).ToString() + Convert.ToChar(34).ToString();
        var sql = "INSERT INTO " + quote + "TestProjects" + quote + " (" + quote + "Name" + quote + ") VALUES (@name) RETURNING " + quote + "Id" + quote + " ";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("name", project.Name);
        var id = await cmd.ExecuteScalarAsync();
        project.Id = Convert.ToInt32(id);
        return CreatedAtAction(nameof(Get), new { id = project.Id }, project);
    }

    // PUT: api/test/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TestProjects project)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        var quote = Convert.ToChar(92).ToString() + Convert.ToChar(34).ToString();
        var sql = "UPDATE " + quote + "TestProjects" + quote + " SET " + quote + "Name" + quote + " = @name WHERE " + quote + "Id" + quote + " = @id ";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("name", project.Name);
        cmd.Parameters.AddWithValue("id", id);
        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        if (rowsAffected == 0) return NotFound();
        return NoContent();
    }

    // DELETE: api/test/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();
        var quote = Convert.ToChar(92).ToString() + Convert.ToChar(34).ToString();
        var sql = "DELETE FROM " + quote + "TestProjects" + quote + " WHERE " + quote + "Id" + quote + " = @id ";
        using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("id", id);
        var rowsAffected = await cmd.ExecuteNonQueryAsync();
        if (rowsAffected == 0) return NotFound();
        return NoContent();
    }
}
