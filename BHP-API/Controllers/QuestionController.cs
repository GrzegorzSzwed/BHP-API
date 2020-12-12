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
    public class QuestionController : ControllerBase
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly ILoggerService _logger;
        private readonly IMapper _mapper;

        public QuestionController(IQuestionRepository questionRepository,
            ILoggerService logger,
            IMapper mapper)
        {
            _questionRepository = questionRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get All questions
        /// </summary>
        /// <returns>List of authors</returns>
        [HttpGet]
        public async Task<IActionResult> GetQuestions()
        {
            try
            {
                _logger.LogInfo("Attempted Get All questions.");
                var questions = await _questionRepository.FindAll();
                var response = _mapper.Map<IList<QuestionDTO>>(questions);
                _logger.LogInfo("Successfully got all questions.");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }
        /// <summary>
        /// Get Question specified by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuestion(int id)
        {
            try
            {
                _logger.LogInfo($"Attempted to get question id {id}");
                var question = await _questionRepository.FindById(id);
                if (question == null)
                {
                    _logger.LogWarn($"Question with id {id} was not found");
                    return NotFound();
                }

                var response = _mapper.Map<QuestionDTO>(question);
                _logger.LogInfo("Successfully got all questions.");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }
        /// <summary>
        /// Creates a new question
        /// </summary>
        /// <param name="questionDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] QuestionCreateDTO questionDTO)
        {
            try
            {
                _logger.LogInfo($"question submission attempted");

                if (questionDTO == null)
                {
                    _logger.LogWarn($"Empty request was submitted");
                    return BadRequest(ModelState);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"Author Data was incomplete");
                    return BadRequest(ModelState);
                }

                var question = _mapper.Map<Question>(questionDTO);
                var isSuccess = await _questionRepository.Create(question);
                if (!isSuccess)
                {
                    return InternalError($"Question creation failed");
                }
                else
                {
                    _logger.LogInfo("Question Created");
                    return Created("Create", new { question });
                }
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Update an question
        /// </summary>
        /// <param name="id"></param>
        /// <param name="questionDTO"></param>
        /// <returns></returns>
        [Authorize(Roles = "Administrator")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] QuestionUpdateDTO questionUpdateDTO)
        {
            try
            {
                _logger.LogInfo($"Question update attempted - id {id}");
                if (id < 1 || questionUpdateDTO == null || id != questionUpdateDTO.Id)
                    return BadRequest();

                var exists = await _questionRepository.Exists(id);
                if (!exists)
                {
                    _logger.LogWarn($"Question with id {id} not found");
                    return NotFound();
                }

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var question = _mapper.Map<Question>(questionUpdateDTO);
                var isSuccess = await _questionRepository.Update(question);
                if (!isSuccess)
                    return InternalError("question update failed");

                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{e.Message} - {e.InnerException}");
            }
        }
        /// <summary>
        /// Delete an question
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
                _logger.LogInfo($"question delete attempted - id {id}");
                var exists = await _questionRepository.Exists(id);
                if (!exists)
                {
                    _logger.LogWarn($"question with id {id} not found");
                    return NotFound();
                }

                if (id < 1)
                {
                    _logger.LogWarn($"Wrong id {id}");
                    return BadRequest();
                }
                var question = await _questionRepository.FindById(id);
                var isSuccess = await _questionRepository.Delete(question);
                if (!isSuccess)
                {
                    return InternalError($"question with id {id} was not deleted");
                }
                _logger.LogInfo($"question with id {id} successfully deleted");
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
