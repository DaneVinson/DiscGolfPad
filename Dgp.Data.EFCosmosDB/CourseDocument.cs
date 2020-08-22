using Dgp.Domain.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dgp.Data.EFCosmosDB
{
    public class CourseDocument
    {
        public CourseDocument()
        { }

        public CourseDocument(Course course)
        {
            CourseData = JsonConvert.SerializeObject(course);
            Id = course.Id;
            Name = course.Name;
            PlayerId = course.PlayerId;
        }

        public string CourseData { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PlayerId { get; set; }
    }
}
