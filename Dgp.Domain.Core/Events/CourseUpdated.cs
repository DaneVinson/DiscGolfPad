using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public class CourseUpdated : Course, IUpdatedEvent<Course>
    {
        public CourseUpdated() : base()
        { }

        public CourseUpdated(Course course) : base(course)
        { }

        public Type EntityType
        {
            get { return typeof(Course); }
            set { }
        }
    }
}
