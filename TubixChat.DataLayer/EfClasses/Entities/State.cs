using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TubixChat.DataLayer.Entities;

[Table("enum_state", Schema = "chat")]
public class State
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("order_code")]
    [MaxLength(50)]
    public string? OrderCode { get; set; }

    [Required]
    [Column("short_name")]
    [MaxLength(250)]
    public string ShortName { get; set; } = string.Empty;

    [Required]
    [Column("full_name")]
    [MaxLength(250)]
    public string FullName { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(5);
}