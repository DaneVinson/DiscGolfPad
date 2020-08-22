using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dgp.Domain.Core;
using FluentValidation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Dgp.Service.Rest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        public CoursesController(
            IMessenger<ICommand<Course>> messenger, 
            IQueryProcessor queryProcessor,
            IValidator<Course> validator,
            IValidator<Delete<Course>> deleteValidator)
        {
            _deleteValidator = deleteValidator ?? throw new ArgumentNullException(nameof(deleteValidator));
            _messenger = messenger ?? throw new ArgumentNullException();
            _queryProcessor = queryProcessor ?? throw new ArgumentNullException();
            _validator = validator ?? throw new ArgumentNullException();
        }

        #region Command

        [HttpPost]
        public async Task<ActionResult<Course>> CreateCourseAsync([FromBody]Course course)
        {
            if (course == null) { return BadRequest(); }
            var validationResult = await _validator.ValidateAsync(course);
            if (!validationResult.IsValid) { return BadRequest(validationResult.Errors); }

            course.PlayerId = this.GetPlayerId();
            var command = new CreateCourse(course);
            await _messenger.SendAsync(command);

            return Accepted(command);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCourse(Guid id)
        {
            if (id == Guid.Empty) { return BadRequest(); }

            var command = new Delete<Course>(this.GetPlayerId(), id);
            var validationResult = await _deleteValidator.ValidateAsync(command);
            if (validationResult.IsValid) { return BadRequest(validationResult.Errors); }

            await _messenger.SendAsync(command);

            return Accepted();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Course>> UpdateCourseAsync(Guid id, [FromBody]Course course)
        {
            if (id == Guid.Empty || course == null || id != course.Id) { return BadRequest(); }
            var validationResult = await _validator.ValidateAsync(course);
            if (!validationResult.IsValid) { return BadRequest(validationResult.Errors); }

            await _messenger.SendAsync(new UpdateCourse(course));
            return Accepted(course);
        }

        #endregion

        #region Query

        [HttpGet("{id}")]
        public async Task<ActionResult<Course>> GetCourseAsync(Guid id)
        {
            var course = await _queryProcessor.GetCourseAsync(new SimpleEntity(id, this.GetPlayerId()));
            return Ok(course);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseInfo>>> GetCoursesAsync()
        {
            var courses = await _queryProcessor.GetCoursesAsync(this.GetPlayerId());
            return Ok(courses);
        }

        #endregion

        private readonly IValidator<Delete<Course>> _deleteValidator;
        private readonly IMessenger<ICommand<Course>> _messenger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IValidator<Course> _validator;
    }
}
