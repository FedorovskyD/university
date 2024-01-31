using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace University.model
{
    [Table("teachers")]
    public partial class Teacher
    {
        [Key]
        [Column("teacher_id")]
        public int Id { get; set; }
        [Required]
        [Column("teacher_name")]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [Column("teacher_lastname")]
        [MaxLength(50)]
        public string LastName { get; set; }
        [Column("adress")]
        [MaxLength(50)]
        public string Address { get; set; }
        [Column("telephone")]
        [MaxLength(50)]
        public string Telephone { get; set; }
        [ForeignKey("Department")]
        [Column("department_id")]
        public int DepartmentID { get; set; }
        public virtual Department Department { get; set; }
    }
}
