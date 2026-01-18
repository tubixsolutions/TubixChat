using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TubixChat.DataLayer.Entities;

[Table("sys_user", Schema = "chat")]
public class User
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [Column("user_name")]
    [MaxLength(250)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [Column("password_hash")]
    [MaxLength(250)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [Column("password_salt")]
    [MaxLength(250)]
    public string PasswordSalt { get; set; } = string.Empty;

    [Required]
    [Column("full_name")]
    [MaxLength(250)]
    public string FullName { get; set; } = string.Empty;

    [Column("phone_number")]
    [MaxLength(50)]
    public string? PhoneNumber { get; set; }

    [Required]
    [Column("state_id")]
    public int StateId { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(5);

    [Column("created_user_id")]
    public int? CreatedUserId { get; set; }

    [Column("modified_at")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modified_user_id")]
    public int? ModifiedUserId { get; set; }

    // Navigation properties
    [ForeignKey("StateId")]
    public virtual State State { get; set; } = null!;

    [NotMapped]
    public bool IsOnline { get; set; }

    [NotMapped]
    public DateTime? LastSeen { get; set; }
}