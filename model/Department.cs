using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University.model
{
    [Table("departments")]
    public class Department
    { 
        [Key]
        [Column("department_id")]
        public int Id { get; set; }
        [Required]
        [Column("department_name")]
        [MaxLength(50)]
        public string Name { get; set; }
        [ForeignKey("Faculty")]
        [Column("faculty_id")]
        public int FacultyId { get; set; }
        public virtual Faculty Faculty{get;set;}
        public virtual ICollection<Teacher> Teachers { get; set; }
    }
}
