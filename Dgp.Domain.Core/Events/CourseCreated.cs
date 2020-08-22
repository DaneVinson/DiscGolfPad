using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public class CourseCreated : Course, ICreatedEvent<Course>
    {
        public CourseCreated() : base()
        { }

        public CourseCreated(Course course) : base(course)
        { }

        public Type EntityType
        {
            get { return typeof(Course); }
            set { }
        }
    }
}
