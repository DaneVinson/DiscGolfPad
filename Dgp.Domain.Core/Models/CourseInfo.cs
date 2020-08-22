using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dgp.Domain.Core
{
    public class CourseInfo : IEntity
    {
        public CourseInfo()
        { }

        public CourseInfo(CourseInfo courseInfo)
        {
            CopyFrom(courseInfo);
        }

        public CourseInfo(Course course)
        {
            CopyFrom(course);
        }

        public void CopyFrom(CourseInfo courseInfo)
        {
            if (courseInfo == null) { return; }

            Holes = courseInfo.Holes;
            Id = courseInfo.Id;
            Name = courseInfo.Name;
            Par = courseInfo.Par;
            PlayerId = courseInfo.PlayerId;
        }

        public void CopyFrom(Course course)
        {
            if (course == null) { return; }

            if (course.Holes == null)
            {
                Holes = 0;
                Par = 0;
            }
            else
            {
                Holes = course.Holes.Length;
                Par = course.Holes.Select(h => h.Par).Sum();
            }
            Id = course.Id;
            Name = course.Name;
            PlayerId = course.PlayerId;
        }

        public int Holes { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Par { get; set; }
        public string PlayerId { get; set; }

        public override string ToString()
        {
            return $"{Name} - {Holes} holes, Par {Par}";
        }
    }
}
