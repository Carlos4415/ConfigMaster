using Microsoft.AspNetCore.Mvc;
using ConfigMaster.Application.Services;
using System.Collections.Generic;

namespace ConfigMaster.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly ConfigurationService _configurationService;

        public ConfigurationController(ConfigurationService configuracaoService)
        {
            _configurationService = configuracaoService;
        }

        [HttpGet("key/{key}")]
        public IActionResult GetValuesByKey(string key)
        {
            var allData = _configurationService.GetValuesByKey(key);
            if (allData == null || allData.Count == 0)
            {
                return NotFound();
            }
            return Ok(allData);
        }

        [HttpGet("section/{section}")]
        public IActionResult GetValuesBySection(string section)
        {
            var values = _configurationService.GetValuesBySection(section);
            if (values == null || values.Count == 0)
            {
                return NotFound();
            }
            return Ok(values);
        }

        [HttpGet("all")]
        public IActionResult GetAllData()
        {
            var allData = _configurationService.GetAllData();
            if (allData == null || allData.Count == 0)
            {
                return NotFound();
            }
            return Ok(allData);
        }

        [HttpPost("section")]
        public IActionResult AddSection([FromBody] NewSectionRequest newSectionRequest)
        {
            try
            {
                _configurationService.AddSection(newSectionRequest.Section, newSectionRequest.Values);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("section")]
        public IActionResult UpdateKeysBySection([FromBody] UpdateSectionRequest updateSectionRequest)
        {
            try
            {
                _configurationService.UpdateKeysBySection(updateSectionRequest.Section, updateSectionRequest.Values);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("section/{section}/key/{key}")]
        public IActionResult DeleteKey(string section, string key)
        {
            try
            {
                _configurationService.DeleteKey(section, key);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("section/{section}")]
        public IActionResult DeleteSection(string section)
        {
            try
            {
                _configurationService.DeleteSection(section);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class NewSectionRequest
    {
        public string Section { get; set; }
        public Dictionary<string, string> Values { get; set; }
    }

    public class UpdateSectionRequest
    {
        public string Section { get; set; }
        public Dictionary<string, string> Values { get; set; }
    }
}
