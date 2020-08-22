using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dgp.Domain.Core
{
    public class CreateCourse : Course, ICommand<Course>
    {
        public CreateCourse()
        { }

        public CreateCourse(Course course) : base(course)
        {
            Id = Guid.NewGuid();
        }
    }
}
