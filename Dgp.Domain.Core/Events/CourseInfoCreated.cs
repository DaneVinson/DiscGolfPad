using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public class CourseInfoCreated : CourseInfo, ICreatedEvent<CourseInfo>
    {
        public CourseInfoCreated() : base()
        { }

        public CourseInfoCreated(CourseInfo courseInfo) : base(courseInfo)
        { }

        public Type EntityType
        {
            get { return typeof(CourseInfo); }
            set { }
        }
    }
}
