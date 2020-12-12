using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BHP_API.Contracts;
using BHP_API.Data;
using BHP_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BHP_API.Controllers
{
    /// <summary>
    /// Endpoint used to iteract with the answers in the bhpdatabase
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class AnswerController : ControllerBase
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public AnswerController(IAnswerRepository answerRepository,
            ILoggerService logger,
            IMapper mapper)
        {
            _answerRepository = answerRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get All answers
        /// </summary>
        /// <returns>List of authors</returns>
        [HttpGet]
        public async Task<IActionResult> Getanswers()
        {
            try
            {
                _logger.LogInfo("Attempted Get All answers.");
                var answers = await _answerRepository.FindAll();
                var response = _mapper.Map<IList<AnswerDTO>>(answers);
                _logger.LogInfo("Successfully got all answers.");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }
        /// <summary>
        /// Get answer specified by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnswer(int id)
        {
            try
            {
                _logger.LogInfo($"Attempted to get answer id {id}");
                var answer = await _answerRepository.FindById(id);
                if (answer == null)
                {
                    _logger.LogWarn($"answer with id {id} was not found");
                    return NotFound();
                }

                var response = _mapper.Map<AnswerDTO>(answer);
                _logger.LogInfo("Successfully got all answers.");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }
        /// <summary>
        /// Creates a new answer
        /// </summary>
        /// <param name="answerDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AnswerCreateDTO answerDTO)
        {
            try
            {
                _logger.LogInfo($"answer submission attempted");

                if (answerDTO == null)
                {
                    _logger.LogWarn($"Empty request was submitted");
                    return BadRequest(ModelState);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"Author Data was incomplete");
                    return BadRequest(ModelState);
                }

                var answer = _mapper.Map<Answer>(answerDTO);
                var isSuccess = await _answerRepository.Create(answer);
                if (!isSuccess)
                {
                    return InternalError($"answer creation failed");
                }
                else
                {
                    _logger.LogInfo("answer Created");
                    return Created("Create", new { answer });
                }
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Update an answer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="answerDTO"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator,Customer")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AnswerUpdateDTO answerUpdateDTO)
        {
            try
            {
                _logger.LogInfo($"answer update attempted - id {id}");
                if (id < 1 || answerUpdateDTO == null || id != answerUpdateDTO.Id)
                    return BadRequest();

                var exists = await _answerRepository.Exists(id);
                if (!exists)
                {
                    _logger.LogWarn($"answer with id {id} not found");
                    return NotFound();
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var answer = _mapper.Map<Answer>(answerUpdateDTO);
                var isSuccess = await _answerRepository.Update(answer);
                if (!isSuccess)
                    return InternalError("answer update failed");

                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }
        /// <summary>
        /// Delete an answer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInfo($"answer delete attempted - id {id}");
                var exists = await _answerRepository.Exists(id);
                if (!exists)
                {
                    _logger.LogWarn($"answer with id {id} not found");
                    return NotFound();
                }

                if (id < 1)
                {
                    _logger.LogWarn($"Wrong id {id}");
                    return BadRequest();
                }
                var answer = await _answerRepository.FindById(id);
                var isSuccess = await _answerRepository.Delete(answer);
                if (!isSuccess)
                {
                    return InternalError($"answer with id {id} was not deleted");
                }
                _logger.LogInfo($"answer with id {id} successfully deleted");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong. Please contact the administrator.");
        }
    }
}
