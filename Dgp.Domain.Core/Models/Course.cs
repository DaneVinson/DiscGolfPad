using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dgp.Domain.Core
{
    public class Course : IEntity
    {
        public Course()
        { }

        public Course(Course course)
        {
            if (course.Holes != null)
            {
                Holes = course.Holes
                                .Select(h => new Hole(h.Par, h.Distance))
                                .ToArray();
            }
            Id = course.Id;
            ImageUri = course.ImageUri;
            Location = course.Location;
            Name = course.Name;
            PlayerId = course.PlayerId;
        }


        public Hole[] Holes { get; set; }
        public Guid Id { get; set; }
        public string ImageUri { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public string PlayerId { get; set; }

        public override string ToString()
        {
            int holes = 0;
            if (Holes != null) { holes = Holes.Length; }
            return $"{Name}, {Location} ({holes} holes)";
        }
    }
}
