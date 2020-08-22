using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Domain.Core
{
    public class CourseInfoUpdated : CourseInfo, IUpdatedEvent<CourseInfo>
    {
        public CourseInfoUpdated() : base()
        { }

        public CourseInfoUpdated(CourseInfo courseInfo) : base(courseInfo)
        { }

        public Type EntityType
        {
            get { return typeof(CourseInfo); }
            set { }
        }
    }
}
