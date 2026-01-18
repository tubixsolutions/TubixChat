using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TubixChat.DataLayer.Entities;

[Table("hl_message", Schema = "chat")]
public class Message
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [Column("sender_user_id")]
    public int SenderUserId { get; set; }

    [Required]
    [Column("reciever_user_id")]
    public int RecieverUserId { get; set; }

    [Required]
    [Column("message_text")]
    public string MessageText { get; set; } = string.Empty;

    [Column("is_pinned")]
    public bool IsPinned { get; set; } = false;

    [Required]
    [Column("is_read")]
    public bool IsRead { get; set; } = false;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(5);

    [Column("created_user_id")]
    public int? CreatedUserId { get; set; }

    [Column("modified_at")]
    public DateTime? ModifiedAt { get; set; }

    [Column("modified_user_id")]
    public int? ModifiedUserId { get; set; }

    [ForeignKey("SenderUserId")]
    public virtual User Sender { get; set; } = null!;

    [ForeignKey("RecieverUserId")]
    public virtual User Reciever { get; set; } = null!;

    [NotMapped]
    public bool IsDelivered { get; set; } = true;
}