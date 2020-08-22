using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dgp.Domain.Core;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Dgp.Service.Rest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScorecardsController : ControllerBase
    {
        public ScorecardsController(
            IMessenger<ICommand<Scorecard>> messenger, 
            IQueryProcessor queryProcessor,
            IValidator<Scorecard> validator,
            IValidator<Delete<Scorecard>> deleteValidator)
        {
            _deleteValidator = deleteValidator ?? throw new ArgumentNullException(nameof(deleteValidator));
            _messenger = messenger ?? throw new ArgumentNullException();
            _queryProcessor = queryProcessor ?? throw new ArgumentNullException();
            _validator = validator ?? throw new ArgumentNullException();
        }

        #region Command

        [HttpPost]
        public async Task<ActionResult<Scorecard>> CreateScorecardAsync([FromBody]Scorecard scorecard)
        {
            if (scorecard == null) { return BadRequest(); }
            var validationResult = await _validator.ValidateAsync(scorecard);
            if (!validationResult.IsValid) { return BadRequest(validationResult.Errors); }

            scorecard.PlayerId = this.GetPlayerId();

            var command = new CreateScorecard(scorecard);
            await _messenger.SendAsync(command);
            return Accepted(command);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteScorecard(Guid id)
        {
            if (id == Guid.Empty) { return BadRequest(); }

            var command = new Delete<Scorecard>(this.GetPlayerId(), id);
            var validationResult = await _deleteValidator.ValidateAsync(command);
            if (validationResult.IsValid) { return BadRequest(validationResult.Errors); }

            await _messenger.SendAsync(command);
            return Accepted();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Scorecard>> UpdateScorecardAsync(Guid id, [FromBody]Scorecard scorecard)
        {
            if (id == Guid.Empty || scorecard == null || id != scorecard.Id) { return BadRequest(); }
            var validationResult = await _validator.ValidateAsync(scorecard);
            if (!validationResult.IsValid) { return BadRequest(validationResult.Errors); }

            var command = new UpdateScorecard(scorecard);
            await _messenger.SendAsync(new UpdateScorecard(scorecard));
            return Accepted(command);
        }

        #endregion

        #region Query

        [HttpGet("{id}")]
        public async Task<ActionResult<Scorecard>> GetScorecardAsync(Guid id)
        {
            var scorecard = await _queryProcessor.GetScorecardAsync(new SimpleEntity(id, this.GetPlayerId()));
            return Ok(scorecard);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScorecardInfo>>> GetScorecardsAsync([FromQuery]Guid? courseId)
        {
            var options = new ScorecardsQueryOptions(this.GetPlayerId());
            if (courseId.HasValue) { options.CourseId = courseId.Value; }
            var scorecards = await _queryProcessor.GetScorecardsAsync(options);
            return Ok(scorecards);
        }

        #endregion

        private readonly IValidator<Delete<Scorecard>> _deleteValidator;
        private readonly IMessenger<ICommand<Scorecard>> _messenger;
        private readonly IQueryProcessor _queryProcessor;
        private readonly IValidator<Scorecard> _validator;
    }
}
