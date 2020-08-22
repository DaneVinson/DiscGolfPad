using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dgp.Domain.Core
{
    public class UpdateCourse : Course, ICommand<Course>
    {
        public UpdateCourse()
        { }

        public UpdateCourse(Course course) : base(course)
        { }
    }
}
