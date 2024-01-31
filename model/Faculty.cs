using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.model
{
    [Table("faculties")]
    public class Faculty
    {   [Key]
        [Column("faculty_id")]
        public int Id { get; set; }
        [Required]
        [Column("faculty_name")]
        [MaxLength(50)]
        public string Name { get; set; }
        [Column("adress")]
        [MaxLength(100)]
        public string Adress { get; set; }
        [Column("telephone")]
        [MaxLength(50)]
        public string Telephone { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
    }
}
