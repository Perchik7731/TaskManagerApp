using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagerApp.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public string Status { get; set; }
        public int? ProjectId { get; set; }
        public int? AssigneeId { get; set; }
        public Project Project { get; set; } 
        public User Assignee { get; set; }
        public Team Team { get; set; }
        public int? TeamId { get; set; }
    }
}
